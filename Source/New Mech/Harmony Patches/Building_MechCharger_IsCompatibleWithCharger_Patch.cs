using RimWorld;
using System;
using HarmonyLib;
using Verse;

namespace MedievalBiotech
{
    [HarmonyPatch(typeof(Building_MechCharger),  nameof(Building_MechCharger.IsCompatibleWithCharger), new Type[] { typeof(PawnKindDef) })]
    public static class Building_MechCharger_IsCompatibleWithCharger_Patch
    {
        public static bool Prefix(ref bool __result, Building_MechCharger __instance, PawnKindDef kindDef)
        {
            if (__instance is Building_SteamCharger steamCharger)
            {
                __result = steamCharger.IsCompatibleWithSteamCharger(kindDef);
                return false;
            }
            return true;
        }

        public static void Postfix(ref bool __result, Building_MechCharger __instance, PawnKindDef kindDef)
        {
            if (__instance.GetType() == typeof(Building_MechCharger))
            {
                var extension = kindDef.race.GetModExtension<Custom_Mech>();
                if (extension != null)
                {
                    __result = false;
                }
            }
        }

    }
}
