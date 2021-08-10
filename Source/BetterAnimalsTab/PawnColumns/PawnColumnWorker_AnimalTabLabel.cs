// PawnColumnWorker_AnimalTabLabel.cs
// Copyright Karel Kroeze, 2017-2017

using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace AnimalTab {
    public class PawnColumnWorker_Label: RimWorld.PawnColumnWorker_Label {
        private static SortMode sortMode = SortMode.Name;

        public override void DoHeader(Rect rect, PawnTable table) {
            bool interacting = Mouse.IsOver( rect );

            if (interacting
                 && Event.current.type == EventType.MouseDown
                 && (Event.current.button == 0 || Event.current.button == 1)) {
                Sort(rect, table, Event.current.control);
                return;
            }

            base.DoHeader(rect, table);

            // replace tooltip
            if (interacting) {
                Logger.Debug("interacting\ntip\t" + GetHeaderTip(table) + "\nrect:\t" + rect);
                // TooltipHandler.ClearTooltipsFrom( rect );
                TooltipHandler.TipRegion(rect, GetHeaderTip(table));
            }
        }

        public void Sort(Rect rect, PawnTable table, bool byKind) {
            sortMode = byKind ? SortMode.PawnKind : SortMode.Name;

            HeaderClicked(rect, table);
        }

        public override int Compare(Pawn a, Pawn b) {
            return sortMode switch {
                SortMode.PawnKind => string.Compare(a.KindLabel, b.KindLabel, StringComparison.CurrentCultureIgnoreCase),
                SortMode.Name => throw new NotImplementedException(),
                _ => string.Compare(a.Name.ToStringShort, b.Name.ToStringShort, StringComparison.CurrentCultureIgnoreCase),
            };
        }

        public override void DoCell(Rect rect, Pawn pawn, PawnTable table) {
            bool interacting = Mouse.IsOver( rect ) && !pawn.Name.Numerical;

            // intercept interactions before base has a chance to do so
            if (interacting
                 && Event.current.control
                 && Event.current.type == EventType.MouseDown
                 && Event.current.button == 0) {
                Find.WindowStack.Add(new Dialog_RenameAnimal(pawn));
                return;
            }

            base.DoCell(rect, pawn, table);

            // replace tooltip
            if (interacting) {
                TooltipHandler.ClearTooltipsFrom(rect);
                TooltipHandler.TipRegion(rect, GetToolTip(pawn));
            }
        }

        private string GetToolTip(Pawn pawn) {
            return "ClickToJumpTo".Translate() + "\n" + "AnimalTab.CtrlClickToRename".Translate() + "\n\n" + pawn.GetTooltip().text;
        }

        protected override string GetHeaderTip(PawnTable table) {
            return "AnimalTab.LabelHeaderTip".Translate();
        }

        private enum SortMode {
            PawnKind,
            Name
        }
    }
}
