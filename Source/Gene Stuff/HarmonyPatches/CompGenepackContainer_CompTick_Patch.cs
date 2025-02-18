using HarmonyLib;
using RimWorld;
using Verse;

namespace MedievalBiotech
{
    [HarmonyPatch(typeof(CompGenepackContainer), "CompTick")]

    public static class CompGenepackContainer_CompTick_Patch
    {
        public static void Postfix(CompGenepackContainer __instance)
        {
            if (__instance.parent.IsHashIntervalTick(250))
            {
                __instance.innerContainer.ThingOwnerTickRare();
            }
        }
    }
}
