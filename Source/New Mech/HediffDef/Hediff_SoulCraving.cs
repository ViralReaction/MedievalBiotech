using RimWorld;
using Verse;
using System;

namespace MedievalBiotech
{
    public class HediffComp_SeverityFromSoul : HediffComp
    {
        public HediffCompProperties_SeverityFromSoul Props
        {
            get
            {
                return (HediffCompProperties_SeverityFromSoul)this.props;
            }
        }
        public override bool CompShouldRemove
        {
            get
            {
                Pawn_GeneTracker genes = base.Pawn.genes;
                return ((genes != null) ? genes.GetFirstGeneOfType<Gene_Soul>() : null) == null;
            }
        }

        private Gene_Soul Soul
        {
            get
            {
                if (this.cachedSoulGene == null)
                {
                    this.cachedSoulGene = base.Pawn.genes.GetFirstGeneOfType<Gene_Soul>();
                }
                return this.cachedSoulGene;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (this.Soul != null)
            {
                severityAdjustment += ((this.Soul.Value > 0f) ? this.Props.severityPerHourSoul : this.Props.severityPerHourEmpty) / 2500f;
            }
        }

        private Gene_Soul cachedSoulGene;
    }
}