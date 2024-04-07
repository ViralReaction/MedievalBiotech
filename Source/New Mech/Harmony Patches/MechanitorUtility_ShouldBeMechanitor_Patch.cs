using HarmonyLib;
using RimWorld;
using Verse;

namespace MedievalBiotech
{
    [HarmonyPatch(typeof(MechanitorUtility), "ShouldBeMechanitor")]
    public static class MechanitorUtility_ShouldBeMechanitor_Patch
    {
        public static bool Prefix(ref bool __result, CompGenepackContainer __instance, Pawn pawn)
        {
            if (pawn.health.hediffSet.HasHediff(MB_DefOf.MB_BloodChaliceImplant))
            {
                __result = true;
                return false;
            }
            return true;
        }
    }
}
