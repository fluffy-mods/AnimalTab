using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace AnimalTab {
    public class FilterWorker_Bonded: FilterWorker {
        public override bool Allows(Pawn pawn) {

            Pawn bondedPawn = pawn.BondedPawn();
            if (bondedPawn is not null && !selected.ContainsKey(bondedPawn)) {
                selected.Add(bondedPawn, false);
            }

            return State switch {
                FilterState.Inactive => true,
                FilterState.Inclusive => bondedPawn is not null && selected[bondedPawn],
                FilterState.Exclusive => bondedPawn is null || !selected[bondedPawn],
                _ => true,
            };
        }

        private readonly Dictionary<Pawn, bool> selected = new();
        private IEnumerable<Pawn> Masters => MainTabWindow_Animals.Instance.AllPawns
            .Select(a => a.BondedPawn())
            .Where(a => a is not null)
            .Distinct();

        public override void Clicked() {
            List<FloatMenuOption> options = new();
            if (State != FilterState.Inclusive) {
                options.Add(new("AnimalTab.ShowBonded".Translate(), SetInclusive));
            }
            if (State != FilterState.Exclusive) {
                options.Add(new("AnimalTab.HideBonded".Translate(), SetExclusive));
            }
            if (State != FilterState.Inactive) {
                options.Add(new("AnimalTab.ClearFilter".Translate(), Deactivate));
            }

            foreach (Pawn master in Masters) {
                options.Add(
                    new("AnimalTab.BondedWithX".Translate(master.NameShortColored.Resolve()),
                        () => Toggle(master),
                        extraPartWidth: 30f,
                        extraPartOnGUI: rect => DrawOptionExtra(rect, master)));
            }

            Find.WindowStack.Add(new FloatMenu(options));
        }

        public void SetInclusive() {
            selected.Clear();
            foreach (Pawn master in Masters) {
                selected.Add(master, true);
            }
            State = FilterState.Inclusive;
            MainTabWindow_Animals.Instance.Notify_PawnsChanged();
        }

        public void SetExclusive() {
            selected.Clear();
            foreach (Pawn master in Masters) {
                selected.Add(master, true);
            }
            State = FilterState.Exclusive;
            MainTabWindow_Animals.Instance.Notify_PawnsChanged();
        }

        public void Deactivate() {
            selected.Clear();
            State = FilterState.Inactive;
            MainTabWindow_Animals.Instance.Notify_PawnsChanged();
        }

        public void Toggle(Pawn master) {
            if (selected.TryGetValue(master, out bool allows)) {
                selected[master] = !allows;
            } else {
                selected.Add(master, true);
            }
            if (State == FilterState.Inactive) {
                State = FilterState.Inclusive;
            }
            MainTabWindow_Animals.Instance.Notify_PawnsChanged();
        }

        public bool DrawOptionExtra(Rect canvas, Pawn master) {
            if (State != FilterState.Inactive && selected[master]) {
                Rect iconRect = canvas.RightPartPixels(canvas.height).ContractedBy(7);
                if (State == FilterState.Inclusive) {
                    GUI.DrawTexture(iconRect, Widgets.CheckboxOnTex);
                } else {
                    GUI.DrawTexture(iconRect, Widgets.CheckboxOffTex);
                }
            }

            return false;
        }

        public override string GetTooltip() {

            return State switch {
                FilterState.Inactive => "AnimalTab.FilterInactiveTip".Translate(),
                FilterState.Inclusive => "AnimalTab.BondedFilterTip.ShowBondedWithX".Translate(selected
                    .Where(p => p.Value)
                    .Select(p => p.Key.NameShortColored.Resolve())
                    .ToStringList("AnimalTab.List.Or".Translate().Resolve())),
                FilterState.Exclusive => "AnimalTab.BondedFilterTip.HideBondedWithX".Translate(selected
                    .Where(p => p.Value)
                    .Select(p => p.Key.NameShortColored.Resolve())
                    .ToStringList("AnimalTab.List.Or".Translate().Resolve())),
                _ => "invalid filter state"
            };
        }
    }
}
