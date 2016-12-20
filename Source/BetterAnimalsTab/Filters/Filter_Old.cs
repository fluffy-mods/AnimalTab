using RimWorld;
using UnityEngine;
using Verse;

namespace Fluffy
{
    public class Filter_Old : Filter
    {
        #region Properties

        public override string Label => "old";

        public override Texture2D[] Textures => new[]
                                                {
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/old_large" ),
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/not_old_large" ),
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/all_large" )
                                                };

        protected internal override FilterType State { get; set; } = FilterType.None;

        #endregion Properties

        #region Methods

        public override bool IsAllowed( Pawn p )
        {
            bool old = p.ageTracker.AgeBiologicalYearsFloat > p.RaceProps.lifeExpectancy * .9;
            if ( State == FilterType.None )
                return true;
            if ( State == FilterType.True && old )
                return true;
            if ( State == FilterType.False && !old )
                return true;
            return false;
        }

        #endregion Methods
    }
}