using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace MedievalBiotech
{
    [HarmonyPatch(typeof(WorkGiver_RepairMech), "HasJobOnThing")]
    public static class WorkGiver_RepairMech_HasJobOnThing_Patch
    {
        public static void Postfix(WorkGiver_RepairMech __instance, ref bool __result,Pawn pawn, Thing t)
        {
            if (__instance is not null)
            {
                Pawn pawn2 = (Pawn)t;
                bool undeadMech = Utility.IsUndeadMech(pawn2);
                bool vampMech = Utility.IsSanguinMech(pawn2);
                if (Utility.IsSanguinMage(pawn))
                {
                    if (!vampMech)
                    __result = false;
                    return;
                }
                if (Utility.IsNecromancer(pawn))
                {
                    if (!undeadMech)
                    __result = false;
                    return;
                }
                if (undeadMech || vampMech)
                    __result = false;
            }
            return;
        }
    }
}
