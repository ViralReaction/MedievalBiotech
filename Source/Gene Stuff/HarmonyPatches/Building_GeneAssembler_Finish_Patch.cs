using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MedievalBiotech
{
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
}
