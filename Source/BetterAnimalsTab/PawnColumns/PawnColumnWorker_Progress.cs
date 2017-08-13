// PawnColumnWorker_Progress.cs
// Copyright Karel Kroeze, 2017-2017

using RimWorld;
using UnityEngine;
using Verse;

namespace AnimalTab
{
    public abstract class PawnColumnWorker_Progress : PawnColumnWorker
    {
        public override void DoCell( Rect rect, Pawn pawn, PawnTable table )
        {
            if ( !ShouldDraw( pawn ) )
                return;

            var progress = GetProgress( pawn );
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label( rect, progress.ToStringPercent() );
            Text.Anchor = TextAnchor.UpperLeft;
            TooltipHandler.TipRegion( rect, GetTip( pawn ) );
        }

        public override int Compare( Pawn a, Pawn b )
        {
            if (ShouldDraw(a) && ShouldDraw(b))
                return GetProgress( a ).CompareTo( GetProgress( b ) );

            return ShouldDraw( a ).CompareTo( ShouldDraw( b ) );
        }

        public override int GetMinWidth( PawnTable table )
        {
            return Constants.TextCellWidth;
        }

        public abstract bool ShouldDraw( Pawn pawn );

        public abstract float GetProgress( Pawn pawn );

        public abstract string GetTip( Pawn pawn );
    }
}