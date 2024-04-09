using RimWorld;
using System.Collections.Generic;
using Verse.AI;
using Verse;

namespace MedievalBiotech
{
    public class WorkGiver_Warden_DeliverSoulGem : WorkGiver_Warden
    {
        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!ModsConfig.BiotechActive)
            {
                return null;
            }
            if (!base.ShouldTakeCareOfPrisoner(pawn, t, false))
            {
                return null;
            }
            Pawn prisoner = (Pawn)t;
            if (!prisoner.guest.CanBeBroughtFood || !prisoner.Position.IsInPrisonCell(prisoner.Map))
            {
                return null;
            }
            if (WardenFeedUtility.ShouldBeFed(prisoner))
            {
                return null;
            }
            Pawn_GeneTracker genes = prisoner.genes;
            Gene_Soul gene_Soul = ((genes != null) ? genes.GetGene(MB_DefOf.MB_SoulStarved) : null) as Gene_Soul;
            if (gene_Soul == null)
            {
                return null;
            }
            if (gene_Soul.soulBasicGemsAllowed)
            {
                if (!gene_Soul.ShouldConsumeSoulNow())
                {
                    return null;
                }
                if (this.SoulGemAlreadyAvailableFor(prisoner))
                {
                    return null;
                }
                Thing thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(MB_DefOf.MB_CorruptedSoulGemBasic), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false, false, false), 9999f, (Thing pack) => !pack.IsForbidden(pawn) && pawn.CanReserve(pack, 1, -1, null, false) && pack.GetRoom(RegionType.Set_All) != prisoner.GetRoom(RegionType.Set_All), null, 0, -1, false, RegionType.Set_Passable, false);
                if (thing == null)
                {
                    return null;
                }
                Job job = JobMaker.MakeJob(JobDefOf.DeliverFood, thing, prisoner);
                job.count = 1;
                job.targetC = RCellFinder.SpotToChewStandingNear(prisoner, thing, null);
                return job;
            }
            if (gene_Soul.soulArcanaGemsAllowed)
            {
                if (!gene_Soul.ShouldConsumeSoulNow())
                {
                    return null;
                }
                if (this.ArcanaStoneAlreadyAvailableFor(prisoner))
                {
                    return null;
                }
                Thing thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(MB_DefOf.MB_ArcanaStone), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false, false, false), 9999f, (Thing pack) => !pack.IsForbidden(pawn) && pawn.CanReserve(pack, 1, -1, null, false) && pack.GetRoom(RegionType.Set_All) != prisoner.GetRoom(RegionType.Set_All), null, 0, -1, false, RegionType.Set_Passable, false);
                if (thing == null)
                {
                    return null;
                }
                Job job = JobMaker.MakeJob(JobDefOf.DeliverFood, thing, prisoner);
                job.count = 1;
                job.targetC = RCellFinder.SpotToChewStandingNear(prisoner, thing, null);
                return job;
            }
            return null;

        }

        private bool SoulGemAlreadyAvailableFor(Pawn prisoner)
        {
            if (prisoner.carryTracker.CarriedCount(MB_DefOf.MB_CorruptedSoulGemBasic) > 0)
            {
                return true;
            }
            if (prisoner.inventory.Count(MB_DefOf.MB_CorruptedSoulGemBasic) > 0)
            {
                return true;
            }
            Room room = prisoner.GetRoom(RegionType.Set_All);
            if (room != null)
            {
                List<Region> regions = room.Regions;
                for (int i = 0; i < regions.Count; i++)
                {
                    if (regions[i].ListerThings.ThingsOfDef(MB_DefOf.MB_CorruptedSoulGemBasic).Count > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool ArcanaStoneAlreadyAvailableFor(Pawn prisoner)
        {
            if (prisoner.carryTracker.CarriedCount(MB_DefOf.MB_ArcanaStone) > 0)
            {
                return true;
            }
            if (prisoner.inventory.Count(MB_DefOf.MB_ArcanaStone) > 0)
            {
                return true;
            }
            Room room = prisoner.GetRoom(RegionType.Set_All);
            if (room != null)
            {
                List<Region> regions = room.Regions;
                for (int i = 0; i < regions.Count; i++)
                {
                    if (regions[i].ListerThings.ThingsOfDef(MB_DefOf.MB_ArcanaStone).Count > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}