using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MedievalBiotech
{
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
            foreach (Thing item in connectedFacilities)
            {
                CompGenepackContainer compGenepackContainer = item.TryGetComp<CompGenepackContainer>();
                if (compGenepackContainer == null)
                {
                    continue;
                }

                bool fueled = item.TryGetComp<CompRefuelable>()?.HasFuel ?? true;
                if (includePowered && fueled)
                {
                    if (compGenepackContainer?.ContainedGenepacks != null)
                    {
                        foreach (var genepack in compGenepackContainer.ContainedGenepacks)
                        {
                            if (genepack.deteriorationPct < 1f)
                            {
                                __instance.tmpGenepacks.Add(genepack);
                            }
                        }
                    }

                }
                else if (includeUnpowered && !fueled)
                {
                    __instance.tmpGenepacks.AddRange(compGenepackContainer.ContainedGenepacks);

                }

            }
            return __instance.tmpGenepacks;
        }
    }
}
