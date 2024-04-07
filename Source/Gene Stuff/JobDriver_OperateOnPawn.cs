using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MedievalBiotech
{

    public class JobDriver_OperateOnPawn : JobDriver
    {
        public Building_GeneExtractor GeneExtractor => TargetA.Thing as Building_GeneExtractor;
        public Pawn PawnOperated => GeneExtractor.ContainedPawn;
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (!pawn.ReserveSittableOrSpot(GeneExtractor.InteractionCell, job, errorOnFailed))
            {
                return false;
            }
            return pawn.Reserve(TargetA, job);
        }

        public static float OperateSpeedMultiplier = 3;
        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOn(() => CanWork(pawn, PawnOperated, GeneExtractor) is false);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            var operateToil = new Toil();
            operateToil.defaultCompleteMode = ToilCompleteMode.Never;
            operateToil.initAction = delegate
            {
                if (PawnOperated.Dead is false)
                {
                    HealthUtility.TryAnesthetize(PawnOperated);
                }
            };
            operateToil.tickAction = delegate
            {
                var speed = pawn.GetStatValue(StatDefOf.MedicalTendSpeed) * OperateSpeedMultiplier;
                GeneExtractor.ticksRemaining -= Mathf.Max(1, (int)speed);
                if (GeneExtractor.ticksRemaining <= 0)
                {
                    GeneExtractor.Finish();
                }
            };
            operateToil.WithEffect(MB_DefOf.ButcherFlesh, TargetIndex.A);
            operateToil.PlaySustainerOrSound(() => MB_DefOf.Recipe_ButcherCorpseFlesh);
            yield return operateToil;
        }

        public static bool CanWork(Pawn worker, Pawn pawnOperated, Building_GeneExtractor geneExtractor)
        {
            if (PawnUtility.WillSoonHaveBasicNeed(worker, -0.05f))
            {
                return false;
            }
            if (pawnOperated is null)
            {
                return false;
            }
            if (pawnOperated.Corpse != null && pawnOperated.Corpse.GetRotStage() != RotStage.Fresh)
            {
                return false;
            }
            if (geneExtractor.ticksRemaining <= 0)
            {
                return false;
            }
            return true;
        }
    }
}
