using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace MedievalBiotech
{
    public class Gene_SoulDrain : Gene, IGeneResourceDrain
    {

        public Gene_Resource Resource
        {
            get
            {
                if (this.cachedSoulGene == null || !this.cachedSoulGene.Active)
                {
                    this.cachedSoulGene = this.pawn.genes.GetFirstGeneOfType<Gene_Soul>();
                }
                return this.cachedSoulGene;
            }
        }

        public bool CanOffset
        {
            get
            {
                return this.Active && !this.pawn.Deathresting;
            }
        }

        public float ResourceLossPerDay
        {
            get
            {
                return this.def.resourceLossPerDay;
            }
        }

        public Pawn Pawn
        {
            get
            {
                return this.pawn;
            }
        }

        public string DisplayLabel
        {
            get
            {
                return this.Label + " (" + "Gene".Translate() + ")";
            }
        }

        public override void Tick()
        {
            base.Tick();
            GeneResourceDrainUtility.TickResourceDrain(this);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (!this.Active)
            {
                yield break;
            }
            foreach (Gizmo gizmo in GeneResourceDrainUtility.GetResourceDrainGizmos(this))
            {
                yield return gizmo;
            }
            IEnumerator<Gizmo> enumerator = null;
            yield break;
        }

        [Unsaved(false)]
        private Gene_Soul cachedSoulGene;

        private const float MinAgeForDrain = 3f;
    }
}
