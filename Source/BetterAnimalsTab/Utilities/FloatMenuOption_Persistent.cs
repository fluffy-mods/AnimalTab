// FloatMenuOption_Persistent.cs
// Copyright Karel Kroeze, 2017-2017

using System;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace AnimalTab
{
    public class FloatMenuOption_Persistent : FloatMenuOption
    {
        public FloatMenuOption_Persistent( string label, Action action,
            MenuOptionPriority priority = MenuOptionPriority.Default, Action mouseoverGuiAction = null,
            Thing revalidateClickTarget = null, float extraPartWidth = 0.0f, Func<Rect, bool> extraPartOnGUI = null,
            WorldObject revalidateWorldClickTarget = null ) : base( label, action, priority, mouseoverGuiAction,
            revalidateClickTarget, extraPartWidth, extraPartOnGUI, revalidateWorldClickTarget )
        {
        }

        public override bool DoGUI( Rect rect, bool colonistOrdering, FloatMenu floatMenu )
        {
            // everything in base is fine and dandy.
            base.DoGUI( rect, colonistOrdering, floatMenu );

            // just return false so the parent window isn't removed.
            return false;
        }
    }
}