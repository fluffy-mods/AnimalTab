// Utilities.cs
// Copyright Karel Kroeze, 2017-2017

using System;
using System.Reflection;
using Harmony;
using RimWorld;
using UnityEngine;
using Verse;
using static AnimalTab.Resources;
using static AnimalTab.Constants;

namespace AnimalTab
{
    public static class Utilities
    {
        public static void DoCheckbox( Rect rect, ref bool value, Func<string> tipGetter = null, bool background = true,
            bool mouseover = true, Texture2D backgroundTexture = null,
            Texture2D checkOn = null, Texture2D checkOff = null )
        {
            // background and hover
            if ( background )
                DrawCheckboxBackground( rect, backgroundTexture );
            if ( mouseover )
                Widgets.DrawHighlightIfMouseover( rect );
            if ( tipGetter != null )
                TooltipHandler.TipRegion( rect, tipGetter, DesignationDefOf.Slaughter.shortHash ^ rect.GetHashCode() );

            // draw textures and click
            if ( value || checkOff != null )
                GUI.DrawTexture( rect, value ? checkOn ?? Widgets.CheckboxOnTex : checkOff );
            if ( Widgets.ButtonInvisible( rect ) )
                value = !value;
        }

        public static void DrawCheckboxBackground( Rect rect, Texture2D background = null )
        {
            GUI.DrawTexture( rect, background ?? CheckboxBackground );
        }

        public static Rect GetCheckboxRect( Rect rect )
        {
            return new Rect( rect.x + ( rect.width - CheckBoxSize ) / 2, rect.y + ( rect.height - CheckBoxSize ) / 2,
                CheckBoxSize, CheckBoxSize );
        }

        public static IntRange GetTrainingProgress( Pawn pawn, TrainableDef trainable )
        {
            var cur = Traverse.Create( pawn.training ).Method( "GetSteps", trainable ).GetValue<int>();
            var max = trainable.steps;
            return new IntRange( cur, max );
        }

        public static void DrawCheckColoured( Rect rect, Color color )
        {
            var curColor = GUI.color;
            GUI.color = color;
            GUI.DrawTexture( rect, CheckOnWhite );
            GUI.color = curColor;
        }

        public static void DrawTrainingProgress( Rect rect, Pawn pawn, TrainableDef trainable, Color color )
        {
            var steps = GetTrainingProgress( pawn, trainable );
            var progressRect = new Rect( rect.xMin, rect.yMax - rect.height / 5f,
                rect.width / steps.max * steps.min, rect.height / 5f );

            Widgets.DrawBoxSolid( progressRect, color );
        }

        public static void DoTrainableTooltip( Rect rect, Pawn pawn, TrainableDef td, AcceptanceReport canTrain )
        {
            Traverse.Create( typeof( TrainingCardUtility ) )
                .Method( "DoTrainableTooltip", rect, pawn, td, canTrain )
                .GetValue(); // invoke
        }



        public static void DoTrainableTooltip( Rect rect, Pawn pawn, TrainableDef td, AcceptanceReport canTrain,
            bool wanted, bool completed, IntRange steps )
        {
            // copy pasta from TrainingCardUtility.DoTrainableTooltip
            TooltipHandler.TipRegion( rect, () =>
            {
                string text = td.LabelCap + "\n\n" + td.description;
                if ( !canTrain.Accepted )
                {
                    text = text + "\n\n" + canTrain.Reason;
                }
                else if ( !td.prerequisites.NullOrEmpty<TrainableDef>() )
                {
                    text += "\n";
                    for ( int i = 0; i < td.prerequisites.Count; i++ )
                    {
                        if ( !pawn.training.HasLearned( td.prerequisites[i] ) )
                        {
                            text = text + "\n" + "TrainingNeedsPrerequisite".Translate( td.prerequisites[i].LabelCap );
                        }
                    }
                }
                if ( completed && steps.min == steps.max )
                {
                    text += "\n" + "Fluffy.AnimalTab.XHasMasteredY".Translate( pawn.Name.ToStringShort, td.LabelCap );
                }
                if ( wanted && !completed )
                {
                    text += "\n" + "Fluffy.AnimalTab.XHasLearnedYOutOfZ".Translate( pawn.Name.ToStringShort, steps.min,
                                steps.max );
                }
                if ( completed && steps.min < steps.max )
                {
                    text += "\n" + "Fluffy.AnimalTab.XHasForgottenYOutOfZ".Translate( pawn.Name.ToStringShort,
                                steps.max - steps.min, steps.max );
                }
                if ( wanted )
                {
                    text += "\n" + "Fluffy.AnimalTab.XIsDesignatedTrainY".Translate( pawn.Name.ToStringShort,
                                td.LabelCap );
                }
                else if ( completed || steps.min > 0 )
                {
                    text += "\n" + "Fluffy.AnimalTab.XIsNotDesignatedTrainY".Translate( pawn.Name.ToStringShort,
                                td.LabelCap );
                }

                return text;
            }, (int) ( rect.y * 612 + rect.x ) );
        }
    }
}