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
            return new Rect( rect.x + ( rect.width - CheckBoxSize ) / 2, rect.y + ( rect.height - CheckBoxSize ) / 2, CheckBoxSize, CheckBoxSize );
        }

        private static MethodInfo _getStepsMethodInfo;
        public static int GetTrainingProgress( Pawn pawn, TrainableDef trainable )
        {
            if ( _getStepsMethodInfo == null )
            {
                _getStepsMethodInfo = AccessTools.Method( typeof( Pawn_TrainingTracker ), "GetSteps" );
                if ( _getStepsMethodInfo == null )
                    throw new Exception( "GetSteps not found" );
            }
            return (int) _getStepsMethodInfo.Invoke( pawn.training, new object[] {trainable} );
        }

        public static void DrawTrainingProgress( Rect rect, Pawn pawn, TrainableDef trainable )
        {
            var progressRect = new Rect( rect.xMin, rect.yMax - rect.height / 5f,
                rect.width / trainable.steps * GetTrainingProgress( pawn, trainable ), rect.height / 5f );

            Widgets.DrawBoxSolid( progressRect, Color.green );
        }

        private static MethodInfo _doTrainableTooltipMethodInfo;
        public static void DoTrainableTooltip( Rect rect, Pawn pawn, TrainableDef td, AcceptanceReport canTrain )
        {
            if ( _doTrainableTooltipMethodInfo == null )
            {
                _doTrainableTooltipMethodInfo = AccessTools.Method(typeof(TrainingCardUtility), "DoTrainableTooltip");
                if ( _doTrainableTooltipMethodInfo == null )
                    throw new Exception( "Could not find DoTrainableTooltip()" );
            }

            _doTrainableTooltipMethodInfo.Invoke( null, new object[] {rect, pawn, td, canTrain} );
        }
    }
}