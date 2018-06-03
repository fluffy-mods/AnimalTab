// Patch_ThingSelectionUtility.cs
// Copyright Karel Kroeze, 2018-2018

using Harmony;
using UnityEngine;

namespace AnimalTab
{
    public class Patch_ThingSelectionUtility
    {
        [HarmonyPatch( typeof( RimWorld.ThingSelectionUtility ) )]
        [HarmonyPatch("SelectNextColonist")]
        public static class Pre_SelectNextColonist
        {
            public static bool Prefix()
            {
                if ( Event.current.shift )
                {
                    ThingSelectionUtility.SelectNextTameAnimal();
                    return false;
                }

                return true;
            }
        }
        
        [HarmonyPatch( typeof( RimWorld.ThingSelectionUtility ) )]
        [HarmonyPatch("SelectPreviousColonist")]
        public static class Pre_SelectPreviousColonist
        {
            public static bool Prefix()
            {
                if ( Event.current.shift )
                {
                    ThingSelectionUtility.SelectPreviousTameAnimal();
                    return false;
                }

                return true;
            }
        }
    }
}