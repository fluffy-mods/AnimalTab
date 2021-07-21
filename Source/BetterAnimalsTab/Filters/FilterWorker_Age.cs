using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace AnimalTab
{
    public class FilterWorker_Age : FilterWorker
    {
        DefMap<LifeStageDef, bool> allowed = new DefMap<LifeStageDef, bool>();
        private IEnumerable<LifeStageDef> LifeStages => MainTabWindow_Animals.Instance.AllPawns.Select(p => p.ageTracker.CurLifeStage).Distinct();

        public override FilterState State
        {
            get { return allowed.Values().Any(v => v) ? FilterState.Inclusive : FilterState.Inactive; }
            set => throw new InvalidOperationException("FilterWorker_Age.set_State() should never be called");
        }

        public override bool Allows(Pawn pawn)
        {
            if (State == FilterState.Inactive)
                return true;
            return allowed[pawn.ageTracker.CurLifeStage];
        }

        public override void Clicked()
        {
            var options = new List<FloatMenuOption>();
            options.Add(new FloatMenuOption("AnimalTab.All".Translate(), Deactivate));
            foreach (var lifeStage in LifeStages)
                options.Add(new FloatMenuOption_Persistent(lifeStage.LabelCap, () => Toggle(lifeStage), extraPartWidth: 30f, extraPartOnGUI: rect => DrawOptionExtra(rect, lifeStage)));

            Find.WindowStack.Add(new FloatMenu(options));
        }

        public void Toggle(LifeStageDef lifeStage)
        {
            allowed[lifeStage] = !allowed[lifeStage];
            MainTabWindow_Animals.Instance.Notify_PawnsChanged();
        }

        public void Deactivate()
        {
            allowed.SetAll(false);
            MainTabWindow_Animals.Instance.Notify_PawnsChanged();
        }

        private bool DrawOptionExtra(Rect rect, LifeStageDef lifeStage)
        {
            if (State == FilterState.Inclusive && allowed[lifeStage])
            {
                Rect checkRect = new Rect(rect.xMax - rect.height + Constants.Margin, rect.yMin, rect.height, rect.height).ContractedBy(7f);
                GUI.DrawTexture(checkRect, Widgets.CheckboxOnTex);
            }

            return false;
        }

        public override string GetTooltip()
        {
            if (State == FilterState.Inactive)
                return "AnimalTab.FilterInactiveTip".Translate();

            var _allowed = new List<string>();
            foreach (var lifeStage in LifeStages)
                if (allowed[lifeStage])
                    _allowed.Add(lifeStage.label);
            return "AnimalTab.PawnKindFilterTip".Translate(_allowed.ToStringList("AnimalTab.Or".Translate()));
        }
    }
}