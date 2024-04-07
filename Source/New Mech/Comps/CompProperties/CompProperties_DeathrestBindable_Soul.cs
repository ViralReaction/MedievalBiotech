using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace MedievalBiotech
{
    public class CompProperties_DeathrestBindable_Soul : CompProperties_DeathrestBindable
    {
        public CompProperties_DeathrestBindable_Soul()
        {
            this.compClass = typeof(CompDeathrestBindable_Soul);
        }

        public float soulLimitOffset;

    }
}
