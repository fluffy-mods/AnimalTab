// PawnColumnWorker_Trainable.cs
// Copyright Karel Kroeze, 2017-2017

using RimWorld;
using UnityEngine;
using Verse;

namespace AnimalTab
{
    public class PawnColumnWorker_Trainable : RimWorld.PawnColumnWorker_Trainable
    {
        public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
        {
            if (pawn.training == null)
                return;

            AcceptanceReport canTrain = pawn.training.CanAssignToTrain( def.trainable, out bool visible);
            if (!visible )
                return;

            // basic stuff
            Rect checkboxRect = Utilities.GetCheckboxRect(rect);
            if ( !canTrain.Accepted )
            {
                Utilities.DoTrainableTooltip( checkboxRect, pawn, def.trainable, canTrain );
                return;
            }

            GUI.DrawTexture(checkboxRect, Resources.Background_Dark);

            var steps = Utilities.GetTrainingProgress(pawn, def.trainable);
            var completed = pawn.training.HasLearned( def.trainable );
            var wanted = pawn.training.GetWanted(def.trainable);
            Utilities.DoTrainableTooltip( checkboxRect, pawn, def.trainable, canTrain, wanted, completed, steps );


            if ( wanted )
            {
                Utilities.DrawCheckColoured( checkboxRect, completed ? Color.white : Color.green );
                if ( steps.min < steps.max )
                    Utilities.DrawTrainingProgress( checkboxRect, pawn, def.trainable, completed ? Color.white : Color.green );
            }
            // not wanted anymore 
            if ( !wanted )
            {
                if ( completed )
                    Utilities.DrawCheckColoured( checkboxRect, Color.grey );
                if ( steps.min > 0 && steps.min < steps.max )
                    Utilities.DrawTrainingProgress( checkboxRect, pawn, def.trainable, Color.grey );
            }

            // interaction
            Widgets.DrawHighlightIfMouseover(checkboxRect);
            var button = Widgets.ButtonInvisibleDraggable(checkboxRect );
            if ( button == Widgets.DraggableResult.Pressed )
            {
                pawn.training.SetWantedRecursive( def.trainable, !wanted );
            }

            if ( button == Widgets.DraggableResult.Dragged )
            {
                pawn.training.SetWantedRecursive( def.trainable, !wanted );
                Utilities.CheckboxPainting      = true;
                Utilities.CheckboxPaintingState = !wanted;
            }

            if ( Mouse.IsOver(checkboxRect ) && Utilities.CheckboxPainting && Input.GetMouseButton( 0 ) 
               && wanted != Utilities.CheckboxPaintingState )
            {
                pawn.training.SetWantedRecursive( def.trainable, Utilities.CheckboxPaintingState );
            }
            if ( Widgets.ButtonInvisible( checkboxRect ) )
                pawn.training.SetWantedRecursive( def.trainable, !wanted );
        }
    }
}