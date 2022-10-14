using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace AnimalTab {
    internal class PawnColumnWorker_Pregnant: RimWorld.PawnColumnWorker_Pregnant {
        private const int ICON_SIZE_I = 16;

        public override int Compare(Pawn a, Pawn b) {
            bool aPregnant = IsPregnant(a);
            bool bPregnant = IsPregnant(b);
            if (aPregnant || bPregnant) {
                return GetGestationProgress(a).CompareTo(GetGestationProgress(b));
            }

            float eggA = EggProgress( a );
            float eggB = EggProgress( b );
            return eggA.CompareTo(eggB);
        }

        private static Hediff_Pregnant GetPregnantHediff(Pawn pawn) => (Hediff_Pregnant)pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Pregnant, true);

        private static double GetGestationProgress(Pawn pawn)
		{
            double gestationProgress = GetPregnantHediff(pawn)?.GestationProgress ?? 0;
            double gestationPeriod = pawn.RaceProps.gestationPeriodDays * 60000.0;

            return gestationProgress / gestationPeriod;
		}

        private static bool IsPregnant(Pawn p) {
            return p.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Pregnant)?.Visible ?? false;
        }

        private static bool IsSterilized(Pawn p) {
            return p.health.hediffSet.HasHediff(HediffDefOf.Sterilized, true);

        }

        private static float EggProgress(Pawn p) {
            CompEggLayer egg = p.AllComps.OfType<CompEggLayer>().FirstOrDefault();
            if (egg != null) {
                return Traverse.Create(egg).Field("eggProgress").GetValue<float>();
            }
			if (IsSterilized(p))
			{
                return 0;
			}

            return -1;
        }

        protected override Texture2D GetIconFor(Pawn pawn) {
            if (EggProgress(pawn) > 0) {
                return Resources.Egg;
            }

			if (IsSterilized(pawn) && !IsPregnant(pawn))
			{
                return Resources.Red_X;
			}

            return base.GetIconFor(pawn);
        }

        protected override string GetIconTip(Pawn pawn) {
            CompEggLayer egg = pawn.AllComps.OfType<CompEggLayer>().FirstOrDefault();
            if (egg != null && EggProgress(pawn) > 0) {
                return egg.CompInspectStringExtra();
            }
            if(IsSterilized(pawn))
			{
                return HediffDefOf.Sterilized.LabelCap;

            }


            return base.GetIconTip(pawn);
        }

        public override void DoCell(Rect rect, Pawn pawn, PawnTable table) {
            CompEggLayer egg = pawn.AllComps.OfType<CompEggLayer>().FirstOrDefault();
            float progress = EggProgress( pawn );
            if (egg != null && progress > 0) {
                Rect iconRect = new Rect( Vector2.zero, GetIconSize( pawn ) )
                              .CenteredOnXIn( rect )
                              .CenteredOnYIn( rect );

                Rect progressRect = iconRect;
                progressRect.yMin += iconRect.height * (1 - progress);

                GUI.color = Color.grey;
                GUI.DrawTexture(iconRect, GetIconFor(pawn));

                GUI.color = egg.FullyFertilized ? Color.white : Color.Lerp(Color.grey, Color.white, 1 / 2f);
                GUI.DrawTextureWithTexCoords(progressRect, GetIconFor(pawn), new Rect(0, 0, 1, progress));
                GUI.color = Color.white;

                TooltipHandler.TipRegion(rect, () => GetIconTip(pawn), pawn.GetHashCode());

                if (Widgets.ButtonInvisible(iconRect)) {
                    ClickedIcon(pawn);
                }

                PaintedIcon(pawn);
            } else {
                base.DoCell(rect, pawn, table);
            }
        }

        protected override Vector2 GetIconSize(Pawn pawn) {
            return new Vector2(ICON_SIZE_I, ICON_SIZE_I);
        }
    }
}
