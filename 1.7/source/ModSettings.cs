using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Frame_Profiler_Quick_Access
{
    public class ModSettings : Verse.ModSettings
    {
        public static bool ShowButtonOnlyInDevMode = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref ShowButtonOnlyInDevMode, "showButtonOnlyInDevMode", false);
        }
    }
}
