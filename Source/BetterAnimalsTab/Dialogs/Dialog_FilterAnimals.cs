// // Karel Kroeze
// // Dialog_FilterAnimals.cs
// // 2016-06-27

using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Fluffy
{
    public class Dialog_FilterAnimals : Window
    {
        public static bool Sticky;

        private static Rect _location;
        private readonly float _iconSize = 24f;
        private readonly float _iconWidthOffset = ( 50f - 24f ) / 2f;
        private readonly List<PawnKindDef> _pawnKinds;

        private readonly float _rowHeight = 30f;

        private float _actualHeight = 500f;
        /*
        float IconHeightOffset => (_rowHeight - _iconSize) / 2f;
*/

        private float _x;
        private float _x2;
        private float _y;

        public Dialog_FilterAnimals()
        {
            _pawnKinds = Find.MapPawns.PawnsInFaction( Faction.OfPlayer ).Where( x => x.RaceProps.Animal )
                             .Select( x => x.kindDef ).Distinct().OrderBy( x => x.LabelCap ).ToList();
            forcePause = true;
            closeOnEscapeKey = true;
            absorbInputAroundWindow = false;
            draggable = true;
        }

        public override Vector2 InitialSize => new Vector2( 600f, 65f + Mathf.Max( 200f, _actualHeight ) );

        private float ColWidth => InitialSize.x / 2f - 10f;

        private float LabWidth => ColWidth - 50f;

        public override void PreClose()
        {
            base.PreClose();
            _location = windowRect;
        }

        // kinda hacky, but sticky can't be set without opening, which populates location.
        public override void PostOpen()
        {
            base.PostOpen();
            if ( Sticky )
            {
                windowRect = _location;
            }
        }

        public override void DoWindowContents( Rect inRect )
        {
            // Close if animals tab closed
            if ( Find.WindowStack.WindowOfType<MainTabWindow_Animals>() == null )
            {
                Find.WindowStack.TryRemove( this );
            }

            Text.Font = GameFont.Small;


            // Pawnkinds on the left.
            _x = 5f;
            _y = 5f;
            _x2 = ColWidth - 45f;

            var rect = new Rect( _x, _y, ColWidth, _rowHeight );
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.LowerLeft;
            Widgets.Label( rect, "Fluffy.FilterByRace".Translate() );
            Text.Font = GameFont.Small;


            _y += _rowHeight;

            if ( _pawnKinds != null )
                foreach ( PawnKindDef pawnKind in _pawnKinds )
                {
                    DrawPawnKindRow( pawnKind );
                }

            // set window's actual height
            if ( _y > _actualHeight )
                _actualHeight = _y;

            // specials on the right.
            _x = inRect.width / 2f + 5f;
            _x2 = inRect.width / 2f + ColWidth - 45f;
            _y = 5f;

            Text.Font = GameFont.Tiny;
            var rectAttributes = new Rect( _x, 5f, ColWidth, _rowHeight );
            Widgets.Label( rectAttributes, "Fluffy.FilterByAttributes".Translate() );
            Text.Font = GameFont.Small;

            _y += _rowHeight;

            // draw filter rows.
            foreach ( Filter filter in Widgets_Filter.Filters )
            {
                DrawFilterRow( filter );
            }

            // set window's actual height
            if ( _y > _actualHeight )
                _actualHeight = _y;

            // sticky option
            var stickyRect = new Rect( 5f, inRect.height - 35f, inRect.width / 4 - 10, 35f );
            Widgets.CheckboxLabeled( stickyRect, "Fluffy.FilterSticky".Translate(), ref Sticky );


            // buttons
            if (
                Widgets.ButtonText(
                                   new Rect( inRect.width / 4f + 5f, inRect.height - 35f, inRect.width / 4f - 10f, 35f ),
                                   "Fluffy.Clear".Translate() ) )
            {
                Widgets_Filter.ResetFilter();
                Widgets_Filter.DisableFilter();
                MainTabWindow_Animals.IsDirty = true;
                Event.current.Use();
            }

            if ( !Widgets_Filter.Filter )
            {
                if ( Widgets.ButtonText( new Rect( _x, inRect.height - 35f, inRect.width / 4f - 10f, 35f ),
                                         "Fluffy.Enable".Translate() ) )
                {
                    Widgets_Filter.EnableFilter();
                    MainTabWindow_Animals.IsDirty = true;
                    Event.current.Use();
                }
            }
            else
            {
                if ( Widgets.ButtonText( new Rect( _x, inRect.height - 35f, inRect.width / 4f - 10f, 35f ),
                                         "Fluffy.Disable".Translate() ) )
                {
                    Widgets_Filter.DisableFilter();
                    MainTabWindow_Animals.IsDirty = true;
                    Event.current.Use();
                }
            }


            if (
                Widgets.ButtonText(
                                   new Rect( _x + inRect.width / 4, inRect.height - 35f, inRect.width / 4f - 10f, 35f ),
                                   "OK".Translate() ) )
            {
                Find.WindowStack.TryRemove( this );
                Event.current.Use();
            }

            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void DrawPawnKindRow( PawnKindDef pawnKind )
        {
            var rectRow = new Rect( _x, _y, ColWidth, _rowHeight );
            var rectLabel = new Rect( _x, _y, LabWidth, _rowHeight );
            if ( pawnKind != null )
            {
                Widgets.Label( rectLabel, pawnKind.LabelCap );
                var rectIcon = new Rect( _x2 + _iconWidthOffset, _y, _iconSize, _iconSize );
                bool inList = Widgets_Filter.FilterPawnKind.Contains( pawnKind );
                GUI.DrawTexture( rectIcon, inList ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex );
                if ( Mouse.IsOver( rectRow ) )
                {
                    GUI.DrawTexture( rectRow, TexUI.HighlightTex );
                }
                if ( Widgets.ButtonInvisible( rectRow ) )
                {
                    if ( inList )
                    {
                        SoundDefOf.CheckboxTurnedOff.PlayOneShotOnCamera();
                    }
                    else
                    {
                        SoundDefOf.CheckboxTurnedOn.PlayOneShotOnCamera();
                    }
                    Widgets_Filter.TogglePawnKindFilter( pawnKind, inList );
                    MainTabWindow_Animals.IsDirty = true;
                }
            }
            _y += _rowHeight;
        }

        private void DrawFilterRow( Filter filter )
        {
            var label = new StringBuilder();
            if ( filter != null )
            {
                label.Append( ( "Fluffy." + filter.Label + "Label" ).Translate() ).Append( " " );
                var rect = new Rect( _x, _y, ColWidth, _rowHeight );
                var rectLabel = new Rect( _x, _y, LabWidth, _rowHeight );
                var rectIcon = new Rect( _x2 + _iconWidthOffset, _y, _iconSize, _iconSize );
                switch ( filter.State )
                {
                    case FilterType.True:
                        GUI.DrawTexture( rectIcon, filter.Textures[0] );
                        label.Append( "(" ).Append( ( "Fluffy." + filter.Label + "Yes" ).Translate() ).Append( ")" );
                        break;
                    case FilterType.False:
                        GUI.DrawTexture( rectIcon, filter.Textures[1] );
                        label.Append( "(" ).Append( ( "Fluffy." + filter.Label + "No" ).Translate() ).Append( ")" );
                        break;
                    default:
                        GUI.DrawTexture( rectIcon, filter.Textures[2] );
                        label.Append( "(" ).Append( "Fluffy.Both".Translate() ).Append( ")" );
                        break;
                }
                Widgets.Label( rectLabel, label.ToString() );
                if ( Widgets.ButtonInvisible( rect ) )
                {
                    filter.Bump();
                    SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                    MainTabWindow_Animals.IsDirty = true;
                }
                if ( Mouse.IsOver( rect ) )
                {
                    GUI.DrawTexture( rect, TexUI.HighlightTex );
                }
            }
            _y += _rowHeight;
        }
    }
}
