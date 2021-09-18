// MainTabWindow_Animals.cs
// Copyright Karel Kroeze, 2017-2017

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using static AnimalTab.Constants;

namespace AnimalTab {
    public class MainTabWindow_Animals: MainTabWindow_PawnTable {
        public MainTabWindow_Animals() {
            Instance = this;
        }

        public static MainTabWindow_Animals Instance { get; internal set; }
        protected override PawnTableDef PawnTableDef => PawnTableDefOf.Animals;
        protected override float ExtraTopSpace => Constants.ExtraTopSpace;

        protected override float ExtraBottomSpace {
            get {
                if (Filter) {
                    return Constants.ExtraBottomSpace + ExtraFilterSpace;
                }

                return Constants.ExtraBottomSpace;
            }
        }

        public override void DoWindowContents(Rect rect) {
            rect = DoExtraDrawers(rect);
            DoFilterBar(rect);
            DoSlaughterButton(rect);
            base.DoWindowContents(rect);
        }

        private List<DefModExtension_DrawerExtra> _extraDrawers;
        public List<DefModExtension_DrawerExtra> ExtraDrawers => _extraDrawers ??= MainButtonDefOf.Animals.modExtensions?.OfType<DefModExtension_DrawerExtra>()
                                                        .ToList()
                                      ?? new List<DefModExtension_DrawerExtra>();

        private Rect DoExtraDrawers(Rect rect) {
            foreach (DefModExtension_DrawerExtra extraDrawer in ExtraDrawers) {
                rect.yMin += extraDrawer.Worker.Draw(rect);
            }

            return rect;
        }

        private static IEnumerable<FilterWorker> _filters;

        public static IEnumerable<FilterWorker> Filters {
            get {
                if (_filters == null) {
                    _filters = DefDatabase<FilterDef>.AllDefsListForReading.Select(f => f.Worker);
                }

                return _filters;
            }
        }

        protected override IEnumerable<Pawn> Pawns => Filter ? FilteredPawns : AllPawns;

        public IEnumerable<Pawn> FilteredPawns {
            get {
                if (_filteredPawns == null) {
                    RecachePawns();
                }

                return _filteredPawns;
            }
        }
        private IEnumerable<Pawn> _filteredPawns;

        public IEnumerable<Pawn> AllPawns {
            get {
                if (_allPawns == null) {
                    RecachePawns();
                }

                return _allPawns;
            }
        }
        private IEnumerable<Pawn> _allPawns;

        // Note that we're overriding this _only_ because it's called from Notify_PawnsChanged, and unlike that method,
        // this one is virtual.
        protected override void SetInitialSizeAndPosition() {
            // don't give a damn about this part.
            base.SetInitialSizeAndPosition();

            // clear our pawn list caches
            _allPawns = null;
            _filteredPawns = null;
        }

        public void RecachePawns() {
            _allPawns = Find.CurrentMap.mapPawns.PawnsInFaction(Faction.OfPlayer).Where(p => p.RaceProps.Animal);
            _filteredPawns = _allPawns.Where(p => Filters.All(f => f.Allows(p)));
        }

        private void DoFilterBar(Rect rect) {
            float barWidth = (Filters.Count() * (FilterButtonSize + Margin)) + Margin;
            Rect buttonRect = new Rect(rect.xMax - Margin - ButtonSize, rect.yMax - Margin - ButtonSize, ButtonSize, ButtonSize);
            Rect barRect = new Rect(buttonRect.xMin - Margin - barWidth, rect.yMax - Margin - ButtonSize, barWidth, ButtonSize);
            Rect countRect = new Rect(rect.xMin + Margin, barRect.yMin - Margin - (ButtonSize * 2 / 3f), rect.width - ButtonSize - (Margin * 3), ButtonSize * 2 / 3f);

            if (Widgets.ButtonImage(buttonRect, Resources.Filter, Filter ? GenUI.MouseoverColor : Color.white,
                Filter ? Color.white : GenUI.MouseoverColor)) {
                Filter = !Filter;
            }

            if (Filter) {
                DrawFilters(barRect, Filters);
                DrawCounts(countRect);
            }
        }

        private void DoSlaughterButton(Rect rect) {
            Rect buttonRect = new Rect(rect.xMin + Margin, rect.yMax - Margin - ButtonSize, ButtonSize, ButtonSize);

            TooltipHandler.TipRegion(buttonRect, "ManageAutoSlaughter".Translate());
            if (Widgets.ButtonImage(buttonRect, Resources.Slaughter)) {
                Find.WindowStack.Add(new Dialog_AutoSlaughter(Find.CurrentMap));
            }

            if (!ModLister.IdeologyInstalled) {
                return;
            }

            if (Faction.OfPlayer.ideos?.AllIdeos.Any(i => i.memes.Contains(MemeDefOf.Rancher)) ?? false) {
                Rect labelRect = new Rect(buttonRect.xMax + Margin, buttonRect.yMin, rect.width - ButtonSize - (Margin * 2), ButtonSize);
                string timeAgo = (Find.TickManager.TicksGame - Faction.OfPlayer.ideos.LastAnimalSlaughterTick).ToStringTicksToPeriod();
                TaggedString label = "LastAnimalSlaughter".Translate() + ": " + "TimeAgo".Translate(timeAgo);
                Text.Anchor = TextAnchor.LowerLeft;
                Widgets.Label(labelRect, label);
                Text.Anchor = TextAnchor.UpperLeft;
            }
        }

        private void DrawCounts(Rect rect) {
            Text.Anchor = TextAnchor.UpperRight;
            Text.Font = GameFont.Tiny;
            GUI.color = Color.grey;
            Widgets.Label(rect, "AnimalTab.XofYShown".Translate(Pawns.Count(), AllPawns.Count()));
            GUI.color = Color.white;
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void DrawFilters(Rect rect, IEnumerable<FilterWorker> filters) {
            Widgets.DrawBoxSolid(rect, new Color(0f, 0f, 0f, .2f));
            Rect filterRect = new Rect(Margin, (ButtonSize - FilterButtonSize) / 2f, FilterButtonSize, FilterButtonSize);
            try {
                GUI.BeginGroup(rect);
                foreach (FilterWorker filter in filters) {
                    filter.Draw(filterRect);
                    filterRect.x += FilterButtonSize + Margin;
                }
            } finally {
                GUI.EndGroup();
            }
        }

        private bool _filter;

        public bool Filter {
            get => _filter;
            set {
                if (_filter == value) {
                    return;
                }

                _filter = value;
                Notify_PawnsChanged();
                Notify_ResolutionChanged();
            }
        }
    }
}
