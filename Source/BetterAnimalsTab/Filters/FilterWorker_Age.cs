using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace AnimalTab {
    public class FilterWorker_Age: FilterWorker {
        private readonly DefMap<LifeStageDef, bool> allowed = new DefMap<LifeStageDef, bool>();
        private IEnumerable<LifeStageDef> LifeStages => MainTabWindow_Animals.Instance.AllPawns.Select(p => p.ageTracker.CurLifeStage).Distinct();

        public override FilterState State => allowed.Values().Any(v => v) ? FilterState.Inclusive : FilterState.Inactive;

        public override bool Allows(Pawn pawn) {
            if (State == FilterState.Inactive) {
                return true;
            }

            return allowed[pawn.ageTracker.CurLifeStage];
        }

        public override void Clicked() {
            List<FloatMenuOption> options = new List<FloatMenuOption> {
                new FloatMenuOption("AnimalTab.All".Translate(), Deactivate)
            };
            foreach (LifeStageDef lifeStage in LifeStages) {
                options.Add(new FloatMenuOption_Persistent(lifeStage.LabelCap, () => Toggle(lifeStage), extraPartWidth: 30f, extraPartOnGUI: rect => DrawOptionExtra(rect, lifeStage)));
            }

            Find.WindowStack.Add(new FloatMenu(options));
        }

        public void Toggle(LifeStageDef lifeStage) {
            allowed[lifeStage] = !allowed[lifeStage];
            MainTabWindow_Animals.Instance.Notify_PawnsChanged();
        }

        public void Deactivate() {
            allowed.SetAll(false);
            MainTabWindow_Animals.Instance.Notify_PawnsChanged();
        }

        private bool DrawOptionExtra(Rect rect, LifeStageDef lifeStage) {
            if (State == FilterState.Inclusive && allowed[lifeStage]) {
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
            foreach (LifeStageDef lifeStage in LifeStages) {
                if (allowed[lifeStage]) {
                    _allowed.Add(lifeStage.label);
                }
            }

            return "AnimalTab.PawnKindFilterTip".Translate(_allowed.ToStringList("AnimalTab.List.Or".Translate()));
        }
    }
}
