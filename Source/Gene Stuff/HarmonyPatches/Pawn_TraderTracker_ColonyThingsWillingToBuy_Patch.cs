using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MedievalBiotech
{
    [HarmonyPatch(typeof(Pawn_TraderTracker), "ColonyThingsWillingToBuy")]
    public static class Pawn_TraderTracker_ColonyThingsWillingToBuy_Patch
    {
        public static IEnumerable<Thing> Postfix(IEnumerable<Thing> __result, Pawn_TraderTracker __instance)
        {
            foreach (Thing thing in __result)
            {
                yield return thing;
            }
            if (ModsConfig.BiotechActive)
            {
                IEnumerable<Building> enumerable2 = __instance.pawn.Map.listerBuildings.AllBuildingsColonistOfDef(MB_DefOf.MB_GenePotionsRack);
                foreach (Building item2 in enumerable2)
                {
                    if (!__instance.ReachableForTrade(item2))
                    {
                        continue;
                    }
                    CompGenepackContainer compGenepackContainer = item2.TryGetComp<CompGenepackContainer>();
                    if (compGenepackContainer == null)
                    {
                        continue;
                    }
                    List<Genepack> containedGenepacks = compGenepackContainer.ContainedGenepacks;
                    foreach (Genepack item3 in containedGenepacks)
                    {
                        yield return item3;
                    }
                }
            }
        }
    }
}
