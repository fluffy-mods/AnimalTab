// FilterWorker_Intelligence.cs
// Copyright Karel Kroeze, 2017-2017

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace AnimalTab {
    public class FilterWorker_Intelligence: FilterWorker {
        private readonly DefMap<TrainabilityDef, bool> _allowed = new DefMap<TrainabilityDef, bool>();
        private IEnumerable<TrainabilityDef> Trainabilities => DefDatabase<TrainabilityDef>.AllDefsListForReading;

        public override FilterState State {
            get => _allowed.Values().Any(v => v) ? FilterState.Inclusive : FilterState.Inactive;
            set => Logger.Error("FilterWorker_Intelligence.set_State should never be called.");
        }

        public override bool Allows(Pawn pawn) {
            if (State == FilterState.Inclusive) {
                return Allows(pawn.RaceProps.trainability);
            }

            return true;
        }

        public bool Allows(TrainabilityDef intelligence) {
            return _allowed[intelligence];
        }

        public override void Clicked() {
            List<FloatMenuOption> options = new List<FloatMenuOption> {
                new FloatMenuOption("AnimalTab.All".Translate(), Disable)
            };
            foreach (TrainabilityDef trainableIntelligence in Trainabilities.OrderBy(i => i.intelligenceOrder)) {
                options.Add(GetOption(trainableIntelligence));
            }

            Find.WindowStack.Add(new FloatMenu(options));
        }

        private void Disable() {
            foreach (TrainabilityDef intelligence in Trainabilities) {
                _allowed[intelligence] = false;
            }

            MainTabWindow_Animals.Instance.Notify_PawnsChanged();
        }

        private void Toggle(TrainabilityDef intelligence) {
            _allowed[intelligence] = !_allowed[intelligence];
            MainTabWindow_Animals.Instance.Notify_PawnsChanged();
        }

        private FloatMenuOption_Persistent GetOption(TrainabilityDef intelligence) {
            TaggedString label = "TrainableIntelligence".Translate() + ": " + intelligence.LabelCap;
            return new FloatMenuOption_Persistent(label, () => Toggle(intelligence), extraPartWidth: 30f,
                extraPartOnGUI: rect => DrawOptionExtra(rect, intelligence));
        }

        private bool DrawOptionExtra(Rect rect, TrainabilityDef intelligence) {
            if (State == FilterState.Inclusive && Allows(intelligence)) {
                Rect checkRect = new Rect( rect.xMax - rect.height + Constants.Margin, rect.yMin, rect.height,
                    rect.height ).ContractedBy( 7f );
                GUI.DrawTexture(checkRect, Widgets.CheckboxOnTex);
            }
            return Widgets.ButtonInvisible(rect);
        }

        public override string GetTooltip() {
            if (State == FilterState.Inactive) {
                return "AnimalTab.FilterInactiveTip".Translate();
            }

            List<string> allowed = ( from intelligence in Trainabilities
                                     where _allowed[intelligence]
                                     select intelligence.label ).ToList();
            return "AnimalTab.IntelligenceFilterTip".Translate(allowed.ToStringList("AnimalTab.List.Or".Translate()));
        }
    }
}
