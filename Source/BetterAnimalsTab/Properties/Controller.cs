// Controller.cs
// Copyright Karel Kroeze, 2017-2017

using RimWorld;
using UnityEngine;
using Verse;
using static AnimalTab.Constants;
using static AnimalTab.PawnColumnDefOf;

namespace AnimalTab.Properties
{
    public class Controller : Mod
    {
        public Controller( ModContentPack content ) : base( content )
        {
            LongEventHandler.QueueLongEvent( Initialize, "AnimalTab.Initialize", false, null );
        }

        public void Initialize()
        {
            var animalTable = PawnTableDefOf.Animals;
            var columns = animalTable.columns;
            
            // replace label column
            var labelIndex = columns.IndexOf( Label );
            columns.RemoveAt(labelIndex);
            columns.Insert( labelIndex, AnimalTabLabel );

            // move master and follow columns after lifestage
            var lifeStageIndex = columns.FindIndex( c => c.workerClass == typeof( PawnColumnWorker_LifeStage ) );
            var masterColumn = columns.Find( c => c == Master );
            var followDraftedColumn = columns.Find( c => c == FollowDrafted );
            var followFieldworkColumn = columns.Find( c => c == FollowFieldwork );
            columns.Remove( masterColumn );
            columns.Insert(lifeStageIndex + 1, masterColumn);
            columns.Remove(followDraftedColumn);
            columns.Insert(lifeStageIndex + 2, followDraftedColumn);
            columns.Remove(followFieldworkColumn);
            columns.Insert(lifeStageIndex + 3, followFieldworkColumn);

            // remove all gaps, insert new ones at appropriate places
            columns.RemoveAll( c => c == GapTiny );
            columns.Insert( lifeStageIndex + 1, GapTiny );
            columns.Insert( columns.IndexOf( followFieldworkColumn ) + 1, GapTiny );
            columns.Insert( columns.FindLastIndex( c => c.workerClass == typeof( PawnColumnWorker_Trainable ) ) + 1, GapTiny );
            columns.Insert( columns.IndexOf( Slaughter ) + 1, GapTiny );

            // make all icons the same size.
            foreach ( var column in columns )
                column.headerIconSize = HeaderIconSize;
        }
    }
}