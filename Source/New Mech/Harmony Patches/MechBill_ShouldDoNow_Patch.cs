using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace MedievalBiotech
{
    [HarmonyPatch(typeof(Bill_Mech), "ShouldDoNow")]
    public static class MechBill_ShouldDoNow_Patch
    {
        public static void Postfix(ref bool __result, Bill_Mech __instance)
        {
            if (__result != null)
            {
                Pawn pawn = __instance.boundPawn;
                if (pawn != null)
                {
                    if (Utility.IsSanguinMage(pawn) || Utility.IsNecrarch(pawn))
                    {
                        JobFailReason.Is("NotAMechanitor".Translate(), null);
                        __result = false;
                    }
                }
            }
        }
    }
}
