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
            float closestDist = float.MaxValue;

            // Fetch all chargers (all are SteamChargers)
            var potentialChargers = mech.Map.GetComponent<RechargerMapComponent>()?.allChargers;
            if (potentialChargers == null || potentialChargers.Count == 0)
            {
                return null;
            }

            var reservationManager = mech.Map.reservationManager;

            foreach (Building_SteamCharger charger in potentialChargers)
            {
                if (!carrier.CanReach(charger, PathEndMode.InteractionCell, danger, false, false, TraverseMode.ByPawn))
                {
                    continue;
                }

                bool isReserved = reservationManager.ReservedBy(charger, carrier, null);
                if ((!forced && isReserved) || (forced && KeyBindingDefOf.QueueOrder.IsDownEvent && isReserved))
                {
                    continue;
                }

                if (charger.IsForbidden(carrier) || !carrier.CanReserve(charger, 1, -1, null, forced))
                {
                    continue;
                }

                if (!charger.CanPawnChargeCurrently(mech))
                {
                    continue;
                }

                // Use squared distance for performance
                float dist = (charger.Position - mech.Position).LengthHorizontalSquared;
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestCharger = charger;
                }
            }

            return closestCharger;
        }


    }
}
