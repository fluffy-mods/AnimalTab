using RimWorld;
using UnityEngine;
using Verse;

namespace AnimalTab {
    public class PawnColumnWorker_Age : PawnColumnWorker {

        public override void DoCell(Rect rect, Pawn pawn, PawnTable table) {
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label( rect, pawn.ageTracker.AgeBiologicalYears.ToString() );
            Text.Anchor = TextAnchor.UpperLeft;
        }

        public override int Compare(Pawn a, Pawn b) {
            return a.ageTracker.AgeChronologicalTicks.CompareTo(b.ageTracker.AgeChronologicalTicks);
        }
    }
}
