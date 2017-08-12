// MainTabWindow_Animals.cs
// Copyright Karel Kroeze, 2017-2017

using RimWorld;

namespace AnimalTab
{
    public class MainTabWindow_Animals : RimWorld.MainTabWindow_Animals
    {
        protected override PawnTableDef PawnTableDef => PawnTableDefOf.Animals;
    }
}