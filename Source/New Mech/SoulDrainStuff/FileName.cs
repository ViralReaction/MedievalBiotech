using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace MedievalBiotech
{
    public class StatWorker_SoulGainFactor : StatWorker
    {
        public override bool ShouldShowFor(StatRequest req) => req.Thing is Pawn thing && thing.genes.HasGene(MB_DefOf.MB_SoulStarved);
    }
}
