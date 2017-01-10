using RimWorld;
using UnityEngine;
using Verse;

namespace Fluffy
{
    public class Filter_Reproductive : Filter
    {
        #region Properties

        public override string Label => "reproductive";

        public override Texture2D[] Textures => new[]
                                                {
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/reproductive_large" ),
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/not_reproductive_large" ),
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/all_large" )
                                                };

        protected internal override FilterType State { get; set; } = FilterType.None;

        #endregion Properties

        #region Methods

        public override bool IsAllowed( Pawn p )
        {
            bool repro = p.IsReproductive();
            if ( State == FilterType.None )
                return true;
            if ( State == FilterType.True && repro )
                return true;
            if ( State == FilterType.False && !repro )
                return true;
            return false;
        }

        #endregion Methods
    }
}