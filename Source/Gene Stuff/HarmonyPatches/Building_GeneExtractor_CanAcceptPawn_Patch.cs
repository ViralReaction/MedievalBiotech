using HarmonyLib;
using RimWorld;
using Verse;

namespace MedievalBiotech
{
    [HarmonyPatch(typeof(Building_GeneExtractor), "CanAcceptPawn")]
    public static class Building_GeneExtractor_CanAcceptPawn_Patch
    {
        public static bool Prefix(Building_GeneExtractor __instance, ref AcceptanceReport __result, Pawn pawn)
        {
            __result = CanAcceptPawn(__instance, pawn);
            return false;
        }

        public static AcceptanceReport CanAcceptPawn(Building_GeneExtractor __instance, Pawn pawn)
        {
            if (!pawn.IsColonist && !pawn.IsSlaveOfColony && !pawn.IsPrisonerOfColony && (!pawn.IsColonyMutant || !pawn.IsGhoul))
            {
                return false;
            }
            if (__instance.selectedPawn != null && __instance.selectedPawn != pawn)
            {
                return false;
            }
            if (!pawn.RaceProps.Humanlike || pawn.IsQuestLodger())
            {
                return false;
            }
            if (__instance.innerContainer.Count > 0)
            {
                return "Occupied".Translate();
            }
            if (!pawn.genes?.GenesListForReading.Any(x => x.def.passOnDirectly) ?? true)
            {
                return "PawnHasNoGenes".Translate(pawn.Named("PAWN"));
            }
            if (!pawn.genes.GenesListForReading.Any(x => x.def.biostatArc == 0))
            {
                return "PawnHasNoNonArchiteGenes".Translate(pawn.Named("PAWN"));
            }
            if (pawn.health.hediffSet.HasHediff(HediffDefOf.XenogerminationComa))
            {
                return "InXenogerminationComa".Translate();
            }
            return true;
        }
    }
}
