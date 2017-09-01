// PawnTableFilterWorker.cs
// Copyright Karel Kroeze, 2017-2017

using System;
using System.Resources;
using UnityEngine;
using Verse;

namespace AnimalTab
{
    public abstract class FilterWorker
    {
        protected FilterState _state = FilterState.Inactive;
        public FilterDef def;

        public virtual string Adjective( FilterState state )
        {
                switch ( state )
                {
                    case FilterState.Inclusive:
                        return !def.inclusiveAdjective.NullOrEmpty() 
                        ? def.inclusiveAdjective 
                        : def.label;
                    case FilterState.Exclusive:
                        return !def.exclusiveAdjective.NullOrEmpty()
                            ? def.exclusiveAdjective
                            : "AnimalTab.Not".Translate( Adjective( FilterState.Inclusive ) );
                    case FilterState.Inactive:
                    default:
                        return !def.inactiveAdjective.NullOrEmpty()
                        // I apologize if any translators hate my built-in space.
                            ? "AnimalTab.Any".Translate() + " " + def.inactiveAdjective
                            : "AnimalTab.Any".Translate();
            }
        }

        public virtual FilterState State
        {
            get => _state;
            set
            {
                if ( value == _state )
                    return;

                _state = value;
                MainTabWindow_Animals.Instance.Notify_PawnsChanged();
            }
        }

        public virtual FilterState NextState => (FilterState) ( ( (int)State + 1 ) % Enum.GetValues( typeof( FilterState ) ).Length );

        public virtual Color Colour
        {
            get
            {
                switch ( State )
                {
                    case FilterState.Inclusive:
                        return GenUI.MouseoverColor;
                    case FilterState.Exclusive:
                        return Constants.DarkRed;
                    default:
                        return Color.grey;
                }
            }
        }

        public abstract bool Allows( Pawn pawn );

        public virtual string GetTooltip()
        {
            return "AnimalTab.FilterTip".Translate( Adjective( State ), Adjective( NextState ) );
        }

        public virtual void Clicked()
        {
            State = NextState;
        }

        public virtual void Draw( Rect rect )
        {
            TooltipHandler.TipRegion( rect, GetTooltip, (int)rect.center.x * 541 );
            if ( Widgets.ButtonImage( rect, Icon, Colour, Colour * 1.5f ) )
                Clicked();
        }

        public virtual Texture2D Icon
        {
            get
            {
                if (def.iconTex == null)
                    def.iconTex = ContentFinder<Texture2D>.Get(def.icon, true);
                return def.iconTex;
            }
        }
    }
}