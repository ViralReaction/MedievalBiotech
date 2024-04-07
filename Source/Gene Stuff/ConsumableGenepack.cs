using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MedievalBiotech
{
    public class ConsumableGenepack : ThingComp {
        public CompProperties_ConsumableGenepack Props => (CompProperties_ConsumableGenepack)props;
        public override void PostIngested(Pawn ingester) {
            if (ingester.RaceProps.IsFlesh) {
                if (ModsConfig.BiotechActive && ingester.genes != null) {
                    Genepack pack = parent as Genepack;
                    if (pack != null) {
                        foreach (GeneDef gene in pack.GeneSet.GenesListForReading) {
                            if (!ingester.genes.HasGene(gene)) {
                                ingester.genes.AddGene(gene, true);
                            }
                        }
                    }
                    if (Props.genes.Any()) {
                        foreach (GeneDef gene in Props.genes) {
                            if (!ingester.genes.HasGene(gene)) {
                                ingester.genes.AddGene(gene, true);
                            }
                        }
                    }
                }
            }
        }
    }
    public class CompProperties_ConsumableGenepack : CompProperties {
        public List<GeneDef> genes = new List<GeneDef>();
        public CompProperties_ConsumableGenepack() {
            compClass = typeof(ConsumableGenepack);
        }
    }
}
