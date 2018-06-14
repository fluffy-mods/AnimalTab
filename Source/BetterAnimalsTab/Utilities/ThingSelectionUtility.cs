// ThingSelectionUtility.cs
// Copyright Karel Kroeze, 2018-2018

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace AnimalTab
{
    public class ThingSelectionUtility
    {
        private static List<Pawn> AllTameAnimalsInOrder
        {
            get
            {
                return Find.Maps.SelectMany( map => map.mapPawns.AllPawnsSpawned )
                    .Where( pawn => pawn.Faction == Faction.OfPlayer &&
                                    pawn.RaceProps.Animal &&
                                    RimWorld.ThingSelectionUtility.SelectableByHotkey( pawn ) )
                    .OrderBy( pawn => pawn.Map?.uniqueID ?? pawn.GetCaravan()?.ID + 500 ?? -1 )
                    .ThenBy( pawn => pawn.kindDef.label )
                    .ThenBy( pawn => pawn.Label )
                    .ToList();
            }
        }
        
        private static bool IsSelected( Pawn pawn )
        {
            bool worldRendered = WorldRendererUtility.WorldRenderedNow;
            
            return 
                // selected on colony map
                !worldRendered && Find.Selector.IsSelected( pawn )
                
                // or part of selected caravan
                || worldRendered && pawn.IsCaravanMember() && Find.WorldSelector.IsSelected( pawn.GetCaravan() );
        }
        
        public static void SelectNextTameAnimal()
        {
            var animals = AllTameAnimalsInOrder;
            var index = -1;
            LogDebug( animals, false );
            for ( int i = animals.Count - 1; i >= 0; i-- )
                // count down because there may be multiple pawns in a caravan, and we cannot select them individually
                // if we were to loop up, and there was more than one pawn in a caravan, we'd select the same caravan forever.
            {
                if ( IsSelected( animals[i] ) )
                {
                    index = i;
                    break;
                }
            }

            index = ( index + 1 ) % animals.Count;
            CameraJumper.TryJumpAndSelect( animals[index] );
        }
        
        public static void SelectPreviousTameAnimal()
        {
            var animals = AllTameAnimalsInOrder;
            var index = 1;
            LogDebug( animals, true );
            for ( int i = 0; i < animals.Count; i++ )
                // count up because there may be multiple pawns in a caravan, and we cannot select them individually
                // if we were to loop down, and there was more than one pawn in a caravan, we'd select the same caravan forever.
            {
                if ( IsSelected( animals[i] ) )
                {
                    index = i;
                    break;
                }
            }

            index = GenMath.PositiveMod( index - 1, animals.Count );
            CameraJumper.TryJumpAndSelect( animals[index] );
        }

        [Conditional( "DEBUG" )]
        public static void LogDebug( List<Pawn> animals, bool up )
        {
            if ( up )
            {
                for ( int i = 0; i < animals.Count; i++ )
                {
                    var animal = animals[i];
                    Log.Message( i + " :: " + animal.Label + " :: " +
                                 ( IsSelected( animal ) ? "SELECTED" : "NOT SELECTED" ) );
                }
            }
            else
            {
                for ( int i = animals.Count - 1; i >= 0; i-- )
                {
                    var animal = animals[i];
                    Log.Message( i + " :: " + animal.Label + " :: " +
                                 ( IsSelected( animal ) ? "SELECTED" : "NOT SELECTED" ) );
                }
            }
        }
    }
}