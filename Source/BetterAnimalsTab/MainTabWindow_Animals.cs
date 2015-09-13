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

        public enum orders
        {
            Default,
            Name,
            Gender,
            LifeStage,
            Slaughter,
            Training
        }

        public static orders order = orders.Default;

        public static TrainableDef trainingOrder;

        public static bool asc = false;

        public override Vector2 RequestedTabSize
        {
            get
            {
                return new Vector2(1050f, 65f + (float)base.PawnsCount * 30f + 65f);
            }
        }

        public static bool isDirty = false;

        public static readonly Texture2D[] GenderTextures = new Texture2D[]
        {
            ContentFinder<Texture2D>.Get("UI/Gender/none", true),
            ContentFinder<Texture2D>.Get("UI/Gender/male", true),
            ContentFinder<Texture2D>.Get("UI/Gender/female", true)
        };

        public static readonly Texture2D[] LifeStageTextures = new Texture2D[]
        {
            ContentFinder<Texture2D>.Get("UI/LifeStage/1", true),
            ContentFinder<Texture2D>.Get("UI/LifeStage/2", true),
            ContentFinder<Texture2D>.Get("UI/LifeStage/3", true),
            ContentFinder<Texture2D>.Get("UI/LifeStage/unknown", true)
        };

        public static readonly Texture2D WorkBoxCheckTex = ContentFinder<Texture2D>.Get("UI/Widgets/WorkBoxCheck", true);
        public static readonly Texture2D SlaughterTex = ContentFinder<Texture2D>.Get("UI/Buttons/slaughter", true);
        private static readonly Texture2D filterTex = ContentFinder<Texture2D>.Get("UI/Buttons/filter_large", true);
        private static readonly Texture2D filterOffTex = ContentFinder<Texture2D>.Get("UI/Buttons/filter_off_large", true);


        protected override void BuildPawnList()
        {
            IEnumerable<Pawn> sorted;
            bool dump;
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
                case orders.Training:
                    sorted = from p in Find.ListerPawns.PawnsInFaction(Faction.OfColony)
                             where p.RaceProps.Animal
                             orderby p.training.IsCompleted(trainingOrder) descending, p.training.GetWanted(trainingOrder) descending, p.training.CanAssignToTrain(trainingOrder, out dump).Accepted descending
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

            if (Filter_Animals.filter)
            {
                this.pawns = Filter_Animals.FilterAnimals(this.pawns);
            }

            if (asc && this.pawns.Count() > 1)
            {
                this.pawns.Reverse();
            }
            
            isDirty = false;
        }

        public override void DoWindowContents(Rect fillRect)
        {
            base.DoWindowContents(fillRect);
            Rect position = new Rect(0f, 0f, fillRect.width, 65f);
            GUI.BeginGroup(position);

            // ARRRGGHHH!!!
            if (isDirty) BuildPawnList();


            Rect filterButton = new Rect(0f, 0f, 200f, Mathf.Round(position.height / 2f));
            Text.Font = GameFont.Small;
            if (Widgets.TextButton(filterButton, "Filter", true, false))
            {
                if (Event.current.button == 0)
                {
                    Find.WindowStack.Add(new Dialog_FilterAnimals());
                } else if (Event.current.button == 1)
                {
                    List<PawnKindDef> list = Find.ListerPawns.PawnsInFaction(Faction.OfColony).Where(p => p.RaceProps.Animal)
                                                 .Select(p => p.kindDef).Distinct().OrderBy(p => p.LabelCap).ToList();

                    if (list.Count > 0)
                    {
                        List<FloatMenuOption> list2 = new List<FloatMenuOption>();
                        list2.AddRange(list.ConvertAll<FloatMenuOption>((PawnKindDef p) => new FloatMenuOption(p.LabelCap, delegate
                        {
                            Filter_Animals.quickFilterPawnKind(p);
                            isDirty = true;
                        }, MenuOptionPriority.Medium, null, null)));
                        Find.WindowStack.Add(new FloatMenu(list2, false));
                    }
                }
            }
            TooltipHandler.TipRegion(filterButton, "Left click to set a filter.\nRight click to set a quick filter.");
            Rect filterIcon = new Rect(205f, (filterButton.height - 24f) / 2f, 24f, 24f);
            if (Filter_Animals.filter)
            {
                if(Widgets.ImageButton(filterIcon, filterOffTex))
                {
                    Filter_Animals.disableFilter();
                    BuildPawnList();
                    SoundDefOf.ClickReject.PlayOneShotOnCamera();
                }
                TooltipHandler.TipRegion(filterIcon, "Disable filter");
            } else if (Filter_Animals.filterPossible)
            {
                if (Widgets.ImageButton(filterIcon, filterTex))
                {
                    Filter_Animals.enableFilter();
                    BuildPawnList();
                    SoundDefOf.Click.PlayOneShotOnCamera();
                }
                TooltipHandler.TipRegion(filterIcon, "Enable filter");
            }

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
                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                BuildPawnList();
            }
            Rect highlightName = new Rect(0f, rectname.height - 30f, rectname.width, 30);
            TooltipHandler.TipRegion(highlightName, "Click to sort by name.");
            if (Mouse.IsOver(highlightName))
            {
                GUI.DrawTexture(highlightName, TexUI.HighlightTex);
            }
            
            Rect rect = new Rect(num, rectname.height - 30f, 90f, 30);
            Widgets.Label(rect, "Master".Translate());
            if(Widgets.InvisibleButton(rect)){
                    SoundDefOf.AmountDecrement.PlayOneShotOnCamera();
                if (order == orders.Default)
                {
                    asc = !asc;
                }
                else
                {
                    order = orders.Default;
                    asc = false;
                }
                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                this.BuildPawnList();
            }
            TooltipHandler.TipRegion(rect, "Click to sort by petness (Default).");
            if (Mouse.IsOver(rect))
            {
                GUI.DrawTexture(rect, TexUI.HighlightTex);
            }
            num += 90f;

            float x = 16f;

            Rect recta = new Rect(num, rectname.height - 30f, 50f, 30f);
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
                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                this.BuildPawnList();
            }
            TooltipHandler.TipRegion(recta, "Click to sort by gender.");
            if (Mouse.IsOver(recta))
            {
                GUI.DrawTexture(recta, TexUI.HighlightTex);
            }

            Rect rectb = new Rect(num, rectname.height - 30f, 50f, 30f);
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
                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                this.BuildPawnList();
            }
            TooltipHandler.TipRegion(rectb, "Click to sort by lifestage and age.");
            if (Mouse.IsOver(rectb))
            {
                GUI.DrawTexture(rectb, TexUI.HighlightTex);
            }

            Rect rectc = new Rect(num, rectname.height - 30f, 50f, 30f);
            Rect rectc1 = new Rect(num + 17f, 48f, 16f, 16f);
            GUI.DrawTexture(rectc1, SlaughterTex);
            if (Widgets.InvisibleButton(rectc1))
            {
                if (Event.current.shift)
                {
                    WidgetsAnimals.SlaughterAllAnimals(pawns);
                }
                else
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
                    SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                    this.BuildPawnList();
                }
            }
            TooltipHandler.TipRegion(rectc, "Click to sort by slaughter designation and bodysize.\nShift-click to toggle slaughter for all.");
            if (Mouse.IsOver(rectc))
            {
                GUI.DrawTexture(rectc, TexUI.HighlightTex);
            }

            num += 50f;
            Rect headers = new Rect(num, rectname.height - 30f, 80f, 30f);
            WidgetsAnimals.DoTrainingHeaders(headers, pawns);

            num += 90f;

            Rect rect2 = new Rect(num, 0f, 350f, Mathf.Round(position.height / 2f));
            Text.Font = GameFont.Small;
            if (Widgets.TextButton(rect2, "ManageAreas".Translate(), true, false))
            {
                Find.WindowStack.Add(new Dialog_ManageAreas());
            }
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.LowerCenter;
            Rect rect3 = new Rect(num, position.height - 27f, 350f, 30f);
            WidgetsAnimals.DoAllowedAreaHeaders(rect3, pawns, AllowedAreaMode.Animal);
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

            Rect rectc = new Rect(num, 0f, 50f, 30f);
            Rect rectc1 = new Rect(num + 17f, heightOffset, x, x);
            bool Slaughter = Find.DesignationManager.DesignationOn(p, DesignationDefOf.Slaughter) != null;

            if (Slaughter)
            {
                GUI.DrawTexture(rectc1, WorkBoxCheckTex);
                TooltipHandler.TipRegion(rectc, "save from the butcher's block");
            } else
            {
                TooltipHandler.TipRegion(rectc, "mark for slaughter");
            }
            if (Widgets.InvisibleButton(rectc))
            {
                if (Slaughter)
                {
                    WidgetsAnimals.UnSlaughterAnimal(p);
                    SoundDefOf.CheckboxTurnedOff.PlayOneShotOnCamera();
                }
                else
                {
                    WidgetsAnimals.SlaughterAnimal(p);
                    SoundDefOf.CheckboxTurnedOn.PlayOneShotOnCamera();
                }
            }
            if (Mouse.IsOver(rectc))
            {
                GUI.DrawTexture(rectc1, TexUI.HighlightTex);
            }

            num += 50f;

            Rect trainingRect = new Rect(num, 0f, 80f, 30f);
            WidgetsAnimals.DoTrainingRow(trainingRect, p);

            num += 90f;

            Rect rect4 = new Rect(num, 0f, 350f, rect.height);
            AreaAllowedGUI.DoAllowedAreaSelectors(rect4, p, AllowedAreaMode.Animal);
            num += 350f;
            GUI.EndGroup();
        }
    }
}
