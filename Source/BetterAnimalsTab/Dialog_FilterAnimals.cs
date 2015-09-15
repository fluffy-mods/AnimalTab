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

        public static readonly Texture2D[] MilkableTextures = new Texture2D[]
        {
            ContentFinder<Texture2D>.Get("UI/LifeStage/milkable_large", true),
            ContentFinder<Texture2D>.Get("UI/LifeStage/not_milkable_large", true),
            ContentFinder<Texture2D>.Get("UI/Buttons/all_large", true)
        };

        public static readonly Texture2D[] ShearableTextures = new Texture2D[]
        {
            ContentFinder<Texture2D>.Get("UI/LifeStage/shearable_large", true),
            ContentFinder<Texture2D>.Get("UI/LifeStage/not_shearable_large", true),
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
            Widgets.Label(rect, "Fluffy.FilterByRace".Translate());


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
            Widgets.Label(rectAttributes, "Fluffy.FilterByAttributes".Translate());
            Text.Font = GameFont.Small;

            y += 30f;


            // Gender
            var genderLabel = new StringBuilder();
            genderLabel.Append( "Fluffy.Gender".Translate() ).Append( " " );
            Rect rectGender = new Rect(x, y, colWidth, rowHeight);
            Rect rectGenderLabel = new Rect(x, y, labWidth, rowHeight);
            switch (Filter_Animals.filterGender)
            {
                case Filter_Animals.filterType.True:
                    Rect GenderIconFemale = new Rect(x2 + iconWidthOffset, y, iconSize, iconSize);
                    GUI.DrawTexture(GenderIconFemale, GenderTextures[0]);
                    genderLabel.Append( "(" ).Append( "Female".Translate() ).Append( ")" );
                    break;
                case Filter_Animals.filterType.False:
                    Rect GenderIconMale = new Rect(x2 + iconWidthOffset, y, iconSize, iconSize);
                    GUI.DrawTexture(GenderIconMale, GenderTextures[1]);
                    genderLabel.Append("(").Append("Male".Translate()).Append(")");
                    break;
                default:
                    Rect GenderIconBoth = new Rect(x2 + iconWidthOffset, y, iconSize, iconSize);
                    GUI.DrawTexture(GenderIconBoth, GenderTextures[2]);
                    genderLabel.Append("(").Append("Fluffy.Both".Translate()).Append(")");
                    break;
            }
            Widgets.Label(rectGenderLabel, genderLabel.ToString());
            if (Widgets.InvisibleButton(rectGender))
            {
                Filter_Animals.filterGender = Filter_Animals.bump(Filter_Animals.filterGender);
                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                MainTabWindow_Animals.isDirty = true;
            }
            if (Mouse.IsOver(rectGender))
            {
                GUI.DrawTexture(rectGender, TexUI.HighlightTex);
            }
            y += 30;

            // Reproductive
            var reproLabel = new StringBuilder();
            reproLabel.Append("Fluffy.Lifestage".Translate() ).Append( " " );
            Rect rectRepro = new Rect(x, y, colWidth, rowHeight);
            Rect rectReproLabel = new Rect(x, y, labWidth, rowHeight);
            switch (Filter_Animals.filterReproductive)
            {
                case Filter_Animals.filterType.True:
                    Rect ReproIconTrue = new Rect(x2 + iconWidthOffset, y, iconSize, iconSize);
                    GUI.DrawTexture(ReproIconTrue, ReproTextures[0]);
                    reproLabel.Append("(").Append("Fluffy.LifestageReproductive".Translate()).Append(")");
                    break;
                case Filter_Animals.filterType.False:
                    Rect ReproIconFalse = new Rect(x2 + iconWidthOffset, y, iconSize, iconSize);
                    GUI.DrawTexture(ReproIconFalse, ReproTextures[1]);
                    reproLabel.Append("(").Append("Fluffy.LifestageNotReproductive".Translate()).Append(")");
                    break;
                default:
                    Rect ReproIconBoth = new Rect(x2 + iconWidthOffset, y, iconSize, iconSize);
                    GUI.DrawTexture(ReproIconBoth, ReproTextures[2]);
                    reproLabel.Append("(").Append("Fluffy.Both".Translate()).Append(")");
                    break;
            }
            Widgets.Label(rectReproLabel, reproLabel.ToString());
            if (Widgets.InvisibleButton(rectRepro))
            {
                Filter_Animals.filterReproductive = Filter_Animals.bump(Filter_Animals.filterReproductive);
                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                MainTabWindow_Animals.isDirty = true;
            }
            if (Mouse.IsOver(rectRepro))
            {
                GUI.DrawTexture(rectRepro, TexUI.HighlightTex);
            }
            y += 30;

            // Training
            var trainingLabel = new StringBuilder();
            trainingLabel.Append("Fluffy.Training".Translate()).Append( " " );
            Rect rectTrained = new Rect(x, y, colWidth, rowHeight);
            Rect rectTrainedLabel = new Rect(x, y, labWidth, rowHeight);
            switch (Filter_Animals.filterTamed)
            {
                case Filter_Animals.filterType.True:
                    Rect TrainedIconTrue = new Rect(x2 + iconWidthOffset, y, iconSize, iconSize);
                    GUI.DrawTexture(TrainedIconTrue, TrainingTextures[0]);
                    trainingLabel.Append("(").Append("Fluffy.Obedience".Translate()).Append(")");
                    break;
                case Filter_Animals.filterType.False:
                    Rect TrainedIconFalse = new Rect(x2 + iconWidthOffset, y, iconSize, iconSize);
                    GUI.DrawTexture(TrainedIconFalse, TrainingTextures[1]);
                    trainingLabel.Append("(").Append("Fluffy.NotObedience".Translate()).Append(")");
                    break;
                default:
                    Rect TrainedIconBoth = new Rect(x2 + iconWidthOffset, y, iconSize, iconSize);
                    GUI.DrawTexture(TrainedIconBoth, TrainingTextures[2]);
                    trainingLabel.Append("(").Append("Fluffy.Both".Translate()).Append(")");
                    break;
            }
            Widgets.Label(rectTrainedLabel, trainingLabel.ToString());
            if (Widgets.InvisibleButton(rectTrained))
            {
                Filter_Animals.filterTamed = Filter_Animals.bump(Filter_Animals.filterTamed);
                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                MainTabWindow_Animals.isDirty = true;
            }
            if (Mouse.IsOver(rectTrained))
            {
                GUI.DrawTexture(rectTrained, TexUI.HighlightTex);
            }
            y += 30;

            // Milkable
            var milkableLabel = new StringBuilder();
            milkableLabel.Append("Fluffy.Milkable".Translate()).Append( " " );
            Rect rectMilkable = new Rect(x, y, colWidth, rowHeight);
            Rect rectMilkableLabel = new Rect(x, y, labWidth, rowHeight);
            switch (Filter_Animals.filterMilkable)
            {
                case Filter_Animals.filterType.True:
                    Rect MilkableIconTrue = new Rect(x2 + iconWidthOffset, y, iconSize, iconSize);
                    GUI.DrawTexture(MilkableIconTrue, MilkableTextures[0]);
                    milkableLabel.Append( "(" ).Append( "Fluffy.Yes".Translate() ).Append( ")" );
                    break;
                case Filter_Animals.filterType.False:
                    Rect MilkableIconFalse = new Rect(x2 + iconWidthOffset, y, iconSize, iconSize);
                    GUI.DrawTexture(MilkableIconFalse, MilkableTextures[1]);
                    milkableLabel.Append("(").Append("Fluffy.No".Translate()).Append(")");
                    break;
                default:
                    Rect MilkableIconBoth = new Rect(x2 + iconWidthOffset, y, iconSize, iconSize);
                    GUI.DrawTexture(MilkableIconBoth, MilkableTextures[2]);
                    milkableLabel.Append("(").Append("Fluffy.Both".Translate()).Append(")");
                    break;
            }
            Widgets.Label(rectMilkableLabel, milkableLabel.ToString());
            if (Widgets.InvisibleButton(rectMilkable))
            {
                Filter_Animals.filterMilkable = Filter_Animals.bump(Filter_Animals.filterMilkable);
                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                MainTabWindow_Animals.isDirty = true;
            }
            if (Mouse.IsOver(rectMilkable))
            {
                GUI.DrawTexture(rectMilkable, TexUI.HighlightTex);
            }
            y += 30;

            // Shearable
            var shearableLabel = new StringBuilder();
            shearableLabel.Append("Fluffy.Shearable".Translate()).Append( " " );
            Rect rectShearable = new Rect(x, y, colWidth, rowHeight);
            Rect rectShearableLabel = new Rect(x, y, labWidth, rowHeight);
            switch (Filter_Animals.filterShearable)
            {
                case Filter_Animals.filterType.True:
                    Rect ShearableIconTrue = new Rect(x2 + iconWidthOffset, y, iconSize, iconSize);
                    GUI.DrawTexture(ShearableIconTrue, ShearableTextures[0]);
                    shearableLabel.Append("(").Append("Fluffy.Yes".Translate()).Append(")");
                    break;
                case Filter_Animals.filterType.False:
                    Rect ShearableIconFalse = new Rect(x2 + iconWidthOffset, y, iconSize, iconSize);
                    GUI.DrawTexture(ShearableIconFalse, ShearableTextures[1]);
                    shearableLabel.Append("(").Append("Fluffy.No".Translate()).Append(")");
                    break;
                default:
                    Rect ShearableIconBoth = new Rect(x2 + iconWidthOffset, y, iconSize, iconSize);
                    GUI.DrawTexture(ShearableIconBoth, ShearableTextures[2]);
                    shearableLabel.Append("(").Append("Fluffy.Both".Translate()).Append(")");
                    break;
            }
            Widgets.Label(rectShearableLabel, shearableLabel.ToString());
            if (Widgets.InvisibleButton(rectShearable))
            {
                Filter_Animals.filterShearable = Filter_Animals.bump(Filter_Animals.filterShearable);
                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                MainTabWindow_Animals.isDirty = true;
            }
            if (Mouse.IsOver(rectShearable))
            {
                GUI.DrawTexture(rectShearable, TexUI.HighlightTex);
            }
            y += 30;



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
    }
}