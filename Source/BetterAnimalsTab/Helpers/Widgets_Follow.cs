using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace BetterAnimalsTab
{
    [StaticConstructorOnStartup]
    public static class Widgets_Follow
    {
        #region Fields

        public static Texture2D FollowDraftIcon = UnityEngine.Resources.Load<Texture2D>( "Textures/UI/Icons/Animal/FollowDrafted" );
        public static Texture2D FollowHuntIcon = UnityEngine.Resources.Load<Texture2D>( "Textures/UI/Icons/Animal/FollowFieldwork" );

        #endregion Fields
        
        #region Methods

        public static void ToggleAllFollowsDrafted( List<Pawn> animals )
        {
            int count = animals.Count();
            bool[] following = animals.Select( a => a.playerSettings.followDrafted ).ToArray();
            bool[] canFollow = animals.Select( a => a.training.IsCompleted( TrainableDefOf.Obedience ) ).ToArray();
            bool anyCanFollow = false;
            bool anyFollowing = false;
            bool all = true;

            for ( int i = 0; i < count; i++ )
            {
                if ( !anyCanFollow && canFollow[i] )
                    anyCanFollow = true;
                if ( !anyFollowing && following[i] )
                    anyFollowing = true;
                if ( all && canFollow[i] && !following[i] )
                    all = false;
            }

            if ( anyCanFollow )
            {
                for ( int i = 0; i < count; i++ )
                {
                    if ( canFollow[i] && !all && !following[i] )
                        animals[i].playerSettings.followDrafted = true;
                    if ( canFollow[i] && all )
                        animals[i].playerSettings.followDrafted = false;
                }
                if ( all )
                    SoundDefOf.CheckboxTurnedOff.PlayOneShotOnCamera();
                else
                    SoundDefOf.CheckboxTurnedOn.PlayOneShotOnCamera();
            }
        }

        public static void ToggleAllFollowsHunter( List<Pawn> animals )
        {
            int count = animals.Count();
            bool[] following = animals.Select( a => a.playerSettings.followFieldwork ).ToArray();
            bool[] canFollow = animals.Select( a => a.training.IsCompleted( TrainableDefOf.Obedience ) ).ToArray();
            bool anyCanFollow = false;
            bool anyFollowing = false;
            bool all = true;

            for ( int i = 0; i < count; i++ )
            {
                if ( !anyCanFollow && canFollow[i] )
                    anyCanFollow = true;
                if ( !anyFollowing && following[i] )
                    anyFollowing = true;
                if ( all && !following[i] )
                    all = false;
            }

            if ( anyCanFollow )
            {
                for ( int i = 0; i < count; i++ )
                {
                    if ( canFollow[i] && !all && !following[i] )
                        animals[i].playerSettings.followFieldwork = true;
                    if ( canFollow[i] && all )
                        animals[i].playerSettings.followFieldwork = false;
                }
                if ( all )
                    SoundDefOf.CheckboxTurnedOff.PlayOneShotOnCamera();
                else
                    SoundDefOf.CheckboxTurnedOn.PlayOneShotOnCamera();
            }
        }

        private static void Initialize()
        {
            
        }

        #endregion Methods
    }
}
