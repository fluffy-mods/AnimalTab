using RimWorld;
using System.Linq;
using Harmony;
using UnityEngine;
using Verse;

namespace AnimalTab
{
    class PawnColumnWorker_Pregnant : RimWorld.PawnColumnWorker_Pregnant
    {

        public override int Compare(Pawn a, Pawn b)
        {
            var aPregnant = IsPregnant(a);
            var bPregnant = IsPregnant(b);
            if ( aPregnant || bPregnant )
                return aPregnant.CompareTo( bPregnant );

            var eggA = EggProgress( a );
            var eggB = EggProgress( b );
            return eggA.CompareTo( eggB );
        }

        private static bool IsPregnant(Pawn p)
        {
            return p.health.hediffSet.GetFirstHediffOfDef( HediffDefOf.Pregnant )?.Visible ?? false;
        }

        private static float EggProgress(Pawn p)
        {
            var egg = p.AllComps.OfType<CompEggLayer>().FirstOrDefault();
            if (egg != null)
                return Traverse.Create( egg ).Field( "eggProgress" ).GetValue<float>();
            return 0;
        }
        
        protected override Texture2D GetIconFor( Pawn pawn )
        {
            if ( EggProgress( pawn ) > 0 )
                return Resources.Egg;
            return base.GetIconFor( pawn );
        }

        protected override string GetIconTip( Pawn pawn )
        {
            var egg = pawn.AllComps.OfType<CompEggLayer>().FirstOrDefault();
            if ( egg != null && EggProgress( pawn ) > 0 )
                return egg.CompInspectStringExtra();
            return base.GetIconTip( pawn );
        }
    }
}
