// // Karel Kroeze
// // MainTabWindow_Animals.cs
// // 2016-06-27

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using static BetterAnimalsTab.Resources;

namespace Fluffy
{
    public class MainTabWindow_Animals : MainTabWindow_PawnList
    {
        //private const float TopAreaHeight = 65f;

        //private const float MasterWidth = 90f;

        //private const float AreaAllowedWidth = 350f;

        public enum Orders
        {
            Default,
            Name,
            Gender,
            LifeStage,
            Slaughter,
            Training
        }

        public static Orders Order = Orders.Default;

        public static TrainableDef TrainingOrder;

        public static bool Asc;

        public static bool IsDirty;

        public override Vector2 RequestedTabSize => new Vector2( 1050f, 65f + PawnsCount * 30f + 65f );

        public override void PostOpen()
        {
            base.PostOpen();
            if ( Dialog_FilterAnimals.Sticky )
            {
                Find.WindowStack.Add( new Dialog_FilterAnimals() );
            }
        }

        protected override void BuildPawnList()
        {
            IEnumerable<Pawn> sorted;
            switch ( Order )
            {
                case Orders.Default:
                    sorted = from p in Find.MapPawns.PawnsInFaction( Faction.OfPlayer )
                             where p.RaceProps.Animal
                             orderby p.RaceProps.petness descending, p.RaceProps.baseBodySize, p.def.label
                             select p;
                    break;
                case Orders.Name:
                    sorted = from p in Find.MapPawns.PawnsInFaction( Faction.OfPlayer )
                             where p.RaceProps.Animal
                             orderby p.Name.Numerical, p.Name.ToStringFull, p.def.label
                             select p;
                    break;
                case Orders.Gender:
                    sorted = from p in Find.MapPawns.PawnsInFaction( Faction.OfPlayer )
                             where p.RaceProps.Animal
                             orderby p.KindLabel, p.gender
                             select p;
                    break;
                case Orders.LifeStage:
                    sorted = from p in Find.MapPawns.PawnsInFaction( Faction.OfPlayer )
                             where p.RaceProps.Animal
                             orderby p.ageTracker.CurLifeStageRace.minAge descending,
                                 p.ageTracker.AgeBiologicalTicks descending
                             select p;
                    break;
                case Orders.Slaughter:
                    sorted = from p in Find.MapPawns.PawnsInFaction( Faction.OfPlayer )
                             where p.RaceProps.Animal
                             orderby
                                 Find.DesignationManager.DesignationOn( p, DesignationDefOf.Slaughter ) != null
                                 descending, p.BodySize descending
                             select p;
                    break;
                case Orders.Training:
                    bool dump;
                    sorted = from p in Find.MapPawns.PawnsInFaction( Faction.OfPlayer )
                             where p.RaceProps.Animal
                             orderby p.training.IsCompleted( TrainingOrder ) descending,
                                 p.training.GetWanted( TrainingOrder ) descending,
                                 p.training.CanAssignToTrain( TrainingOrder, out dump ).Accepted descending
                             select p;
                    break;
                default:
                    sorted = from p in Find.MapPawns.PawnsInFaction( Faction.OfPlayer )
                             where p.RaceProps.Animal
                             orderby p.RaceProps.petness descending, p.RaceProps.baseBodySize, p.def.label
                             select p;
                    break;
            }

            Pawns = sorted.ToList();

            if ( Widgets_Filter.Filter )
            {
                Pawns = Widgets_Filter.FilterAnimals( Pawns );
            }

            if ( Asc && Pawns.Count > 1 )
            {
                Pawns.Reverse();
            }

            IsDirty = false;
        }

        public override void DoWindowContents( Rect fillRect )
        {
            base.DoWindowContents( fillRect );
            var position = new Rect( 0f, 0f, fillRect.width, 65f );
            GUI.BeginGroup( position );

            // ARRRGGHHH!!!
            // Allow other panels to trigger rebuilding the pawn list. (This took me forever to figure out...)
            if ( IsDirty )
                BuildPawnList();


            var filterButton = new Rect( 0f, 0f, 200f, Mathf.Round( position.height / 2f ) );
            Text.Font = GameFont.Small;
            if ( Widgets.ButtonText( filterButton, "Fluffy.Filter".Translate() ) )
            {
                if ( Event.current.button == 0 )
                {
                    Find.WindowStack.Add( new Dialog_FilterAnimals() );
                }
                else if ( Event.current.button == 1 )
                {
                    List<PawnKindDef> list =
                        Find.MapPawns.PawnsInFaction( Faction.OfPlayer ).Where( p => p.RaceProps.Animal )
                            .Select( p => p.kindDef ).Distinct().OrderBy( p => p.LabelCap ).ToList();

                    if ( list.Count > 0 )
                    {
                        var list2 = new List<FloatMenuOption>();
                        list2.AddRange( list.ConvertAll( p => new FloatMenuOption( p.LabelCap, delegate
                                                                                                   {
                                                                                                       Widgets_Filter
                                                                                                           .QuickFilterPawnKind
                                                                                                           ( p );
                                                                                                       IsDirty = true;
                                                                                                   } ) ) );
                        Find.WindowStack.Add( new FloatMenu( list2 ) );
                    }
                }
            }
            TooltipHandler.TipRegion( filterButton, "Fluffy.FilterTooltip".Translate() );
            var filterIcon = new Rect( 205f, ( filterButton.height - 24f ) / 2f, 24f, 24f );
            if ( Widgets_Filter.Filter )
            {
                if ( Widgets.ButtonImage( filterIcon, FilterOffTex ) )
                {
                    Widgets_Filter.DisableFilter();
                    BuildPawnList();
                    SoundDefOf.ClickReject.PlayOneShotOnCamera();
                }
                TooltipHandler.TipRegion( filterIcon, "Fluffy.DisableFilter".Translate() );
            }
            else if ( Widgets_Filter.FilterPossible )
            {
                if ( Widgets.ButtonImage( filterIcon, FilterTex ) )
                {
                    Widgets_Filter.EnableFilter();
                    BuildPawnList();
                    SoundDefOf.Click.PlayOneShotOnCamera();
                }
                TooltipHandler.TipRegion( filterIcon, "Fluffy.EnableFilter".Translate() );
            }

            var num = 175f;
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.LowerCenter;
            var rectname = new Rect( 0f, 0f, num, position.height + 3f );
            Widgets.Label( rectname, "Fluffy.Name".Translate() );
            if ( Widgets.ButtonInvisible( rectname ) )
            {
                if ( Order == Orders.Name )
                {
                    Asc = !Asc;
                }
                else
                {
                    Order = Orders.Name;
                    Asc = false;
                }
                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                BuildPawnList();
            }
            var highlightName = new Rect( 0f, rectname.height - 30f, rectname.width, 30 );
            TooltipHandler.TipRegion( highlightName, "Fluffy.SortByName".Translate() );
            if ( Mouse.IsOver( highlightName ) )
            {
                GUI.DrawTexture( highlightName, TexUI.HighlightTex );
            }

            var rect = new Rect( num, rectname.height - 30f, 90f, 30 );
            Widgets.Label( rect, "Master".Translate() );
            if ( Widgets.ButtonInvisible( rect ) )
            {
                SoundDefOf.AmountDecrement.PlayOneShotOnCamera();
                if ( Order == Orders.Default )
                {
                    Asc = !Asc;
                }
                else
                {
                    Order = Orders.Default;
                    Asc = false;
                }
                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                BuildPawnList();
            }
            TooltipHandler.TipRegion( rect, "Fluffy.SortByPetness".Translate() );
            if ( Mouse.IsOver( rect ) )
            {
                GUI.DrawTexture( rect, TexUI.HighlightTex );
            }
            num += 90f;

            var x = 16f;

            var recta = new Rect( num, rectname.height - 30f, 50f, 30f );
            var recta1 = new Rect( num + 9, 48f, x, x );
            GUI.DrawTexture( recta1, GenderTextures[1] );
            num += 25f;

            var recta2 = new Rect( num, 48f, x, x );
            GUI.DrawTexture( recta2, GenderTextures[2] );
            num += 25f;

            if ( Widgets.ButtonInvisible( recta ) )
            {
                if ( Order == Orders.Gender )
                {
                    Asc = !Asc;
                }
                else
                {
                    Order = Orders.Gender;
                    Asc = false;
                }
                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                BuildPawnList();
            }
            TooltipHandler.TipRegion( recta, "Fluffy.SortByGender".Translate() );
            if ( Mouse.IsOver( recta ) )
            {
                GUI.DrawTexture( recta, TexUI.HighlightTex );
            }

            var rectb = new Rect( num, rectname.height - 30f, 50f, 30f );
            var rectb1 = new Rect( num + 1, 48f, x, x );
            GUI.DrawTexture( rectb1, LifeStageTextures[0] );
            num += 17f;

            var rectb2 = new Rect( num, 48f, x, x );
            GUI.DrawTexture( rectb2, LifeStageTextures[1] );
            num += 16f;

            var rectb3 = new Rect( num, 48f, x, x );
            GUI.DrawTexture( rectb3, LifeStageTextures[2] );
            num += 17f;

            if ( Widgets.ButtonInvisible( rectb ) )
            {
                if ( Order == Orders.LifeStage )
                {
                    Asc = !Asc;
                }
                else
                {
                    Order = Orders.LifeStage;
                    Asc = false;
                }
                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                BuildPawnList();
            }
            TooltipHandler.TipRegion( rectb, "Fluffy.SortByAge".Translate() );
            if ( Mouse.IsOver( rectb ) )
            {
                GUI.DrawTexture( rectb, TexUI.HighlightTex );
            }

            var rectc = new Rect( num, rectname.height - 30f, 50f, 30f );
            var rectc1 = new Rect( num + 17f, 48f, 16f, 16f );
            GUI.DrawTexture( rectc1, SlaughterTex );
            if ( Widgets.ButtonInvisible( rectc1 ) )
            {
                if ( Event.current.shift )
                {
                    Widgets_Animals.SlaughterAllAnimals( Pawns );
                }
                else
                {
                    if ( Order == Orders.Slaughter )
                    {
                        Asc = !Asc;
                    }
                    else
                    {
                        Order = Orders.Slaughter;
                        Asc = false;
                    }
                    SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                    BuildPawnList();
                }
            }
            TooltipHandler.TipRegion( rectc, "Fluffy.SortByBodysizeSlaughter".Translate() );
            if ( Mouse.IsOver( rectc ) )
            {
                GUI.DrawTexture( rectc, TexUI.HighlightTex );
            }

            num += 50f;
            var headers = new Rect( num, rectname.height - 30f, 80f, 30f );
            Widgets_Animals.DoTrainingHeaders( headers, Pawns );

            num += 90f;

            var rect2 = new Rect( num, 0f, 350f, Mathf.Round( position.height / 2f ) );
            Text.Font = GameFont.Small;
            if ( Widgets.ButtonText( rect2, "ManageAreas".Translate() ) )
            {
                Find.WindowStack.Add( new Dialog_ManageAreas() );
            }
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.LowerCenter;
            var rect3 = new Rect( num, position.height - 27f, 350f, 30f );
            Widgets_Animals.DoAllowedAreaHeaders( rect3, Pawns, AllowedAreaMode.Animal );
            GUI.EndGroup();
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;
            var outRect = new Rect( 0f, position.height, fillRect.width, fillRect.height - position.height );
            DrawRows( outRect );
        }

        protected override void DrawPawnRow( Rect rect, Pawn p )
        {
            // sizes for stuff
            var x = 16f;

            float heightOffset = ( rect.height - x ) / 2;
            float widthOffset = ( 50 - x ) / 2;

            GUI.BeginGroup( rect );
            var num = 175f;

            if ( p.training.IsCompleted( TrainableDefOf.Obedience ) )
            {
                var rect2 = new Rect( num, 0f, 90f, rect.height );
                Rect rect3 = rect2.ContractedBy( 2f );
                string label = p.playerSettings.master == null
                                   ? "NoneLower".Translate()
                                   : p.playerSettings.master.LabelShort;
                Text.Font = GameFont.Small;
                if ( Widgets.ButtonText( rect3, label ) )
                {
                    TrainableUtility.OpenMasterSelectMenu( p );
                }
            }
            num += 90f;

            var recta = new Rect( num + widthOffset, heightOffset, x, x );
            Texture2D labelSex = GenderTextures[(int) p.gender];
            TipSignal tipSex = p.gender.ToString();
            GUI.DrawTexture( recta, labelSex );
            TooltipHandler.TipRegion( recta, tipSex );
            num += 50f;

            var rectb = new Rect( num + widthOffset, heightOffset, x, x );
            Texture2D labelAge = p.RaceProps.lifeStageAges.Count > 3
                                     ? LifeStageTextures[3]
                                     : LifeStageTextures[p.ageTracker.CurLifeStageIndex];
            TipSignal tipAge = p.ageTracker.CurLifeStage.LabelCap + ", " + p.ageTracker.AgeBiologicalYears;
            GUI.DrawTexture( rectb, labelAge );
            TooltipHandler.TipRegion( rectb, tipAge );
            num += 50f;

            var rectc = new Rect( num, 0f, 50f, 30f );
            var rectc1 = new Rect( num + 17f, heightOffset, x, x );
            bool slaughter = Find.DesignationManager.DesignationOn( p, DesignationDefOf.Slaughter ) != null;

            if ( slaughter )
            {
                GUI.DrawTexture( rectc1, WorkBoxCheckTex );
                TooltipHandler.TipRegion( rectc, "Fluffy.StopSlaughter".Translate() );
            }
            else
            {
                TooltipHandler.TipRegion( rectc, "Fluffy.MarkSlaughter".Translate() );
            }
            if ( Widgets.ButtonInvisible( rectc ) )
            {
                if ( slaughter )
                {
                    Widgets_Animals.UnSlaughterAnimal( p );
                    SoundDefOf.CheckboxTurnedOff.PlayOneShotOnCamera();
                }
                else
                {
                    Widgets_Animals.SlaughterAnimal( p );
                    SoundDefOf.CheckboxTurnedOn.PlayOneShotOnCamera();
                }
            }
            if ( Mouse.IsOver( rectc ) )
            {
                GUI.DrawTexture( rectc1, TexUI.HighlightTex );
            }

            num += 50f;

            var trainingRect = new Rect( num, 0f, 80f, 30f );
            Widgets_Animals.DoTrainingRow( trainingRect, p );

            num += 90f;

            var rect4 = new Rect( num, 0f, 350f, rect.height );
            AreaAllowedGUI.DoAllowedAreaSelectors( rect4, p, AllowedAreaMode.Animal );
            GUI.EndGroup();
        }
    }
}
