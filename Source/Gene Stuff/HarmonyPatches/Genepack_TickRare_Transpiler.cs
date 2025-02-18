using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace MedievalBiotech
{
    [HarmonyPatch(typeof(Genepack), "TickRare")]
    public static class Genepack_TickRare_Transpiler
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            var codes = codeInstructions.ToList();
            var get_ParentContainerInfo = AccessTools.Method(typeof(Genepack), "get_ParentContainer");
            Label? label = null;
            for (var i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                if (code.opcode == OpCodes.Brfalse && codes[i - 1].Calls(get_ParentContainerInfo))
                {
                    label = (Label)code.operand;
                    yield return code;
                }
                else if (label != null && code.labels.Contains(label.Value))
                {
                    label = null;
                }
                if (label is null)
                {
                    yield return code;
                }
            }
        }

        public static void Postfix(Genepack __instance)
        {
            if (__instance.Deteriorating)
            {
                __instance.deteriorationPct += 1f / (5f * 60000f) * 250f;
                __instance.deteriorationPct = Mathf.Clamp01(__instance.deteriorationPct);
            }
            else
            {
                __instance.deteriorationPct = 0;
            }
        }
    }
}
