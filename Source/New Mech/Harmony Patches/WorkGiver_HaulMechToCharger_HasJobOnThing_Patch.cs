using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace MedievalBiotech
{
    [HarmonyPatch(typeof(WorkGiver_HaulMechToCharger), "HasJobOnThing")]
    public static class WorkGiver_HaulMechToCharger_HasJobOnThing_Patch
    {
        public static void Postfix(ref bool __result, Pawn pawn, Thing t)
        {
            if (__result != null)
            {
                Pawn pawn2 = (Pawn)t;
                if (Utility.IsSanguinMech(pawn2) || Utility.IsUndeadMech(pawn2))
                {
                    __result = false;
                }
            }
            return;
        }
    }
}
