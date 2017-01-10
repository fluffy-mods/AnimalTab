using RimWorld;
using UnityEngine;
using Verse;

namespace Fluffy
{
    public class Filter_Pregnant : Filter
    {
        #region Properties

        public override string Label => "pregnant";

        public override Texture2D[] Textures => new[]
                                                {
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/pregnant_large" ),
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/not_pregnant_large" ),
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/all_large" )
                                                };

        protected internal override FilterType State { get; set; } = FilterType.None;

        #endregion Properties

        #region Methods

        public override bool IsAllowed( Pawn p )
        {
            bool pregnant = p.IsPregnant();
            if ( State == FilterType.None )
                return true;
            if ( State == FilterType.True && pregnant )
                return true;
            if ( State == FilterType.False && !pregnant )
                return true;
            return false;
        }

        #endregion Methods
    }
}