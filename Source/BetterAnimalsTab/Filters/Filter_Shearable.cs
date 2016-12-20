using RimWorld;
using UnityEngine;
using Verse;

namespace Fluffy
{
    public class Filter_Shearable : Filter
    {
        #region Properties

        public override string Label => "shearable";

        public override Texture2D[] Textures => new[]
                                                {
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/shearable_large" ),
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/not_shearable_large" ),
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/all_large" )
                                                };

        protected internal override FilterType State { get; set; } = FilterType.None;

        #endregion Properties

        #region Methods

        public override bool IsAllowed( Pawn p )
        {
            bool shearable = p.ageTracker.CurLifeStage.shearable && p.GetComp<CompShearable>() != null;
            if ( State == FilterType.None )
                return true;
            if ( State == FilterType.True && shearable )
                return true;
            if ( State == FilterType.False && !shearable )
                return true;
            return false;
        }

        #endregion Methods
    }
}