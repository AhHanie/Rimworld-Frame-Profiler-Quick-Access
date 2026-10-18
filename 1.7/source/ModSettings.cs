using Verse;

namespace Frame_Profiler_Quick_Access
{
    public class ModSettings : Verse.ModSettings
    {
        public static bool ShowButtonOnlyInDevMode = false;
        public static bool InstrumentTrivialMethods = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref ShowButtonOnlyInDevMode, "showButtonOnlyInDevMode", false);
            Scribe_Values.Look(ref InstrumentTrivialMethods, "instrumentTrivialMethods", false);
        }
    }
}
