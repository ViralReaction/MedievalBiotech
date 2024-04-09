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
            bool undeadMech = Utility.IsUndeadMech(mech);
            bool vampMech = Utility.IsSanguinMech(mech);
            if ((Utility.IsSanguinMage(pawn) == vampMech) || (Utility.IsNecromancer(pawn) == undeadMech))
            {
                return false;
            }
            else if (undeadMech || vampMech)
                return false;
            return true;
        }
    }
}