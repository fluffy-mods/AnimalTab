using System;
using System.Linq;
using UnityEngine;
using Verse;

using RimWorld;

namespace Fluffy
{
    public class MainTabWindow_Animals : MainTabWindow_PawnList
    {
        private const float TopAreaHeight = 65f;

        private const float MasterWidth = 90f;

        private const float AreaAllowedWidth = 350f;

        public override Vector2 RequestedTabSize
        {
            get
            {
                return new Vector2(1050f, 65f + (float)base.PawnsCount * 30f + 65f);
            }
        }

        private static readonly Texture2D[] GenderTextures = new Texture2D[]
        {
            ContentFinder<Texture2D>.Get("UI/Gender/none", true),
            ContentFinder<Texture2D>.Get("UI/Gender/male", true),
            ContentFinder<Texture2D>.Get("UI/Gender/female", true)
        };

        private static readonly Texture2D[] LifeStageTextures = new Texture2D[]
        {
            ContentFinder<Texture2D>.Get("UI/LifeStage/1", true),
            ContentFinder<Texture2D>.Get("UI/LifeStage/2", true),
            ContentFinder<Texture2D>.Get("UI/LifeStage/3", true),
            ContentFinder<Texture2D>.Get("UI/LifeStage/unknown", true)
        };

        protected override void BuildPawnList()
        {
            this.pawns = (from p in Find.ListerPawns.PawnsInFaction(Faction.OfColony)
                          where p.RaceProps.Animal
                          orderby p.RaceProps.petness descending, p.RaceProps.baseBodySize, p.def.label
                          select p).ToList<Pawn>();
        }

        public override void DoWindowContents(Rect fillRect)
        {
            base.DoWindowContents(fillRect);
            Rect position = new Rect(0f, 0f, fillRect.width, 65f);
            GUI.BeginGroup(position);
            float num = 175f;
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.LowerLeft;
            Rect rect = new Rect(num, 0f, 90f, position.height + 3f);
            Widgets.Label(rect, "Master".Translate());
            num += 90f;

            float x = 16f;

            Rect recta1 = new Rect(num + 9, 48f, x, x);
            GUI.DrawTexture(recta1, GenderTextures[1]);
            num += 25f;

            Rect recta2 = new Rect(num, 48f, x, x);
            GUI.DrawTexture(recta2, GenderTextures[2]);
            num += 25f;


            Rect rectb1 = new Rect(num + 1, 48f, x, x);
            GUI.DrawTexture(rectb1, LifeStageTextures[0]);
            num += 17f;

            Rect rectb2 = new Rect(num, 48f, x, x);
            GUI.DrawTexture(rectb2, LifeStageTextures[1]);
            num += 16f;

            Rect rectb3 = new Rect(num, 48f, x, x);
            GUI.DrawTexture(rectb3, LifeStageTextures[2]);
            num += 17f;

            Rect rect2 = new Rect(num, 0f, 350f, Mathf.Round(position.height / 2f));
            Text.Font = GameFont.Small;
            if (Widgets.TextButton(rect2, "ManageAreas".Translate(), true, false))
            {
                Find.WindowStack.Add(new Dialog_ManageAreas());
            }
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.LowerCenter;
            Rect rect3 = new Rect(num, 0f, 350f, position.height + 3f);
            Widgets.Label(rect3, "AllowedArea".Translate());
            num += 350f;
            GUI.EndGroup();
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;
            Rect outRect = new Rect(0f, position.height, fillRect.width, fillRect.height - position.height);
            base.DrawRows(outRect);
        }

        protected override void DrawPawnRow(Rect rect, Pawn p)
        {
            GUI.BeginGroup(rect);
            float num = 175f;
            if (p.training.IsCompleted(TrainableDefOf.Obedience))
            {
                Rect rect2 = new Rect(num, 0f, 90f, rect.height);
                Rect rect3 = rect2.ContractedBy(2f);
                string label = (p.playerSettings.master == null) ? "NoneLower".Translate() : p.playerSettings.master.LabelBaseShort;
                Text.Font = GameFont.Small;
                if (Widgets.TextButton(rect3, label, true, false))
                {
                    TrainableUtility.OpenMasterSelectMenu(p);
                }
            }
            num += 90f;

            float x = 16f;
            float heightOffset = (rect.height - 16) / 2;
            float widthOffset = (50 - 16) / 2;

            Rect recta = new Rect(num + widthOffset, heightOffset, x, x);
            Texture2D labelSex = GenderTextures[(int)p.gender];
            GUI.DrawTexture(recta, labelSex);
            num += 50f;

            Rect rectb = new Rect(num + widthOffset, heightOffset, x, x);
            Texture2D labelAge;
            if (p.RaceProps.lifeStageAges.Count > 3)
            {
                labelAge = LifeStageTextures[3];
            } else
            {
                labelAge = LifeStageTextures[p.ageTracker.CurLifeStageIndex];
            }
            GUI.DrawTexture(rectb, labelAge);
            num += 50f;
            
            Rect rect4 = new Rect(num, 0f, 350f, rect.height);
            AreaAllowedGUI.DoAllowedAreaSelectors(rect4, p, AllowedAreaMode.Animal);
            num += 350f;
            GUI.EndGroup();
        }
    }
}
