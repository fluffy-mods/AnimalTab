using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;
using RimWorld;
using UnityEngine;
using System.Reflection;

namespace Fluffy
{
    public static class WidgetsAnimals
    {
        public static readonly Texture2D[] trainingTextures = new Texture2D[]
        {
            ContentFinder<Texture2D>.Get("UI/Training/obedience", true),
            ContentFinder<Texture2D>.Get("UI/Training/release", true),
            ContentFinder<Texture2D>.Get("UI/Training/rescue", true),
            ContentFinder<Texture2D>.Get("UI/Training/haul", true)
        };

        public static readonly Texture2D CheckWhite = ContentFinder<Texture2D>.Get("UI/Buttons/CheckOnWhite");

        public static readonly Texture2D barBG = SolidColorMaterials.NewSolidColorTexture(0f, 1f, 0f, 0.8f);

        public static List<TrainableDef> trainables
        {
            get
            {
                return TrainableUtility.TrainableDefsInListOrder;
            }
        }

        public static void SlaughterAnimal(Pawn pawn)
        {
            Find.DesignationManager.AddDesignation(new Designation(pawn, DesignationDefOf.Slaughter));
        }

        public static void UnSlaughterAnimal(Pawn pawn)
        {
            Find.DesignationManager.DesignationOn(pawn, DesignationDefOf.Slaughter).Delete();
        }

        public static void SlaughterAllAnimals(List<Pawn> pawns)
        {
            if (pawns.All(p => Find.DesignationManager.DesignationOn(p, DesignationDefOf.Slaughter) != null))
            { 
                SoundDefOf.CheckboxTurnedOff.PlayOneShotOnCamera();
                for (int i = 0; i < pawns.Count; i++)
                {
                    UnSlaughterAnimal(pawns[i]);
                }
            } else
            {
                SoundDefOf.CheckboxTurnedOff.PlayOneShotOnCamera();
                for (int i = 0; i < pawns.Count; i++)
                {
                    if (Find.DesignationManager.DesignationOn(pawns[i], DesignationDefOf.Slaughter) == null) SlaughterAnimal(pawns[i]);
                }
            }
        }

        public static void DoAllowedAreaHeaders(Rect rect, List<Pawn> pawns, AllowedAreaMode mode)
        {
            List<Area> allAreas = Find.AreaManager.AllAreas;
            int num = 1;
            for (int i = 0; i < allAreas.Count; i++)
            {
                if (allAreas[i].AssignableAsAllowed(mode))
                {
                    num++;
                }
            }
            float num2 = rect.width / (float)num;
            Text.WordWrap = false;
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.LowerCenter;
            Rect rect2 = new Rect(rect.x, rect.y, num2, rect.height);
            Widgets.Label(rect2, "NoAreaAllowed".Translate());
            if (Widgets.InvisibleButton(rect2))
            {
                for (int i = 0; i < pawns.Count; i++)
                {
                    SoundDefOf.DesignateDragStandardChanged.PlayOneShotOnCamera();
                    pawns[i].playerSettings.AreaRestriction = null;
                }
            }
            if (Mouse.IsOver(rect2))
            {
                GUI.DrawTexture(rect2, TexUI.HighlightTex);
            }
            TooltipHandler.TipRegion(rect2, "NoAreaAllowed".Translate());
            int num3 = 1;
            for (int j = 0; j < allAreas.Count; j++)
            {
                if (allAreas[j].AssignableAsAllowed(mode))
                {
                    float num4 = (float)num3 * num2;
                    Rect rect3 = new Rect(rect.x + num4, rect.y, num2, rect.height);
                    Widgets.Label(rect3, allAreas[j].Label);
                    if (Widgets.InvisibleButton(rect3))
                    {
                        for (int i = 0; i < pawns.Count; i++)
                        {
                            SoundDefOf.DesignateDragStandardChanged.PlayOneShotOnCamera();
                            pawns[i].playerSettings.AreaRestriction = allAreas[j];
                        }
                    }
                    TooltipHandler.TipRegion(rect3, "Restrict all to " + allAreas[j].Label);
                    if (Mouse.IsOver(rect3))
                    {
                        GUI.DrawTexture(rect3, TexUI.HighlightTex);
                    }
                    num3++;
                }
            }
            Text.WordWrap = true;
        }

        public static void DoTrainingHeaders(Rect rect, List<Pawn> pawns)
        {
            List<TrainableDef> trainables = TrainableUtility.TrainableDefsInListOrder;
            float width = rect.width / trainables.Count();
            float iconSize = 16f;
            float widthOffset = (width - iconSize) / 2;
            float heightOffset = (rect.height - iconSize) / 2;
            float x = rect.xMin;
            float y = rect.yMin;

            for (int i = 0; i < trainables.Count(); i++)
            {
                Rect bg = new Rect(x, y, width, rect.height);
                Rect icon = new Rect(x + widthOffset, y + heightOffset, iconSize, iconSize);
                x += width;
                if (Mouse.IsOver(bg))
                {
                    GUI.DrawTexture(bg, TexUI.HighlightTex);
                    Log.Message(trainables[i].label);
                }
                TooltipHandler.TipRegion(bg, "Click to sort by" + trainables[i].LabelCap + "\nShift-click to train all\n\n" + trainables[i].description);
                GUI.DrawTexture(icon, trainingTextures[i]);
                if(Widgets.InvisibleButton(bg))
                {
                    if (!Event.current.shift)
                    {
                        if (MainTabWindow_Animals.order == MainTabWindow_Animals.orders.Training && MainTabWindow_Animals.trainingOrder == trainables[i])
                        {
                            MainTabWindow_Animals.asc = !MainTabWindow_Animals.asc;
                        }
                        else
                        {
                            MainTabWindow_Animals.order = MainTabWindow_Animals.orders.Training;
                            MainTabWindow_Animals.asc = false;
                            MainTabWindow_Animals.trainingOrder = trainables[i];
                        }
                    } 
                    else if (Event.current.shift)
                    {
                        ToggleAllTraining(trainables[i], pawns);
                    }
                    SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                    MainTabWindow_Animals.isDirty = true;
                }
            }
        }

        public static void DoTrainingRow(Rect rect, Pawn pawn)
        {
            List<TrainableDef> trainables = TrainableUtility.TrainableDefsInListOrder;
            float width = rect.width / trainables.Count();
            float iconSize = 16f;
            float widthOffset = (width - iconSize) / 2;
            float heightOffset = (rect.height - iconSize) / 2;
            float x = rect.xMin;
            float y = rect.yMin;

            for (int i = 0; i < trainables.Count(); i++)
            {
                Rect bg = new Rect(x, y, width, rect.height);
                Rect icon = new Rect(x + widthOffset, y + heightOffset, iconSize, iconSize);
                x += width;
                bool vis;
                AcceptanceReport report = pawn.training.CanAssignToTrain(trainables[i], out vis);
                TooltipHandler.TipRegion(bg, getTrainingTip(pawn, trainables[i], report));
                if (vis)
                {
                    drawTrainingButton(icon, pawn, trainables[i], report);
                    if (report.Accepted && !pawn.training.IsCompleted(trainables[i]))
                    {
                        if (Widgets.InvisibleButton(bg))
                        {
                            ToggleTraining(trainables[i], pawn, report);
                        }
                        if (Mouse.IsOver(icon))
                        {
                            GUI.DrawTexture(icon, TexUI.HighlightTex);
                        }
                    }
                }
            }
        }

        public static void ToggleTraining(TrainableDef td, Pawn pawn, AcceptanceReport ar)
        {
            bool train = !pawn.training.GetWanted(td);
            if (ar.Accepted)
            {
                SetWantedRecursive(td, pawn, train);
            }
        }

        public static void drawTrainingButton(Rect rect, Pawn pawn, TrainableDef td, AcceptanceReport ar)
        {
            if (ar.Accepted)
            {
                if (pawn.training.IsCompleted(td))
                {
                    GUI.DrawTexture(rect, CheckWhite);
                }
                else if (pawn.training.GetWanted(td))
                {
                    GUI.DrawTexture(rect, Widgets.CheckboxOnTex);
                    //due to stupidity in core, this won't work. (pawn.training.GetSteps is internal)
                    //int steps = td.steps;
                    //float barHeight = (rect.height / steps) * pawn.training.GetSteps(td);
                    //Rect bar = new Rect(rect.xMax - 5f, rect.yMax - barHeight, 3f, barHeight);
                    //GUI.DrawTexture(bar, barBG);
                }
            }
        }

        public static string getTrainingTip(Pawn pawn, TrainableDef td, AcceptanceReport ar)
        {
            string label = td.LabelCap + "\n";
            if (!ar.Accepted)
            {
                label += ar.Reason + "\n";
            }
            else
            {
                if (pawn.training.IsCompleted(td))
                {
                    label += "Training completed.";
                }
                else
                {
                    if (!pawn.training.GetWanted(td))
                    {
                        label += "Not training.\n";
                    }
                    else
                    {
                        label += "Currently training.\n";
                    }
                    //due to stupidity in core, this won't work. (pawn.training.GetSteps is internal)
                    //DefMap<TrainableDef, int> stepsComplete = new DefMap<TrainableDef, int>();
                    //int steps = td.steps;
                    //label += stepsComplete[td].ToString() + " out of " + steps + " completed.";
                }
            }
            return label;
        }

        public static void ToggleAllTraining(TrainableDef td, List<Pawn> pawns)
        {
            bool[] visible = new bool[pawns.Count];
            AcceptanceReport[] canAssign = new AcceptanceReport[pawns.Count];
            bool[] assigned = pawns.Select(p => p.training.GetWanted(td)).ToArray();
            bool[] trained = pawns.Select(p => p.training.IsCompleted(td)).ToArray();
            bool all = true;

            for (int i = 0; i < pawns.Count; i++)
            {
                pawns[i].training.CanAssignToTrain(td, out visible[i]);
                if (!assigned[i] && !trained[i] && canAssign[i].Accepted) all = false;
            }

            for (int i = 0; i < pawns.Count; i++)
            {
                if (all && assigned[i])
                {
                    SetWantedRecursive(td, pawns[i], false);
                } else if (!assigned[i] && canAssign[i].Accepted && !trained[i])
                {
                    SetWantedRecursive(td, pawns[i], true);
                }
            }            
        }

        private static void SetWantedRecursive(TrainableDef td, Pawn pawn, bool checkOn)
        {
            pawn.training.SetWanted(td, checkOn);
            if (checkOn)
            {

                SoundDefOf.CheckboxTurnedOn.PlayOneShotOnCamera();
                if (td.prerequisites != null)
                {
                    for (int i = 0; i < td.prerequisites.Count; i++)
                    {
                        SetWantedRecursive(td.prerequisites[i], pawn, true);
                    }
                }
            }
            else
            {
                SoundDefOf.CheckboxTurnedOff.PlayOneShotOnCamera();
                IEnumerable<TrainableDef> enumerable = from t in DefDatabase<TrainableDef>.AllDefsListForReading
                                                       where t.prerequisites != null && t.prerequisites.Contains(td)
                                                       select t;
                foreach (TrainableDef current in enumerable)
                {
                    SetWantedRecursive(current, pawn, false);
                }
            }
        }
    }
}
