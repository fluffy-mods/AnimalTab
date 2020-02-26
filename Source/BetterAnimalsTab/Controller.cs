// Controller.cs
// Copyright Karel Kroeze, 2017-2017

using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace AnimalTab.Properties
{
    public class Controller : Mod
    {
        public Controller( ModContentPack content ) : base( content )
        {
            // execute them patches.
            var harmony = new Harmony( "Fluffy.AnimalTab" );
            harmony.PatchAll( Assembly.GetExecutingAssembly() );
        }
    }
}