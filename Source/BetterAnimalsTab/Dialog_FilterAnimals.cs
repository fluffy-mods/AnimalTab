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
            this.absorbInputAroundWindow = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;
            // Pawnkinds on the left.
            float num = 35f;
            float x = 5f;
            float colWidth = (inRect.width / 2f) - 60f;
            float x2 = colWidth + 5f;

            Rect rect = new Rect(5f, 5f, colWidth, 30f);
            Widgets.Label(rect, "Filter by race");
            Text.Anchor = TextAnchor.LowerLeft;
            for (int i = 0; i < pawnKinds.Count; i++)
            {
                Rect recta = new Rect(x, num, colWidth, 30f);
                Widgets.Label(recta, pawnKinds[i].LabelCap);
                Rect rectb = new Rect(x2, num, 30f, 30f);
                bool inList = Filter_Animals.filterPawnKind == null || Filter_Animals.filterPawnKind.Contains(pawnKinds[i]);
                if (Widgets.ImageButton(rectb, inList ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex))
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
                num += 30;
            }

            // specials on the right.
            x = inRect.width / 2f + 5f;
            x2 = inRect.width / 2f + 205f;
            num = 35f;


            Rect rectAttributes = new Rect(x, 5f, colWidth, 30f);
            Widgets.Label(rectAttributes, "Filter by attributes");
            // gender
            Filter_Animals.filterType gender = Filter_Animals.filterGender;
            string genderLabel = "Gender ";
            Texture2D genderTex;
            switch (gender)
            {
                case Filter_Animals.filterType.True:
                    genderTex = MainTabWindow_Animals.GenderTextures[2];
                    genderLabel += "(female)";
                    break;
                case Filter_Animals.filterType.False:
                    genderTex = MainTabWindow_Animals.GenderTextures[1];
                    genderLabel += "(male)";
                    break;
                case Filter_Animals.filterType.None:
                default:
                    genderTex = Widgets.CheckboxPartialTex;
                    genderLabel += "(all)";
                    break;
            }

            Rect rectGender1 = new Rect(x, num, colWidth, 30f);
            Widgets.Label(rectGender1, genderLabel);
            Rect rectGender2 = new Rect(x2, num, 30f, 30f);
            if (Widgets.ImageButton(rectGender2, genderTex))
            {
                Filter_Animals.filterGender = Filter_Animals.bump(Filter_Animals.filterGender);
                Log.Message(Filter_Animals.filterGender.ToString());
                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                MainTabWindow_Animals.isDirty = true;
            }
            num += 30;

            // reproductive
            Filter_Animals.filterType repro = Filter_Animals.filterReproductive;
            string reproLabel = "Reproductive ";
            Texture2D reproTex;
            switch (repro)
            {
                case Filter_Animals.filterType.True:
                    reproTex = MainTabWindow_Animals.GenderTextures[2];
                    reproLabel += "(reproductive)";
                    break;
                case Filter_Animals.filterType.False:
                    reproTex = MainTabWindow_Animals.GenderTextures[1];
                    reproLabel += "(not reproductive)";
                    break;
                case Filter_Animals.filterType.None:
                default:
                    reproTex = Widgets.CheckboxPartialTex;
                    reproLabel += "(all)";
                    break;
            }

            Rect rectrepro1 = new Rect(x, num, colWidth, 30f);
            Widgets.Label(rectrepro1, reproLabel);
            Rect rectrepro2 = new Rect(x2, num, 30f, 30f);
            if (Widgets.ImageButton(rectrepro2, reproTex))
            {
                Filter_Animals.filterReproductive = Filter_Animals.bump(Filter_Animals.filterReproductive);
                Log.Message(Filter_Animals.filterReproductive.ToString());
                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                MainTabWindow_Animals.isDirty = true;
            }
            num += 30f;

            // Trained
            Filter_Animals.filterType tamed = Filter_Animals.filterTamed;
            string tamedLabel = "Filter by obedience ";
            Texture2D tamedTex;
            switch (tamed)
            {
                case Filter_Animals.filterType.True:
                    tamedTex = MainTabWindow_Animals.GenderTextures[2];
                    tamedLabel += "(trained)";
                    break;
                case Filter_Animals.filterType.False:
                    tamedTex = MainTabWindow_Animals.GenderTextures[1];
                    tamedLabel += "(not trained)";
                    break;
                case Filter_Animals.filterType.None:
                default:
                    tamedTex = Widgets.CheckboxPartialTex;
                    tamedLabel += "(both)";
                    break;
            }

            Rect recttamed1 = new Rect(x, num, colWidth, 30f);
            Widgets.Label(recttamed1, tamedLabel);
            Rect recttamed2 = new Rect(x2, num, 30f, 30f);
            if (Widgets.ImageButton(recttamed2, tamedTex))
            {
                Filter_Animals.filterTamed = Filter_Animals.bump(Filter_Animals.filterTamed);
                Log.Message(Filter_Animals.filterTamed.ToString());
                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                MainTabWindow_Animals.isDirty = true;
            }


            if (Widgets.TextButton(new Rect(20f, inRect.height - 35f, inRect.width / 2f - 20f, 35f), "Cancel".Translate(), true, false))
            {
                Filter_Animals.disableFilter();
                Find.WindowStack.TryRemove(this, true);
                Event.current.Use();
            }

            if (Widgets.TextButton(new Rect(inRect.width / 2f + 20f, inRect.height - 35f, inRect.width / 2f - 20f, 35f), "OK".Translate(), true, false))
            {
                Find.WindowStack.WindowOfType<MainTabWindow_Animals>().WindowUpdate();
                Find.WindowStack.TryRemove(this, true);
                Event.current.Use();
            }

            Text.Anchor = TextAnchor.UpperLeft;
        }
    }
}