// FilterWorker_PawnKind.cs
// Copyright Karel Kroeze, 2017-2017

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace AnimalTab {
    public class FilterWorker_PawnKind: FilterWorker {
        private readonly DefMap<PawnKindDef, bool> allowed = new DefMap<PawnKindDef, bool>();
        private IEnumerable<PawnKindDef> PawnKinds => MainTabWindow_Animals.Instance.AllPawns.Select(p => p.kindDef).Distinct();

        public override FilterState State {
            get => allowed.Values().Any(v => v) ? FilterState.Inclusive : FilterState.Inactive;
            set => throw new InvalidOperationException("FilterWorker_PawnKind.set_State() should never be called");
        }

        public override bool Allows(Pawn pawn) {
            if (State == FilterState.Inactive) {
                return true;
            }

            return allowed[pawn.kindDef];
        }

        public override void Clicked() {
            List<FloatMenuOption> options = new List<FloatMenuOption> {
                new FloatMenuOption("AnimalTab.All".Translate(), Deactivate)
            };
            foreach (PawnKindDef pawnkind in PawnKinds) {
                options.Add(new FloatMenuOption_Persistent(pawnkind.LabelCap, () => Toggle(pawnkind), extraPartWidth: 30f, extraPartOnGUI: rect => DrawOptionExtra(rect, pawnkind)));
            }

            Find.WindowStack.Add(new FloatMenu(options));
        }

        public void Toggle(PawnKindDef pawnkind) {
            allowed[pawnkind] = !allowed[pawnkind];
            MainTabWindow_Animals.Instance.Notify_PawnsChanged();
        }

        public void Deactivate() {
            allowed.SetAll(false);
            MainTabWindow_Animals.Instance.Notify_PawnsChanged();
        }

        private bool DrawOptionExtra(Rect rect, PawnKindDef pawnkind) {
            if (State == FilterState.Inclusive && allowed[pawnkind]) {
                Rect checkRect = new Rect(rect.xMax - rect.height + Constants.Margin, rect.yMin, rect.height, rect.height).ContractedBy(7f);
                GUI.DrawTexture(checkRect, Widgets.CheckboxOnTex);
            }

            return false;
        }

        public override string GetTooltip() {
            if (State == FilterState.Inactive) {
                return "AnimalTab.FilterInactiveTip".Translate();
            }

            List<string> _allowed = new List<string>();
            foreach (PawnKindDef pawnkind in PawnKinds) {
                if (allowed[pawnkind]) {
                    _allowed.Add(pawnkind.label);
                }
            }

            return "AnimalTab.PawnKindFilterTip".Translate(_allowed.ToStringList("AnimalTab.Or".Translate()));
        }
    }
}
