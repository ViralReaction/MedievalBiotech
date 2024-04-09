using HarmonyLib;
using RimWorld;
using Verse;

namespace MedievalBiotech
{
    [HarmonyPatch(typeof(MechanitorUtility), "ShouldBeMechanitor")]
    public static class MechanitorUtility_ShouldBeMechanitor_Patch
    {
        public static bool Prefix(ref bool __result, Pawn pawn)
        {
            if (Utility.IsSanguinMage(pawn) || Utility.IsNecromancer(pawn))
            {
                __result = true;
                return false;
            }
            return true;
        }
    }
}
