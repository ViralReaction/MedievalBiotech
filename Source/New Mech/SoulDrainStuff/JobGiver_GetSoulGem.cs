using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MedievalBiotech
{
    public class JobGiver_GetSoulGem : ThinkNode_JobGiver
    {
        public static float SoulGemEnergyGain
        {
            get
            {
                if (JobGiver_GetSoulGem.cachedSoulGenEnergyGain == null)
                {
                    if (!ModsConfig.BiotechActive)
                    {
                        JobGiver_GetSoulGem.cachedSoulGenEnergyGain = new float?(0f);
                    }
                    else
                    {
                        IngestibleProperties ingestible = MB_DefOf.MB_CorruptedSoulGemBasic.ingestible;
                        object obj;
                        if (ingestible == null)
                        {
                            obj = null;
                        }
                        else
                        {
                            List<IngestionOutcomeDoer> outcomeDoers = ingestible.outcomeDoers;
                            if (outcomeDoers == null)
                            {
                                obj = null;
                            }
                            else
                            {
                                obj = outcomeDoers.FirstOrDefault((IngestionOutcomeDoer x) => x is IngestionOutcomeDoer_OffsetSoul);
                            }
                        }
                        IngestionOutcomeDoer_OffsetSoul ingestionOutcomeDoer_OffsetHemogen = obj as IngestionOutcomeDoer_OffsetSoul;
                        if (ingestionOutcomeDoer_OffsetHemogen == null)
                        {
                            JobGiver_GetSoulGem.cachedSoulGenEnergyGain = new float?(0f);
                        }
                        else
                        {
                            JobGiver_GetSoulGem.cachedSoulGenEnergyGain = new float?(ingestionOutcomeDoer_OffsetHemogen.offset);
                        }
                    }
                }
                return JobGiver_GetSoulGem.cachedSoulGenEnergyGain.Value;
            }
        }

        public static void ResetStaticData()
        {
            JobGiver_GetSoulGem.cachedSoulGenEnergyGain = null;
        }
        public override float GetPriority(Pawn pawn)
        {
            if (!ModsConfig.BiotechActive)
            {
                return 0f;
            }
            Pawn_GeneTracker genes = pawn.genes;
            if (((genes != null) ? genes.GetFirstGeneOfType<Gene_Soul>() : null) == null)
            {
                return 0f;
            }
            return 9.1f;
        }
        public override Job TryGiveJob(Pawn pawn)
        {
            if (!ModsConfig.BiotechActive)
            {
                return null;
            }
            Pawn_GeneTracker genes = pawn.genes;
            Gene_Soul gene_Hemogen = (genes != null) ? genes.GetFirstGeneOfType<Gene_Soul>() : null;
            if (gene_Hemogen == null)
            {
                return null;
            }
            if (!gene_Hemogen.ShouldConsumeSoulNow())
            {
                return null;
            }
            if (pawn.IsBloodfeeder())
            {
                Pawn prisoner = this.GetPrisoner(pawn);
                if (prisoner != null)
                {
                    return JobMaker.MakeJob(JobDefOf.PrisonerBloodfeed, prisoner);
                }
            }
            if (gene_Hemogen.soulGemsAllowed)
            {
                int num = Mathf.FloorToInt((gene_Hemogen.Max - gene_Hemogen.Value) / JobGiver_GetSoulGem.SoulGemEnergyGain);
                if (num > 0)
                {
                    Thing hemogenPack = this.GetSoulGem(pawn);
                    if (hemogenPack != null)
                    {
                        Job job = JobMaker.MakeJob(JobDefOf.Ingest, hemogenPack);
                        job.count = Mathf.Min(hemogenPack.stackCount, num);
                        job.ingestTotalCount = true;
                        return job;
                    }
                }
            }
            return null;
        }

        private Thing GetSoulGem(Pawn pawn)
        {
            Thing carriedThing = pawn.carryTracker.CarriedThing;
            if (carriedThing != null && carriedThing.def == MB_DefOf.MB_CorruptedSoulGemBasic)
            {
                return carriedThing;
            }
            for (int i = 0; i < pawn.inventory.innerContainer.Count; i++)
            {
                if (pawn.inventory.innerContainer[i].def == MB_DefOf.MB_CorruptedSoulGemBasic)
                {
                    return pawn.inventory.innerContainer[i];
                }
            }
            return GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, pawn.Map.listerThings.ThingsOfDef(MB_DefOf.MB_CorruptedSoulGemBasic), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false, false, false), 9999f, (Thing t) => pawn.CanReserve(t, 1, -1, null, false) && !t.IsForbidden(pawn), null);
        }

        public static AcceptanceReport CanFeedOnPrisoner(Pawn bloodfeeder, Pawn prisoner)
        {
            if (prisoner.WouldDieFromAdditionalBloodLoss(0.4499f))
            {
                return "CannotFeedOnWouldKill".Translate(prisoner.Named("PAWN"));
            }
            if (!prisoner.IsPrisonerOfColony || !prisoner.guest.PrisonerIsSecure || prisoner.guest.IsInteractionDisabled(PrisonerInteractionModeDefOf.Bloodfeed) || prisoner.IsForbidden(bloodfeeder) || !bloodfeeder.CanReserveAndReach(prisoner, PathEndMode.OnCell, bloodfeeder.NormalMaxDanger(), 1, -1, null, false) || prisoner.InAggroMentalState)
            {
                return false;
            }
            return true;
        }

        private Pawn GetPrisoner(Pawn pawn)
        {
            return (Pawn)GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, pawn.Map.mapPawns.PrisonersOfColonySpawned, PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false, false, false), 9999f, delegate (Thing t)
            {
                Pawn prisoner;
                return (prisoner = (t as Pawn)) != null && JobGiver_GetSoulGem.CanFeedOnPrisoner(pawn, prisoner).Accepted;
            }, null);
        }

        private static float? cachedSoulGenEnergyGain;
    }
}