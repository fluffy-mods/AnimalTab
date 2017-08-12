// PawnColumnWorker_AnimalTabLabel.cs
// Copyright Karel Kroeze, 2017-2017

using RimWorld;
using UnityEngine;
using Verse;

namespace AnimalTab
{
    public class PawnColumnWorker_AnimalTabLabel : PawnColumnWorker_Label
    {
        public override void DoCell( Rect rect, Pawn pawn, PawnTable table )
        {
            var interacting = Mouse.IsOver( rect ) && !pawn.Name.Numerical;

            // intercept interactions before base has a chance to do so
            if ( interacting
                 && Event.current.control
                 && Event.current.type == EventType.MouseDown
                 && Event.current.button == 0 )
            {
                Find.WindowStack.Add( new Dialog_RenameAnimal( pawn ) );
                return;
            }
            
            base.DoCell( rect, pawn, table );

            // replace tooltip
            if ( interacting )
            {
                TooltipHandler.ClearTooltipsFrom( rect );

                TipSignal tooltip = pawn.GetTooltip();
                tooltip.text = "ClickToJumpTo".Translate() + "\n" + "AnimalTab.CtrlClickToRename".Translate() + "\n\n" + tooltip.text;
                TooltipHandler.TipRegion( rect, tooltip.text );
            }
        }
    }
}