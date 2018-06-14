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

            bool visible;
            AcceptanceReport canTrain = pawn.training.CanAssignToTrain( def.trainable, out visible);
            if (!visible )
                return;

            // basic stuff
            Rect checkboxRect = Utilities.GetCheckboxRect(rect);
            Utilities.DoTrainableTooltip(checkboxRect, pawn, def.trainable, canTrain);
            if ( !canTrain.Accepted )
                return;

            GUI.DrawTexture(checkboxRect, Resources.CheckboxBackground);

            // if completed, draw white checkmark
            var completed = pawn.training.HasLearned( def.trainable );
            if ( completed )
                GUI.DrawTexture( checkboxRect, Resources.CheckOnWhite );

            // if in progress, draw green checkmark + progress bar
            var wanted = pawn.training.GetWanted( def.trainable );
            if ( !completed && wanted )
            {
                GUI.DrawTexture( checkboxRect, Widgets.CheckboxOnTex );
                Utilities.DrawTrainingProgress( checkboxRect, pawn, def.trainable );
            }

            // interaction
            if ( !completed )
            {
                Widgets.DrawHighlightIfMouseover(checkboxRect);
                if ( Widgets.ButtonInvisible( checkboxRect ) )
                    pawn.training.SetWantedRecursive( def.trainable, !wanted );
            }
        }
    }
}