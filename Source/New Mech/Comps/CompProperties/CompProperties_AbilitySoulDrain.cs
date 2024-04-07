using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MedievalBiotech
{
    public class CompProperties_AbilitySoulDrain : CompProperties_AbilityEffect
    {
        public CompProperties_AbilitySoulDrain()
        {
            this.compClass = typeof(CompAbilityEffect_SoulDrain);
        }

        public override IEnumerable<string> ExtraStatSummary()
        {
            yield return "AbilitySoulGain".Translate() + ": " + (this.soulGain * 100f).ToString("F0");
            yield break;
        }

        public float soulGain;
        public ThoughtDef thoughtDefToGiveTarget;
        public ThoughtDef opinionThoughtDefToGiveTarget;
        public float resistanceGain;
        public float targetBloodLoss = 0.4499f;

    }
}