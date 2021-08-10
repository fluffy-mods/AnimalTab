// Patch_WorkGiver_Tame_JobOnThing.cs
// Copyright Karel Kroeze, 2019-2019

using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace AnimalTab {
    [HarmonyPatch(typeof(WorkGiver_Tame), nameof(WorkGiver_Tame.JobOnThing))]
    internal class Patch_WorkGiver_Tame_JobOnThing {
        public static void Postfix(Pawn pawn, Thing t, ref Job __result) {
            if (__result == null) {
                return;
            }

            Pawn target = t as Pawn;
            CompHandlerSettings handlerSettings = target?.GetComp<CompHandlerSettings>();

            if (!handlerSettings.Allows(pawn, out string reason)) {
                JobFailReason.Is(reason);
                __result = null;
            }
        }
    }
}
