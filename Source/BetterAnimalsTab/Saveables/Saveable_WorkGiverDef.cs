// // Karel Kroeze
// // Saveable_WorkGiverDef.cs
// // 2016-06-27

using RimWorld;
using UnityEngine;
using Verse;

namespace Fluffy
{
    internal class Saveable_WorkGiverDef : IExposable
    {
        public string defName;
        public int priority;

        public Saveable_WorkGiverDef()
        {
            // empty constructor for scribe.
        }

        public Saveable_WorkGiverDef( WorkGiverDef def )
        {
            defName = def.defName;
            priority = def.priorityInType;
        }

        public void ExposeData()
        {
            Scribe_Values.LookValue( ref defName, "defName" );
            Scribe_Values.LookValue( ref priority, "priority" );
        }
    }
}
