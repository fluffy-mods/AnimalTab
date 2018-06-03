// FilterWorker_CaravanCarryingCapacity.cs
// Copyright Karel Kroeze, 2018-2018

using RimWorld;
using Verse;

namespace AnimalTab
{
    public class FilterWorker_PackAnimal: FilterWorker
    {
        public override bool Allows( Pawn pawn )
        {
            var packAnimal = pawn.RaceProps.packAnimal;
            switch ( State )
            {
                case FilterState.Inactive:
                case FilterState.Exclusive when !packAnimal:
                case FilterState.Inclusive when packAnimal:
                    return true;
                default:
                    return false;
            }
        }
    }
}