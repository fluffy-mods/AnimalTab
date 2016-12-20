using RimWorld;
using UnityEngine;
using Verse;

namespace Fluffy
{
    public class Filter_Milkable : Filter
    {
        #region Properties

        public override string Label => "milkable";

        public override Texture2D[] Textures => new[]
                                                {
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/milkable_large" ),
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/not_milkable_large" ),
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/all_large" )
                                                };

        protected internal override FilterType State { get; set; } = FilterType.None;

        #endregion Properties

        #region Methods

        public override bool IsAllowed( Pawn p )
        {
            bool milkable = p.ageTracker.CurLifeStage.milkable && p.GetComp<CompMilkable>() != null &&
                            p.gender == Gender.Female;
            if ( State == FilterType.None )
                return true;
            if ( State == FilterType.True && milkable )
                return true;
            if ( State == FilterType.False && !milkable )
                return true;
            return false;
        }

        #endregion Methods
    }
}