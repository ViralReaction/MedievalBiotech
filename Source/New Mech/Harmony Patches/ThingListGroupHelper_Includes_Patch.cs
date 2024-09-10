using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace MedievalBiotech
{
    [HarmonyPatch(typeof(ThingListGroupHelper))]
    [HarmonyPatch("Includes")]
    internal static class ThingListGroupHelper_Includes_Patch
    {
        private static void Postfix(ref bool __result, ThingRequestGroup group, ThingDef def)
        {
            if (group == ThingRequestGroup.MechCharger)
            {
                __result = (__result || def.thingClass == typeof(Building_SteamCharger));
            }
        }
    }
}
