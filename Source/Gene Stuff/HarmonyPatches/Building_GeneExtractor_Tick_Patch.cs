using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;

namespace MedievalBiotech
{ 
    [HarmonyPatch(typeof(Building_GeneExtractor), "Tick")]
    public static class Building_GeneExtractor_Tick_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            var codes = codeInstructions.ToList();
            var isHashOffset = AccessTools.Method(typeof(Gen), nameof(Gen.IsHashIntervalTick), new Type[] { typeof(Thing), typeof(int) });
            var powerOn = AccessTools.PropertyGetter(typeof(CompPowerTrader), nameof(CompPowerTrader.PowerOn));
            var powerCutField = AccessTools.Field(typeof(Building_GeneExtractor), "powerCutTicks");
            for (var i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                if (code.Calls(isHashOffset))
                {
                    code.operand = AccessTools.Method(typeof(Building_GeneExtractor_Tick_Patch), nameof(AlwaysFalse));
                }
                if (code.Calls(powerOn))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    code.operand = AccessTools.Method(typeof(Building_GeneExtractor_Tick_Patch), nameof(ShouldWork));
                }
                if (code.opcode == OpCodes.Ldc_I4 && code.OperandIs(60000))
                {
                    code.operand = int.MaxValue;
                }
                yield return code;
            }
        }

        public static bool AlwaysFalse(Thing thing, int hash)
        {
            return false;
        }
        public static bool ShouldWork(CompPowerTrader trader, Building_GeneExtractor parent)
        {
            parent.powerCutTicks = 0;
            return false;
        }
    }
}
