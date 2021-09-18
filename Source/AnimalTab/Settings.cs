// Settings.cs
// Copyright Karel Kroeze, -2020

using UnityEngine;
using Verse;

namespace AnimalTab {
    public class Settings: ModSettings {
        public bool HighContrast = false;

        public void DoWindowContents(Rect canvas) {
            Listing_Standard options = new Listing_Standard();
            options.Begin(canvas);
            options.CheckboxLabeled("Fluffy.AnimalTab.HighContrast".Translate(),
                                     ref HighContrast,
                                     "Fluffy.AnimalTab.HighContrast.Tooltip".Translate());
            options.End();
        }

        public override void ExposeData() {
            base.ExposeData();

            Scribe_Values.Look(ref HighContrast, "highContrast");
        }
    }
}
