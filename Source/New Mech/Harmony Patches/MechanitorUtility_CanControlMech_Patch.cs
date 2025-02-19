using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace MedievalBiotech
{
    [HarmonyPatch(typeof(MechanitorUtility), "CanControlMech")]
    public static class MechanitorUtility_CanControlMech_Patch
    {
        public static bool Prefix(Pawn pawn, Pawn mech)
        {
            var extension = pawn.def.GetModExtension<Custom_Mech>();
            if (extension == null) return true;

            if (extension.UndeadMech) return Utility.IsNecromancer(pawn);
            if (extension.DemonMech) return Utility.IsSanguinMage(pawn);

            return true;
        }
    }
}