using System.ComponentModel.Design;
using Verse;

namespace AnimalTab
{
    public class FilterWorker_Age : FilterWorker
    {
        public override bool Allows( Pawn pawn )
        {
            if ( State == FilterState.Inactive )
                return true;

            var old = pawn.ageTracker.AgeBiologicalYearsFloat > pawn.RaceProps.lifeExpectancy * .9;
            if ( State == FilterState.Inclusive && old )
                return true;
            if ( State == FilterState.Exclusive && !old )
                return true;
            return false;
        }
    }
}