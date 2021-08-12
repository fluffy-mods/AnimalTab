// PawnTableFilterWorker.cs
// Copyright Karel Kroeze, 2017-2017

using System;
using UnityEngine;
using Verse;

namespace AnimalTab {
    public abstract class FilterWorker {
        protected FilterState _state = FilterState.Inactive;
        public FilterDef def;

        public virtual string Adjective(FilterState state) {
            return state switch {
                FilterState.Inclusive when !def.inclusiveAdjective.NullOrEmpty() => def.inclusiveAdjective,
                FilterState.Inclusive => def.label,
                FilterState.Exclusive when !def.exclusiveAdjective.NullOrEmpty() => def.exclusiveAdjective,
                FilterState.Exclusive => "AnimalTab.Not".Translate(Adjective(FilterState.Inclusive)).Resolve(),
                _ when !def.inactiveAdjective.NullOrEmpty() => "AnimalTab.Any".Translate() + " " + def.inactiveAdjective,
                _ => "AnimalTab.Any".Translate()
            };
        }

        public virtual FilterState State {
            get => _state;
            set {
                if (value == _state) {
                    return;
                }

                _state = value;
                MainTabWindow_Animals.Instance.Notify_PawnsChanged();
            }
        }

        public virtual FilterState NextState => (FilterState) (((int) State + 1) % Enum.GetValues(typeof(FilterState)).Length);

        public virtual Color Colour => State switch {
            FilterState.Inclusive => GenUI.MouseoverColor,
            FilterState.Exclusive => Constants.DarkRed,
            _ => Color.grey,
        };

        public abstract bool Allows(Pawn pawn);

        public virtual string GetTooltip() {
            return "AnimalTab.FilterTip".Translate(Adjective(State), Adjective(NextState));
        }

        public virtual void Clicked() {
            State = NextState;
        }

        public virtual void Draw(Rect rect) {
            TooltipHandler.TipRegion(rect, GetTooltip, (int) rect.center.x * 541);
            if (Widgets.ButtonImage(rect, Icon, Colour, Colour * 1.5f)) {
                Clicked();
            }
        }

        public virtual Texture2D Icon {
            get {
                if (def.iconTex == null) {
                    def.iconTex = ContentFinder<Texture2D>.Get(def.icon, true);
                }

                return def.iconTex;
            }
        }
    }
}
