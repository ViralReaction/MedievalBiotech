using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;
using System.Diagnostics;

namespace MedievalBiotech
{
    [HarmonyPatch(typeof(Building_MechCharger),  nameof(Building_MechCharger.IsCompatibleWithCharger), new Type[] { typeof(PawnKindDef) })]
    public static class Building_MechCharger_IsCompatibleWithCharger_Patch
    {
        public static void Postfix(ref bool __result, Building_MechCharger __instance, PawnKindDef kindDef)
        {
            var extension = kindDef.race.GetModExtension<Custom_Mech>();
            var instanceThingClass = __instance.def.thingClass;
            if (instanceThingClass == typeof(Building_SteamCharger))
            {
                if (extension != null && extension.ArtificeMech)
                {
                    __result = true;
                }
            }
            else if (instanceThingClass == typeof(Building_MechCharger))
            {
                if (extension != null)
                {
                    __result = false;
                }
            }
        }
    }
}
