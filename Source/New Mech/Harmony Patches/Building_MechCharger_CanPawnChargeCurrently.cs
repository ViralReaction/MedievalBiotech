using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace MedievalBiotech
{
    [HarmonyPatch(typeof(Building_MechCharger), "CanPawnChargeCurrently")]
    public static class Building_MechCharger_CanPawnChargeCurrently
    {
        public static bool Prefix(ref bool __result, Building_MechCharger __instance, Pawn pawn)
        {
            if (__instance is Building_SteamCharger steamCharger)
            {
                var extension = pawn.def.GetModExtension<Custom_Mech>();
                if (extension != null && extension.ArtificeMech)
                {
                    __result = steamCharger.CanPawnChargeCurrentlySteam(pawn);

                    return false;
                }
            }
            return true;
        }
    }
}