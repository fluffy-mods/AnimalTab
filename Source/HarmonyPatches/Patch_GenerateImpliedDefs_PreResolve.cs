// Patch_GenerateImpliedDefs_PreResolve.cs
// Copyright Karel Kroeze, 2017-2017

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using static AnimalTab.Constants;
using static AnimalTab.PawnColumnDefOf;

namespace AnimalTab {
    [HarmonyPatch(typeof(DefGenerator), "GenerateImpliedDefs_PreResolve")]
    public class Patch_GenerateImpliedDefs_PreResolve {
        public static void Postfix() {
            PawnTableDef animalTable = PawnTableDefOf.Animals;
            List<PawnColumnDef> columns = animalTable.columns;

#if DEBUG
            int i = 0;
            foreach (PawnColumnDef column in columns) {
                Log.Message($"{i++}\t{column.defName}\t{column.workerClass}");
            }
#endif

            // remove all gaps, we'll insert new ones at appropriate places
            columns.RemoveAll(c => c == GapTiny);

            // replace label column
            // NOTE: We can't simply replace the workerType, as these columns are used in multiple tabs.
            int labelIndex = columns.IndexOf(LabelWithIcon);
            columns.RemoveAt(labelIndex);
            columns.Insert(labelIndex, AnimalTabLabel);

            int areaIndex = columns.IndexOf(AllowedAreaWide);
            columns.RemoveAt(areaIndex);
            columns.Insert(areaIndex, AnimalTabAllowedArea);

            // move pregnant, master, bond and follow columns after lifestage
            int lifeStageIndex = columns.IndexOf(LifeStage);
            columns.Remove(Pregnant);
            columns.Insert(lifeStageIndex + 1, Pregnant);
            columns.Remove(Master);
            columns.Insert(lifeStageIndex + 2, Master);
            columns.Remove(Bond);
            columns.Insert(lifeStageIndex + 3, Bond);
            columns.Remove(FollowDrafted);
            columns.Insert(lifeStageIndex + 4, FollowDrafted);
            columns.Remove(FollowFieldwork);
            columns.Insert(lifeStageIndex + 5, FollowFieldwork);

            // insert gaps
            columns.Insert(lifeStageIndex + 1, GapTiny);
            columns.Insert(columns.IndexOf(FollowFieldwork) + 1, GapTiny);

            // insert wool, milk & meat columns before slaughter
            int slaughterIndex = columns.IndexOf(Slaughter);
            columns.Insert(slaughterIndex, Meat);
            columns.Insert(slaughterIndex, Milk);
            columns.Insert(slaughterIndex, Wool);

            // insert handler column before trainables
            int handlerIndex = columns.FindIndex(c => c.workerClass == typeof(RimWorld.PawnColumnWorker_Trainable));
            columns.Insert(handlerIndex, Handler);

            // swap medical and release
            int medicalIndex = columns.IndexOf(MedicalCare);
            columns.Remove(ReleaseAnimalToWild);
            columns.Insert(medicalIndex, ReleaseAnimalToWild);

            // add header tips to gender, lifestage and pregnant
            PawnColumnDefOf.Gender.headerTip = "Gender";
            LifeStage.headerTip = "Lifestage";
            Pregnant.headerTip = "Pregnant";

            // make all icons the same size.
            foreach (PawnColumnDef column in columns) {
                column.headerIconSize = HeaderIconSize;
            }

            // set new worker for master, slaughter and follow columns
            Pregnant.sortable = true;
            Pregnant.workerClass = typeof(PawnColumnWorker_Pregnant);
            Master.workerClass = typeof(PawnColumnWorker_Master);
            Slaughter.workerClass = typeof(PawnColumnWorker_Slaughter);
            FollowDrafted.workerClass = typeof(PawnColumnWorker_FollowDrafted);
            FollowFieldwork.workerClass = typeof(PawnColumnWorker_FollowFieldwork);
            ReleaseAnimalToWild.workerClass = typeof(PawnColumnWorker_Release);

            // set new workers for trainable columns
            foreach (PawnColumnDef column in columns.Where(c => c.workerClass == typeof(RimWorld.PawnColumnWorker_Trainable))) {
                column.workerClass = typeof(PawnColumnWorker_Trainable);
            }

            // insert gap after trainables
            columns.Insert(columns.FindLastIndex(c => c.workerClass == typeof(PawnColumnWorker_Trainable)) + 1, GapTiny);

            // reset all workers to make sure they are resolved to the new type
            // NOTE: Vanilla inserts columns by checking for 'Worker', which resolves some workers - we need to reset that.
            FieldInfo workerField = AccessTools.Field(typeof(PawnColumnDef), "workerInt");
            foreach (PawnColumnDef column in columns) {
                workerField.SetValue(column, null);
            }

            foreach (PawnColumnDef column in columns) {
                Logger.Debug(
                    $"{column.defName} <{column.workerClass.FullName}> | <{column.Worker?.GetType().FullName ?? "NULL"}>");
            }

            // add handler column to Wildlife tab
            PawnTableDefOf.Wildlife.columns.Add(Handler);
        }
    }
}
