using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace Fluffy
{
    public static class Filter_Animals
    {
        public static List<PawnKindDef> filterPawnKind = Find.ListerPawns.PawnsInFaction(Faction.OfColony).Where(x => x.RaceProps.Animal)
                .Select(x => x.kindDef).Distinct().ToList();

        public static List<IFilter> Filters = new List<IFilter>()
        {
            new Filter_Gender(),
            new Filter_Training(),
            new Filter_Reproductive(),
            new Filter_Pregnant(),
            new Filter_Old(),
            new Filter_Milkable(),
            new Filter_Shearable()
        };
        
        public static bool filter = false;

        public static bool filterPossible = false;

        public static void togglePawnKindFilter(PawnKindDef pawnKind, bool remove = true)
        {
            if (remove)
            {
                filterPawnKind.Remove(pawnKind);
            }
            else
            {
                if (filterPawnKind == null) resetPawnKindFilter();
                filterPawnKind.Add(pawnKind);
            }
            if (!filter) enableFilter();
            filterPossible = true;
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
            resetPawnKindFilter();
            foreach (Filter filter in Filters)
            {
                filter.state = FilterType.None;
            }
            filterPossible = false;
        }

        public static void resetPawnKindFilter()
        {
            filterPawnKind = Find.ListerPawns.PawnsInFaction(Faction.OfColony).Where(x => x.RaceProps.Animal)
                .Select(x => x.kindDef).Distinct().ToList();
        }

        public static void filterAllPawnKinds()
        {
            filterPawnKind = new List<PawnKindDef>();
        }

        public static void quickFilterPawnKind(PawnKindDef def)
        {
            resetFilter();
            filterAllPawnKinds();
            filterPawnKind.Add(def);
            enableFilter();
        }

        public static List<Pawn> FilterAnimals(List<Pawn> pawns)
        {
            pawns = pawns.Where(p => filterPawnKind.Contains(p.kindDef) &&
                                     Filters.All(f => f.filter(p))
                                     ).ToList();
            return pawns;
        }
    }
}
