using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AnimalTab {
    public class CompHandlerSettings: ThingComp {
        private HandlerMode _mode = HandlerMode.Any;
        private Pawn _handler = null;
        private IntRange _level = new IntRange( 0, 20 );

        public Pawn Target => parent as Pawn;

        public HandlerMode Mode {
            get => _mode;
            set {
                if (_mode == value) {
                    return;
                }

                _mode = value;
                SetDefaults();
            }
        }

        protected virtual void SetDefaults() {
            _level = new IntRange(TrainableUtility.MinimumHandlingSkill(parent as Pawn), 20);

            if (_mode == HandlerMode.Specific) {
                IEnumerable<Pawn> handlers = parent.Map.mapPawns.FreeColonistsSpawned
                                     .Where( p => p.workSettings.GetPriority( WorkTypeDefOf.Handling ) > 0 );

                if (!handlers.Any()) {
                    handlers = parent.Map.mapPawns.FreeColonistsSpawned;
                }

                _handler = handlers.Any() ? handlers.MaxBy(p => p.skills.AverageOfRelevantSkillsFor(WorkTypeDefOf.Handling)) : null;
            } else {
                _handler = null;
            }
        }

        public Pawn Handler {
            get => _handler;
            set {
                _mode = value == null ? HandlerMode.Any : HandlerMode.Specific;

                _handler = value;
            }
        }

        public IntRange Level {
            get => _level;
            set {
                if (value.min != 0 || value.max != 20) {
                    _mode = HandlerMode.Level;
                }

                _level = value;
            }
        }

        public override void PostExposeData() {
            base.PostExposeData();

            Scribe_References.Look(ref _handler, "animalTab.handler.pawn");
            Scribe_Values.Look(ref _mode, "animalTab.handler.mode", forceSave: true);
            Scribe_Values.Look(ref _level, "animalTab.handler.level");
        }

        public IEnumerable<Gizmo> GetGizmos() {
            yield return new Command_HandlerSettings(this);
        }

        public bool Allows(Pawn handler, out string reason) {
            reason = null;
            if (Mode == HandlerMode.Specific && handler != Handler) {
                reason = "Fluffy.AnimalTab.NotAllowed.SpecificHandler".Translate(
                    Target?.LabelShort, Handler.LabelShort);
                return false;
            }

            if (Mode == HandlerMode.Level) {
                int handlerSkill = handler.skills.GetSkill( SkillDefOf.Animals ).Level;
                if (handlerSkill < Level.min) {
                    reason = "Fluffy.AnimalTab.NotAllowed.SkillTooLow".Translate(
                        Target?.LabelShort, handlerSkill, Level.min);
                    return false;
                }

                if (handlerSkill > Level.max) {
                    reason = "Fluffy.AnimalTab.NotAllowed.SkillTooHigh".Translate(
                        Target?.LabelShort, handlerSkill, Level.max);
                    return false;
                }
            }

            return true;
        }
    }

    public static class CompHandlerSettingsExtensions {
        public static CompHandlerSettings handlerSettings(this Pawn pawn) {
            CompHandlerSettings handler = pawn.GetComp<CompHandlerSettings>();
            if (handler == null) {
                throw new InvalidOperationException($"tried to get handlerSettings for {pawn.LabelShort}, who has none.");
            }

            return handler;
        }
    }
}
