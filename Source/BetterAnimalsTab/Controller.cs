// Controller.cs
// Copyright Karel Kroeze, 2017-2017

using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace AnimalTab {
    public class Controller: Mod {
        public static Settings Settings { get; private set; }

        public Controller(ModContentPack content) : base(content) {
            Settings = GetSettings<Settings>();

            // execute them patches.
            Harmony harmony = new Harmony( "Fluffy.AnimalTab" );
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override string SettingsCategory() {
            return "Fluffy.AnimalTab".Translate();
        }

        public override void DoSettingsWindowContents(Rect canvas) {
            Settings.DoWindowContents(canvas);
        }
    }
}
