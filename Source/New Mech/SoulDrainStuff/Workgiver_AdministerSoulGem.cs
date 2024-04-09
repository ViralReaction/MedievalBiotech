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
    public class Workgiver_AdministerSoul : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                return ThingRequest.ForGroup(ThingRequestGroup.Pawn);
            }
        }

        public override PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.ClosestTouch;
            }
        }

        public override Danger MaxPathDanger(Pawn pawn)
        {
            return Danger.Deadly;
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Pawn pawn2 = t as Pawn;
            if (pawn2 == null || pawn2 == pawn)
            {
                return false;
            }
            Pawn_GeneTracker genes = pawn2.genes;
            Gene_Soul gene_Soul = (genes != null) ? genes.GetFirstGeneOfType<Gene_Soul>() : null;
            if (gene_Soul == null)
            {
                return false;
            }
            if (gene_Soul.ValuePercent >= 0.95f)
            {
                return false;
            }
            if (!forced && gene_Soul.Value >= 0.25f)
            {
                return false;
            }
            if (!FeedPatientUtility.ShouldBeFed(pawn2))
            {
                return false;
            }
            if (!gene_Soul.ShouldConsumeSoulNow())
            {
                JobFailReason.Is("MB_NotAllowedSoulgem".Translate(), null);
                return false;
            }
            if (!pawn.CanReserve(t, 1, -1, null, forced))
            {
                return false;
            }
            bool arcanaFlag = gene_Soul.soulArcanaGemsAllowed;
            bool arcanaExist = true;
            if (arcanaFlag)
            {
                //bool arcanaFlag = true;
                
                if (GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(MB_DefOf.MB_ArcanaStone), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false, false, false), 9999f, (Thing pack) => !pack.IsForbidden(pawn) && pawn.CanReserve(pack, 1, -1, null, false), null, 0, -1, false, RegionType.Set_Passable, false) == null)
                {
                    arcanaExist = false;
                }
            }
            bool soulGemFlag = gene_Soul.soulBasicGemsAllowed;
            bool soulGemExist = true;
            if (soulGemFlag)
            {
                
                if (GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(MB_DefOf.MB_CorruptedSoulGemBasic), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false, false, false), 9999f, (Thing pack) => !pack.IsForbidden(pawn) && pawn.CanReserve(pack, 1, -1, null, false), null, 0, -1, false, RegionType.Set_Passable, false) == null)
                {
                    soulGemExist = false;
                }
            }
            if ((arcanaFlag && !arcanaExist) && (soulGemFlag && !soulGemExist))
            {
                JobFailReason.Is("NoIngredient".Translate(MB_DefOf.MB_CorruptedSoulGemBasic), null);
                return false;
            }
            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Pawn t2 = (Pawn)t;
            Pawn_GeneTracker genes = t2.genes;
            Gene_Soul gene_Soul = (genes != null) ? genes.GetFirstGeneOfType<Gene_Soul>() : null;
            bool flag1 = gene_Soul.soulArcanaGemsAllowed;
            bool flag2 = gene_Soul.soulBasicGemsAllowed;
            Thing thing = new Thing();
            if (flag1 && !flag2)
            {
                thing = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, pawn.Map.listerThings.ThingsOfDef(MB_DefOf.MB_CorruptedSoulGemBasic), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false, false, false), 9999f, (Thing t3) => pawn.CanReserve(t3, 1, -1, null, false) && !t3.IsForbidden(pawn), null);
            }
            if (flag2 && !flag1)
            {
                thing = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, pawn.Map.listerThings.ThingsOfDef(MB_DefOf.MB_CorruptedSoulGemBasic), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false, false, false), 9999f, (Thing t3) => pawn.CanReserve(t3, 1, -1, null, false) && !t3.IsForbidden(pawn), null);
            }
            if (flag1 && flag2)
            {
                List<Thing> addThings = new List<Thing>();
                addThings.AddRange(pawn.Map.listerThings.ThingsOfDef(MB_DefOf.MB_ArcanaStone));
                addThings.AddRange(pawn.Map.listerThings.ThingsOfDef(MB_DefOf.MB_CorruptedSoulGemBasic));
                IEnumerable<Thing> things = addThings;
                thing = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, things, PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false, false, false), 9999f, (Thing t3) => pawn.CanReserve(t3, 1, -1, null, false) && !t3.IsForbidden(pawn), null);
            }
            if (thing != null)
            {
                Job job = JobMaker.MakeJob(JobDefOf.FeedPatient, thing, t2);
                job.count = 1;
                return job;
            }
            return null;
        }

        private const float MinLevelForFeedingHemogenUnforced = 0.25f;

        private const float HemogenPctMax = 0.95f;
    }
}
