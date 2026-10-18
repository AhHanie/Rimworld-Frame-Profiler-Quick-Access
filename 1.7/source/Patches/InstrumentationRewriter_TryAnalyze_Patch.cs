using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Waylith.Profiling.Instrumentation;

namespace Frame_Profiler_Quick_Access.Patches
{
    [HarmonyPatch]
    internal static class InstrumentationRewriter_TryAnalyze_Patch
    {
        private const string HarmonyId = "sk.frameprofilerquickaccess";
        private const string RewriterTypeName = "Waylith.Profiling.Instrumentation.InstrumentationRewriter";
        private static readonly Lazy<int> TrivialValue = new Lazy<int>(() => GetTrivialityValue("Trivial"));
        private static readonly Lazy<int> NonTrivialValue = new Lazy<int>(() => GetTrivialityValue("NonTrivial"));

        private static Type RewriterType =>
            typeof(ProfilerInstrumentation).Assembly.GetType(RewriterTypeName, throwOnError: true);

        [HarmonyPrepare]
        private static bool Prepare()
        {
            return ModSettings.InstrumentTrivialMethods;
        }

        private static MethodBase TargetMethod()
        {
            return AccessTools.DeclaredMethod(RewriterType, "TryAnalyze")
                ?? throw new MissingMethodException(RewriterTypeName, "TryAnalyze");
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo computeTriviality = AccessTools.DeclaredMethod(RewriterType, "ComputeTriviality")
                ?? throw new MissingMethodException(RewriterTypeName, "ComputeTriviality");
            MethodInfo adjustTriviality = AccessTools.DeclaredMethod(
                typeof(InstrumentationRewriter_TryAnalyze_Patch), nameof(AdjustTriviality));

            List<CodeInstruction> result = new List<CodeInstruction>();
            int matches = 0;

            foreach (CodeInstruction instruction in instructions)
            {
                result.Add(instruction);
                if (instruction.Calls(computeTriviality))
                {
                    result.Add(new CodeInstruction(OpCodes.Call, adjustTriviality));
                    matches++;
                }
            }

            if (matches != 1)
            {
                throw new InvalidOperationException(
                    $"Expected one ComputeTriviality call in TryAnalyze; found {matches}.");
            }

            return result;
        }

        // The private Waylith enum has an Int32 underlying type. The call-site stack
        // carries that Int32; this method changes only the "Trivial" classification.
        private static int AdjustTriviality(int classification)
        {
            return ModSettings.InstrumentTrivialMethods && classification == TrivialValue.Value
                ? NonTrivialValue.Value
                : classification;
        }

        private static int GetTrivialityValue(string name)
        {
            Type enumType = RewriterType.GetNestedType("Triviality", BindingFlags.NonPublic)
                ?? throw new MissingMemberException(RewriterTypeName, "Triviality");

            if (!enumType.IsEnum || Enum.GetUnderlyingType(enumType) != typeof(int))
            {
                throw new InvalidOperationException("Waylith's Triviality enum no longer has an Int32 underlying type.");
            }

            return Convert.ToInt32(Enum.Parse(enumType, name));
        }

        internal static void RefreshForSettingChange(bool previousValue)
        {
            bool instrumentationWasEnabled = ProfilerInstrumentation.IsEnabled;
            if (instrumentationWasEnabled)
            {
                ProfilerInstrumentation.SetEnabled(false);
                if (ProfilerInstrumentation.IsEnabled)
                {
                    ModSettings.InstrumentTrivialMethods = previousValue;
                    Logger.Error("Could not pause active instrumentation to change the trivial-method patch.");
                    return;
                }
            }

            try
            {
                PatchClassProcessor processor = new Harmony(HarmonyId)
                    .CreateClassProcessor(typeof(InstrumentationRewriter_TryAnalyze_Patch));

                if (ModSettings.InstrumentTrivialMethods)
                {
                    processor.Patch();
                }
                else
                {
                    processor.Unpatch();
                }
            }
            catch (Exception exception)
            {
                ModSettings.InstrumentTrivialMethods = previousValue;
                Logger.Exception(exception, "Changing trivial-method instrumentation");
            }
            finally
            {
                if (instrumentationWasEnabled)
                {
                    ProfilerInstrumentation.SetEnabled(true);
                }
            }
        }
    }
}
