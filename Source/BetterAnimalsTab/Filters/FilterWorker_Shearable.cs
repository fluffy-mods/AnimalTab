// FilterWorker_Shearable.cs
// Copyright Karel Kroeze, 2017-2017

using Verse;

namespace AnimalTab {
    public class FilterWorker_Shearable: FilterWorker {
        public override bool Allows(Pawn pawn) {
            if (State == FilterState.Inactive) {
                return true;
            }

            bool shearable = pawn.Shearable();

            if (State == FilterState.Inclusive && shearable) {
                return true;
            }

            if (State == FilterState.Exclusive && !shearable) {
                return true;
            }

            return false;
        }
    }
}
