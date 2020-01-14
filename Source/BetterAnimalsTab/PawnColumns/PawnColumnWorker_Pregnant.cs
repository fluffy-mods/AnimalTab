using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace AnimalTab
{
    class PawnColumnWorker_Pregnant : RimWorld.PawnColumnWorker_Pregnant
    {

        public override int Compare(Pawn a, Pawn b)
        {
            var aP = PregnantProgress(a);
            var aB = PregnantProgress(b);

            if (aP != null && aB != null)
                return aP.Value.CompareTo(aB.Value);
            if (aP != null && aB == null)
                return -1;
            if (aP == null && aB != null)
                return 1;

            var eggA = EggProgress(a);
            var eggB = EggProgress(b);

            return eggA.CompareTo(eggB);
        }

        private static bool? PregnantProgress(Pawn p)
        {
            var _hediff = p.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Pregnant);
            if (_hediff == null)
                return null;
            return _hediff.Visible;
        }

        private static float EggProgress(Pawn p)
        {
            var egg = p.AllComps.OfType<CompEggLayer>().FirstOrDefault();
            if (egg != null)
            {
                var fullString = egg.CompInspectStringExtra();

                string textFor = fullString?
                    .Replace("EggProgress".Translate() + ": ", "")?
                    .Replace("%", "")?
                    .Replace("Fertilized".Translate(), "")?
                    .Replace("ProgressStoppedUntilFertilized".Translate(), "")?
                    .Replace("\n", "");

                if (float.TryParse(textFor, out float progress))
                    return progress;
            }
            return 0;
        }

        public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
        {
            var egg = pawn.AllComps.OfType<CompEggLayer>().FirstOrDefault();
            if (egg != null)
            {
                Rect rect2 = new Rect(rect.x, rect.y, rect.width, Mathf.Min(rect.height, 30f));

                var fullString = egg.CompInspectStringExtra();

                string textFor = fullString?
                    .Replace("EggProgress".Translate() + ": ", "")?
                    .Replace("%", "")?
                    .Replace("Fertilized".Translate(), "Fertilized".Translate().Substring(0, 1))?
                    .Replace("ProgressStoppedUntilFertilized".Translate(), "")?
                    .Replace("\n", "");

                if (textFor != null)
                {
                    Text.Font = GameFont.Small;
                    Text.Anchor = TextAnchor.MiddleLeft;
                    Text.WordWrap = false;
                    Widgets.Label(rect2, textFor);
                    Text.WordWrap = true;
                    Text.Anchor = TextAnchor.UpperLeft;
                    if (!fullString.NullOrEmpty())
                    {
                        TooltipHandler.TipRegion(rect2, fullString);
                    }
                }
            }
            else
            {
                base.DoCell(rect, pawn, table);
            }
        }
    }
}
