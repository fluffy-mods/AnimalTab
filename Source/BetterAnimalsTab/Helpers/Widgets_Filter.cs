// // Karel Kroeze
// // Widgets_Filter.cs
// // 2016-06-27

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Fluffy
{
    public static class Widgets_Filter
    {
        public static List<PawnKindDef> FilterPawnKind =
            Find.MapPawns.PawnsInFaction( Faction.OfPlayer ).Where( x => x.RaceProps.Animal )
                .Select( x => x.kindDef ).Distinct().ToList();

        public static readonly List<Filter> Filters = new List<Filter>
                                             {
                                                 new Filter_Gender(),
                                                 new Filter_Training(),
                                                 new Filter_Reproductive(),
                                                 new Filter_Pregnant(),
                                                 new Filter_Old(),
                                                 new Filter_Milkable(),
                                                 new Filter_Shearable()
                                             };

        public static bool Filter;

        public static bool FilterPossible;

        public static void TogglePawnKindFilter( PawnKindDef pawnKind, bool remove = true )
        {
            if ( remove )
            {
                FilterPawnKind.Remove( pawnKind );
            }
            else
            {
                if ( FilterPawnKind == null )
                    ResetPawnKindFilter();
                // ReSharper disable once PossibleNullReferenceException
                FilterPawnKind.Add( pawnKind );
            }
            if ( !Filter )
                EnableFilter();
            FilterPossible = true;
        }

        public static void EnableFilter() { Filter = true; }

        public static void DisableFilter() { Filter = false; }

        public static void ResetFilter()
        {
            ResetPawnKindFilter();
            foreach ( Filter filter in Filters )
            {
                filter.State = FilterType.None;
            }
            FilterPossible = false;
        }

        public static void ResetPawnKindFilter()
        {
            FilterPawnKind = Find.MapPawns.PawnsInFaction( Faction.OfPlayer ).Where( x => x.RaceProps.Animal )
                                 .Select( x => x.kindDef ).Distinct().ToList();
        }

        public static void FilterAllPawnKinds() { FilterPawnKind = new List<PawnKindDef>(); }

        public static void QuickFilterPawnKind( PawnKindDef def )
        {
            ResetFilter();
            FilterAllPawnKinds();
            FilterPawnKind.Add( def );
            EnableFilter();
        }

        public static List<Pawn> FilterAnimals( List<Pawn> pawns )
        {
            pawns = pawns.Where( p => FilterPawnKind.Contains( p.kindDef ) &&
                                      Filters.All( f => f.IsAllowed( p ) )
                ).ToList();
            return pawns;
        }
    }
}
