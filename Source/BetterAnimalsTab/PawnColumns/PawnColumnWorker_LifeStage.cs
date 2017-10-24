// PawnColumnWorker_AreaSelection.cs
// Copyright Karel Kroeze, 2017-2017

using System.Text;
using Verse;
using RimWorld;

namespace AnimalTab {
    public class PawnColumnWorker_LifeStage : RimWorld.PawnColumnWorker_LifeStage {
        protected override string GetIconTip( Pawn pawn )
        {
            var tip = new StringBuilder();
            tip.AppendLine( base.GetIconTip( pawn ) );
            tip.AppendLine( pawn.ageTracker.AgeTooltipString );
            return tip.ToString();
        }
    }
}