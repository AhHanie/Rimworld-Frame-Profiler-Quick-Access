using Frame_Profiler_Quick_Access.Patches;
using UnityEngine;
using Verse;

namespace Frame_Profiler_Quick_Access
{
    public static class ModSettingsWindow
    {
        public static void Draw(Rect parent)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(parent);
            listing.CheckboxLabeled(
                "FrameProfilerQuickAccess.ShowButtonOnlyInDevMode".Translate(),
                ref ModSettings.ShowButtonOnlyInDevMode,
                "FrameProfilerQuickAccess.ShowButtonOnlyInDevModeTip".Translate());

            bool wasEnabled = ModSettings.InstrumentTrivialMethods;
            listing.CheckboxLabeled(
                "FrameProfilerQuickAccess.InstrumentTrivialMethods".Translate(),
                ref ModSettings.InstrumentTrivialMethods,
                "FrameProfilerQuickAccess.InstrumentTrivialMethodsTip".Translate());
            listing.End();

            if (wasEnabled != ModSettings.InstrumentTrivialMethods)
            {
                InstrumentationRewriter_TryAnalyze_Patch.RefreshForSettingChange(wasEnabled);
            }
        }
    }
}
