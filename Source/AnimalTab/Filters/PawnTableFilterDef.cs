// PawnTableFilterDef.cs
// Copyright Karel Kroeze, 2017-2017

using System;
using UnityEngine;
using Verse;

namespace AnimalTab {
    public class FilterDef: Def {
        public string icon;
        public string inactiveAdjective;
        public string inclusiveAdjective;
        public string exclusiveAdjective;
        internal Texture2D iconTex;
        public Type workerClass;

        private FilterWorker workerInt;

        public FilterWorker Worker {
            get {
                if (workerInt == null) {
                    workerInt = (FilterWorker) Activator.CreateInstance(workerClass);
                    workerInt.def = this;
                }
                return workerInt;
            }
        }
    }
}
