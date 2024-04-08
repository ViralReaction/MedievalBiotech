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
            if (__instance != null)
            {
                if (__instance.boundPawn != null)
                {
                    var extension = __instance.parent.def.GetModExtension<Deathrest_SoulOffset>();
                    if (extension != null && extension.soulGainOffset != null)
                    {
                        Pawn_GeneTracker genes = __instance.boundPawn.genes;
                        Gene_Soul gene_Soul = (genes != null) ? genes.GetFirstGeneOfType<Gene_Soul>() : null;
                        if (gene_Soul != null)
                        {
                            gene_Soul.SetMax(gene_Soul.Max + extension.soulGainOffset);
                        }
                    }
                }
            }
        }
    }
}
