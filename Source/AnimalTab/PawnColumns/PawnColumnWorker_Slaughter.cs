// PawnColumnWorker_Slaughter.cs
// Copyright Karel Kroeze, 2017-2017

using RimWorld;
using UnityEngine;
using Verse;

namespace AnimalTab {
    public class PawnColumnWorker_Slaughter: RimWorld.PawnColumnWorker_Slaughter {
        public override void DoCell(Rect rect, Pawn pawn, PawnTable table) {
            if (!HasCheckbox(pawn)) {
                return;
            }

            Rect checkboxRect = Utilities.GetCheckboxRect(rect);
            bool value = GetValue(pawn);
            bool flag = value;

            Utilities.DoCheckbox(checkboxRect, ref value, () => GetTip(pawn), backgroundTexture: pawn.IsBonded() ? Resources.Background_Danger : null);

            if (flag != value) {
                SetValue(pawn, value, table);
            }
        }

    }
}
