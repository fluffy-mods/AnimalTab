// // Karel Kroeze
// // Filters.cs
// // 2016-06-27

using RimWorld;
using UnityEngine;
using Verse;

namespace Fluffy
{
    public enum FilterType
    {
        True,
        False,
        None
    }

    public abstract class Filter
    {
        protected internal virtual FilterType State { get; set; }

        public virtual string Label => "Filter";

        public abstract Texture2D[] Textures { get; }

        public virtual bool IsAllowed( Pawn p ) { return true; }

        public void Bump()
        {
            int next = (int) State + 1;
            if ( next > 2 )
            {
                State = FilterType.True;
            }
            else if ( next == 1 )
            {
                State = FilterType.False;
            }
            else
            {
                State = FilterType.None;
            }
            Widgets_Filter.Filter = true;
            Widgets_Filter.FilterPossible = true;
        }
    }

    public class Filter_Gender : Filter
    {
        public override string Label => "gender";

        protected internal override FilterType State { get; set; } = FilterType.None;

        public override Texture2D[] Textures => new[]
                                                {
                                                    ContentFinder<Texture2D>.Get( "UI/Gender/female" ),
                                                    ContentFinder<Texture2D>.Get( "UI/Gender/male" ),
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/all_large" )
                                                };

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
    }

    public class Filter_Reproductive : Filter
    {
        public override string Label => "reproductive";

        protected internal override FilterType State { get; set; } = FilterType.None;

        public override Texture2D[] Textures => new[]
                                                {
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/reproductive_large" ),
                                                    ContentFinder<Texture2D>.Get(
                                                                                 "UI/FilterStates/not_reproductive_large" ),
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/all_large" )
                                                };

        public override bool IsAllowed( Pawn p )
        {
            bool repro = p.ageTracker.CurLifeStage.reproductive;
            if ( State == FilterType.None )
                return true;
            if ( State == FilterType.True && repro )
                return true;
            if ( State == FilterType.False && !repro )
                return true;
            return false;
        }
    }

    public class Filter_Training : Filter
    {
        public override string Label => "training";


        protected internal override FilterType State { get; set; } = FilterType.None;

        public override Texture2D[] Textures => new[]
                                                {
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/obedience_large" ),
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/not_obedience_large" ),
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/all_large" )
                                                };

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
    }


    public class Filter_Milkable : Filter
    {
        public override string Label => "milkable";

        protected internal override FilterType State { get; set; } = FilterType.None;

        public override Texture2D[] Textures => new[]
                                                {
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/milkable_large" ),
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/not_milkable_large" ),
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/all_large" )
                                                };

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
    }

    public class Filter_Shearable : Filter
    {
        public override string Label => "shearable";

        protected internal override FilterType State { get; set; } = FilterType.None;

        public override Texture2D[] Textures => new[]
                                                {
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/shearable_large" ),
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/not_shearable_large" ),
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/all_large" )
                                                };

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
    }

    public class Filter_Pregnant : Filter
    {
        public override string Label => "pregnant";

        protected internal override FilterType State { get; set; } = FilterType.None;

        public override Texture2D[] Textures => new[]
                                                {
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/pregnant_large" ),
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/not_pregnant_large" ),
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/all_large" )
                                                };

        public override bool IsAllowed( Pawn p )
        {
            Hediff diff = p.health.hediffSet.GetFirstHediffOfDef( HediffDefOf.Pregnant );
            bool pregnant = diff != null && diff.Visible;
            if ( State == FilterType.None )
                return true;
            if ( State == FilterType.True && pregnant )
                return true;
            if ( State == FilterType.False && !pregnant )
                return true;
            return false;
        }
    }

    public class Filter_Old : Filter
    {
        public override string Label => "old";

        protected internal override FilterType State { get; set; } = FilterType.None;

        public override Texture2D[] Textures => new[]
                                                {
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/old_large" ),
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/not_old_large" ),
                                                    ContentFinder<Texture2D>.Get( "UI/FilterStates/all_large" )
                                                };

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
    }
}
