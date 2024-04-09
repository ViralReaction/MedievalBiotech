using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace MedievalBiotech
{
    public class CompProperties_CreateArcanaStone : CompProperties_AbilityEffect
    {
        public CompProperties_CreateArcanaStone()
        {
            this.compClass = typeof(CompAbilityEffect_CreateArcanaStone);
        }

        public ThingDef stoneCreated;
    }
}
