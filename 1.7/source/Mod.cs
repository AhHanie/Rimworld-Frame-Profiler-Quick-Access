using HarmonyLib;
using UnityEngine;
using Verse;

namespace Frame_Profiler_Quick_Access
{
    public class Mod : Verse.Mod
    {
        public Mod(ModContentPack content) : base(content)
        {
            LongEventHandler.QueueLongEvent(Init, "FrameProfilerQuickAccess.LoadingLabel", doAsynchronously: true, null);
        }

        private void Init()
        {
            GetSettings<ModSettings>();
            new Harmony("sk.frameprofilerquickaccess").PatchAll();
        }

        public override string SettingsCategory()
        {
            return "FrameProfilerQuickAccess.SettingsTitle".Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            ModSettingsWindow.Draw(inRect);
            base.DoSettingsWindowContents(inRect);
        }
    }
}
