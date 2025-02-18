using HarmonyLib;
using MedievalBiotech.New_Mech;
using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace MedievalBiotech
{
    [HarmonyPatch(typeof(JobGiver_GetEnergy_Charger ), "GetClosestCharger")]
    public static class JobGiver_GetEnergy_Charger_GetClosestCharger
    {
        public static bool Prefix(ref Building_MechCharger __result, JobGiver_GetEnergy_Charger __instance, Pawn mech, Pawn carrier, bool forced)
        {

            var extension = mech.def.GetModExtension<Custom_Mech>();
            if (extension != null && extension.ArtificeMech)
            {
                __result = GetClosestCharger(carrier, mech, forced);
                return false;
            }
            return true;
        }
        public static Building_MechCharger GetClosestCharger(Pawn carrier, Pawn mech, bool forced)
        {
            if (mech == null)
            {
                Log.Error("[ERROR] GetClosestCharger called with null mech.");
                return null;
            }

            if (mech.Map == null)
            {
                Log.Error($"[ERROR] Mech {mech} has no valid map.");
                return null;
            }

            Danger danger = forced ? Danger.Deadly : Danger.Some;
            Building_MechCharger closestCharger = null;
            float closestDist = 9999f;

            List<Thing> potentialChargers = mech.Map.listerThings?.ThingsOfDef(MB_DefOf.MB_BasicRecharger);

            if (potentialChargers == null || potentialChargers.Count == 0)
            {
                return null;
            }


            foreach (Thing t in potentialChargers)
            {
                if (t == null)
                {
                    continue;
                }

                // Ensure the type is correct before casting
                if (!(t is Building_SteamCharger building_MechCharger))
                {
                    continue;
                }

                if (!carrier.CanReach(t, PathEndMode.InteractionCell, danger, false, false, TraverseMode.ByPawn))
                {
                    continue;
                }

                if (carrier != mech)
                {
                    if (!forced && building_MechCharger.Map.reservationManager.ReservedBy(building_MechCharger, carrier, null))
                    {
                        continue;
                    }
                    if (forced && KeyBindingDefOf.QueueOrder.IsDownEvent && building_MechCharger.Map.reservationManager.ReservedBy(building_MechCharger, carrier, null))
                    {
                        continue;
                    }
                }

                if (t.IsForbidden(carrier))
                {
                    continue;
                }

                if (!carrier.CanReserve(t, 1, -1, null, forced))
                {
                    continue;
                }

                if (!building_MechCharger.CanPawnChargeCurrently(mech))
                {
                    continue;
                }

                float dist = (t.Position - mech.Position).LengthHorizontalSquared;
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestCharger = building_MechCharger;
                }
            }

            return closestCharger;
        }

    }
}
