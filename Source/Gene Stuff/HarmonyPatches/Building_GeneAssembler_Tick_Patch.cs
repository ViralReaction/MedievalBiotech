using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using System.Linq;

namespace MedievalBiotech
{

    [HarmonyPatch(typeof(Building_GeneAssembler), "Tick")]
    public static class Building_GeneAssembler_Tick_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            var codes = codeInstructions.ToList();
            var isHashOffset = AccessTools.Method(typeof(Gen), nameof(Gen.IsHashIntervalTick), new Type[] { typeof(Thing), typeof(int) });
            for (var i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                if (code.Calls(isHashOffset) && codes[i - 1].OperandIs(250))
                {
                    code.operand = AccessTools.Method(typeof(Building_GeneAssembler_Tick_Patch), nameof(AlwaysFalse));
                }
                yield return code;
            }
        }

        public static bool AlwaysFalse(Thing thing, int hash)
        {
            return false;
        }
    }
 
}
