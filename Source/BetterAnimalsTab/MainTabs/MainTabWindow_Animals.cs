// // Karel Kroeze
// // MainTabWindow_Animals.cs
// // 2016-06-27

using System;
using BetterAnimalsTab;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using UnityEngine;
using Verse;
using Verse.Sound;
using static BetterAnimalsTab.Resources;

namespace Fluffy
{
    public class MainTabWindow_Animals : MainTabWindow_PawnList
    {
        public enum Orders
        {
            Default,
            Name,
            Gender,
            LifeStage,
            Pregnant,
            Slaughter,
            Training
        }

        public static Orders Order = Orders.Default;
        public static TrainableDef TrainingOrder;
        public static bool Asc;
        private static FieldInfo _dirtyField = typeof( MainTabWindow_PawnList ).GetField( "pawnListDirty", (BindingFlags) 60 );

        public bool PawnsDirty
        {
            get
            {
                if (_dirtyField == null)
                    throw new ArgumentNullException( "dirtyField" );

                return (bool)_dirtyField.GetValue( this );
            }
            set
            {
                if ( _dirtyField == null )
                    throw new ArgumentNullException( "dirtyField" );

                _dirtyField.SetValue( this, value );
            }
        }

        private static MainTabWindow_Animals _instance;
        public static MainTabWindow_Animals Instance
        {
            get
            {
                if ( _instance == null )
                    _instance = (MainTabWindow_Animals)DefDatabase<MainTabDef>.GetNamed( "Animals" )?.Window;

                if ( _instance == null )
                    throw new ArgumentNullException( "Def" );

                return _instance;
            }
        }

        public static float iconSize = 16f;

        public override Vector2 RequestedTabSize => new Vector2( 1050f, 65f + PawnsCount * 30f + 65f );

        public override void PostOpen()
        {
            base.PostOpen();
            if ( Dialog_FilterAnimals.Sticky )
                Find.WindowStack.Add( new Dialog_FilterAnimals() );
        }

        protected override void BuildPawnList()
        {
            IEnumerable<Pawn> sorted;
            switch ( Order )
            {
                case Orders.Default:
                    sorted = from p in Find.VisibleMap.mapPawns.PawnsInFaction( Faction.OfPlayer )
                             where p.RaceProps.Animal
                             orderby p.RaceProps.petness descending, p.RaceProps.baseBodySize, p.def.label
                             select p;
                    break;

                case Orders.Name:
                    sorted = from p in Find.VisibleMap.mapPawns.PawnsInFaction( Faction.OfPlayer )
                             where p.RaceProps.Animal
                             orderby p.Name.Numerical, p.Name.ToStringFull, p.def.label
                             select p;
                    break;

                case Orders.Gender:
                    sorted = from p in Find.VisibleMap.mapPawns.PawnsInFaction( Faction.OfPlayer )
                             where p.RaceProps.Animal
                             orderby p.KindLabel, p.gender
                             select p;
                    break;

                case Orders.LifeStage:
                    sorted = from p in Find.VisibleMap.mapPawns.PawnsInFaction( Faction.OfPlayer )
                             where p.RaceProps.Animal
                             orderby p.ageTracker.CurLifeStageRace.minAge descending,
                                 p.ageTracker.AgeBiologicalTicks descending
                             select p;
                    break;

                case Orders.Pregnant:
                    sorted = from p in Find.VisibleMap.mapPawns.PawnsInFaction( Faction.OfPlayer )
                             where p.RaceProps.Animal
                             orderby p.IsPregnant() descending
                             select p;
                    break;

                case Orders.Slaughter:
                    sorted = from p in Find.VisibleMap.mapPawns.PawnsInFaction( Faction.OfPlayer )
                             where p.RaceProps.Animal
                             orderby
                                 Find.VisibleMap.designationManager.DesignationOn( p, DesignationDefOf.Slaughter ) != null
                                 descending, p.BodySize descending
                             select p;
                    break;

                case Orders.Training:
                    bool dump;
                    sorted = from p in Find.VisibleMap.mapPawns.PawnsInFaction( Faction.OfPlayer )
                             where p.RaceProps.Animal
                             orderby p.training.IsCompleted( TrainingOrder ) descending,
                                 p.training.GetWanted( TrainingOrder ) descending,
                                 p.training.CanAssignToTrain( TrainingOrder, out dump ).Accepted descending
                             select p;
                    break;

                default:
                    sorted = from p in Find.VisibleMap.mapPawns.PawnsInFaction( Faction.OfPlayer )
                             where p.RaceProps.Animal
                             orderby p.playerSettings.master descending, p.RaceProps.petness descending
                             select p;
                    break;
            }

            pawns = sorted.ToList();

            if ( Widgets_Filter.Filter )
                pawns = Widgets_Filter.FilterAnimals( pawns );

            if ( Asc && pawns.Count > 1 )
                pawns.Reverse();

            PawnsDirty = false;
        }

        public override void DoWindowContents( Rect fillRect )
        {
            // allow rebuilding pawnlist from outside of the maintab.
            if ( PawnsDirty )
                BuildPawnList();
            
            base.DoWindowContents( fillRect );
            var position = new Rect( 0f, 0f, fillRect.width, 65f );
            GUI.BeginGroup( position );

            var filterButton = new Rect( 0f, 0f, 200f, Mathf.Round( position.height / 2f ) );
            DrawFilterButton( filterButton );

            var curX = 175f;
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.LowerCenter;
            var nameRect = new Rect( 0f, 0f, curX, position.height + 3f );
            DrawColumnHeader_Name( nameRect );

            var masterRect = new Rect( curX, nameRect.height - 30f, 90f, 30 );
            DrawColumnHeader_Master( ref curX, masterRect );

            var followRect = new Rect( curX, nameRect.height - 30f, 50f, 30f );
            DrawColumnHeader_Follow( ref curX, followRect );

            var genderRect = new Rect( curX, nameRect.height - 30f, 50f, 30f );
            DrawColumnHeader_Gender( ref curX, genderRect );

            var ageRect = new Rect( curX, nameRect.height - 30f, 50f, 30f );
            DrawColumnHeader_Age( ref curX, ageRect );

            var pregnantRect = new Rect( curX, nameRect.height - 30f, 50f, 30f );
            DrawColumnHeader_Pregnant( ref curX, pregnantRect );

            var slaughterRect = new Rect( curX, nameRect.height - 30f, 50f, 30f );
            DrawColumnHeader_Slaughter( ref curX, slaughterRect );

            var trainingRect = new Rect( curX, nameRect.height - 30f, widthPerTrainable * TrainableUtility.TrainableDefsInListOrder.Count, 30f );
            DrawColumnHeader_Training( ref curX, trainingRect );
            curX += 10f; // some margin

            var manageAreasRect = new Rect( curX, 0f, 350f, Mathf.Round( position.height / 2f ) );
            Text.Font = GameFont.Small;
            if ( Widgets.ButtonText( manageAreasRect, "ManageAreas".Translate() ) )
            {
                Find.WindowStack.Add( new Dialog_ManageAreas( Find.VisibleMap ) );
            }

            var areaLabelRect = new Rect( curX, position.height - 27f, 350f, 30f );
            DrawColumnHeader_Areas( areaLabelRect );

            GUI.EndGroup();

            var outRect = new Rect( 0f, position.height, fillRect.width, fillRect.height - position.height );
            DrawRows( outRect );
        }

        private void DrawColumnHeader_Follow( ref float curX, Rect rect )
        {
            Rect draftedRect = new Rect( rect.xMin, rect.yMin, rect.width / 2f, rect.height );
            Rect draftedIconRect = new Rect( 0f, 0f, iconSize, iconSize ).CenteredOnXIn( draftedRect ).CenteredOnYIn( draftedRect );
            Rect hunterRect = draftedRect, hunterIconRect = draftedIconRect;
            hunterRect.x += rect.width / 2f;
            hunterIconRect.x += rect.width / 2f;

            GUI.DrawTexture( draftedIconRect, Widgets_Follow.FollowDraftIcon );
            Widgets.DrawHighlightIfMouseover( draftedRect );
            TooltipHandler.TipRegion( draftedRect, "Fluffy.PetFollow.DraftedColumnTip".Translate() );
            if ( Widgets.ButtonInvisible( draftedRect ) && Event.current.shift )
                Widgets_Follow.ToggleAllFollowsDrafted( pawns );

            GUI.DrawTexture( hunterIconRect, Widgets_Follow.FollowHuntIcon );
            Widgets.DrawHighlightIfMouseover( hunterRect );
            TooltipHandler.TipRegion( hunterRect, "Fluffy.PetFollow.HunterColumnTip".Translate() );
            if ( Widgets.ButtonInvisible( hunterRect ) && Event.current.shift )
                Widgets_Follow.ToggleAllFollowsHunter( pawns );

            curX += 50f;
        }

        private void DrawColumnHeader_Pregnant( ref float curX, Rect pregnantRect )
        {
            var iconRect = new Rect( 0f, 0f, iconSize, iconSize )
                .CenteredOnXIn( pregnantRect )
                .CenteredOnYIn( pregnantRect );

            GUI.DrawTexture( iconRect, PregnantTex );
            if ( Widgets.ButtonInvisible( pregnantRect ) )
            {
                if ( Order == Orders.Pregnant )
                {
                    Asc = !Asc;
                }
                else
                {
                    Order = Orders.Pregnant;
                    Asc = false;
                }
                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                BuildPawnList();
            }
            TooltipHandler.TipRegion( pregnantRect, "Fluffy.SortByPregnancy".Translate() );
            if ( Mouse.IsOver( pregnantRect ) )
            {
                GUI.DrawTexture( pregnantRect, TexUI.HighlightTex );
            }

            curX += 50f;
        }

        private void DrawColumnHeader_Areas( Rect rect, AllowedAreaMode mode = AllowedAreaMode.Animal )
        {
            List<Area> allAreas = Find.VisibleMap.areaManager.AllAreas;
            var num = 1;
            foreach ( Area t in allAreas )
            {
                if ( t.AssignableAsAllowed( mode ) )
                {
                    num++;
                }
            }
            float num2 = rect.width / num;
            Text.WordWrap = false;
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.LowerCenter;
            var rect2 = new Rect( rect.x, rect.y, num2, rect.height );
            Widgets.Label( rect2, "NoAreaAllowed".Translate() );
            if ( Widgets.ButtonInvisible( rect2 ) )
            {
                foreach ( Pawn t in pawns )
                {
                    SoundDefOf.DesignateDragStandardChanged.PlayOneShotOnCamera();
                    t.playerSettings.AreaRestriction = null;
                }
            }
            if ( Mouse.IsOver( rect2 ) )
            {
                GUI.DrawTexture( rect2, TexUI.HighlightTex );
            }
            TooltipHandler.TipRegion( rect2, "NoAreaAllowed".Translate() );
            var num3 = 1;
            foreach ( Area area in allAreas )
            {
                if ( area.AssignableAsAllowed( mode ) )
                {
                    float num4 = num3 * num2;
                    var rect3 = new Rect( rect.x + num4, rect.y, num2, rect.height );
                    Widgets.Label( rect3, area.Label );
                    if ( Widgets.ButtonInvisible( rect3 ) )
                    {
                        foreach ( Pawn p in pawns )
                        {
                            SoundDefOf.DesignateDragStandardChanged.PlayOneShotOnCamera();
                            p.playerSettings.AreaRestriction = area;
                        }
                    }
                    TooltipHandler.TipRegion( rect3, "Fluffy.RestrictAllTo".Translate( area.Label ) );
                    if ( Mouse.IsOver( rect3 ) )
                    {
                        GUI.DrawTexture( rect3, TexUI.HighlightTex );
                    }
                    num3++;
                }
            }
            Text.WordWrap = true;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;
        }

        public override void PreClose()
        {
            base.PreClose();

            if ( Find.WindowStack.IsOpen<Dialog_FilterAnimals>() )
                Find.WindowStack.TryRemove( typeof( Dialog_FilterAnimals ), false );
        }
        
        private void DrawColumnHeader_Training( ref float curX, Rect rect )
        {
            List<TrainableDef> trainables = TrainableUtility.TrainableDefsInListOrder;
            float width = rect.width / trainables.Count;
            float widthOffset = ( width - iconSize ) / 2;
            float heightOffset = ( rect.height - iconSize ) / 2;
            float x = rect.xMin;
            float y = rect.yMin;

            for ( var i = 0; i < trainables.Count; i++ )
            {
                var bgRect = new Rect( x, y, width, rect.height );
                var iconRect = new Rect( x + widthOffset, y + heightOffset, iconSize, iconSize );
                x += width;
                if ( Mouse.IsOver( bgRect ) )
                {
                    GUI.DrawTexture( bgRect, TexUI.HighlightTex );
                }
                var tooltip = new StringBuilder();
                tooltip.AppendLine( "Fluffy.SortByTrainables".Translate( trainables[i].LabelCap ) );
                tooltip.AppendLine( "Fluffy.ShiftToTrainAll".Translate() ).AppendLine()
                       .Append( trainables[i].description );
                TooltipHandler.TipRegion( bgRect, tooltip.ToString() );

                var iconTexture = trainables[i].GetUIIcon();
                if ( iconTexture != null )
                    GUI.DrawTexture( iconRect, iconTexture );
                if ( Widgets.ButtonInvisible( bgRect ) )
                {
                    if ( !Event.current.shift )
                    {
                        if ( MainTabWindow_Animals.Order == MainTabWindow_Animals.Orders.Training &&
                             MainTabWindow_Animals.TrainingOrder == trainables[i] )
                        {
                            MainTabWindow_Animals.Asc = !MainTabWindow_Animals.Asc;
                        }
                        else
                        {
                            MainTabWindow_Animals.Order = MainTabWindow_Animals.Orders.Training;
                            MainTabWindow_Animals.Asc = false;
                            MainTabWindow_Animals.TrainingOrder = trainables[i];
                        }
                    }
                    else if ( Event.current.shift )
                    {
                        Widgets_Animals.ToggleAllTraining( trainables[i], pawns );
                    }
                    SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                    PawnsDirty = true;
                }
            }

            curX += rect.width;
        }

        private void DrawColumnHeader_Slaughter( ref float curX, Rect slaughterRect )
        {
            var slaughterIconRect = new Rect( curX + 17f, 48f, 16f, 16f );
            GUI.DrawTexture( slaughterIconRect, SlaughterTex );
            if ( Widgets.ButtonInvisible( slaughterIconRect ) )
            {
                if ( Event.current.shift )
                {
                    Widgets_Animals.SlaughterAllAnimals( pawns );
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
            TooltipHandler.TipRegion( slaughterRect, "Fluffy.SortByBodysizeSlaughter".Translate() );
            if ( Mouse.IsOver( slaughterRect ) )
            {
                GUI.DrawTexture( slaughterRect, TexUI.HighlightTex );
            }

            curX += 50f;
        }

        private void DrawColumnHeader_Age( ref float curX, Rect ageRect )
        {
            var ageRectLeft = new Rect( curX + 1, 48f, iconSize, iconSize );
            GUI.DrawTexture( ageRectLeft, LifeStageTextures[0] );
            curX += 17f;

            var ageRectMid = new Rect( curX, 48f, iconSize, iconSize );
            GUI.DrawTexture( ageRectMid, LifeStageTextures[1] );
            curX += 16f;

            var ageRectRight = new Rect( curX, 48f, iconSize, iconSize );
            GUI.DrawTexture( ageRectRight, LifeStageTextures[2] );
            curX += 17f;

            if ( Widgets.ButtonInvisible( ageRect ) )
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
            TooltipHandler.TipRegion( ageRect, "Fluffy.SortByAge".Translate() );
            if ( Mouse.IsOver( ageRect ) )
            {
                GUI.DrawTexture( ageRect, TexUI.HighlightTex );
            }
        }

        private void DrawColumnHeader_Gender( ref float curX, Rect genderRect )
        {
            var genderRectLeft = new Rect( curX + 9, 48f, iconSize, iconSize );
            GUI.DrawTexture( genderRectLeft, GenderTextures[1] );
            curX += 25f;

            var genderRectRight = new Rect( curX, 48f, iconSize, iconSize );
            GUI.DrawTexture( genderRectRight, GenderTextures[2] );
            curX += 25f;

            if ( Widgets.ButtonInvisible( genderRect ) )
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
            TooltipHandler.TipRegion( genderRect, "Fluffy.SortByGender".Translate() );
            if ( Mouse.IsOver( genderRect ) )
            {
                GUI.DrawTexture( genderRect, TexUI.HighlightTex );
            }
        }

        private void DrawColumnHeader_Master( ref float curX, Rect rect )
        {
            Widgets.Label( rect, "Master".Translate() );
            if ( Widgets.ButtonInvisible( rect ) )
            {
                // left click
                if ( Event.current.button == 0 )
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
                // right click
                if ( Event.current.button == 1 )
                {
                    var options = new List<FloatMenuOption>();

                    if ( Widgets_Animals.ObedientAnimalsOfColony.Any() )
                    {
                        // none
                        options.Add( Widgets_Animals.MassAssignMaster_FloatMenuOption( null ) );

                        // clever
                        options.Add( new FloatMenuOption( "Fluffy.MassAssignMasterBonded".Translate(),
                                                          Widgets_Animals.MassAssignMasterBonded ) );

                        // loop over pawns
                        foreach ( Pawn pawn in Find.VisibleMap.mapPawns.FreeColonistsSpawned )
                            options.Add( Widgets_Animals.MassAssignMaster_FloatMenuOption( pawn ) );
                    }
                    else
                    {
                        options.Add( new FloatMenuOption( "Fluffy.NoObedientAnimals".Translate(), null ) );
                    }

                    Find.WindowStack.Add( new FloatMenu( options ) );
                }
            }
            TooltipHandler.TipRegion( rect, "Fluffy.SortByPetness".Translate() );
            if ( Mouse.IsOver( rect ) )
            {
                GUI.DrawTexture( rect, TexUI.HighlightTex );
            }
            curX += 90f;
        }

        private void DrawColumnHeader_Name( Rect nameRect )
        {
            Widgets.Label( nameRect, "Fluffy.Name".Translate() );
            if ( Widgets.ButtonInvisible( nameRect ) )
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
            var highlightName = new Rect( 0f, nameRect.height - 30f, nameRect.width, 30 );
            TooltipHandler.TipRegion( highlightName, "Fluffy.SortByName".Translate() );
            if ( Mouse.IsOver( highlightName ) )
            {
                GUI.DrawTexture( highlightName, TexUI.HighlightTex );
            }
        }

        private void DrawFilterButton( Rect filterButton )
        {
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
                        Find.VisibleMap.mapPawns.PawnsInFaction( Faction.OfPlayer ).Where( p => p.RaceProps.Animal )
                            .Select( p => p.kindDef ).Distinct().OrderBy( p => p.LabelCap ).ToList();

                    if ( list.Count > 0 )
                    {
                        var list2 = new List<FloatMenuOption>();
                        list2.AddRange( list.ConvertAll( p => new FloatMenuOption( p.LabelCap, delegate
                                                                                                   {
                                                                                                       Widgets_Filter
                                                                                                           .QuickFilterPawnKind
                                                                                                           ( p );
                                                                                                       PawnsDirty = true;
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
        }

        protected override void DrawPawnRow( Rect canvas, Pawn pawn )
        {
            // sizes for stuff
            float heightOffset = ( canvas.height - iconSize ) / 2;
            float widthOffset = ( 50 - iconSize ) / 2;

            GUI.BeginGroup( canvas );
            var curX = 175f;

            DrawCell_Master( canvas, pawn, ref curX );
            DrawCell_Follow( pawn, ref curX );

            var genderRect = new Rect( curX + widthOffset, heightOffset, iconSize, iconSize );
            DrawCell_Gender( pawn, genderRect, ref curX );

            var ageRect = new Rect( curX + widthOffset, heightOffset, iconSize, iconSize );
            DrawCell_Age( pawn, ageRect, ref curX );

            var pregnantRect = new Rect( curX + widthOffset, heightOffset, iconSize, iconSize );
            DrawCell_Pregnant( pawn, pregnantRect, ref curX );
            
            var slaughterRect = new Rect( curX + widthOffset, heightOffset, iconSize, iconSize );
            DrawCell_Slaughter( pawn, slaughterRect, ref curX );
            
            Widgets_Animals.DoTrainingRow( ref curX, pawn );
            curX += 10f; // some margin

            var areaRect = new Rect( curX, 0f, 350f, canvas.height );
            AreaAllowedGUI.DoAllowedAreaSelectors( areaRect, pawn, AllowedAreaMode.Animal );

            GUI.EndGroup();
        }

        private static void DrawCell_Slaughter( Pawn p, Rect canvas, ref float curX )
        {
            bool slaughter = Find.VisibleMap.designationManager.DesignationOn( p, DesignationDefOf.Slaughter ) != null;

            if ( slaughter )
            {
                GUI.DrawTexture( canvas, WorkBoxCheckTex );
                TooltipHandler.TipRegion( canvas, "Fluffy.StopSlaughter".Translate() );
            }
            else
            {
                TooltipHandler.TipRegion( canvas, "Fluffy.MarkSlaughter".Translate() );
            }
            if ( Widgets.ButtonInvisible( canvas ) )
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
            if ( Mouse.IsOver( canvas ) )
            {
                GUI.DrawTexture( canvas, TexUI.HighlightTex );
            }

            curX += 50f;
        }

        private static void DrawCell_Pregnant( Pawn p, Rect pregnantRect, ref float curX )
        {
            Hediff_Pregnant hediff;
            if ( p.Pregnant( out hediff ) )
            {
                GUI.DrawTexture( pregnantRect, PregnantTex );
                int total = (int) p.RaceProps.gestationPeriodDays * GenDate.TicksPerDay;
                int progress = (int) ( hediff.GestationProgress * total );
                TooltipHandler.TipRegion( pregnantRect,
                                          "PregnantIconDesc".Translate( total.ToStringTicksToPeriod(),
                                                                        progress.ToStringTicksToPeriod() ) );
            }
            curX += 50f;
        }

        private static void DrawCell_Age( Pawn p, Rect rectb, ref float curX )
        {
            Texture2D labelAge = p.RaceProps.lifeStageAges.Count > 3
                                     ? LifeStageTextures[3]
                                     : LifeStageTextures[p.ageTracker.CurLifeStageIndex];
            TipSignal tipAge = p.ageTracker.CurLifeStage.LabelCap + ", " + p.ageTracker.AgeBiologicalYears;
            GUI.DrawTexture( rectb, labelAge );
            TooltipHandler.TipRegion( rectb, tipAge );
            curX += 50f;
        }

        private static void DrawCell_Gender( Pawn p, Rect recta, ref float curX )
        {
            Texture2D labelSex = GenderTextures[(int) p.gender];
            TipSignal tipSex = p.gender.ToString();
            GUI.DrawTexture( recta, labelSex );
            TooltipHandler.TipRegion( recta, tipSex );
            curX += 50f;
        }

        private static void DrawCell_Follow( Pawn p, ref float curX )
        {
            Rect draftedRect = new Rect( curX, 0f, 25f, 30f );
            Rect hunterRect = new Rect( curX + 25f, 0f, 25f, 30f );
            curX += 50f;

            if ( p.training.IsCompleted( TrainableDefOf.Obedience ) )
            {
                Rect draftedIconRect =
                    new Rect( 0f, 0f, iconSize, iconSize ).CenteredOnYIn( draftedRect ).CenteredOnXIn( draftedRect );
                Rect hunterIconRect =
                    new Rect( 0f, 0f, iconSize, iconSize ).CenteredOnYIn( hunterRect ).CenteredOnXIn( hunterRect );

                // handle drafted follow
                bool followDrafted = p.playerSettings.followDrafted;
                string draftedTip = followDrafted
                                        ? "Fluffy.PetFollow.FollowingDrafted".Translate()
                                        : "Fluffy.PetFollow.NotFollowingDrafted".Translate();
                TooltipHandler.TipRegion( draftedRect, draftedTip );

                if ( followDrafted )
                    GUI.DrawTexture( draftedIconRect, WorkBoxCheckTex );

                if ( Mouse.IsOver( draftedRect ) )
                    Widgets.DrawHighlight( draftedIconRect );

                if ( Widgets.ButtonInvisible( draftedRect ) )
                {
                    p.playerSettings.followDrafted = !followDrafted;
                    if ( followDrafted )
                        SoundDefOf.CheckboxTurnedOff.PlayOneShotOnCamera();
                    else
                        SoundDefOf.CheckboxTurnedOn.PlayOneShotOnCamera();
                }

                // handle hunter follow
                bool followHunter = p.playerSettings.followFieldwork;
                string hunterTip = followHunter
                                       ? "Fluffy.PetFollow.FollowingHunter".Translate()
                                       : "Fluffy.PetFollow.NotFollowingHunter".Translate();
                TooltipHandler.TipRegion( hunterRect, hunterTip );

                if ( followHunter )
                    GUI.DrawTexture( hunterIconRect, WorkBoxCheckTex );

                if ( Mouse.IsOver( hunterRect ) )
                    Widgets.DrawHighlight( hunterIconRect );

                if ( Widgets.ButtonInvisible( hunterRect ) )
                {
                    p.playerSettings.followFieldwork = !followHunter;
                    if ( followHunter )
                        SoundDefOf.CheckboxTurnedOff.PlayOneShotOnCamera();
                    else
                        SoundDefOf.CheckboxTurnedOn.PlayOneShotOnCamera();
                }
            }
        }

        private static void DrawCell_Master( Rect rect, Pawn p, ref float curX )
        {
            if ( p.training.IsCompleted( TrainableDefOf.Obedience ) )
            {
                var rect2 = new Rect( curX, 0f, 90f, rect.height );
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
            curX += 90f;
        }
    }
}
