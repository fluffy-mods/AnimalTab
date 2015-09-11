using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

using RimWorld;

namespace Fluffy
{
    public class Dialog_FilterAnimals : Window
    {
        public List<PawnKindDef> pawnKinds;

        public static readonly Texture2D[] GenderTextures = new Texture2D[]
        {
            ContentFinder<Texture2D>.Get("UI/Gender/female", true),
            ContentFinder<Texture2D>.Get("UI/Gender/male", true),
            ContentFinder<Texture2D>.Get("UI/Buttons/all_large", true)
        };

        public static readonly Texture2D[] ReproTextures = new Texture2D[]
        {
            ContentFinder<Texture2D>.Get("UI/LifeStage/reproductive_large", true),
            ContentFinder<Texture2D>.Get("UI/LifeStage/not_reproductive_large", true),
            ContentFinder<Texture2D>.Get("UI/Buttons/all_large", true)
        };

        public static readonly Texture2D[] TrainingTextures = new Texture2D[]
        {
            ContentFinder<Texture2D>.Get("UI/Training/obedience_large", true),
            ContentFinder<Texture2D>.Get("UI/Training/not_obedience_large", true),
            ContentFinder<Texture2D>.Get("UI/Buttons/all_large", true)
        };


        public override Vector2 InitialWindowSize
        {
            get
            {
                float dynHeight = (float)Find.ListerPawns.PawnsInFaction(Faction.OfColony).Where(x => x.RaceProps.Animal)
                                          .Select(x => x.kindDef).Distinct().OrderBy(x => x.LabelCap).Count() * 30f + 65f;
                return new Vector2(600f, 65f + Mathf.Max(200f, dynHeight));
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

        public override void DoWindowContents(Rect inRect)
        {
            // Close if animals tab closed
            if (Find.WindowStack.WindowOfType<MainTabWindow_Animals>() == null)
            {
                Find.WindowStack.TryRemove(this, true);
            }

            Text.Font = GameFont.Small;
            

            // Pawnkinds on the left.
            float x = 5f;
            float y = 5f;
            float colWidth = (inRect.width / 2f) - 10f;
            float x2 = colWidth - 45f;
            float rowHeight = 30f;
            float iconSize = 24f;
            float labWidth = colWidth - 50f;
            float iconWidthOffset = (50f - 24f) / 2f;
            float iconHeightOffset = (rowHeight - iconSize) / 2f;
            

            Rect rect = new Rect(x, y, colWidth, rowHeight);
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.LowerLeft;
            Widgets.Label(rect, "Filter by race");


            y += 30f;
            Text.Font = GameFont.Small;
            for (int i = 0; i < pawnKinds.Count; i++)
            {
                Rect rectRow = new Rect(x, y, colWidth, rowHeight);
                Rect rectLabel = new Rect(x, y, labWidth, rowHeight);
                Widgets.Label(rectLabel, pawnKinds[i].LabelCap);
                Rect rectIcon = new Rect(x2 + iconWidthOffset, y, iconSize, iconSize);
                bool inList = Filter_Animals.filterPawnKind.Contains(pawnKinds[i]);
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
                    Filter_Animals.togglePawnKindFilter(pawnKinds[i], inList);
                    MainTabWindow_Animals.isDirty = true;
                }
                y += 30;
            }

            // specials on the right.
            x = inRect.width / 2f + 5f;
            x2 = inRect.width / 2f + colWidth - 45f;
            y = 5f;

            Text.Font = GameFont.Tiny;
            Rect rectAttributes = new Rect(x, 5f, colWidth, rowHeight);
            Widgets.Label(rectAttributes, "Filter by attributes");
            Text.Font = GameFont.Small;

            y += 30f;


            // Gender
            string genderLabel = "Gender ";
            Rect rectGender = new Rect(x, y, colWidth, rowHeight);
            Rect rectGenderLabel = new Rect(x, y, labWidth, rowHeight);
            switch (Filter_Animals.filterGender)
            {
                case Filter_Animals.filterType.True:
                    Rect GenderIconFemale = new Rect(x2 + iconWidthOffset, y, iconSize, iconSize);
                    GUI.DrawTexture(GenderIconFemale, GenderTextures[0]);
                    genderLabel += "(female)";
                    break;
                case Filter_Animals.filterType.False:
                    Rect GenderIconMale = new Rect(x2 + iconWidthOffset, y, iconSize, iconSize);
                    GUI.DrawTexture(GenderIconMale, GenderTextures[1]);
                    genderLabel += "(male)";
                    break;
                case Filter_Animals.filterType.None:
                default:
                    Rect GenderIconBoth = new Rect(x2 + iconWidthOffset, y, iconSize, iconSize);
                    GUI.DrawTexture(GenderIconBoth, GenderTextures[2]);
                    genderLabel += "(both)";
                    break;
            }
            Widgets.Label(rectGenderLabel, genderLabel);
            if (Widgets.InvisibleButton(rectGender))
            {
                Filter_Animals.filterGender = Filter_Animals.bump(Filter_Animals.filterGender);
                Log.Message(Filter_Animals.filterGender.ToString());
                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                MainTabWindow_Animals.isDirty = true;
            }
            if (Mouse.IsOver(rectGender))
            {
                GUI.DrawTexture(rectGender, TexUI.HighlightTex);
            }
            y += 30;

            // Reproductive
            string reproLabel = "Lifestage ";
            Rect rectRepro = new Rect(x, y, colWidth, rowHeight);
            Rect rectReproLabel = new Rect(x, y, labWidth, rowHeight);
            switch (Filter_Animals.filterReproductive)
            {
                case Filter_Animals.filterType.True:
                    Rect ReproIconTrue = new Rect(x2 + iconWidthOffset, y, iconSize, iconSize);
                    GUI.DrawTexture(ReproIconTrue, ReproTextures[0]);
                    reproLabel += "(reproductive)";
                    break;
                case Filter_Animals.filterType.False:
                    Rect ReproIconFalse = new Rect(x2 + iconWidthOffset, y, iconSize, iconSize);
                    GUI.DrawTexture(ReproIconFalse, ReproTextures[1]);
                    reproLabel += "(not reproductive)";
                    break;
                case Filter_Animals.filterType.None:
                default:
                    Rect ReproIconBoth = new Rect(x2 + iconWidthOffset, y, iconSize, iconSize);
                    GUI.DrawTexture(ReproIconBoth, ReproTextures[2]);
                    reproLabel += "(both)";
                    break;
            }
            Widgets.Label(rectReproLabel, reproLabel);
            if (Widgets.InvisibleButton(rectRepro))
            {
                Filter_Animals.filterReproductive = Filter_Animals.bump(Filter_Animals.filterReproductive);
                //Log.Message(Filter_Animals.filterReproductive.ToString());
                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                MainTabWindow_Animals.isDirty = true;
            }
            if (Mouse.IsOver(rectRepro))
            {
                GUI.DrawTexture(rectRepro, TexUI.HighlightTex);
            }
            y += 30;

            // Training
            string trainingLabel = "Training ";
            Rect rectTrained = new Rect(x, y, colWidth, rowHeight);
            Rect rectTrainedLabel = new Rect(x, y, labWidth, rowHeight);
            switch (Filter_Animals.filterTamed)
            {
                case Filter_Animals.filterType.True:
                    Rect TrainedIconTrue = new Rect(x2 + iconWidthOffset, y, iconSize, iconSize);
                    GUI.DrawTexture(TrainedIconTrue, TrainingTextures[0]);
                    trainingLabel += "(obedience trained)";
                    break;
                case Filter_Animals.filterType.False:
                    Rect TrainedIconFalse = new Rect(x2 + iconWidthOffset, y, iconSize, iconSize);
                    GUI.DrawTexture(TrainedIconFalse, TrainingTextures[1]);
                    trainingLabel += "(not obedience trained)";
                    break;
                case Filter_Animals.filterType.None:
                default:
                    Rect TrainedIconBoth = new Rect(x2 + iconWidthOffset, y, iconSize, iconSize);
                    GUI.DrawTexture(TrainedIconBoth, TrainingTextures[2]);
                    trainingLabel += "(both)";
                    break;
            }
            Widgets.Label(rectTrainedLabel, trainingLabel);
            if (Widgets.InvisibleButton(rectTrained))
            {
                Filter_Animals.filterTamed = Filter_Animals.bump(Filter_Animals.filterTamed);
                Log.Message(Filter_Animals.filterTamed.ToString());
                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                MainTabWindow_Animals.isDirty = true;
            }
            if (Mouse.IsOver(rectTrained))
            {
                GUI.DrawTexture(rectTrained, TexUI.HighlightTex);
            }
            y += 30;


            if (Widgets.TextButton(new Rect(inRect.width / 4f + 5f, inRect.height - 35f, inRect.width / 4f - 10f, 35f), "Clear", true, false))
            {
                Filter_Animals.resetFilter();
                Filter_Animals.disableFilter();
                MainTabWindow_Animals.isDirty = true;
                Event.current.Use();
            }

            if (!Filter_Animals.filter)
            {
                if (Widgets.TextButton(new Rect(x, inRect.height - 35f, inRect.width / 4f - 10f, 35f), "Enable", true, false))
                {
                    Filter_Animals.enableFilter();
                    MainTabWindow_Animals.isDirty = true;
                    Event.current.Use();
                }
            }
            else
            {
                if (Widgets.TextButton(new Rect(x, inRect.height - 35f, inRect.width / 4f - 10f, 35f), "Disable", true, false))
                {
                    Filter_Animals.disableFilter();
                    MainTabWindow_Animals.isDirty = true;
                    Event.current.Use();
                }
            }


            if (Widgets.TextButton(new Rect(x + inRect.width / 4, inRect.height - 35f, inRect.width / 4f - 10f, 35f), "OK".Translate(), true, false))
            {
                Find.WindowStack.TryRemove(this, true);
                Event.current.Use();
            }

            Text.Anchor = TextAnchor.UpperLeft;
        }
    }
}