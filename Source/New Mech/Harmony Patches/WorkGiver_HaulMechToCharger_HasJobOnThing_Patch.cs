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

            if (__instance == null) return false;
            if (t is not Pawn mech) return true;

            var extension = mech.def.GetModExtension<Custom_Mech>();
            if (extension != null && (extension.UndeadMech || extension.DemonMech))
            {
                return false;
            }
            // No need to check extension == null again, just check ArtificeMech directly
            if (!extension.ArtificeMech)
            {
                return true;
            }
            if (!mech.RaceProps.IsMechanoid || !mech.IsColonyMech)
            {
                return false;
            }
            if (mech.needs.energy == null || (!mech.Downed && !mech.needs.energy.IsLowEnergySelfShutdown))
            {
                return false;
            }
            MechanitorControlGroup mechControlGroup = mech.GetMechControlGroup();
            if (mechControlGroup?.WorkMode == MechWorkModeDefOf.SelfShutdown)
            {
                return false;
            }
            if (mech.needs.energy.CurLevel >= JobGiver_GetEnergy.GetMaxRechargeLimit(mech))
            {
                return false;
            }
            Building_MechCharger closestCharger = Utility.GetClosestCharger(mech, pawn, forced);
            bool canReserve = pawn.CanReserve(t, 1, -1, null, forced);

            Log.Message($"[DEBUG] Checking reservation and availability. Forbidden: {t.IsForbidden(pawn)}, Can Reserve: {canReserve}, Closest Charger: {(closestCharger != null ? closestCharger.ToString() : "None")}");

            __result = mech.CurJobDef != JobDefOf.MechCharge && !t.IsForbidden(pawn) && canReserve && closestCharger != null;
            return false;
        }
    }
}