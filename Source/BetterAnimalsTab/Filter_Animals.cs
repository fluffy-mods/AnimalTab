using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace Fluffy
{
    public static class Filter_Animals
    {
        public static List<PawnKindDef> filterPawnKind = new List<PawnKindDef>();

        public enum filterType
        {
            True,
            False,
            None
        }

        public static filterType bump(filterType current)
        {
            int next = (int)current + 1;
            if (next > 2)
            {
                return filterType.True;
            }
            else
            {
                return (filterType)next;
            }
        }

        public static filterType filterReproductive = filterType.None;

        public static filterType filterTamed = filterType.None;

        public static filterType filterGender = filterType.None;

        public static bool filter = false;

        public static void togglePawnKindFilter(PawnKindDef pawnKind, bool remove = true)
        {
            if (remove)
            {
                filterPawnKind.Remove(pawnKind);
            }
            else
            {
                if (filterPawnKind == null) filterPawnKind = new List<PawnKindDef>();
                filterPawnKind.Add(pawnKind);
            }
            if (!filter) enableFilter();
        }

        public static void enableFilter()
        {
            filter = true;
        }

        public static void disableFilter()
        {
            filter = false;
        }

        public static void resetFilter()
        {
            filterPawnKind = new List<PawnKindDef>();
            filterGender = filterType.None;
            filterTamed = filterType.None;
            filterReproductive = filterType.None;
        }

        public static List<Pawn> FilterAnimals(List<Pawn> pawns)
        {
            pawns = pawns.Where(p => filterPawnKind.Contains(p.kindDef) &&
                                     genderFilter(p.gender) &&
                                     reproductiveFilter(p.ageTracker.CurLifeStage.reproductive) &&
                                     tamedFilter(p.training.IsCompleted(TrainableDefOf.Obedience))).ToList();
            return pawns;
        }

        public static bool genderFilter(Gender gender)
        {
            if (filterGender == filterType.None) return true;
            if (filterGender == filterType.True && gender == Gender.Female) return true;
            if (filterGender == filterType.False && gender == Gender.Male) return true;
            return false;
        }

        public static bool reproductiveFilter(bool repro)
        {
            if (filterReproductive == filterType.None) return true;
            if (filterReproductive == filterType.True && repro) return true;
            if (filterReproductive == filterType.False && !repro) return true;
            return false;
        }

        public static bool tamedFilter(bool tamed)
        {
            if (filterTamed == filterType.None) return true;
            if (filterTamed == filterType.True && tamed) return true;
            if (filterTamed == filterType.False && !tamed) return true;
            return false;
        }
    }
}
