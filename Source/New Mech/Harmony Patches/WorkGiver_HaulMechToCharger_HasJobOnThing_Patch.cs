using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace MedievalBiotech
{
    [HarmonyPatch(typeof(WorkGiver_HaulMechToCharger), "HasJobOnThing")]
    public static class WorkGiver_HaulMechToCharger_HasJobOnThing_Patch
    {
        public static bool Prefix(ref bool __result, WorkGiver_HaulMechToCharger __instance, Pawn pawn, Thing t, bool forced)
        {

            if (__instance is null)
            {
                return false;
            }
            Pawn pawn2 = (Pawn)t;
            var extension = pawn2.def.GetModExtension<Custom_Mech>();
            if (extension != null && (!extension.UndeadMech || !extension.DemonMech))
            {
                if (extension.ArtificeMech)
                {

                    if (!pawn2.RaceProps.IsMechanoid || !pawn2.IsColonyMech)
                    {
                        return false;
                    }

                    if (pawn2.needs.energy != null)
                    {

                        if (!pawn2.Downed && !pawn2.needs.energy.IsLowEnergySelfShutdown)
                        {
                            return false;
                        }

                        MechanitorControlGroup mechControlGroup = pawn2.GetMechControlGroup();
                        if (mechControlGroup != null)
                        {
                            if (mechControlGroup.WorkMode == MechWorkModeDefOf.SelfShutdown)
                            {
                                return false;
                            }
                        }

                        float maxRechargeLimit = JobGiver_GetEnergy.GetMaxRechargeLimit(pawn2);
                        if (pawn2.needs.energy.CurLevel >= maxRechargeLimit)
                        {
                            return false;
                        }

                        Building_MechCharger closestCharger = JobGiver_GetEnergy_Charger.GetClosestCharger(pawn2, pawn, forced);
                        bool canReserve = pawn.CanReserve(t, 1, -1, null, forced);

                        Log.Message($"[DEBUG] Checking reservation and availability. Forbidden: {t.IsForbidden(pawn)}, Can Reserve: {canReserve}, Closest Charger: {(closestCharger != null ? closestCharger.ToString() : "None")}");

                        __result = pawn2.CurJobDef != JobDefOf.MechCharge && !t.IsForbidden(pawn) && canReserve && closestCharger != null;
                    }

                    return false;
                }
            }
            return true;
        }
    }
}
