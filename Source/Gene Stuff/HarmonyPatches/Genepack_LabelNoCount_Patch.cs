using HarmonyLib;
using RimWorld;
using Verse;

namespace MedievalBiotech
{

    [HarmonyPatch(typeof(Genepack), "LabelNoCount", MethodType.Getter)]
    public static class Genepack_LabelNoCount_Patch
    {
        public static void Postfix(Genepack __instance, ref string __result)
        {
            if (__instance.deteriorationPct == 1f)
            {
                __result = "MB.GenepackInactive".Translate(__result);
            }
        }
    }
}
