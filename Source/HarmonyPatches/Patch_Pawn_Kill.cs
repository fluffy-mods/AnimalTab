
using HarmonyLib;
using Verse;

namespace AnimalTab {

    [HarmonyPatch(typeof(Pawn), nameof(Pawn.Kill))]
    public static class Patch_Pawn_Kill {
        public static void Postfix(Pawn __instance) {
            HandlerUtility.Notify_PawnDied(__instance);
        }
    }
}
