// Utilities.cs
// Copyright Karel Kroeze, 2017-2017

using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using static AnimalTab.Constants;
using static AnimalTab.Resources;

namespace AnimalTab {
    public static class Utilities {
        private static readonly FieldInfo checkboxPaintingFieldInfo = AccessTools.Field(typeof(Widgets), "checkboxPainting");

        public static bool CheckboxPainting {
            get => (bool) checkboxPaintingFieldInfo.GetValue(null);
            set => checkboxPaintingFieldInfo.SetValue(null, value);
        }

        private static readonly FieldInfo checkboxPaintingStateFieldInfo = AccessTools.Field(typeof(Widgets), "checkboxPaintingState");

        public static bool CheckboxPaintingState {
            get => (bool) checkboxPaintingStateFieldInfo.GetValue(null);
            set => checkboxPaintingStateFieldInfo.SetValue(null, value);
        }

        public static bool IsBonded(this Pawn pawn) {
            return pawn.BondedPawn() != null;
        }

        public static Pawn BondedPawn(this Pawn pawn) {
            return pawn.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Bond);
        }

        public static void DoCheckbox(Rect rect, ref bool value, Func<string> tipGetter = null, bool background = true,
                                       bool mouseover = true, Texture2D backgroundTexture = null,
                                       Texture2D checkOn = null, Texture2D checkOff = null) {
            // background and hover
            if (background) {
                DrawCheckboxBackground(rect, backgroundTexture);
            }

            if (mouseover) {
                Widgets.DrawHighlightIfMouseover(rect);
            }

            if (tipGetter != null) {
                TooltipHandler.TipRegion(rect, tipGetter, rect.GetHashCode());
            }

            // draw textures
            if (value || checkOff != null) {
                GUI.DrawTexture(rect, value ? checkOn ?? Widgets.CheckboxOnTex : checkOff);
            }

            // interactions
            Widgets.DraggableResult button = Widgets.ButtonInvisibleDraggable(rect);
            if (button == Widgets.DraggableResult.Pressed) {
                value = !value;
            } else if (Widgets.ButtonInvisible(rect)) {
                // just to add right-click interactions as well. Ugh.
                value = !value;
            }

            if (button == Widgets.DraggableResult.Dragged) {
                value = !value;
                CheckboxPainting = true;
                CheckboxPaintingState = value;
            }

            if (Mouse.IsOver(rect) && CheckboxPainting && Input.GetMouseButton(0) &&
                 value != CheckboxPaintingState) {
                value = CheckboxPaintingState;
            }
        }

        public static void DrawCheckboxBackground(Rect rect, Texture2D background = null) {
            GUI.DrawTexture(rect, background ?? Background_Dark);
            if (Controller.Settings.HighContrast) {
                Color color = GUI.color;
                GUI.color = new Color(.3f, .3f, .3f, 1f);
                Widgets.DrawBox(rect);
                GUI.color = color;
            }
        }

        public static Rect GetCheckboxRect(Rect rect) {
            return new Rect(rect.x + ((rect.width - CheckBoxSize) / 2), rect.y + ((rect.height - CheckBoxSize) / 2),
                CheckBoxSize, CheckBoxSize);
        }

        public static IntRange GetTrainingProgress(Pawn pawn, TrainableDef trainable) {
            int cur = Traverse.Create(pawn.training).Method("GetSteps", trainable).GetValue<int>();
            int max = trainable.steps;
            return new IntRange(cur, max);
        }

        public static void DrawCheckColoured(Rect rect, Color color) {
            Color curColor = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, CheckOnWhite);
            GUI.color = curColor;
        }

        public static void DrawTrainingProgress(Rect rect, Pawn pawn, TrainableDef trainable, Color color) {
            IntRange steps = GetTrainingProgress(pawn, trainable);
            Rect progressRect = new Rect(rect.xMin, rect.yMax - (rect.height / 5f),
                rect.width / steps.max * steps.min, rect.height / 5f);

            Widgets.DrawBoxSolid(progressRect, color);
        }

        public static void DoTrainableTooltip(Rect rect, Pawn pawn, TrainableDef td, AcceptanceReport canTrain) {
            Traverse.Create(typeof(TrainingCardUtility))
                .Method("DoTrainableTooltip", rect, pawn, td, canTrain)
                .GetValue(); // invoke
        }



        public static void DoTrainableTooltip(Rect rect, Pawn pawn, TrainableDef td, AcceptanceReport canTrain,
            bool wanted, bool completed, IntRange steps) {
            // copy pasta from TrainingCardUtility.DoTrainableTooltip
            TooltipHandler.TipRegion(rect, () => {
                string text = td.LabelCap + "\n\n" + td.description;
                if (!canTrain.Accepted) {
                    text = text + "\n\n" + canTrain.Reason;
                } else if (!td.prerequisites.NullOrEmpty()) {
                    text += "\n";
                    for (int i = 0; i < td.prerequisites.Count; i++) {
                        if (!pawn.training.HasLearned(td.prerequisites[i])) {
                            text = text + "\n" + "TrainingNeedsPrerequisite".Translate(td.prerequisites[i].LabelCap);
                        }
                    }
                }
                if (completed && steps.min == steps.max) {
                    text += "\n" + "Fluffy.AnimalTab.XHasMasteredY".Translate(pawn.Name.ToStringShort, td.LabelCap);
                }
                if (wanted && !completed) {
                    text += "\n" + "Fluffy.AnimalTab.XHasLearnedYOutOfZ".Translate(pawn.Name.ToStringShort, steps.min,
                                steps.max);
                }
                if (completed && steps.min < steps.max) {
                    text += "\n" + "Fluffy.AnimalTab.XHasForgottenYOutOfZ".Translate(pawn.Name.ToStringShort,
                                steps.max - steps.min, steps.max);
                }
                if (wanted) {
                    text += "\n" + "Fluffy.AnimalTab.XIsDesignatedTrainY".Translate(pawn.Name.ToStringShort,
                                td.LabelCap);
                } else if (completed || steps.min > 0) {
                    text += "\n" + "Fluffy.AnimalTab.XIsNotDesignatedTrainY".Translate(pawn.Name.ToStringShort,
                                td.LabelCap);
                }

                return text;
            }, (int) ((rect.y * 612) + rect.x));
        }
    }
}
