// PawnColumnWorker_AreaSelection.cs
// Copyright Karel Kroeze, 2017-2017

using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AnimalTab {
    public class PawnColumnWorker_AllowedArea: RimWorld.PawnColumnWorker_AllowedArea {
        protected override GameFont DefaultHeaderFont => GameFont.Tiny;

        public override void DoHeader(Rect rect, PawnTable table) {
            if (Event.current.shift) {
                DoMassAreaSelector(rect, table);
            } else {
                base.DoHeader(rect, table);
            }
        }

        private int GetValueToCompare(Pawn pawn) {
            if (pawn.Faction != Faction.OfPlayer) {
                return int.MinValue;
            }

            Area areaRestriction = pawn.playerSettings.AreaRestriction;
            return areaRestriction?.ID ?? int.MinValue;
        }

        protected override string GetHeaderTip(PawnTable table) {
            return base.GetHeaderTip(table) + "\n" + "AnimalTab.AllowedAreaTip".Translate();
        }

        protected override void HeaderClicked(Rect headerRect, PawnTable table) {
            if (Event.current.control) {
                Find.WindowStack.Add(new Dialog_ManageAreas(Find.CurrentMap));
            } else {
                base.HeaderClicked(headerRect, table);
            }
        }

        public void DoMassAreaSelector(Rect rect, PawnTable table) {
            Map map = Find.CurrentMap;
            System.Collections.Generic.IEnumerable<Area> areas = map.areaManager.AllAreas.Where( a => a.AssignableAsAllowed() );
            int areaCount = areas.Count() + 1;
            float widthPerArea = rect.width / areaCount;
            Rect areaRect = new Rect( rect.x, rect.y, widthPerArea, rect.height );

            Text.WordWrap = false;
            Text.Font = GameFont.Tiny;

            if (DoAreaSelector(areaRect, null)) {
                RestrictAllTo(null, table);
            }

            areaRect.x += widthPerArea;
            foreach (Area area in areas) {
                if (DoAreaSelector(areaRect, area)) {
                    RestrictAllTo(area, table);
                }

                areaRect.x += widthPerArea;
            }

            Text.WordWrap = true;
            Text.Font = GameFont.Small;
        }

        private void RestrictAllTo(Area area, PawnTable table) {
            foreach (Pawn pawn in table.PawnsListForReading.Where(p => p.playerSettings.SupportsAllowedAreas)) {
                pawn.playerSettings.AreaRestriction = area;
            }
        }

        private bool DoAreaSelector(Rect rect, Area area) {
            rect = rect.ContractedBy(1f);
            GUI.DrawTexture(rect, area == null ? BaseContent.GreyTex : area.ColorTexture);
            Text.Anchor = TextAnchor.MiddleLeft;
            Rect labelRect = rect.ContractedBy( 3f );
            string label = AreaUtility.AreaAllowedLabel_Area( area );
            Widgets.Label(labelRect, label);
            TooltipHandler.TipRegion(rect, label);
            if (Mouse.IsOver(rect)) {
                area?.MarkForDraw();
                if (Input.GetMouseButton(0)) {
                    SoundDefOf.Designate_DragStandard_Changed.PlayOneShotOnCamera();
                    return true;
                }
            }
            return false;
        }
    }
}
