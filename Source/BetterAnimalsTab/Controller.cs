// Controller.cs
// Copyright Karel Kroeze, 2017-2017

using System.Reflection;
using Harmony;
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
            var harmony = HarmonyInstance.Create( "Fluffy.AnimalTab" );
            harmony.PatchAll( Assembly.GetExecutingAssembly() );
        }
    }
}