using HarmonyLib;
using RimWorld;

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
}
