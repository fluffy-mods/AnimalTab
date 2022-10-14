// PawnColumnWorker_Wool.cs
// Copyright Karel Kroeze, 2017-2017

using Verse;

namespace AnimalTab {
    public class PawnColumnWorker_Wool: PawnColumnWorker_Progress {
        public override bool ShouldDraw(Pawn pawn) {
            return pawn.Shearable();
        }

        public override float GetProgress(Pawn pawn) {
            return pawn.CompShearable().Fullness;
        }

        public override string GetTip(Pawn pawn) {
            return "AnimalTab.GatherableTip".Translate(pawn.CompShearable().Props.woolDef.LabelCap, pawn.CompShearable().Props.woolAmount);
        }
    }
}
