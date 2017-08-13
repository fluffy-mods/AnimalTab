// PawnColumnWorker_Slaughter.cs
// Copyright Karel Kroeze, 2017-2017

using System;
using RimWorld;
using UnityEngine;
using Verse;
using static AnimalTab.Constants;

namespace AnimalTab
{
    public class PawnColumnWorker_Slaughter : RimWorld.PawnColumnWorker_Slaughter
    {
        public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
        {
            if (!HasCheckbox(pawn))
                return;
            
            Rect checkboxRect = Utilities.GetCheckboxRect( rect );
            bool value = GetValue(pawn);
            bool flag = value;

            Utilities.DoCheckbox(checkboxRect, ref value, () => GetTip( pawn ), backgroundTexture: IsBonded( pawn ) ? Resources.CheckboxBackground_Danger : null );

            if ( flag != value )
                SetValue( pawn, value );
        }

        public bool IsBonded( Pawn pawn )
        {
            if ( !pawn.RaceProps.IsFlesh )
                return false;

            return pawn.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Bond) != null;
        }
    }
}