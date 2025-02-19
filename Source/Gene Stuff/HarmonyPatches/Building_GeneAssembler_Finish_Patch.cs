using HarmonyLib;
using RimWorld;
using Verse;

namespace MedievalBiotech
{
    [HarmonyPatch(typeof(Building_GeneAssembler), "Finish")]
    public static class Building_GeneAssembler_Finish_Patch
    {
        public static void Prefix(Building_GeneAssembler __instance)
        {
            float currentComplexity = __instance.TotalGCX - 6;
            if (currentComplexity <= 0)
            {
                return; // No need to process if complexity is already at or below the threshold
            }
            foreach (var facility in __instance.ConnectedFacilities)
            {
                if (facility.def == MB_DefOf.GeneProcessor)
                {
                    currentComplexity -= facility.GetStatValue(StatDefOf.GeneticComplexityIncrease);

                    Thing waste = ThingMaker.MakeThing(ThingDefOf.Wastepack);
                    waste.stackCount = 1;
                    GenSpawn.Spawn(waste, facility.Position, facility.Map);

                    if (currentComplexity <= 0)
                    {
                        return; // Stop processing if we've reduced complexity sufficiently
                    }
                }
            }
        }
    }
}
