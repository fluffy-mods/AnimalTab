// Patch_InjectTrainerSettingsGizmo.cs
// Copyright Karel Kroeze, 2019-2019

using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace AnimalTab
{
    [HarmonyPatch( typeof( Pawn ), nameof( Pawn.GetGizmos ) )]
    public class Patch_InjectTrainerSettingsGizmo
    {
        public static IEnumerable<Gizmo> Postfix( IEnumerable<Gizmo> __result, Pawn __instance )
        {
            foreach ( var result in __result )
                yield return result;

            if ( TameUtility.CanTame( __instance ) || __instance.training != null )
                foreach ( var gizmo in __instance.GetComp<CompHandlerSettings>()?.GetGizmos() ?? new List<Gizmo>() )
                    yield return gizmo;
        }
    }
}