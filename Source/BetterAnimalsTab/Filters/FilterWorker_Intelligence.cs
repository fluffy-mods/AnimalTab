// FilterWorker_Intelligence.cs
// Copyright Karel Kroeze, 2017-2017

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace AnimalTab
{
    public class FilterWorker_Intelligence: FilterWorker
    {
        private DefMap<TrainableIntelligenceDef, bool> _allowed = new DefMap<TrainableIntelligenceDef, bool>();
        private IEnumerable<TrainableIntelligenceDef> Intelligences => DefDatabase<TrainableIntelligenceDef>.AllDefsListForReading;
        
        public override FilterState State
        {
            get => _allowed.Values().Any( v => v ) ? FilterState.Inclusive : FilterState.Inactive;
            set => Logger.Error( "FilterWorker_Intelligence.set_State should never be called." );
        }

        public override bool Allows( Pawn pawn )
        {
            if ( State == FilterState.Inclusive )
                return Allows( pawn.RaceProps.TrainableIntelligence );

            return true;
        }

        public bool Allows( TrainableIntelligenceDef intelligence )
        {
            return _allowed[intelligence];
        }

        public override void Clicked()
        {
            var options = new List<FloatMenuOption>();
            options.Add( new FloatMenuOption( "AnimalTab.All".Translate(), Disable ) );
            foreach ( var trainableIntelligence in Intelligences.OrderBy( i => i.intelligenceOrder ) )
                options.Add( GetOption( trainableIntelligence ) );

            Find.WindowStack.Add(new FloatMenu(options));
        }

        private void Disable()
        {
            foreach ( var intelligence in Intelligences )
                _allowed[intelligence] = false;
            MainTabWindow_Animals.Instance.Notify_PawnsChanged();
        }

        private void Toggle( TrainableIntelligenceDef intelligence )
        {
            _allowed[intelligence] = !_allowed[intelligence];
            MainTabWindow_Animals.Instance.Notify_PawnsChanged();
        }

        private FloatMenuOption_Persistent GetOption( TrainableIntelligenceDef intelligence )
        {
            var label = "TrainableIntelligence".Translate() + ": " + intelligence.LabelCap;
            return new FloatMenuOption_Persistent( label, () => Toggle( intelligence ), extraPartWidth: 30f,
                extraPartOnGUI: rect => DrawOptionExtra( rect, intelligence ) );
        }

        private bool DrawOptionExtra(Rect rect, TrainableIntelligenceDef intelligence )
        {
            if ( State == FilterState.Inclusive && Allows( intelligence ) )
            {
                Rect checkRect = new Rect( rect.xMax - rect.height + Constants.Margin, rect.yMin, rect.height,
                    rect.height ).ContractedBy( 7f );
                GUI.DrawTexture( checkRect, Widgets.CheckboxOnTex );
            }
            return Widgets.ButtonInvisible( rect );
        }

        public override string GetTooltip()
        {
            if (State == FilterState.Inactive)
                return "AnimalTab.FilterInactiveTip".Translate();

            var allowed = ( from intelligence in Intelligences
                where _allowed[intelligence]
                select intelligence.label ).ToList();
            return "AnimalTab.IntelligenceFilterTip".Translate( allowed.ToStringList( "AnimalTab.Or".Translate() ) );
        }
    }
}