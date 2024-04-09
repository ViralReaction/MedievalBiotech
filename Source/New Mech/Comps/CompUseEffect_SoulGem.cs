using System;
using Verse;
using RimWorld;

namespace MedievalBiotech
{
    public class CompUseEffect_SoulGem : CompUseEffect
    {

        public CompProperties_UseEffect Props
        {
            get
            {
                return (CompProperties_UseEffect)this.props;
            }
        }
        public override AcceptanceReport CanBeUsedBy(Pawn p)
        {
            if (!p.genes.HasGene(MB_DefOf.MB_SoulStarved))
            {
                return "MB_CantUseSoulGem".Translate();
            }
            return true;
        }

    }
}
