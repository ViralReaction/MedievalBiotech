using System.Collections.Generic;
using Verse;

namespace MedievalBiotech
{
    public class CompProperties_ConsumableGenepack : CompProperties
    {
        public List<GeneDef> genes = new List<GeneDef>();
        public CompProperties_ConsumableGenepack()
        {
            compClass = typeof(ConsumableGenepack);
        }
    }
}
