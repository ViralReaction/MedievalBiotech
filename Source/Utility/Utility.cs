using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;

namespace MedievalBiotech
{
    public class Utility
    {
        public static bool IsSanguinMage(Pawn pawn)
        {
            if (pawn.health.hediffSet.HasHediff(MB_DefOf.MB_BloodChaliceImplant))
            {
                return true;
            }
            return false;
        }

        public static bool IsNecromancer(Pawn pawn)
        {
            if (pawn.health.hediffSet.HasHediff(MB_DefOf.MB_NecronomiconImplant))
            {
                return true;
            }
            return false;
        }

        public static bool IsUndeadMech(Pawn pawn)
        {
            var extension = pawn.def.GetModExtension<Undead_Mech>();
            if (extension != null)
            {
                return true;
            }
            return false;
        }

        public static bool IsSanguinMech(Pawn pawn)
        {
            var extension = pawn.def.GetModExtension<Undead_Mech>();
            if (extension != null)
            {
                return true;
            }
            return false;
        }

        public static void OffsetSoul(Pawn pawn, float offset, bool applyStatFactor = true)
        {
            if (!ModsConfig.BiotechActive)
            {
                return;
            }
            if (offset > 0f && applyStatFactor)
            {
                offset *= pawn.GetStatValue(MB_DefOf.SoulGainFactor, true, -1);
            }
            Pawn_GeneTracker genes = pawn.genes;
            Gene_SoulDrain gene_SoulDrain = (genes != null) ? genes.GetFirstGeneOfType<Gene_SoulDrain>() : null;
            if (gene_SoulDrain != null)
            {
                GeneResourceDrainUtility.OffsetResource(gene_SoulDrain, offset);
                return;
            }
            Pawn_GeneTracker genes2 = pawn.genes;
            Gene_Soul gene_Soul = (genes2 != null) ? genes2.GetFirstGeneOfType<Gene_Soul>() : null;
            if (gene_Soul != null)
            {
                gene_Soul.Value += offset;
            }
        }

        public static void DoDrain(Pawn biter, Pawn victim, float targetSoulGain, float victimResistanceGain, ThoughtDef thoughtDefToGiveTarget = null, ThoughtDef opinionThoughtToGiveTarget = null)
        {
            if (!ModLister.CheckBiotech("Sanguophage bite"))
            {
                return;
            }
            float num2 = targetSoulGain * victim.BodySize;
            Utility.OffsetSoul(biter, num2, true);
            Utility.OffsetSoul(victim, -num2, true);
            Pawn_NeedsTracker needs = biter.needs;
            if (thoughtDefToGiveTarget != null)
            {
                Pawn_NeedsTracker needs2 = victim.needs;
                if (needs2 != null)
                {
                    Need_Mood mood = needs2.mood;
                    if (mood != null)
                    {
                        ThoughtHandler thoughts = mood.thoughts;
                        if (thoughts != null)
                        {
                            MemoryThoughtHandler memories = thoughts.memories;
                            if (memories != null)
                            {
                                memories.TryGainMemory((Thought_Memory)ThoughtMaker.MakeThought(thoughtDefToGiveTarget), biter);
                            }
                        }
                    }
                }
            }
            if (opinionThoughtToGiveTarget != null)
            {
                Pawn_NeedsTracker needs3 = victim.needs;
                if (needs3 != null)
                {
                    Need_Mood mood2 = needs3.mood;
                    if (mood2 != null)
                    {
                        ThoughtHandler thoughts2 = mood2.thoughts;
                        if (thoughts2 != null)
                        {
                            MemoryThoughtHandler memories2 = thoughts2.memories;
                            if (memories2 != null)
                            {
                                memories2.TryGainMemory((Thought_Memory)ThoughtMaker.MakeThought(opinionThoughtToGiveTarget), biter);
                            }
                        }
                    }
                }
            }
            //if (targetBloodLoss > 0f)
            //{
            //    victim.health.AddHediff(HediffDefOf.BloodfeederMark, ExecutionUtility.ExecuteCutPart(victim), null, null);
            //    Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.BloodLoss, victim, null);
            //    hediff.Severity = targetBloodLoss;
            //    victim.health.AddHediff(hediff, null, null, null);
            //}
            if (victim.IsPrisoner && victimResistanceGain > 0f)
            {
                victim.guest.resistance = Mathf.Min(victim.guest.resistance + victimResistanceGain, victim.kindDef.initialResistanceRange.Value.TrueMax);
            }
        }
    }
}
