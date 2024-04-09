using HarmonyLib;
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
                        IngestionOutcomeDoer_OffsetSoul ingestionOutcomeDoer_OffsetSoul = obj as IngestionOutcomeDoer_OffsetSoul;
                        if (ingestionOutcomeDoer_OffsetSoul == null)
                        {
                            JobGiver_GetSoulGem.cachedSoulGenEnergyGain = new float?(0f);
                        }
                        else
                        {
                            JobGiver_GetSoulGem.cachedSoulGenEnergyGain = new float?(ingestionOutcomeDoer_OffsetSoul.offset);
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
            Gene_Soul gene_Soul = (genes != null) ? genes.GetFirstGeneOfType<Gene_Soul>() : null;
            if (gene_Soul == null)
            {
                return null;
            }
            if (!gene_Soul.ShouldConsumeSoulNow())
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
            if (gene_Soul.soulArcanaGemsAllowed || gene_Soul.soulBasicGemsAllowed)
            {
                int num = Mathf.FloorToInt((gene_Soul.Max - gene_Soul.Value) / JobGiver_GetSoulGem.SoulGemEnergyGain);
                if (num > 0)
                {
                    Thing soulGem = this.GetSoulGem(pawn, gene_Soul);
                    if (soulGem != null)
                    {
                        Job job = JobMaker.MakeJob(JobDefOf.Ingest, soulGem);
                        job.count = Mathf.Min(soulGem.stackCount, num);
                        job.ingestTotalCount = true;
                        return job;
                    }
                }
            }
            return null;
        }

        private Thing GetSoulGem(Pawn pawn, Gene_Soul gene_Soul)
        {
            bool flag1 = gene_Soul.soulArcanaGemsAllowed;
            bool flag2 = gene_Soul.soulBasicGemsAllowed;

            //if (gene_Soul.soulArcanaGemsAllowed)
            //{

            //    flag1 = true;
            //}
            //if (gene_Soul.soulBasicGemsAllowed)
            //{

            //    flag2 = true;
            //}
            Thing carriedThing = pawn.carryTracker.CarriedThing;
            if (carriedThing != null)
            {
                if (flag1 && carriedThing.def == MB_DefOf.MB_ArcanaStone)
                {
                    return carriedThing;
                }
                if (flag2 && carriedThing.def == MB_DefOf.MB_CorruptedSoulGemBasic)
                {
                    return carriedThing;
                }
            }
            for (int i = 0; i < pawn.inventory.innerContainer.Count; i++)
            {
                if (flag1 && pawn.inventory.innerContainer[i].def == MB_DefOf.MB_ArcanaStone)
                {
                    return pawn.inventory.innerContainer[i];
                }
                if (flag2 && pawn.inventory.innerContainer[i].def == MB_DefOf.MB_CorruptedSoulGemBasic)
                {
                    return pawn.inventory.innerContainer[i];
                }
            }
            if (flag1 && !flag2)
            {
                return GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, pawn.Map.listerThings.ThingsOfDef(MB_DefOf.MB_CorruptedSoulGemBasic), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false, false, false), 9999f, (Thing t) => pawn.CanReserve(t, 1, -1, null, false) && !t.IsForbidden(pawn), null);
            }
            if (flag2 && !flag1)
            {
                return GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, pawn.Map.listerThings.ThingsOfDef(MB_DefOf.MB_CorruptedSoulGemBasic), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false, false, false), 9999f, (Thing t) => pawn.CanReserve(t, 1, -1, null, false) && !t.IsForbidden(pawn), null);
            }
            List<Thing> addThings = new List<Thing>();
            addThings.AddRange(pawn.Map.listerThings.ThingsOfDef(MB_DefOf.MB_ArcanaStone));
            addThings.AddRange(pawn.Map.listerThings.ThingsOfDef(MB_DefOf.MB_CorruptedSoulGemBasic));
            IEnumerable<Thing> things = addThings;
            return GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, things, PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false, false, false), 9999f, (Thing t) => pawn.CanReserve(t, 1, -1, null, false) && !t.IsForbidden(pawn), null);
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