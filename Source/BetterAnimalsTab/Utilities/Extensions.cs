// Extensions.cs
// Copyright Karel Kroeze, 2017-2017

using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace AnimalTab {
    public static class Extensions {
        private static readonly Dictionary<PawnKindDef, bool> _milkablePawnkinds = new Dictionary<PawnKindDef, bool>();
        private static readonly Dictionary<Pawn, CompMilkable> _milkableComps = new Dictionary<Pawn, CompMilkable>();
        private static readonly Dictionary<PawnKindDef, bool> _shearablePawnkinds = new Dictionary<PawnKindDef, bool>();
        private static readonly Dictionary<Pawn, CompShearable> _shearableComps = new Dictionary<Pawn, CompShearable>();

        private static MethodInfo _milkableCompActiveMethodInfo;
        private static MethodInfo _shearableCompActiveMethodInfo;

        private static bool _birdsAndBeesChecked;
        private static bool _birdsAndBeesActive;
        public static PawnCapacityDef PawnCapacityDef_Reproductive;

        public static bool BirdsAndBeesActive {
            get {
                if (!_birdsAndBeesChecked) {
                    PawnCapacityDef_Reproductive = DefDatabase<PawnCapacityDef>.GetNamedSilentFail("Reproduction");
                    _birdsAndBeesActive = PawnCapacityDef_Reproductive != null;
                    _birdsAndBeesChecked = true;

                    Logger.Message("BirdsAndBees detected, adding fertility capacityDef to fertility filter.");
                }
                return _birdsAndBeesActive;
            }
        }

        public static bool Milkable(this PawnKindDef pawnkind) {
            if (_milkablePawnkinds.TryGetValue(pawnkind, out bool milkable)) {
                return milkable;
            }

            milkable = pawnkind.race.GetCompProperties<CompProperties_Milkable>() != null;
            _milkablePawnkinds[pawnkind] = milkable;
            return milkable;
        }

        public static bool Shearable(this PawnKindDef pawnkind) {
            if (_shearablePawnkinds.TryGetValue(pawnkind, out bool shearable)) {
                return shearable;
            }

            shearable = pawnkind.race.GetCompProperties<CompProperties_Shearable>() != null;
            _shearablePawnkinds[pawnkind] = shearable;
            return shearable;
        }

        private static bool _milkableCompActive(Pawn pawn) {
            if (_milkableCompActiveMethodInfo == null) {
                _milkableCompActiveMethodInfo = AccessTools.Property(typeof(CompMilkable), "Active")
                    .GetGetMethod(true);
                if (_milkableCompActiveMethodInfo == null) {
                    throw new Exception("Could not find CompMilkable.Active property");
                }
            }
            if (pawn == null) {
                throw new ArgumentNullException(nameof(pawn));
            }

            CompMilkable comp = pawn.CompMilkable();
            if (comp == null) {
                return false;
            }

            return (bool) _milkableCompActiveMethodInfo.Invoke(comp, null);
        }

        private static bool _shearableCompActive(Pawn pawn) {
            if (_shearableCompActiveMethodInfo == null) {
                _shearableCompActiveMethodInfo = AccessTools.Property(typeof(CompShearable), "Active")
                    .GetGetMethod(true);
                if (_shearableCompActiveMethodInfo == null) {
                    throw new Exception("Could not find CompShearable.Active property");
                }
            }
            if (pawn == null) {
                throw new ArgumentNullException(nameof(pawn));
            }

            CompShearable comp = pawn.CompShearable();
            if (comp == null) {
                return false;
            }

            return (bool) _shearableCompActiveMethodInfo.Invoke(comp, null);
        }

        public static bool Pregnant(this Pawn pawn) {
            // get hediff
            Hediff _hediff = pawn.health.hediffSet.GetFirstHediffOfDef( HediffDefOf.Pregnant );

            // if pregnant, and pregnancy is far enough advanced to be visible
            if (_hediff?.Visible ?? false) {
                return true;
            }

            // not (visibly) pregnant.
            return false;
        }


        public static bool Reproductive(this Pawn pawn) {
            bool reproductive = pawn.ageTracker.CurLifeStage.reproductive;

            if (reproductive && BirdsAndBeesActive) {
                return pawn.health.capacities.CapableOf(PawnCapacityDef_Reproductive);
            }

            return reproductive;
        }

        public static IEnumerable<T> Values<D, T>(this DefMap<D, T> map) where D : Def, new() where T : new() {
            List<T> values = new List<T>();
            for (int i = 0; i < map.Count; i++) {
                values.Add(map[i]);
            }

            return values;
        }

        public static bool Milkable(this Pawn pawn) {
            return pawn.kindDef.Milkable() && _milkableCompActive(pawn);
        }

        public static bool Shearable(this Pawn pawn) {
            return pawn.kindDef.Shearable() && _shearableCompActive(pawn);
        }

        public static string ToStringList(this List<string> list, string and) {
            string str = "";
            int n = list.Count;
            for (int i = 0; i < n; i++) {
                str += list[i];
                if (i < n - 2) {
                    str += "AnimalTab.Comma".Translate();
                }

                if (i == n - 2) {
                    str += and;
                }
            }
            return str;
        }

        public static CompMilkable CompMilkable(this Pawn pawn) {
            if (_milkableComps.TryGetValue(pawn, out CompMilkable comp)) {
                return comp;
            }

            comp = pawn.TryGetComp<CompMilkable>();
            _milkableComps[pawn] = comp;
            return comp;
        }

        public static CompShearable CompShearable(this Pawn pawn) {
            if (_shearableComps.TryGetValue(pawn, out CompShearable comp)) {
                return comp;
            }

            comp = pawn.TryGetComp<CompShearable>();
            _shearableComps[pawn] = comp;
            return comp;
        }
    }
}
