using RimWorld;
using Verse;
using Verse.AI;

namespace MedievalBiotech
{
    public class WorkGiver_OperateOnPawn : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(ThingDefOf.GeneExtractor);
        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Building_GeneExtractor geneExtractor = t as Building_GeneExtractor;
            if (geneExtractor == null)
            {
                return false;
            }
            if (JobDriver_OperateOnPawn.CanWork(pawn, geneExtractor.ContainedPawn, geneExtractor) is false)
            {
                return false;
            }
            if (!pawn.CanReserve(t, 1, -1, null, forced) || (t.def.hasInteractionCell 
                && !pawn.CanReserveSittableOrSpot(t.InteractionCell, forced)))
            {
                return false;
            }
            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Building_GeneExtractor geneExtractor = t as Building_GeneExtractor;
            return JobMaker.MakeJob(MB_DefOf.MB_OperateOnPawn, t, geneExtractor.ContainedPawn);
        }
    }
}
