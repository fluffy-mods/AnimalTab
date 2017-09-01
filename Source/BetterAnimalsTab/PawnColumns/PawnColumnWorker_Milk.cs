// PawnColumnWorker_Wool.cs
// Copyright Karel Kroeze, 2017-2017

using System.Collections.Generic;
using RimWorld;
using Verse;

namespace AnimalTab
{
    public class PawnColumnWorker_Milk : PawnColumnWorker_Progress
    {
        public override bool ShouldDraw( Pawn pawn )
        {
            return pawn.Milkable();
        }

        public override float GetProgress( Pawn pawn )
        {
            return pawn.CompMilkable().Fullness;
        }

        public override string GetTip(Pawn pawn)
        {
            return "AnimalTab.GatherableTip".Translate( pawn.CompMilkable().Props.milkDef.LabelCap, pawn.CompMilkable().Props.milkAmount );
        }
    }
}