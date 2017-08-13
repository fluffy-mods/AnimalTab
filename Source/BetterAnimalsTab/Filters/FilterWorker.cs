// PawnTableFilterWorker.cs
// Copyright Karel Kroeze, 2017-2017

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
                // TODO: Rebuild table
            }
        }

        public virtual Color Colour
        {
            get
            {
                switch ( State )
                {
                    case FilterState.Inclusive:
                        return Color.white;
                    case FilterState.Exclusive:
                        return Color.red;
                    default:
                        return Color.grey;
                }
            }
        }

        public abstract bool Allows( Pawn pawn );

        public virtual string GetTooltip()
        {
            return "AnimalTab.FilterTip".Translate( Adjective( State ) );
        }

        public virtual void Clicked()
        {
            switch ( State )
            {
                case FilterState.Inactive:
                    State = FilterState.Inclusive;
                    break;
                case FilterState.Inclusive:
                    State = FilterState.Exclusive;
                    break;
                case FilterState.Exclusive:
                    State = FilterState.Inactive;
                    break;
            }
        }

        public virtual void Draw( Rect rect )
        {
            if ( Widgets.ButtonImage( rect, Icon, Colour, GenUI.MouseoverColor ) )
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