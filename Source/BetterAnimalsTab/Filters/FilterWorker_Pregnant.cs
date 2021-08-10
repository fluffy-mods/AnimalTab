// FilterWorker_Pregnant.cs
// Copyright Karel Kroeze, 2017-2017

using Verse;

namespace AnimalTab {
    public class FilterWorker_Pregnant: FilterWorker {
        public override bool Allows(Pawn pawn) {
            if (State == FilterState.Inactive) {
                return true;
            }

            bool pregnant = pawn.Pregnant();

            if (State == FilterState.Inclusive && pregnant) {
                return true;
            }

            if (State == FilterState.Exclusive && !pregnant) {
                return true;
            }

            return false;
        }
    }
}
