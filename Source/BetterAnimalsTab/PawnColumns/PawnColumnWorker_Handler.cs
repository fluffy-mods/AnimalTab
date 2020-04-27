// PawnColumnWorker_Handler.cs
// Copyright Karel Kroeze, 2019-2019

using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace AnimalTab
{
    public class PawnColumnWorker_Handler: PawnColumnWorker
    {
        public override int GetMinWidth( PawnTable table ) => 100;
        public override int GetMaxWidth( PawnTable table ) => 100;

        public override void DoCell( Rect rect, Pawn target, PawnTable table )
        {
            var settings = target.handlerSettings();

            if ( settings.Mode == HandlerMode.Level )
            {
                if ( Mouse.IsOver( rect ) && Input.GetMouseButtonDown( 1 ) )
                    DoHandlerFloatMenu( target );

                var sliderRect = new Rect(
                        rect.xMin,
                        rect.yMin,
                        rect.width,
                        rect.height * 2f / 3f )
                   .CenteredOnYIn( rect );

                var level = settings.Level;
                Widgets.IntRange( sliderRect, target.GetHashCode(), ref level, 0, 20 );
                if ( level != settings.Level )
                    settings.Level = level.Clamp( target );

                TooltipHandler.TipRegion( rect, "Fluffy.AnimalTab.RightClickToChangeHandler".Translate() );
            }
            else
            {
                string label = settings.Handler?.LabelShort ??
                               $"({"Fluffy.AnimalTab.HandlerMode.Any".Translate().RawText.ToLowerInvariant()})";
                if ( Widgets.ButtonText( rect.ContractedBy( 2f ), label ) )
                    DoHandlerFloatMenu( target );
            }
        }

        private void DoHandlerFloatMenu( Pawn target )
        {
            var settings = target.handlerSettings();
            var minSkill = TrainableUtility.MinimumHandlingSkill( target );
            var options = new List<FloatMenuOption>();
            options.Add( new FloatMenuOption( HandlerMode.Any.Label(), () => settings.Mode   = HandlerMode.Any ) );
            options.Add( new FloatMenuOption( HandlerMode.Level.Label(), () => settings.Mode = HandlerMode.Level ) );

            foreach ( var handler in HandlerUtility.HandlersOrdered( target.Map ) )
                options.Add( new FloatMenuOption( HandlerUtility.HandlerLabel( handler, minSkill ),
                                                  () => settings.Handler = handler ) );

            Find.WindowStack.Add( new FloatMenu( options ) );
        }

        private void DoMassHandlerFloatMenu( List<Pawn> targets, Map map )
        {
            var options = new List<FloatMenuOption>();
            options.Add( new FloatMenuOption( HandlerMode.Any.Label(), () => targets.ForEach( t => t.handlerSettings().Mode = HandlerMode.Any ) ) );
            options.Add( new FloatMenuOption( HandlerMode.Level.Label(), () => targets.ForEach( t => t.handlerSettings().Mode = HandlerMode.Level ) ) );

            foreach ( var handler in HandlerUtility.HandlersOrdered( map ) )
                options.Add( new FloatMenuOption( HandlerUtility.HandlerLabel( handler ), () => targets.ForEach( t => t.handlerSettings().Handler = handler ) ) );

            Find.WindowStack.Add( new FloatMenu( options ) );
        }

        public override int Compare( Pawn a, Pawn b )
        {
            var settingsA = a.handlerSettings();
            var settingsB = b.handlerSettings();
            if ( settingsA.Mode != settingsB.Mode )
                return settingsA.Mode.CompareTo( settingsB.Mode );

            if ( settingsA.Mode == HandlerMode.Specific )
                return string.Compare( settingsA.Handler.LabelShort, settingsB.Handler.LabelShort, StringComparison.Ordinal );
            if ( settingsA.Mode == HandlerMode.Level )
            {
                if ( settingsA.Level.min != settingsB.Level.min ) 
                    return settingsA.Level.min.CompareTo( settingsB.Level.min );
                if ( settingsA.Level.max != settingsB.Level.max )
                    return settingsA.Level.min.CompareTo( settingsB.Level.max );
            }

            return 0;
        }

        public override void DoHeader( Rect rect, PawnTable table )
        {
            var targets = table.PawnsListForReading;
            if ( Input.GetKey( KeyCode.LeftShift ) || Input.GetKey( KeyCode.RightShift ) )
            {
                if ( targets.Any() && targets.All( p => p.handlerSettings()?.Mode == HandlerMode.Level ) )
                {
                    if ( Mouse.IsOver( rect ) && Input.GetMouseButtonDown( 1 ) )
                        DoMassHandlerFloatMenu( targets, Find.CurrentMap );

                    var sliderRect = new Rect(
                            rect.xMin,
                            rect.yMin,
                            rect.width,
                            rect.height * 2f / 3f )
                       .CenteredOnYIn( rect );

                    var level = targets.First().handlerSettings().Level;
                    var _level = level;
                    Widgets.IntRange( sliderRect, 24, ref level, 0, 20 );
                    if ( level != _level )
                        foreach ( var target in targets )
                            target.handlerSettings().Level = level.Clamp( target );

                    TooltipHandler.TipRegion( rect, "Fluffy.AnimalTab.HandlerHeaderTip_RightClick".Translate() );
                }
                else
                {
                    if ( Widgets.ButtonInvisible( rect ) )
                        DoMassHandlerFloatMenu( targets, Find.CurrentMap );
                    base.DoHeader( rect, table );
                }
            }
            else
            {
                base.DoHeader( rect, table );
            }
        }

        protected override string GetHeaderTip( PawnTable table )
        {
            return base.GetHeaderTip( table ) + "\n" + "Fluffy.AnimalTab.HandlerHeaderTip".Translate();
        }
    }
}