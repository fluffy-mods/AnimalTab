// PawnColumnWorker_AreaSelection.cs
// Copyright Karel Kroeze, 2017-2017

using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AnimalTab
{
    public class PawnColumnWorker_AllowedArea : PawnColumnWorker
    {
        protected override GameFont DefaultHeaderFont => GameFont.Tiny;

        public override int GetMinWidth( PawnTable table )
        {
            return Mathf.Max( base.GetMinWidth( table ), 200 );
        }

        public override int GetMinHeaderHeight( PawnTable table )
        {
            return Mathf.Max( 32, base.GetMinHeaderHeight( table ) );
        }

        public override int GetOptimalWidth( PawnTable table )
        {
            return Mathf.Clamp( 273, GetMinWidth( table ), GetMaxWidth( table ) );
        }

        public override void DoHeader( Rect rect, PawnTable table )
        {
            if ( Event.current.shift )
                DoMassAreaSelector( rect, table );
            else
                base.DoHeader( rect, table );
        }

        public override int Compare( Pawn a, Pawn b )
        {
            return GetValueToCompare( a ).CompareTo( GetValueToCompare( b ) );
        }

        private int GetValueToCompare( Pawn pawn )
        {
            if ( pawn.Faction != Faction.OfPlayer )
                return int.MinValue;

            var areaRestriction = pawn.playerSettings.AreaRestriction;
            return areaRestriction?.ID ?? int.MinValue;
        }

        public override void DoCell( Rect rect, Pawn pawn, PawnTable table )
        {
            if ( pawn.Faction != Faction.OfPlayer )
                return;
            AreaAllowedGUI.DoAllowedAreaSelectors( rect, pawn );
        }

        protected override string GetHeaderTip( PawnTable table )
        {
            return base.GetHeaderTip( table ) + "\n" + "AnimalTab.AllowedAreaTip".Translate();
        }

        protected override void HeaderClicked( Rect headerRect, PawnTable table )
        {
            if ( Event.current.control )
                Find.WindowStack.Add( new Dialog_ManageAreas( Find.CurrentMap ) );
            else
                base.HeaderClicked( headerRect, table );
        }

        public void DoMassAreaSelector( Rect rect, PawnTable table )
        {
            var map = Find.CurrentMap;
            var areas = map.areaManager.AllAreas.Where( a => a.AssignableAsAllowed() );
            var areaCount = areas.Count() + 1;
            var widthPerArea = rect.width / areaCount;
            var areaRect = new Rect( rect.x, rect.y, widthPerArea, rect.height );

            Text.WordWrap = false;
            Text.Font = GameFont.Tiny;

            if ( DoAreaSelector( areaRect, null ) )
                RestrictAllTo( null, table );
            areaRect.x += widthPerArea;
            foreach ( var area in areas )
            {
                if ( DoAreaSelector( areaRect, area ) )
                    RestrictAllTo( area, table );
                areaRect.x += widthPerArea;
            }

            Text.WordWrap = true;
            Text.Font = GameFont.Small;
        }

        private void RestrictAllTo( Area area, PawnTable table )
        {
            foreach ( var pawn in table.PawnsListForReading )
                pawn.playerSettings.AreaRestriction = area;
        }

        private bool DoAreaSelector( Rect rect, Area area )
        {
            rect = rect.ContractedBy( 1f );
            GUI.DrawTexture( rect, area == null ? BaseContent.GreyTex : area.ColorTexture );
            Text.Anchor = TextAnchor.MiddleLeft;
            var labelRect = rect.ContractedBy( 3f );
            var label = AreaUtility.AreaAllowedLabel_Area( area );
            Widgets.Label( labelRect, label );
            TooltipHandler.TipRegion( rect, label );
            if ( Mouse.IsOver( rect ) )
            {
                area?.MarkForDraw();
                if ( Input.GetMouseButton( 0 ) )
                {
                    SoundDefOf.Designate_DragStandard_Changed.PlayOneShotOnCamera();
                    return true;
                }
            }
            return false;
        }
    }
}