// Alert_HandlerInvalid.cs
// Copyright Karel Kroeze, 2019-2019

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AnimalTab {
    public class Alert_HandlerInvalid: Alert {
        public IEnumerable<Pawn> InvalidHandlers {
            get {
                foreach (Map map in Find.Maps.Where(m => m.IsPlayerHome)) {
                    foreach (Pawn pawn in map.mapPawns.AllPawns) {
                        CompHandlerSettings handler = pawn?.HandlerSettings();
                        if (handler is not null &&
                            handler.Mode == HandlerMode.Specific &&
                            !handler.IsValid) {
                            yield return pawn;
                        }
                    }
                }
            }
        }

        public override AlertReport GetReport() {
            return AlertReport.CulpritsAre(InvalidHandlers.ToList());
        }

        public override string GetLabel() {
            return "Fluffy.AnimalTab.InvalidHandlers".Translate();
        }

        public override TaggedString GetExplanation() {
            return "Fluffy.AnimalTab.InvalidHandlers.Tip".Translate(
                string.Join("\n    ", InvalidHandlers.Select(p => p.LabelShort).ToArray()));
        }

        public override AlertPriority Priority => AlertPriority.Medium;
    }
}
