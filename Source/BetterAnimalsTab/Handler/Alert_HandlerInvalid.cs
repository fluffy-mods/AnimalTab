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
                        CompHandlerSettings settings = pawn.HandlerSettings();
                        if (settings?.Mode == HandlerMode.Specific &&
                             (settings.Handler.workSettings.GetPriority(WorkTypeDefOf.Handling) == 0 ||
                               settings.Handler.skills.GetSkill(SkillDefOf.Animals).Level < TrainableUtility.MinimumHandlingSkill(pawn))) {
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
