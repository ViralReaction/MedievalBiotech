using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace MedievalBiotech
{
    public class HediffCompProperties_SeverityFromSoul : HediffCompProperties
    {
        public HediffCompProperties_SeverityFromSoul()
        {
            this.compClass = typeof(HediffComp_SeverityFromSoul);
        }

        public float severityPerHourEmpty;

        public float severityPerHourSoul;
    }
}