using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;

namespace MedievalBiotech
{
    public class CompAbilityEffect_SoulCost : CompAbilityEffect
    {
        public new CompProperties_AbilitySoulCost Props
        {
            get
            {
                return (CompProperties_AbilitySoulCost)this.props;
            }
        }
        private bool HasEnoughSoul
        {
            get
            {
                Pawn_GeneTracker genes = this.parent.pawn.genes;
                Gene_Soul gene_Soul = (genes != null) ? genes.GetFirstGeneOfType<Gene_Soul>() : null;
                return gene_Soul != null && gene_Soul.Value >= this.Props.soulCost;
            }
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Utility.OffsetSoul(this.parent.pawn, -this.Props.soulCost, true);
        }

        public override bool GizmoDisabled(out string reason)
        {
            Pawn_GeneTracker genes = this.parent.pawn.genes;
            Gene_Soul gene_Soul = (genes != null) ? genes.GetFirstGeneOfType<Gene_Soul>() : null;
            if (gene_Soul == null)
            {
                reason = "AbilityDisabledNoHemogenGene".Translate(this.parent.pawn);
                return true;
            }
            if (gene_Soul.Value < this.Props.soulCost)
            {
                reason = "AbilityDisabledNoHemogen".Translate(this.parent.pawn);
                return true;
            }
            float num = this.TotalSoulCostOfQueuedAbilities();
            float num2 = this.Props.soulCost + num;
            if (this.Props.soulCost > 1E-45f && num2 > gene_Soul.Value)
            {
                reason = "AbilityDisabledNoHemogen".Translate(this.parent.pawn);
                return true;
            }
            reason = null;
            return false;
        }

        public override bool AICanTargetNow(LocalTargetInfo target)
        {
            return this.HasEnoughSoul;
        }

        private float TotalSoulCostOfQueuedAbilities()
        {
            Pawn_JobTracker jobs = this.parent.pawn.jobs;
            object obj;
            if (jobs == null)
            {
                obj = null;
            }
            else
            {
                Job curJob = jobs.curJob;
                obj = ((curJob != null) ? curJob.verbToUse : null);
            }
            Verb_CastAbilitySoul verb_CastAbility = obj as Verb_CastAbilitySoul;
            float num;
            if (verb_CastAbility == null)
            {
                num = 0f;
            }
            else
            {
                AbilitySoul ability = verb_CastAbility.ability;
                num = ((ability != null) ? ability.SoulCost() : 0f);
            }
            float num2 = num;
            if (this.parent.pawn.jobs != null)
            {
                for (int i = 0; i < this.parent.pawn.jobs.jobQueue.Count; i++)
                {
                    Verb_CastAbilitySoul verb_CastAbility2;
                    if ((verb_CastAbility2 = (this.parent.pawn.jobs.jobQueue[i].job.verbToUse as Verb_CastAbilitySoul)) != null)
                    {
                        float num3 = num2;
                        AbilitySoul ability2 = verb_CastAbility2.ability;
                        num2 = num3 + ((ability2 != null) ? ability2.SoulCost() : 0f);
                    }
                }
            }
            return num2;
        }
    }
}
