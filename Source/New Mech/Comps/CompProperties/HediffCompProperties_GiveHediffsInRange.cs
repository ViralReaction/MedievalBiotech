using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace MedievalBiotech
{
    public class HediffCompProperties_GiveHediffsInRangeUndead : HediffCompProperties
    {
        public HediffCompProperties_GiveHediffsInRangeUndead()
        {
            this.compClass = typeof(HediffComp_GiveHediffsInRangeUndead);
        }
        public GeneDef geneDef;
        public float range;

        public TargetingParameters targetingParameters;

        public HediffDef hediff;

        public ThingDef mote;

        public bool hideMoteWhenNotDrafted;

        public float initialSeverity = 1f;

        public bool onlyPawnsInSameFaction = true;
    }
}
