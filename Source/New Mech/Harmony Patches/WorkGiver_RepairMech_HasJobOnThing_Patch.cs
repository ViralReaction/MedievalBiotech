using HarmonyLib;
using RimWorld;
using Verse;

namespace MedievalBiotech
{
    [HarmonyPatch(typeof(WorkGiver_RepairMech), "HasJobOnThing")]
    public static class WorkGiver_RepairMech_HasJobOnThing_Patch
    {
        public static void Postfix(ref bool __result,Pawn pawn, Thing t)
        {
            if (t is not Pawn mech)
            {
                return; // Ensure t is a Pawn before proceeding
            }
            
            bool vampMech = Utility.IsSanguinMech(mech);
            if (Utility.IsSanguinMage(pawn))
            {
                
                __result = vampMech; // Only allow repair if it is a SanguinMech and SanguinMage
                return;
            }
            bool undeadMech = Utility.IsUndeadMech(mech);
            if (Utility.IsNecromancer(pawn))
            {
                __result = undeadMech; // Only allow repair if it is an UndeadMech and Necromancer
                return;
            }
            __result = !(undeadMech || vampMech); // Prevent repair of these mechs
        }
    }
}
