// Patch_InjectTrainerSettingsComp.cs
// Copyright Karel Kroeze, 2019-2019

using HarmonyLib;
using RimWorld;
using Verse;

namespace AnimalTab
{
    [HarmonyPatch( typeof( DefOfHelper ), nameof( DefOfHelper.RebindAllDefOfs ) )]
    public class Patch_InjectTrainerSettingsComp
    {
        public static void Postfix()
        {
            // inject handler comp into all races
            foreach ( var thing in DefDatabase<ThingDef>.AllDefsListForReading )
            {
                // only inject on defs with race props, and only if not already present.
                if ( thing.race != null && !thing.comps.Any( cp => cp.compClass == typeof( CompHandlerSettings ) ) )
                {
                    thing.comps.Add( new CompProperties( typeof( CompHandlerSettings ) ) );
                    Logger.Debug( $"injected CompHandlerSettings into {thing.defName}" );
                }
            }
        }
    }
}