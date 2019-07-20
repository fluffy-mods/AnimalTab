// HandlerUtility.cs
// Copyright Karel Kroeze, 2019-2019

using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using RimWorld;
using Verse;

namespace AnimalTab
{
    public static class HandlerUtility
    {
        private static IEnumerable<HandlerMode> _handlerModes;
        public static IEnumerable<HandlerMode> HandlerModes
        {
            get
            {
                if ( _handlerModes == null )
                    _handlerModes = Enum.GetValues( typeof( HandlerMode ) ).Cast<HandlerMode>();
                return _handlerModes;
            }
        }

        public static string Label( this HandlerMode mode ) => $"Fluffy.AnimalTab.HandlerMode.{mode}".Translate();

        public static IntRange Clamp( this IntRange level, Pawn target )
        {
            var minSkill = TrainableUtility.MinimumHandlingSkill( target );
            return new IntRange( Math.Max( level.min, minSkill ), Math.Max( level.max, minSkill ) );
        }

        public static bool Contains( this IntRange range, int level )
        {
            return level >= range.min && level <= range.max;
        }
        
        public static IEnumerable<Pawn> HandlersOrdered( Map map )
        {
            return map.mapPawns.FreeColonistsSpawned
                      .Where( p => p?.story != null && p.skills != null )
                      .Select( p => new
                       {
                           pawn = p,
                           disabled = HandlingDisabled( p ),
                           assigned = HandlingAssigned( p ),
                           level    = HandlingSkill( p )
                       } )
                      .OrderBy( t => t.disabled )
                      .ThenByDescending( t => t.assigned )
                      .ThenByDescending( t => t.level )
                      .Select( h => h.pawn );
        }

        public static bool HandlingDisabled( Pawn handler )
        {
            return handler.story.WorkTagIsDisabled( WorkTags.Animals ) ||
                   handler.story.WorkTypeIsDisabled( WorkTypeDefOf.Handling );
        }

        public static bool HandlingAssigned( Pawn handler )
        {
            return handler.workSettings.GetPriority( WorkTypeDefOf.Handling ) > 0;
        }

        public static int HandlingSkill( this Pawn handler )
        {
            return (int)handler.skills.AverageOfRelevantSkillsFor( WorkTypeDefOf.Handling );
        }

        public static string HandlerLabel( Pawn handler, int minSkill = 0 )
        {
            string label = handler.Name.ToStringShort + " (" + HandlingSkill( handler );
            if ( HandlingDisabled( handler ) )
                label += "Fluffy.AnimalTab.CanNeverDoHandling".Translate();
            else if ( !HandlingAssigned( handler ) )
                label += "Fluffy.AnimalTab.NotAssignedToHandling".Translate();
            else if ( handler.skills.GetSkill( SkillDefOf.Animals ).Level < minSkill )
                label += "Fluffy.AnimalTab.InsufficientSkill".Translate();
            label += ")";

            return label;
        }
    }
}