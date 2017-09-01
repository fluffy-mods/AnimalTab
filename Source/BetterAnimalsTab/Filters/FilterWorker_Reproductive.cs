// FilterWorker_Reproductive.cs
// Copyright Karel Kroeze, 2017-2017

using Verse;

namespace AnimalTab
{
    public class FilterWorker_Reproductive : FilterWorker
    {
        public override bool Allows( Pawn pawn )
        {
            if (State == FilterState.Inactive)
                return true;

            var reproductive = pawn.Reproductive();

            if (State == FilterState.Inclusive && reproductive)
                return true;
            if (State == FilterState.Exclusive && !reproductive)
                return true;
            return false;
        }
    }
}