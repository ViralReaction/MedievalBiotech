using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse.AI;
using Verse;

namespace MedievalBiotech
{
    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
    public static class FloatMenuMakerMap_AddHumanlikeOrders_Patch
    {
        public static void Postfix(Vector3 clickPos, Pawn pawn, ref List<FloatMenuOption> opts)
        {
            IntVec3 c = IntVec3.FromVector3(clickPos);
            List<Thing> thingList = c.GetThingList(pawn.Map);
            for (int i = 0; i < thingList.Count; i++)
            {
                Thing t = thingList[i];
                if (t is Corpse corpse && corpse.GetRotStage() == RotStage.Fresh)
                {
                    var geneExtractor = (Building_GeneExtractor)GenClosest.ClosestThingReachable(corpse.PositionHeld, corpse.MapHeld,
                        ThingRequest.ForDef(ThingDefOf.GeneExtractor), PathEndMode.InteractionCell, TraverseParms.For(pawn), 9999f,
                        (Thing x) => ((Building_GeneExtractor)x).CanAcceptPawn(corpse.InnerPawn) && pawn.CanReserve(x, 1, -1, null, true));
                    if (geneExtractor is null || !pawn.CanReserveAndReach(corpse, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, ignoreOtherReservations: true))
                    {
                        continue;
                    }
                    opts.Add(new FloatMenuOption("MB.CarryCorpseToTable".Translate(corpse.InnerPawn.LabelShortCap), delegate
                    {
                        Job job = JobMaker.MakeJob(MB_DefOf.MB_CarryCorpseToTable, geneExtractor, corpse);
                        job.count = 1;
                        pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                    }));
                }
            }
        }
    }
}
