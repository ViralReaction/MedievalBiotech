using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace MedievalBiotech
{
    [HarmonyPatch(typeof(CompDeathrestBindable), "Apply")]
    public static class CompDeathrestBindable_Apply_Patch
    {
        public static void Postfix(CompDeathrestBindable __instance)
        {
            if (__instance.boundPawn == null)
            {
                return;
            }
            var extension = __instance.parent.def.GetModExtension<Deathrest_SoulOffset>();
            if (extension?.soulGainOffset == null)
            {
                return;
            }
            Gene_Soul gene_Soul = __instance.boundPawn.genes?.GetFirstGeneOfType<Gene_Soul>();
            if (gene_Soul != null)
            {
                gene_Soul.SetMax(gene_Soul.Max + extension.soulGainOffset);
            }
        }
    }
}
