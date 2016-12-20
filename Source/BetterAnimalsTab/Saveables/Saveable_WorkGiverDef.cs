// // Karel Kroeze
// // Saveable_WorkGiverDef.cs
// // 2016-06-27

using RimWorld;
using Verse;

namespace Fluffy
{
    internal class Saveable_WorkGiverDef : IExposable
    {
        #region Fields

        public string defName;
        public int priority;

        #endregion Fields

        #region Constructors

        public Saveable_WorkGiverDef()
        {
            // empty constructor for scribe.
        }

        public Saveable_WorkGiverDef( WorkGiverDef def )
        {
            defName = def.defName;
            priority = def.priorityInType;
        }

        #endregion Constructors

        #region Methods

        public void ExposeData()
        {
            Scribe_Values.LookValue( ref defName, "defName" );
            Scribe_Values.LookValue( ref priority, "priority" );
        }

        #endregion Methods
    }
}
