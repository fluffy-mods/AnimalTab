using RimWorld;
using UnityEngine;
using Verse;

namespace Fluffy
{
    public class Filter_Training : Filter
    {
        #region Properties

        public override string Label => "training";

        public override Texture2D[] Textures => new[]
                                                {
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/obedience_large" ),
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/not_obedience_large" ),
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/all_large" )
                                                };

        protected internal override FilterType State { get; set; } = FilterType.None;

        #endregion Properties

        #region Methods

        public override bool IsAllowed( Pawn p )
        {
            bool tamed = p.training.IsCompleted( TrainableDefOf.Obedience );
            if ( State == FilterType.None )
                return true;
            if ( State == FilterType.True && tamed )
                return true;
            if ( State == FilterType.False && !tamed )
                return true;
            return false;
        }

        #endregion Methods
    }
}