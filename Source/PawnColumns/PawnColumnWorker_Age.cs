using RimWorld;
using UnityEngine;
using Verse;

namespace AnimalTab {
    public class PawnColumnWorker_Age: PawnColumnWorker {

        public override void DoCell(Rect rect, Pawn pawn, PawnTable table) {
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect, pawn.ageTracker.AgeBiologicalYears.ToString());
            Text.Anchor = TextAnchor.UpperLeft;
            TooltipHandler.TipRegion(rect, () => CellTip(pawn), "AnimalTab_Age_Tip".GetHashCode());
        }

        public virtual string CellTip(Pawn pawn) {
            return pawn.ageTracker.CurLifeStage.LabelCap + "\n" + pawn.ageTracker.AgeTooltipString;
        }

        public override int Compare(Pawn a, Pawn b) {
            return a.ageTracker.AgeChronologicalTicks.CompareTo(b.ageTracker.AgeChronologicalTicks);
        }

        private int Width => 26;

        public override int GetMinWidth(PawnTable table) {
            return Width;
        }

        public override int GetMaxWidth(PawnTable table) {
            return Width;
        }
    }
}
