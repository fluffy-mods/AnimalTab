using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

using RimWorld;

namespace Fluffy
{
    public class Dialog_FilterAnimals : Window
    {
        public List<PawnKindDef> pawnKinds;

        public static bool sticky = false;

        public static Rect location;

        public override void PreClose()
        {
            base.PreClose();
            location = currentWindowRect;
        }

        // kinda hacky, but sticky can't be set without opening, which populates location.
        public override void PostOpen()
        {
            base.PostOpen();
            if (sticky)
            {
                currentWindowRect = location;
            }
        }

        private float _actualHeight = 500f;

        public override Vector2 InitialWindowSize
        {
            get
            {
                return new Vector2(600f, 65f + Mathf.Max(200f, _actualHeight));
            }
        }

        public Dialog_FilterAnimals()
        {
            this.pawnKinds = Find.ListerPawns.PawnsInFaction(Faction.OfColony).Where(x => x.RaceProps.Animal)
                .Select(x => x.kindDef).Distinct().OrderBy(x => x.LabelCap).ToList();
            this.forcePause = true;
            this.closeOnEscapeKey = true;
            this.absorbInputAroundWindow = false;
            this.draggable = true;
        }

        float colWidth
        {
            get
            {
                return (InitialWindowSize.x / 2f) - 10f;
            }
        }

        float rowHeight = 30f;
        float iconSize = 24f;
        float labWidth => colWidth - 50f;
        float iconWidthOffset = (50f - 24f) / 2f;
        float iconHeightOffset => (rowHeight - iconSize) / 2f;

        float x, y, x2;

        public override void DoWindowContents(Rect inRect)
        {
            // Close if animals tab closed
            if (Find.WindowStack.WindowOfType<MainTabWindow_Animals>() == null)
            {
                Find.WindowStack.TryRemove(this);
            }

            Text.Font = GameFont.Small;
            

            // Pawnkinds on the left.
            x = 5f;
            y = 5f;
            x2 = colWidth - 45f;
            
            Rect rect = new Rect(x, y, colWidth, rowHeight);
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.LowerLeft;
            Widgets.Label(rect, "Fluffy.FilterByRace".Translate());
            Text.Font = GameFont.Small;


            y += rowHeight;

            foreach (PawnKindDef pawnKind in pawnKinds)
            {
                DrawPawnKindRow(pawnKind);
            }

            // set window's actual height
            if (y > _actualHeight) _actualHeight = y;

            // specials on the right.
            x = inRect.width / 2f + 5f;
            x2 = inRect.width / 2f + colWidth - 45f;
            y = 5f;

            Text.Font = GameFont.Tiny;
            Rect rectAttributes = new Rect(x, 5f, colWidth, rowHeight);
            Widgets.Label(rectAttributes, "Fluffy.FilterByAttributes".Translate());
            Text.Font = GameFont.Small;

            y += rowHeight;

            // draw filter rows.
            foreach (IFilter filter in Filter_Animals.Filters)
            {
                DrawFilterRow(filter);
            }

            // set window's actual height
            if (y > _actualHeight) _actualHeight = y;

            // sticky option
            Rect stickyRect = new Rect(5f, inRect.height - 35f, (inRect.width / 4) - 10, 35f);
            Widgets.LabelCheckbox(stickyRect, "Fluffy.FilterSticky".Translate(), ref sticky);


            // buttons
            if (Widgets.TextButton(new Rect(inRect.width / 4f + 5f, inRect.height - 35f, inRect.width / 4f - 10f, 35f),
                                            "Fluffy.Clear".Translate()))
            {
                Filter_Animals.resetFilter();
                Filter_Animals.disableFilter();
                MainTabWindow_Animals.isDirty = true;
                Event.current.Use();
            }

            if (!Filter_Animals.filter)
            {
                if (Widgets.TextButton(new Rect(x, inRect.height - 35f, inRect.width / 4f - 10f, 35f),
                                            "Fluffy.Enable".Translate()))
                {
                    Filter_Animals.enableFilter();
                    MainTabWindow_Animals.isDirty = true;
                    Event.current.Use();
                }
            }
            else
            {
                if (Widgets.TextButton(new Rect(x, inRect.height - 35f, inRect.width / 4f - 10f, 35f),
                                            "Fluffy.Disable".Translate()))
                {
                    Filter_Animals.disableFilter();
                    MainTabWindow_Animals.isDirty = true;
                    Event.current.Use();
                }
            }


            if (Widgets.TextButton(new Rect(x + inRect.width / 4, inRect.height - 35f, inRect.width / 4f - 10f, 35f),
                                            "OK".Translate()))
            {
                Find.WindowStack.TryRemove(this);
                Event.current.Use();
            }

            Text.Anchor = TextAnchor.UpperLeft;
        }

        public void DrawPawnKindRow(PawnKindDef pawnKind)
        {
            Rect rectRow = new Rect(x, y, colWidth, rowHeight);
            Rect rectLabel = new Rect(x, y, labWidth, rowHeight);
            Widgets.Label(rectLabel, pawnKind.LabelCap);
            Rect rectIcon = new Rect(x2 + iconWidthOffset, y, iconSize, iconSize);
            bool inList = Filter_Animals.filterPawnKind.Contains(pawnKind);
            GUI.DrawTexture(rectIcon, inList ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex);
            if (Mouse.IsOver(rectRow))
            {
                GUI.DrawTexture(rectRow, TexUI.HighlightTex);
            }
            if (Widgets.InvisibleButton(rectRow))
            {
                if (inList)
                {
                    SoundDefOf.CheckboxTurnedOff.PlayOneShotOnCamera();
                }
                else
                {
                    SoundDefOf.CheckboxTurnedOn.PlayOneShotOnCamera();
                }
                Filter_Animals.togglePawnKindFilter(pawnKind, inList);
                MainTabWindow_Animals.isDirty = true;
            }
            y += rowHeight;
        }

        public void DrawFilterRow(IFilter filter)
        {
            var label = new StringBuilder();
            label.Append(("Fluffy." + filter.label + "Label").Translate()).Append(" ");
            Rect rect = new Rect(x, y, colWidth, rowHeight);
            Rect rectLabel = new Rect(x, y, labWidth, rowHeight);
            Rect rectIcon = new Rect(x2 + iconWidthOffset, y, iconSize, iconSize);
            switch (filter.state)
            {
                case FilterType.True:
                    GUI.DrawTexture(rectIcon, filter.textures[0]);
                    label.Append("(").Append(("Fluffy." + filter.label + "Yes").Translate()).Append(")");
                    break;
                case FilterType.False:
                    GUI.DrawTexture(rectIcon, filter.textures[1]);
                    label.Append("(").Append(("Fluffy." + filter.label + "No").Translate()).Append(")");
                    break;
                default:
                    GUI.DrawTexture(rectIcon, filter.textures[2]);
                    label.Append("(").Append(("Fluffy.Both").Translate()).Append(")");
                    break;
            }
            Widgets.Label(rectLabel, label.ToString());
            if (Widgets.InvisibleButton(rect))
            {
                filter.bump();
                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                MainTabWindow_Animals.isDirty = true;
            }
            if (Mouse.IsOver(rect))
            {
                GUI.DrawTexture(rect, TexUI.HighlightTex);
            }
            y += rowHeight;
        }
    }
}