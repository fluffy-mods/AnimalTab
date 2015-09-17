using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;
using Verse.Sound;

namespace Fluffy
{
    public class MainTabWindow_Work : MainTabWindow_PawnList
    {
        private const float TopAreaHeight = 40f;

        protected const float LabelRowHeight = 50f;

        private float workColumnSpacing = -1f;

        private static List<WorkTypeDef> VisibleWorkTypeDefsInPriorityOrder;

        private static readonly Texture2D copyTex = ContentFinder<Texture2D>.Get("UI/Buttons/Copy", true);
        private static readonly Texture2D pasteTex = ContentFinder<Texture2D>.Get("UI/Buttons/Paste", true);
        private static readonly Texture2D cancelTex = ContentFinder<Texture2D>.Get("UI/Buttons/cancel", true);

        private WorkTypeDef workOrder;

        public enum order
        {
            Work,
            Name,
            Default
        }

        public order orderBy = order.Default;

        private bool asc = false;

        private List<int> copy = null;

        public static Pawn copied = null;

        public override Vector2 RequestedTabSize
        {
            get
            {
                return new Vector2(1050f, 90f + (float)base.PawnsCount * 30f + 65f);
            }
        }

        public override void PreOpen()
        {
            base.PreOpen();
            MainTabWindow_Work.Reinit();
        }

        public static void Reinit()
        {
            MainTabWindow_Work.VisibleWorkTypeDefsInPriorityOrder = (from def in WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder
                                                                     where def.visible
                                                                     select def).ToList<WorkTypeDef>();
        }

        protected void Copy(Pawn p)
        {
            copy = (from def in VisibleWorkTypeDefsInPriorityOrder
                    select p.story.WorkTypeIsDisabled(def) ? -1 : p.workSettings.GetPriority(def)).ToList<int>();
            copied = p;

#if DEBUG
            var temp = "Priorities: ";
            for (int i = 0; i < copy.Count; i++)
            {
                temp += ", " + copy[i].ToString();
            }
            Log.Message(temp);
#endif
        }

        protected void Paste(Pawn p)
        {
            for (int i = 0; i < VisibleWorkTypeDefsInPriorityOrder.Count; i++)
            {
#if DEBUG
                Log.Message("Attempting paste...");
#endif
                if (p.story != null && !p.story.WorkTypeIsDisabled(VisibleWorkTypeDefsInPriorityOrder[i]) && copy[i] >= 0)
                {
#if DEBUG
                    Log.Message(VisibleWorkTypeDefsInPriorityOrder[i].LabelCap);
#endif
                    p.workSettings.SetPriority(VisibleWorkTypeDefsInPriorityOrder[i], copy[i]);
                }
            }
        }

        protected void ClearCopied()
        {
            copy = null;
            copied = null;
        }

        protected override void BuildPawnList()
        {
            this.pawns.Clear();
            IEnumerable<Pawn> sorted;

            switch (orderBy)
            {
                case order.Work:
                    sorted = from p in Find.ListerPawns.FreeColonists
                             orderby (p.story == null || p.story.WorkTypeIsDisabled(workOrder)),
                                      p.skills.AverageOfRelevantSkillsFor(workOrder) descending
                             select p;
                    break;
                case order.Name:
                    sorted = from p in Find.ListerPawns.FreeColonists
                             orderby p.LabelCap ascending
                             select p;
                    break;
                case order.Default:
                default:
                    sorted = Find.ListerPawns.FreeColonists;
                    break;
            }

            this.pawns = sorted.ToList<Pawn>();
            if (asc && this.pawns.Count() > 1)
            {
                this.pawns.Reverse();
                SoundDefOf.CheckboxTurnedOff.PlayOneShotOnCamera();
            }
            else
            {
                SoundDefOf.CheckboxTurnedOn.PlayOneShotOnCamera();
            }
        }

        protected void incrementJobPriority(WorkTypeDef work, bool toggle)
        {
            int start = toggle ? 3 : 4;
            bool max = pawns.All(p => (p.workSettings.GetPriority(work) == 1 || (p.story == null || p.story.WorkTypeIsDisabled(work))));

            for (int i = 0; i < pawns.Count; i++)
            {
                if (!(pawns[i].story == null || pawns[i].story.WorkTypeIsDisabled(work)))
                {
                    int cur = pawns[i].workSettings.GetPriority(work);
                    if (cur > 1)
                    {
                        pawns[i].workSettings.SetPriority(work, cur - 1);
                    }
                    if (cur == 0)
                    {
                        if (toggle) SoundDefOf.CheckboxTurnedOn.PlayOneShotOnCamera();
                        pawns[i].workSettings.SetPriority(work, start);
                    }
                    if (toggle && max)
                    {
                        SoundDefOf.CheckboxTurnedOff.PlayOneShotOnCamera();
                        pawns[i].workSettings.SetPriority(work, 0);
                    }
                }
            }
        }

        protected void decrementJobPriority(WorkTypeDef work, bool toggle)
        {
            int reset = toggle ? 1 : 4;
            bool min = pawns.All(p => (p.workSettings.GetPriority(work) == 0 || (p.story == null || p.story.WorkTypeIsDisabled(work))));

            for (int i = 0; i < pawns.Count; i++)
            {
                if (!(pawns[i].story == null || pawns[i].story.WorkTypeIsDisabled(work)))
                {
                    int cur = pawns[i].workSettings.GetPriority(work);
                    if (!toggle && cur > 0 && cur < 4)
                    {
                        pawns[i].workSettings.SetPriority(work, cur + 1);
                    }
                    if (cur == 4 || (toggle && cur == 1))
                    {
                        pawns[i].workSettings.SetPriority(work, 0);
                        if (toggle) SoundDefOf.CheckboxTurnedOff.PlayOneShotOnCamera();
                    }
                    if (min && toggle)
                    {
                        pawns[i].workSettings.SetPriority(work, 3);
                        if (toggle) SoundDefOf.CheckboxTurnedOn.PlayOneShotOnCamera();
                    }
                }
            }
        }

        public override void DoWindowContents(Rect rect)
        {
            base.DoWindowContents(rect);
            Rect position = new Rect(0f, 0f, rect.width, 40f);
            GUI.BeginGroup(position);
            Text.Font = GameFont.Small;
            GUI.color = Color.white;
            float num3 = 175f;

            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect2 = new Rect(5f, 5f, 140f, 30f);
            bool useWorkPriorities = Find.Map.playSettings.useWorkPriorities;
            Widgets.LabelCheckbox(rect2, "ManualPriorities".Translate(), ref Find.Map.playSettings.useWorkPriorities, false);
            if (useWorkPriorities != Find.Map.playSettings.useWorkPriorities)
            {
                foreach (Pawn current in Find.ListerPawns.FreeColonists)
                {
                    current.workSettings.Notify_UseWorkPrioritiesChanged();
                }
            }
            float num = position.width / 3f;
            float num2 = position.width * 2f / 3f;
            Rect rect3 = new Rect(num - 50f, 5f, 160f, 30f);
            Rect rect4 = new Rect(num2 - 50f, 5f, 160f, 30f);
            GUI.color = new Color(1f, 1f, 1f, 0.5f);
            Text.Anchor = TextAnchor.UpperCenter;
            Text.Font = GameFont.Tiny;
            Widgets.Label(rect3, "<= " + "HigherPriority".Translate());
            Widgets.Label(rect4, "LowerPriority".Translate() + " =>");
            Text.Font = GameFont.Small;
            GUI.EndGroup();
            Rect position2 = new Rect(0f, 40f, rect.width, rect.height - 40f);
            GUI.BeginGroup(position2);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.LowerCenter;
            GUI.color = Color.white;
            Rect rectname = new Rect(0f, 20f, num3, 33f);
            Widgets.Label(rectname, "Fluffy.Name".Translate());
            Widgets.DrawHighlightIfMouseover(rectname);
            if (Widgets.InvisibleButton(rectname))
            {
                if (Event.current.button == 0)
                {
                    if (orderBy == order.Name)
                    {
                        asc = !asc;
                    }
                    else
                    {
                        orderBy = order.Name;
                        asc = false;
                    }
                }
                else if (Event.current.button == 1)
                {
                    if (orderBy != order.Default)
                    {
                        orderBy = order.Default;
                        asc = false;
                    }
                }
                BuildPawnList();
            }
            TooltipHandler.TipRegion(rectname, "Fluffy.SortByNameWork".Translate());
            Text.Anchor = TextAnchor.UpperLeft;
            Rect outRect = new Rect(0f, 50f, position2.width, position2.height - 50f);
            this.workColumnSpacing = (position2.width - 16f - 225f) / (float)MainTabWindow_Work.VisibleWorkTypeDefsInPriorityOrder.Count;
            int num4 = 0;
            foreach (WorkTypeDef current2 in MainTabWindow_Work.VisibleWorkTypeDefsInPriorityOrder)
            {
                Vector2 vector = Text.CalcSize(current2.labelShort);
                float num5 = num3 + 15f;
                Rect rect5 = new Rect(num5 - vector.x / 2f, 0f, vector.x, vector.y);
                if (num4 % 2 == 1)
                {
                    rect5.y += 20f;
                }
                if (Mouse.IsOver(rect5))
                {
                    Widgets.DrawHighlight(rect5);
                }
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect5, current2.labelShort);
                WorkTypeDef localDef = current2;
                var tiptext = new StringBuilder();
                if (useWorkPriorities)
                {
                    tiptext.AppendLine("Fluffy.LeftSortBySkill".Translate());
                    tiptext.AppendLine("Fluffy.ShiftToChangePriority".Translate());
                }
                else
                {
                    tiptext.AppendLine("Fluffy.SortBySkill".Translate());
                    tiptext.AppendLine("Fluffy.ShiftToToggle".Translate());
                }
                tiptext.AppendLine().AppendLine(localDef.gerundLabel).
                        AppendLine().Append(localDef.description);
                TooltipHandler.TipRegion(rect5, new TipSignal(() => tiptext.ToString(), localDef.GetHashCode()));
                if (Event.current.type == EventType.MouseDown && Mouse.IsOver(rect5))
                {
                    if (Event.current.shift)
                    {
                        if (Event.current.button == 0)
                        {
                            if (useWorkPriorities)
                            {
                                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                                incrementJobPriority(current2, false);
                            }
                            else
                            {
                                incrementJobPriority(current2, true);
                            }
                        }
                        else if (Event.current.button == 1)
                        {
                            if (useWorkPriorities)
                            {
                                SoundDefOf.AmountDecrement.PlayOneShotOnCamera();
                                decrementJobPriority(current2, false);
                            }
                            else
                            {
                                decrementJobPriority(current2, true);
                            }
                        }
                    }
                    else
                    {
                        //Log.Message("Clicked on " + current2.LabelCap);
                        if (orderBy == order.Work && workOrder == current2)
                        {
                            asc = !asc;
                        }
                        else
                        {
                            orderBy = order.Work;
                            workOrder = current2;
                            asc = false;
                        }
                        BuildPawnList();
                        //Log.Message("Sort order is now " + order.LabelCap + ", " + (asc ? "asc" : "desc"));
                    }
                    Event.current.Use();
                }
                GUI.color = new Color(1f, 1f, 1f, 0.3f);
                Widgets.DrawLineVertical(num5, rect5.yMax - 3f, 50f - rect5.yMax + 3f);
                Widgets.DrawLineVertical(num5 + 1f, rect5.yMax - 3f, 50f - rect5.yMax + 3f);
                GUI.color = Color.white;
                num3 += this.workColumnSpacing;
                num4++;
            }
            base.DrawRows(outRect);
            GUI.EndGroup();
        }

        protected override void DrawPawnRow(Rect rect, Pawn p)
        {
            float num = 175f;
            Text.Font = GameFont.Medium;
            for (int i = 0; i < MainTabWindow_Work.VisibleWorkTypeDefsInPriorityOrder.Count; i++)
            {
                WorkTypeDef workTypeDef = MainTabWindow_Work.VisibleWorkTypeDefsInPriorityOrder[i];
                Vector2 topLeft = new Vector2(num, rect.y + 2.5f);
                WidgetsWork.DrawWorkBoxFor(topLeft, p, workTypeDef);
                Rect rect2 = new Rect(topLeft.x, topLeft.y, 25f, 25f);
                TooltipHandler.TipRegion(rect2, WidgetsWork.TipForPawnWorker(p, workTypeDef));
                num += this.workColumnSpacing;
            }

            if (copied == p)
            {
                Rect rectCancel = new Rect(num + 17f, rect.y + 6f, 16f, 16f);
                if (Widgets.ImageButton(rectCancel, cancelTex))
                {
                    SoundDefOf.ClickReject.PlayOneShotOnCamera();
                    ClearCopied();
                }
                TooltipHandler.TipRegion(rectCancel, "Fluffy.ClearCopy".Translate());
            }
            else
            {
                Rect rectCopy = new Rect(num + 6f, rect.y + 6f, 16f, 16f);
                if (Widgets.ImageButton(rectCopy, copyTex))
                {
                    SoundDefOf.Click.PlayOneShotOnCamera();
                    Copy(p);
                }
                TooltipHandler.TipRegion(rectCopy, "Fluffy.Copy".Translate());
            }
            if (copy != null && copied != p)
            {
                Rect rectPaste = new Rect(num + 28f, rect.y + 6f, 16f, 16f);
                if (Widgets.ImageButton(rectPaste, pasteTex))
                {
                    SoundDefOf.Click.PlayOneShotOnCamera();
                    Paste(p);
                }
                TooltipHandler.TipRegion(rectPaste, "Fluffy.Paste".Translate());
            }
        }
    }
}

