using Verse;

namespace AnimalTab
{
    public class FilterWorker_Gender : FilterWorker
    {
        public Gender GenderState
        {
            get
            {
                switch ( State )
                {
                    case FilterState.Inclusive:
                        return Gender.Female;
                    case FilterState.Exclusive:
                        return Gender.Male;
                    default:
                        return Gender.None;
                }
            }
        }

        public override bool Allows( Pawn pawn )
        {
            if ( GenderState == Gender.None )
                return true;
            return pawn.gender == GenderState;
        }
    }
}