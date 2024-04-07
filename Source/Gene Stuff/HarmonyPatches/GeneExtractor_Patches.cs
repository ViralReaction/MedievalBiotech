using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MedievalBiotech
{
    [HarmonyPatch(typeof(Building_GeneExtractor), "PowerOn", MethodType.Getter)]
    public static class Building_GeneExtractor_PowerOn_Patch
    {
        public static bool Prefix(ref bool __result)
        {
            __result = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(Building_GeneExtractor), "ContainedPawn", MethodType.Getter)]
    public static class Building_GeneExtractor_ContainedPawn_Patch
    {
        public static bool Prefix(Building_GeneExtractor __instance, ref Pawn __result)
        {
            var firstThing = __instance.innerContainer.FirstOrDefault();
            if (firstThing != null)
            {
                if (firstThing is Corpse corpse)
                {
                    __result = corpse.InnerPawn;
                }
                else if (firstThing is Pawn pawn)
                {
                    __result = pawn;
                }
            }
            return false;
        }
    }

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



    [HarmonyPatch(typeof(Building_GeneExtractor), "CanAcceptPawn")]
    public static class Building_GeneExtractor_CanAcceptPawn_Patch
    {
        public static bool Prefix(Building_GeneExtractor __instance, ref AcceptanceReport __result, Pawn pawn)
        {
            __result = CanAcceptPawn(__instance, pawn);
            return false;
        }

        public static AcceptanceReport CanAcceptPawn(Building_GeneExtractor __instance, Pawn pawn)
        {
            if (__instance.selectedPawn != null && __instance.selectedPawn != pawn)
            {
                return false;
            }
            if (!pawn.RaceProps.Humanlike || pawn.IsQuestLodger())
            {
                return false;
            }
            if (__instance.innerContainer.Count > 0)
            {
                return "Occupied".Translate();
            }
            if (pawn.genes == null || !pawn.genes.GenesListForReading.Any((Gene x) => x.def.passOnDirectly))
            {
                return "PawnHasNoGenes".Translate(pawn.Named("PAWN"));
            }
            if (!pawn.genes.GenesListForReading.Any((Gene x) => x.def.biostatArc == 0))
            {
                return "PawnHasNoNonArchiteGenes".Translate(pawn.Named("PAWN"));
            }
            if (pawn.health.hediffSet.HasHediff(HediffDefOf.XenogerminationComa))
            {
                return "InXenogerminationComa".Translate();
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Building_GeneExtractor), "TryAcceptPawn")]
    public static class Building_GeneExtractor_TryAcceptPawn_Patch
    {
        public static bool Prefix(Building_GeneExtractor __instance, Pawn pawn)
        {
            TryAcceptPawn(__instance, pawn);
            return false;
        }

        public static void TryAcceptPawn(Building_GeneExtractor __instance, Pawn pawn)
        {
            if ((bool)__instance.CanAcceptPawn(pawn))
            {
                __instance.selectedPawn = pawn;
                bool num = pawn.Corpse != null ? pawn.Corpse.DeSpawnOrDeselect() : pawn.DeSpawnOrDeselect();
                if (__instance.innerContainer.TryAddOrTransfer(pawn.Corpse != null ? (Thing)pawn.Corpse : pawn))
                {
                    __instance.startTick = Find.TickManager.TicksGame;
                    __instance.ticksRemaining = 30000;
                }
                if (num)
                {
                    Find.Selector.Select(pawn.Corpse != null ? (Thing)pawn.Corpse : pawn, playSound: false, forceDesignatorDeselect: false);
                }
            }
        }
    }

    // Below needs to be fixed. Currently commented out until such time.
    //[HarmonyPatch(typeof(Building_GeneExtractor), "DynamicDrawPhaseAt")]
    //public static class Building_GeneExtractor_Draw_Patch
    //{
    //    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
    //    {
    //        var codes = codeInstructions.ToList();
    //        for (var i = 0; i < codes.Count; i++)
    //        {
    //            if (i > 1)
    //            {
    //                yield return new CodeInstruction(OpCodes.Ldarg_0);
    //                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Building_GeneExtractor_Draw_Patch), nameof(DrawCustom)));
    //                yield return new CodeInstruction(OpCodes.Ret);
    //                yield break;
    //            }
    //            else
    //            {
    //                yield return codes[i];
    //            }
    //        }
    //    }

    //    public static void DrawCustom(Building_GeneExtractor building_GeneExtractor)
    //    {
    //        if (building_GeneExtractor.Working && building_GeneExtractor.selectedPawn != null && ContainsCorpseOrPawn(building_GeneExtractor.innerContainer, building_GeneExtractor.selectedPawn))
    //        {
    //            var oldHoldingOwner = building_GeneExtractor.selectedPawn.holdingOwner;
    //            if (building_GeneExtractor.selectedPawn.Corpse != null)
    //            {
    //                var newHoldingOwner = building_GeneExtractor.selectedPawn.Corpse.holdingOwner;
    //                building_GeneExtractor.selectedPawn.holdingOwner = newHoldingOwner;
    //            }
    //            building_GeneExtractor.selectedPawn.Drawer.renderer.RenderPawnAt(building_GeneExtractor.DrawPos + building_GeneExtractor.PawnDrawOffset, null, neverAimWeapon: true);
    //            building_GeneExtractor.selectedPawn.holdingOwner = oldHoldingOwner;
    //        }
    //    }

    //    public static bool ContainsCorpseOrPawn(ThingOwner innerContainer, Pawn selectedPawn)
    //    {
    //        foreach (var thing in innerContainer)
    //        {
    //            if (thing is Corpse corpse && corpse.InnerPawn == selectedPawn)
    //            {
    //                return true;
    //            }
    //            else if (thing is Pawn pawn && pawn == selectedPawn)
    //            {
    //                return true;
    //            }
    //        }
    //        return false;
    //    }
    //}

    [HarmonyPatch(typeof(Building_GeneExtractor), "Tick")]
    public static class Building_GeneExtractor_Tick_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            var codes = codeInstructions.ToList();
            var isHashOffset = AccessTools.Method(typeof(Gen), nameof(Gen.IsHashIntervalTick), new Type[] { typeof(Thing), typeof(int) });
            var powerOn = AccessTools.PropertyGetter(typeof(CompPowerTrader), nameof(CompPowerTrader.PowerOn));
            var powerCutField = AccessTools.Field(typeof(Building_GeneExtractor), "powerCutTicks");
            for (var i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                if (code.Calls(isHashOffset))
                {
                    code.operand = AccessTools.Method(typeof(Building_GeneExtractor_Tick_Patch), nameof(AlwaysFalse));
                }
                if (code.Calls(powerOn))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    code.operand = AccessTools.Method(typeof(Building_GeneExtractor_Tick_Patch), nameof(ShouldWork));
                }
                if (code.opcode == OpCodes.Ldc_I4 && code.OperandIs(60000))
                {
                    code.operand = int.MaxValue;
                }
                yield return code;
            }
        }

        public static bool AlwaysFalse(Thing thing, int hash)
        {
            return false;
        }
        public static bool ShouldWork(CompPowerTrader trader, Building_GeneExtractor parent)
        {
            parent.powerCutTicks = 0;
            return false;
        }
    }
}
