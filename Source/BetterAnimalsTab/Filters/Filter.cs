using RimWorld;
using UnityEngine;
using Verse;

namespace Fluffy
{
    public abstract class Filter
    {
        #region Properties

        public virtual string Label => "Filter";
        public abstract Texture2D[] Textures { get; }
        protected internal virtual FilterType State { get; set; }

        #endregion Properties

        #region Methods

        public void Bump()
        {
            int next = (int)State + 1;
            if ( next > 2 )
            {
                State = FilterType.True;
            }
            else if ( next == 1 )
            {
                State = FilterType.False;
            }
            else
            {
                State = FilterType.None;
            }
            Widgets_Filter.Filter = true;
            Widgets_Filter.FilterPossible = true;
        }

        public abstract bool IsAllowed( Pawn p );

        #endregion Methods
    }
}