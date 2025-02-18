using HarmonyLib;
using RimWorld;
using Verse;

namespace MedievalBiotech
{
    [HarmonyPatch(typeof(Building_GeneExtractor), "TryAcceptPawn")]
    public static class Building_GeneExtractor_TryAcceptPawn_Patch
    {
        public static bool Prefix(Building_GeneExtractor __instance, Pawn pawn)
        {
            TryAcceptPawn(__instance, pawn);
            return false;
        }

        public static void TryAcceptPawn(Building_GeneExtractor __instance, Pawn pawn)
        {
            if ((bool)__instance.CanAcceptPawn(pawn))
            {
                __instance.selectedPawn = pawn;
                bool num = pawn.Corpse != null ? pawn.Corpse.DeSpawnOrDeselect() : pawn.DeSpawnOrDeselect();
                if (__instance.innerContainer.TryAddOrTransfer(pawn.Corpse != null ? (Thing)pawn.Corpse : pawn))
                {
                    __instance.startTick = Find.TickManager.TicksGame;
                    __instance.ticksRemaining = 30000;
                }
                if (num)
                {
                    Find.Selector.Select(pawn.Corpse != null ? (Thing)pawn.Corpse : pawn, playSound: false, forceDesignatorDeselect: false);
                }
            }
        }
    }
}
