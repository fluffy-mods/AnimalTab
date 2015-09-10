using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.Sound;
using RimWorld;
using UnityEngine;

namespace Fluffy
{
    public static class AllowedAreaWidgets
    {
        public static void DoAllowedAreaHeaders(Rect rect, List<Pawn> pawns, AllowedAreaMode mode)
        {
            List<Area> allAreas = Find.AreaManager.AllAreas;
            int num = 1;
            for (int i = 0; i < allAreas.Count; i++)
            {
                if (allAreas[i].AssignableAsAllowed(mode))
                {
                    num++;
                }
            }
            float num2 = rect.width / (float)num;
            Text.WordWrap = false;
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.LowerCenter;
            Rect rect2 = new Rect(rect.x, rect.y, num2, rect.height);
            Widgets.Label(rect2, "NoAreaAllowed".Translate());
            if (Widgets.InvisibleButton(rect2))
            {
                for (int i = 0; i < pawns.Count; i++)
                {
                    SoundDefOf.DesignateDragStandardChanged.PlayOneShotOnCamera();
                    pawns[i].playerSettings.AreaRestriction = null;
                }
            }
            if (Mouse.IsOver(rect2))
            {
                GUI.DrawTexture(rect2, TexUI.HighlightTex);
            }
            TooltipHandler.TipRegion(rect2, "NoAreaAllowed".Translate());
            int num3 = 1;
            for (int j = 0; j < allAreas.Count; j++)
            {
                if (allAreas[j].AssignableAsAllowed(mode))
                {
                    float num4 = (float)num3 * num2;
                    Rect rect3 = new Rect(rect.x + num4, rect.y, num2, rect.height);
                    Widgets.Label(rect3, allAreas[j].Label);
                    if (Widgets.InvisibleButton(rect3))
                    {
                        for (int i = 0; i < pawns.Count; i++)
                        {
                            SoundDefOf.DesignateDragStandardChanged.PlayOneShotOnCamera();
                            pawns[i].playerSettings.AreaRestriction = allAreas[j];
                        }
                    }
                    TooltipHandler.TipRegion(rect3, "Restrict all to " + allAreas[j].Label);
                    if (Mouse.IsOver(rect3))
                    {
                        GUI.DrawTexture(rect3, TexUI.HighlightTex);
                    }
                    num3++;
                }
            }
            Text.WordWrap = true;
        }

    }
}
