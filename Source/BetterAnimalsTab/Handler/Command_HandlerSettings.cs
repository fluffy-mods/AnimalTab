using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace AnimalTab
{
    public class Command_HandlerSettings: Command_Action
    {
        private CompHandlerSettings comp;
        private const float Margin = 6f;
        private const float HandleIconSize = Height / 3f;

        public Command_HandlerSettings( CompHandlerSettings comp )
        {
            this.comp = comp;
        }

        public override float GetWidth( float maxWidth )
        {
            return comp.Mode == HandlerMode.Any ? base.GetWidth( maxWidth ) : 2 * base.GetWidth( maxWidth );
        }

        protected virtual IEnumerable<Command_HandlerSettings> AffectedGizmos
        {
            get
            {
                return Find.Selector.SelectedObjects.SelectMany( obj => ( obj as ISelectable )?.GetGizmos() )
                           .Where( g => g.GroupsWith( this ) )
                           .Cast<Command_HandlerSettings>()
                           .ToList();
            }
        }

        public override GizmoResult GizmoOnGUI( Vector2 topLeft, float maxWidth )
        {
            var gizmoRect = new Rect( topLeft.x, topLeft.y, GetWidth( maxWidth ), Height );
            var mouseOver = Mouse.IsOver( gizmoRect );
            if ( mouseOver )
                GUI.color = GenUI.MouseoverColor;
            Widgets.DrawAtlas( gizmoRect, BGTex );
            GUI.color = Color.white;

            // mode selector, left side.
            var target = comp.parent as Pawn;
            var modeRect = new Rect( topLeft.x, topLeft.y, Height, Height );
            var targetIconRect = modeRect.ContractedBy( Margin );
            var handleIconRect = new Rect( targetIconRect.x, targetIconRect.y, HandleIconSize, HandleIconSize );
            GUI.DrawTexture( targetIconRect, PortraitsCache.Get( target, modeRect.size ) );
            GUI.DrawTexture( handleIconRect, Find.ReverseDesignatorDatabase.Get<Designator_Tame>().icon );
            Widgets.DrawHighlightIfMouseover( modeRect );
            if ( Widgets.ButtonInvisible( modeRect ) )
                Find.WindowStack.Add( FloatMenu_HandlerMode );

            if ( comp.Mode == HandlerMode.Specific )
            {
                var handler = comp.Handler;
                var handlerRect = modeRect;
                handlerRect.x += modeRect.width;

                var handlerIconRect = handlerRect.ContractedBy( Margin );
                if ( handler != null )
                    GUI.DrawTexture( handlerIconRect, PortraitsCache.Get( handler, handlerRect.size ) );
                Widgets.DrawHighlightIfMouseover( handlerRect );
                if ( Widgets.ButtonInvisible( handlerRect ) )
                    Find.WindowStack.Add( FloatMenu_Handler );
            }

            if ( comp.Mode == HandlerMode.Level )
            {
                var level     = comp.Level;
                var levelRect = modeRect.ContractedBy( Margin );
                levelRect.x += modeRect.width;
                levelRect.y += levelRect.height / 4f;
                levelRect.height /= 2f;

                var newLevel = level;
                Widgets.IntRange( levelRect, comp.GetHashCode(), ref newLevel, 0, 20 );
                if ( newLevel != level )
                    MassSetLevel( newLevel );
            }

            // draw label
            if ( !LabelCap.NullOrEmpty() )
            {
                Text.Font = GameFont.Tiny;
                float labelHeight = Text.CalcHeight( LabelCap, gizmoRect.width );
                Rect labelRect = new Rect( gizmoRect.x, gizmoRect.yMax - labelHeight + 12f, gizmoRect.width, labelHeight );
                GUI.DrawTexture( labelRect, TexUI.GrayTextBG );
                GUI.color   = Color.white;
                Text.Anchor = TextAnchor.UpperCenter;
                Widgets.Label( labelRect, LabelCap );
                Text.Anchor = TextAnchor.UpperLeft;
                Text.Font = GameFont.Small;
                GUI.color   = Color.white;
            }

            return new GizmoResult( mouseOver ? GizmoState.Mouseover : GizmoState.Clear );
        }

        public override string Label
        {
            get
            {
                if ( comp.Mode == HandlerMode.Any )
                    return "Fluffy.AnimalTab.HandlerX".Translate( HandlerMode.Any.Label() );
                if ( comp.Mode == HandlerMode.Level )
                    return "Fluffy.AnimalTab.HandlerX".Translate(
                        "Fluffy.AnimalTab.Level".Translate( comp.Level.min, comp.Level.max ) );
                if ( comp.Mode == HandlerMode.Specific )
                    return "Fluffy.AnimalTab.HandlerX".Translate( comp.Handler?.Name.ToStringShort ?? "NULL" );
                return "Fluffy.AnimalTab.Handler".Translate();
            }
        }

        public override bool GroupsWith( Gizmo other )
        {
            return other is Command_HandlerSettings otherHandler   && // same type of command
                   otherHandler.comp.parent.def == comp.parent.def && // same type of animal
                   otherHandler.comp.Mode       == comp.Mode;         // currently in same mode
        }

        protected virtual FloatMenu FloatMenu_HandlerMode
        {
            get
            {
                var options = new List<FloatMenuOption>();
                foreach ( var mode in HandlerUtility.HandlerModes )
                    options.Add( new FloatMenuOption( $"Fluffy.AnimalTab.HandlerMode.{mode}".Translate(),
                                                      () => MassSetMode( mode ) ) );

                return new FloatMenu( options );
            }
        }

        protected virtual void MassSetMode( HandlerMode mode )
        {
            foreach ( var gizmo in AffectedGizmos )
                gizmo.comp.Mode = mode;
        }

        protected virtual FloatMenu FloatMenu_Handler
        {
            get
            {
                var options = new List<FloatMenuOption>();
                var handlers = HandlerUtility.HandlersOrdered( comp.parent.Map );
                var minSkill = TrainableUtility.MinimumHandlingSkill( comp.Target );

                foreach ( var handler in handlers )
                    options.Add( new FloatMenuOption( HandlerUtility.HandlerLabel( handler, minSkill ), () => MassSetHandler( handler ) ) );

                return new FloatMenu( options );
            }
        }

        protected void MassSetHandler( Pawn handler )
        {
            foreach ( var gizmo in AffectedGizmos )
                gizmo.comp.Handler = handler;
        }

        protected void MassSetLevel( IntRange level )
        {
            foreach ( var gizmo in AffectedGizmos )
                gizmo.comp.Level = level.Clamp( gizmo.comp.Target );
        }
    }
}
