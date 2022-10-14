using Verse;

namespace AnimalTab {
    public class FilterWorker_Gender: FilterWorker {
        public Gender GenderState => State switch {
            FilterState.Inclusive => Gender.Female,
            FilterState.Exclusive => Gender.Male,
            _ => Gender.None,
        };

        public override bool Allows(Pawn pawn) {
            if (GenderState == Gender.None) {
                return true;
            }

            return pawn.gender == GenderState;
        }
    }
}
