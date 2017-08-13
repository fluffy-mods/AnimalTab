// Extensions.cs
// Copyright Karel Kroeze, 2017-2017

using System;
using System.Collections.Generic;
using System.Reflection;
using Harmony;
using RimWorld;
using Verse;

namespace AnimalTab
{
    public static class Extensions
    {
        private static Dictionary<PawnKindDef, bool> _milkablePawnkinds = new Dictionary<PawnKindDef, bool>();
        private static Dictionary<Pawn, CompMilkable> _milkableComps = new Dictionary<Pawn, CompMilkable>();
        private static Dictionary<PawnKindDef, bool> _shearablePawnkinds = new Dictionary<PawnKindDef, bool>();
        private static Dictionary<Pawn, CompShearable> _shearableComps = new Dictionary<Pawn, CompShearable>();

        public static bool Milkable( this PawnKindDef pawnkind )
        {
            bool milkable;
            if ( _milkablePawnkinds.TryGetValue( pawnkind, out milkable ) )
                return milkable;

            milkable = pawnkind.race.GetCompProperties<CompProperties_Milkable>() != null;
            _milkablePawnkinds[pawnkind] = milkable;
            return milkable;
        }

        public static bool Shearable( this PawnKindDef pawnkind )

        {
            bool shearable;
            if (_shearablePawnkinds.TryGetValue(pawnkind, out shearable))
                return shearable;

            shearable = pawnkind.race.GetCompProperties<CompProperties_Shearable>() != null;
            _shearablePawnkinds[pawnkind] = shearable;
            return shearable;
        }

        private static MethodInfo _milkableCompActiveMethodInfo;
        private static MethodInfo _shearableCompActiveMethodInfo;

        private static bool _milkableCompActive( Pawn pawn )
        {
            if ( _milkableCompActiveMethodInfo == null )
            {
                _milkableCompActiveMethodInfo = AccessTools.Property( typeof( CompMilkable ), "Active" )
                    .GetGetMethod( true );
                if (_milkableCompActiveMethodInfo == null )
                    throw new Exception("Could not find CompMilkable.Active property");
            }
            if ( pawn == null )
                throw new ArgumentNullException( nameof( pawn ) );
            var comp = pawn.CompMilkable();
            if ( comp == null )
                return false;
            return (bool) _milkableCompActiveMethodInfo.Invoke( comp, null );
        }

        private static bool _shearableCompActive(Pawn pawn)
        {
            if (_shearableCompActiveMethodInfo == null)
            {
                _shearableCompActiveMethodInfo = AccessTools.Property(typeof(CompShearable), "Active")
                    .GetGetMethod(true);
                if (_shearableCompActiveMethodInfo == null)
                    throw new Exception("Could not find CompShearable.Active property");
            }
            if (pawn == null)
                throw new ArgumentNullException(nameof(pawn));
            var comp = pawn.CompShearable();
            if (comp == null)
                return false;
            return (bool)_shearableCompActiveMethodInfo.Invoke(comp, null);
        }

        public static bool Milkable( this Pawn pawn )
        {
            return pawn.kindDef.Milkable() && _milkableCompActive(pawn);
        }

        public static bool Shearable(this Pawn pawn)
        {
            return pawn.kindDef.Shearable() && _shearableCompActive(pawn);
        }

        public static CompMilkable CompMilkable( this Pawn pawn )
        {
            CompMilkable comp;
            if ( _milkableComps.TryGetValue( pawn, out comp ) )
                return comp;

            comp = pawn.TryGetComp<CompMilkable>();
            _milkableComps[pawn] = comp;
            return comp;
        }

        public static CompShearable CompShearable(this Pawn pawn)
        {
            CompShearable comp;
            if (_shearableComps.TryGetValue(pawn, out comp))
                return comp;

            comp = pawn.TryGetComp<CompShearable>();
            _shearableComps[pawn] = comp;
            return comp;
        }
    }
}