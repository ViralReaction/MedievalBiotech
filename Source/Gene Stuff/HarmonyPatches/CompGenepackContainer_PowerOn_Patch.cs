using HarmonyLib;
using RimWorld;
using Verse;

namespace MedievalBiotech
{
    [HarmonyPatch(typeof(CompGenepackContainer), "PowerOn", MethodType.Getter)]
    public static class CompGenepackContainer_PowerOn_Patch
    {
        public static bool Prefix(ref bool __result, CompGenepackContainer __instance)
        {
            if (__instance.parent.def == ThingDefOf.GeneBank)
            {
                __result = __instance.parent.TryGetComp<CompRefuelable>().HasFuel;
            }
            return false;
        }
    }
}
