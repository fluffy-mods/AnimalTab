// PawnColumnWorker_Meat.cs
// Copyright Karel Kroeze, 2017-2017

using RimWorld;
using UnityEngine;
using Verse;

namespace AnimalTab {
    public class PawnColumnWorker_Meat: PawnColumnWorker {
        public override void DoCell(Rect rect, Pawn pawn, PawnTable table) {
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect, ExpectedAmount(pawn).ToString());
            Text.Anchor = TextAnchor.UpperLeft;

            TooltipHandler.TipRegion(rect,
                "AnimalTab.GatherableTip".Translate(pawn.kindDef.race.race.meatLabel, ExpectedAmount(pawn)));
        }

        public override int GetMinWidth(PawnTable table) {
            return Constants.TextCellWidth;
        }

        public int ExpectedAmount(Pawn pawn) {
            return (int) (StatDefOf.MeatAmount.defaultBaseValue * pawn.BodySize);
        }

        public override int Compare(Pawn a, Pawn b) {
            return ExpectedAmount(a).CompareTo(ExpectedAmount(b));
        }
    }
}
