using Verse;

namespace AnimalTab {
    public class FilterWorker_Milkable: FilterWorker {
        public override bool Allows(Pawn pawn) {
            if (State == FilterState.Inactive) {
                return true;
            }

            bool milkable = pawn.Milkable();

            if (State == FilterState.Inclusive && milkable) {
                return true;
            }

            if (State == FilterState.Exclusive && !milkable) {
                return true;
            }

            return false;
        }
    }
}
