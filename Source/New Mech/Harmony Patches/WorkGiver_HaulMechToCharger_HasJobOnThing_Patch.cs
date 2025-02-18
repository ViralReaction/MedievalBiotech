using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace MedievalBiotech
{
    [HarmonyPatch(typeof(WorkGiver_HaulMechToCharger), "HasJobOnThing")]
    public static class WorkGiver_HaulMechToCharger_HasJobOnThing_Patch
    {
        public static bool Prefix(WorkGiver_HaulMechToCharger __instance, Pawn pawn, Thing t)
        {

            if (__instance is null)
            {
                return false;
            }
            Pawn pawn2 = (Pawn)t;
            var extension = pawn.def.GetModExtension<Custom_Mech>();
            if (extension != null && (extension.UndeadMech || extension.DemonMech))
            {
                if (extension.ArtificeMech)
                {
                    Log.Message("WorkGiver True");
                    return true;
                }
                return false;
            }
            return true;
        }
    }
}
