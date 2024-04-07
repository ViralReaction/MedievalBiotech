using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MedievalBiotech
{
    [HarmonyPatch(typeof(Building_GeneAssembler), "PowerOn", MethodType.Getter)]
    public static class Building_GeneAssembler_PowerOn_Patch
    {
        public static bool Prefix(ref bool __result)
        {
            __result = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(Building_GeneAssembler), "DoWork")]
    public static class Building_GeneAssembler_DoWork_Patch
    {
        public static Dictionary<Building_GeneAssembler, IntermittentSteamSprayer> sprayers = new Dictionary<Building_GeneAssembler, IntermittentSteamSprayer>();
        public static void Prefix(Building_GeneAssembler __instance)
        {
            if (!sprayers.TryGetValue(__instance, out var sprayer))
            {
                sprayers[__instance] = sprayer = new IntermittentSteamSprayer(__instance);
            }
            sprayer.SteamSprayerTick();
        }
    }

    public class IntermittentSteamSprayer
    {
        private Thing parent;

        private int ticksUntilSpray = 500;

        private int sprayTicksLeft;
        public IntermittentSteamSprayer(Thing parent)
        {
            this.parent = parent;
        }

        public void SteamSprayerTick()
        {
            if (sprayTicksLeft > 0)
            {
                sprayTicksLeft--;
                if (Rand.Value < 0.6f)
                {
                    FleckMaker.ThrowAirPuffUp(parent.TrueCenter() + new UnityEngine.Vector3(0, 0, 1), parent.Map);
                }
                if (Find.TickManager.TicksGame % 20 == 0)
                {
                    GenTemperature.PushHeat(parent, 40f);
                }
                if (sprayTicksLeft <= 0)
                {
                    ticksUntilSpray = Rand.RangeInclusive(500, 2000);
                }
                return;
            }
            ticksUntilSpray--;
            if (ticksUntilSpray <= 0)
            {
                sprayTicksLeft = Rand.RangeInclusive(200, 500);
            }
        }
    }

    public class SteamSprayer : ThingComp
    {
        public IntermittentSteamSprayer steamSprayer;
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
        }
    }

    [HarmonyPatch(typeof(Building_GeneAssembler), "Tick")]
    public static class Building_GeneAssembler_Tick_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            var codes = codeInstructions.ToList();
            var isHashOffset = AccessTools.Method(typeof(Gen), nameof(Gen.IsHashIntervalTick), new Type[] { typeof(Thing), typeof(int) });
            for (var i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                if (code.Calls(isHashOffset) && codes[i - 1].OperandIs(250))
                {
                    code.operand = AccessTools.Method(typeof(Building_GeneAssembler_Tick_Patch), nameof(AlwaysFalse));
                }
                yield return code;
            }
        }

        public static bool AlwaysFalse(Thing thing, int hash)
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(Building_GeneAssembler), "MaxComplexity")]
    public static class Building_GeneAssembler_MaxComplexity_Patch
    {
        public static bool Prefix(Building_GeneAssembler __instance, ref int __result)
        {
            __result = MaxComplexity(__instance);
            return false;
        }

        public static int MaxComplexity(Building_GeneAssembler __instance)
        {
            int num = 6;
            List<Thing> connectedFacilities = __instance.ConnectedFacilities;
            if (connectedFacilities != null)
            {
                foreach (Thing item in connectedFacilities)
                {
                    CompRefuelable compRefuelable = item.TryGetComp<CompRefuelable>();
                    if (compRefuelable == null || compRefuelable.HasFuel)
                    {
                        num += (int)item.GetStatValue(StatDefOf.GeneticComplexityIncrease);
                    }
                }
                return num;
            }
            return num;
        }
    }

    [HarmonyPatch(typeof(Building_GeneAssembler), "Finish")]
    public static class Building_GeneAssembler_Finish_Patch
    {
        public static void Prefix(Building_GeneAssembler __instance)
        {
            float currentComplexity = __instance.TotalGCX - 6;
            if (currentComplexity > 0)
            {
                List<Thing> connectedFacilities = __instance.ConnectedFacilities;
                foreach (var facility in connectedFacilities)
                {
                    if (facility.def == MB_DefOf.GeneProcessor)
                    {
                        currentComplexity -= facility.GetStatValue(StatDefOf.GeneticComplexityIncrease);
                        var waste = ThingMaker.MakeThing(ThingDefOf.Wastepack);
                        waste.stackCount = 1;
                        GenSpawn.Spawn(waste, facility.Position, facility.Map);
                        if (currentComplexity <= 0)
                        {
                            return;
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Building_GeneAssembler), "GetGenepacks")]
    public static class Building_GeneAssembler_GetGenepacks_Patch
    {
        public static bool Prefix(Building_GeneAssembler __instance, ref List<Genepack> __result, bool includePowered, bool includeUnpowered)
        {
            __result = GetGenepacks(__instance, includePowered, includeUnpowered);
            return false;
        }

        public static List<Genepack> GetGenepacks(Building_GeneAssembler __instance, bool includePowered, bool includeUnpowered)
        {
            __instance.tmpGenepacks.Clear();
            List<Thing> connectedFacilities = __instance.ConnectedFacilities;
            if (connectedFacilities != null)
            {
                foreach (Thing item in connectedFacilities)
                {
                    CompGenepackContainer compGenepackContainer = item.TryGetComp<CompGenepackContainer>();
                    if (compGenepackContainer != null)
                    {
                        var fueled = item.TryGetComp<CompRefuelable>()?.HasFuel ?? true;
                        if (includePowered && fueled)
                        {
                            __instance.tmpGenepacks.AddRange(compGenepackContainer.ContainedGenepacks.Where(x => x.deteriorationPct < 1f));
                        }
                        else if (includeUnpowered && !fueled)
                        {
                            __instance.tmpGenepacks.AddRange(compGenepackContainer.ContainedGenepacks);

                        }
                    }
                }
            }
            return __instance.tmpGenepacks;
        }
    }
}
