using HarmonyLib;
using RimWorld;
using Verse;

namespace MedievalBiotech
{
    [HarmonyPatch(typeof(Building_GeneExtractor), "ContainedPawn", MethodType.Getter)]
    public static class Building_GeneExtractor_ContainedPawn_Patch
    {
        public static bool Prefix(Building_GeneExtractor __instance, ref Pawn __result)
        {
            Thing firstThing = null;
            if (__instance.innerContainer != null)
            {
                foreach (var thing in __instance.innerContainer)
                {
                    firstThing = thing;
                    break;
                }
            }
            if (firstThing != null)
            {
                if (firstThing is Corpse corpse)
                {
                    __result = corpse.InnerPawn;
                }
                else if (firstThing is Pawn pawn)
                {
                    __result = pawn;
                }
            }
            return false;
        }
    }
}
