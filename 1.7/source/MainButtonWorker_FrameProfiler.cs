using RimWorld;
using Verse;
using Verse.ProfilerUI;

namespace Frame_Profiler_Quick_Access
{
    public class MainButtonWorker_FrameProfiler : MainButtonWorker
    {
        public override bool Visible => base.Visible && (!ModSettings.ShowButtonOnlyInDevMode || Prefs.DevMode);

        public override void Activate()
        {
            if (!Find.WindowStack.TryRemove(typeof(Window_FrameProfiler)))
            {
                Find.WindowStack.Add(new Window_FrameProfiler());
            }
        }
    }
}
