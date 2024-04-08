using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace MedievalBiotech
{
    public class IngestionOutcomeDoer_OffsetSoul : IngestionOutcomeDoer
    {
        public override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested, int ingestedCount)
        {
            Utility.OffsetSoul(pawn, this.offset * (float)ingestedCount, true);
        }

        public override IEnumerable<StatDrawEntry> SpecialDisplayStats(ThingDef parentDef)
        {
            if (ModsConfig.BiotechActive)
            {
                string arg = (this.offset >= 0f) ? "+" : string.Empty;
                yield return new StatDrawEntry(StatCategoryDefOf.BasicsNonPawnImportant, "MB_DarkArcana".Translate().CapitalizeFirst(), arg + Mathf.RoundToInt(this.offset * 100f), "MB_DarkArcana_Desc".Translate(), 1000, null, null, false, false);
            }
            yield break;
        }

        public float offset;
    }
}
