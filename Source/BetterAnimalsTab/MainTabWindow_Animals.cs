using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

using RimWorld;

namespace Fluffy
{
    public class MainTabWindow_Animals : Fluffy.MainTabWindow_PawnList
    {
        private const float TopAreaHeight = 65f;

        private const float MasterWidth = 90f;

        private const float AreaAllowedWidth = 350f;

        private enum orders
        {
            Default,
            Name,
            Gender,
            LifeStage,
            Slaughter
        }

        private orders order = orders.Default;

        private bool asc = false;

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

        private static readonly Texture2D WorkBoxCheckTex = ContentFinder<Texture2D>.Get("UI/Widgets/WorkBoxCheck", true);
        private static readonly Texture2D SlaughterTex = ContentFinder<Texture2D>.Get("UI/Buttons/slaughter", true);

        protected override void BuildPawnList()
        {
            IEnumerable<Pawn> sorted;
            switch (order)
            {
                case orders.Default:
                    sorted = from p in Find.ListerPawns.PawnsInFaction(Faction.OfColony)
                             where p.RaceProps.Animal
                             orderby p.RaceProps.petness descending, p.RaceProps.baseBodySize, p.def.label
                             select p;
                    break;
                case orders.Name:
                    sorted = from p in Find.ListerPawns.PawnsInFaction(Faction.OfColony)
                             where p.RaceProps.Animal
                             orderby p.Name.Numerical, p.Name.ToStringFull, p.def.label
                             select p;
                    break;
                case orders.Gender:
                    sorted = from p in Find.ListerPawns.PawnsInFaction(Faction.OfColony)
                             where p.RaceProps.Animal
                             orderby p.KindLabel, p.gender
                             select p;
                    break;
                case orders.LifeStage:
                    sorted = from p in Find.ListerPawns.PawnsInFaction(Faction.OfColony)
                             where p.RaceProps.Animal
                             orderby p.ageTracker.CurLifeStageRace.minAge descending, p.ageTracker.AgeBiologicalTicks descending
                             select p;
                    break;
                case orders.Slaughter:
                    sorted = from p in Find.ListerPawns.PawnsInFaction(Faction.OfColony)
                             where p.RaceProps.Animal
                             orderby Find.DesignationManager.DesignationOn(p, DesignationDefOf.Slaughter) != null descending, p.BodySize descending
                             select p;
                    break;
                default:
                    sorted = from p in Find.ListerPawns.PawnsInFaction(Faction.OfColony)
                             where p.RaceProps.Animal
                             orderby p.RaceProps.petness descending, p.RaceProps.baseBodySize, p.def.label
                             select p;
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

        public override void DoWindowContents(Rect fillRect)
        {
            base.DoWindowContents(fillRect);
            Rect position = new Rect(0f, 0f, fillRect.width, 65f);
            GUI.BeginGroup(position);
            float num = 175f;
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.LowerCenter;
            Rect rectname = new Rect(0f, 0f, num, position.height + 3f);
            Widgets.Label(rectname, "Name");
            if (Widgets.InvisibleButton(rectname))
            {
                if (order == orders.Name)
                {
                    asc = !asc;
                }
                else
                {
                    order = orders.Name;
                    asc = false;
                }
                BuildPawnList();
            }
            TooltipHandler.TipRegion(rectname, "Click to sort by name.");

            Text.Anchor = TextAnchor.LowerLeft;
            Rect rect = new Rect(num, 0f, 90f, position.height + 3f);
            Widgets.Label(rect, "Master".Translate());
            if(Widgets.InvisibleButton(rect)){
                if (order == orders.Default)
                {
                    asc = !asc;
                }
                else
                {
                    order = orders.Default;
                    asc = false;
                }
                this.BuildPawnList();
            }
            TooltipHandler.TipRegion(rect, "Click to sort by petness (Default).");
            num += 90f;

            float x = 16f;

            Rect recta = new Rect(num, 48f, 50f, x);
            Rect recta1 = new Rect(num + 9, 48f, x, x);
            GUI.DrawTexture(recta1, GenderTextures[1]);
            num += 25f;

            Rect recta2 = new Rect(num, 48f, x, x);
            GUI.DrawTexture(recta2, GenderTextures[2]);
            num += 25f;

            if (Widgets.InvisibleButton(recta))
            {
                if (order == orders.Gender)
                {
                    asc = !asc;
                }
                else
                {
                    order = orders.Gender;
                    asc = false;
                }
                this.BuildPawnList();
            }
            TooltipHandler.TipRegion(recta, "Click to sort by gender.");

            Rect rectb = new Rect(num, 48f, 50f, x);
            Rect rectb1 = new Rect(num + 1, 48f, x, x);
            GUI.DrawTexture(rectb1, LifeStageTextures[0]);
            num += 17f;

            Rect rectb2 = new Rect(num, 48f, x, x);
            GUI.DrawTexture(rectb2, LifeStageTextures[1]);
            num += 16f;

            Rect rectb3 = new Rect(num, 48f, x, x);
            GUI.DrawTexture(rectb3, LifeStageTextures[2]);
            num += 17f;

            if (Widgets.InvisibleButton(rectb))
            {
                if (order == orders.LifeStage)
                {
                    asc = !asc;
                }
                else
                {
                    order = orders.LifeStage;
                    asc = false;
                }
                this.BuildPawnList();
            }
            TooltipHandler.TipRegion(rectb, "Click to sort by lifestage and age.");

            Rect rectc1 = new Rect(num + 13f, 48f, x, x);
            GUI.DrawTexture(rectc1, SlaughterTex);
            num += 50f;

            if (Widgets.InvisibleButton(rectc1))
            {
                if (order == orders.Slaughter)
                {
                    asc = !asc;
                }
                else
                {
                    order = orders.Slaughter;
                    asc = false;
                }
                this.BuildPawnList();
            }
            TooltipHandler.TipRegion(rectc1, "Click to sort by slaughter designation and bodysize.");

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
            // sizes for stuff
            float x = 16f;

            float heightOffset = (rect.height - x) / 2;
            float widthOffset = (50 - x) / 2;
            
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
            
            Rect recta = new Rect(num + widthOffset, heightOffset, x, x);
            Texture2D labelSex = GenderTextures[(int)p.gender];
            TipSignal tipSex = p.gender.ToString();
            GUI.DrawTexture(recta, labelSex);
            TooltipHandler.TipRegion(recta, tipSex);
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
            TipSignal tipAge = p.ageTracker.CurLifeStage.LabelCap;
            GUI.DrawTexture(rectb, labelAge);
            TooltipHandler.TipRegion(rectb, tipAge);
            num += 50f;

            Rect rectc = new Rect(num + 13f, heightOffset, x, x);
            bool Slaughter = Find.DesignationManager.DesignationOn(p, DesignationDefOf.Slaughter) != null;

            if (Slaughter)
            {
                GUI.DrawTexture(rectc, WorkBoxCheckTex);
                TooltipHandler.TipRegion(rectc, "save from the butcher's block");
            } else
            {
                TooltipHandler.TipRegion(rectc, "mark for slaughter");
            }
            if (Widgets.InvisibleButton(rectc))
            {
                if (Slaughter)
                {
                    Find.DesignationManager.DesignationOn(p, DesignationDefOf.Slaughter).Delete();
                    SoundDefOf.CheckboxTurnedOff.PlayOneShotOnCamera();
                }
                else
                {
                    Find.DesignationManager.AddDesignation(new Designation(p, DesignationDefOf.Slaughter));
                    SoundDefOf.CheckboxTurnedOn.PlayOneShotOnCamera();
                }
            }
            if (Mouse.IsOver(rectc))
            {
                GUI.DrawTexture(rectc, TexUI.HighlightTex);
            }

            num += 50;

            Rect rect4 = new Rect(num, 0f, 350f, rect.height);
            AreaAllowedGUI.DoAllowedAreaSelectors(rect4, p, AllowedAreaMode.Animal);
            num += 350f;
            GUI.EndGroup();
        }
    }
}
