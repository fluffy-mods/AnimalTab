// PawnColumnWorker_FollowFieldwork.cs
// Copyright Karel Kroeze, 2017-2017

using RimWorld;
using UnityEngine;
using Verse;

namespace AnimalTab
{
    public class PawnColumnWorker_FollowFieldwork : RimWorld.PawnColumnWorker_FollowFieldwork
    {
        public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
        {
            if (!HasCheckbox(pawn))
                return;

            Rect checkboxRect = Utilities.GetCheckboxRect(rect);
            bool value = GetValue(pawn);
            bool flag = value;

            Utilities.DoCheckbox(checkboxRect, ref value);

            if (flag != value)
                SetValue(pawn, value, table);
        }
    }
}