// Alert_NoHandlerInRange.cs
// Copyright Karel Kroeze, -2019

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AnimalTab {
    public class Alert_NoHandlerInRange: Alert {
        public IEnumerable<Pawn> InvalidRanges {
            get {
                foreach (Map map in Find.Maps.Where(m => m.IsPlayerHome)) {
                    List<int> handlerSkills = map.mapPawns.FreeColonistsSpawned
                                           .Where( h => h.workSettings.GetPriority( WorkTypeDefOf.Handling ) > 0 )
                                           .Select( h => h.skills.GetSkill( SkillDefOf.Animals ).Level )
                                           .ToList();

                    foreach (Pawn pawn in map.mapPawns.AllPawns) {
                        CompHandlerSettings settings = pawn.HandlerSettings();
                        if (settings?.Mode == HandlerMode.Level &&
                             !handlerSkills.Any(hs => settings.Level.Contains(hs))) {
                            yield return pawn;
                        }
                    }
                }
            }
        }

        public override AlertReport GetReport() {
            return AlertReport.CulpritsAre(InvalidRanges.ToList());
        }

        public override string GetLabel() {
            return "Fluffy.AnimalTab.NoHandlerInRange".Translate(InvalidRanges.Count());
        }

        public override TaggedString GetExplanation() {
            return "Fluffy.AnimalTab.NoHandlerInRange.Tip".Translate(string.Join("\n    ", InvalidRanges.Select(p => p.LabelShort).ToArray()));
        }

        public override AlertPriority Priority => AlertPriority.Medium;
    }
}
