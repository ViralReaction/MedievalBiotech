using HarmonyLib;
using RimWorld;
using Verse;

namespace MedievalBiotech
{
    [HarmonyPatch(typeof(JobGiver_GetEnergy_Charger ), "GetClosestCharger")]
    public static class JobGiver_GetEnergy_Charger_GetClosestCharger
    {
        public static bool Prefix(ref Building_MechCharger __result, Pawn mech, Pawn carrier, bool forced)
        {
            var extension = mech.def.GetModExtension<Custom_Mech>();
            if (extension != null && extension.ArtificeMech)
            {
                __result = Utility.GetClosestCharger(carrier, mech, forced);
                return false;
            }
            return true;
        }
        
    }
}
