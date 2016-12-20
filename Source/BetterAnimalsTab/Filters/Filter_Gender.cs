using RimWorld;
using UnityEngine;
using Verse;

namespace Fluffy
{
    public class Filter_Gender : Filter
    {
        #region Properties

        public override string Label => "gender";

        public override Texture2D[] Textures => new[]
                                                {
                                                    ContentFinder<Texture2D>.Get( "UI/Gender/female" ),
                                                    ContentFinder<Texture2D>.Get( "UI/Gender/male" ),
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/all_large" )
                                                };

        protected internal override FilterType State { get; set; } = FilterType.None;

        #endregion Properties

        #region Methods

        public override bool IsAllowed( Pawn p )
        {
            if ( State == FilterType.None )
                return true;
            if ( State == FilterType.False && p.gender == Gender.Male )
                return true;
            if ( State == FilterType.True && p.gender == Gender.Female )
                return true;
            return false;
        }

        #endregion Methods
    }
}