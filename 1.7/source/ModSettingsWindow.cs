using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            listing.End();
        }
    }
}
