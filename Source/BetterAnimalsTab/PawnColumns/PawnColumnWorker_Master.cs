// PawnColumnWorker_Master.cs
// Copyright Karel Kroeze, 2017-2017

using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace AnimalTab
{
    public class PawnColumnWorker_Master : RimWorld.PawnColumnWorker_Master
    {
        public static IEnumerable<Pawn> ObedientAnimals( PawnTable table )
        {
            return table.PawnsListForReading.Where( p => p.training?.HasLearned( TrainableDefOf.Obedience ) ?? false );
        } 

        protected override void HeaderClicked( Rect headerRect, PawnTable table )
        {
            if ( Event.current.shift )
                DoMassAssignFloatMenu( table );
            else
                base.HeaderClicked( headerRect, table );
        }

        protected override string GetHeaderTip( PawnTable table )
        {
            return base.GetHeaderTip( table ) + "\n" + "AnimalTab.MasterHeaderTip".Translate();
        }

        private void DoMassAssignFloatMenu( PawnTable table )
        {
            var options = new List<FloatMenuOption>();

            if (ObedientAnimals( table ).Any() )
            {
                // none
                options.Add( MassAssignMaster_FloatMenuOption( null, table ) );

                // clever
                options.Add( new FloatMenuOption( "AnimalTab.MassAssignMasterBonded".Translate(),
                    () => MassAssignMasterBonded( table ) ) );

                // loop over pawns
                foreach ( var pawn in Find.CurrentMap.mapPawns.FreeColonistsSpawned )
                    options.Add( MassAssignMaster_FloatMenuOption( pawn, table ) );
            }
            else
            {
                options.Add( new FloatMenuOption( "AnimalTab.NoObedientAnimals".Translate(), null ) );
            }

            Find.WindowStack.Add( new FloatMenu( options ) );
        }

        public static FloatMenuOption MassAssignMaster_FloatMenuOption( Pawn colonist, PawnTable table )
        {
            var animals = ObedientAnimals( table );
            if ( colonist == null )
                return new FloatMenuOption( "AnimalTab.MassAssignMasterNone".Translate(),
                    delegate { MassAssignMaster( null, animals ); } );

            // get number of animals this pawn could be the master of.
            var skill = colonist.skills.GetSkill( SkillDefOf.Animals ).Level;
            var eligibleAnimals =
                animals.Where( p => Mathf.RoundToInt( p.GetStatValue( StatDefOf.MinimumHandlingSkill ) ) < skill );
            Action action = () => MassAssignMaster( colonist, eligibleAnimals );

            return new FloatMenuOption(
                "AnimalTab.MassAssignMaster".Translate( colonist.Name.ToStringShort, skill, eligibleAnimals.Count(),
                    animals.Count() ),
                eligibleAnimals.Any() ? action : null );
        }

        private static void MassAssignMaster( Pawn pawn, IEnumerable<Pawn> animals )
        {
            foreach ( var animal in animals )
                animal.playerSettings.Master = pawn;
        }

        public static void MassAssignMasterBonded( PawnTable table )
        {
            // assign bonded animals to their bond-master, if not bonded, or bonded has low skill, do not touch.
            foreach ( var animal in ObedientAnimals( table ) )
            {
                // get bond
                var bond = animal.relations.GetFirstDirectRelationPawn( PawnRelationDefOf.Bond,
                    p => p.Faction == Faction.OfPlayer );
                if ( bond == null )
                    continue;
                animal.playerSettings.Master = bond;
            }
        }
    }
}