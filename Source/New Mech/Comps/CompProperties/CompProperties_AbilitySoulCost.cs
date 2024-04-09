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
    public class CompProperties_AbilitySoulCost : CompProperties_AbilityEffect
    {
        public CompProperties_AbilitySoulCost()
        {
            this.compClass = typeof(CompAbilityEffect_SoulCost);
        }

        public override IEnumerable<string> ExtraStatSummary()
        {
            yield return "MB_AbilitySoulCost".Translate() + ": " + Mathf.RoundToInt(this.soulCost * 100f);
            yield break;
        }

        public float soulCost;
    }
}