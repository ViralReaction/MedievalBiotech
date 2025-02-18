using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MedievalBiotech
{

    // Below needs to be fixed. Currently commented out until such time.
    //[HarmonyPatch(typeof(Building_GeneExtractor), "DynamicDrawPhaseAt")]
    //public static class Building_GeneExtractor_Draw_Patch
    //{
    //    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
    //    {
    //        var codes = codeInstructions.ToList();
    //        for (var i = 0; i < codes.Count; i++)
    //        {
    //            if (i > 1)
    //            {
    //                yield return new CodeInstruction(OpCodes.Ldarg_0);
    //                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Building_GeneExtractor_Draw_Patch), nameof(DrawCustom)));
    //                yield return new CodeInstruction(OpCodes.Ret);
    //                yield break;
    //            }
    //            else
    //            {
    //                yield return codes[i];
    //            }
    //        }
    //    }

    //    public static void DrawCustom(Building_GeneExtractor building_GeneExtractor)
    //    {
    //        if (building_GeneExtractor.Working && building_GeneExtractor.selectedPawn != null && ContainsCorpseOrPawn(building_GeneExtractor.innerContainer, building_GeneExtractor.selectedPawn))
    //        {
    //            var oldHoldingOwner = building_GeneExtractor.selectedPawn.holdingOwner;
    //            if (building_GeneExtractor.selectedPawn.Corpse != null)
    //            {
    //                var newHoldingOwner = building_GeneExtractor.selectedPawn.Corpse.holdingOwner;
    //                building_GeneExtractor.selectedPawn.holdingOwner = newHoldingOwner;
    //            }
    //            building_GeneExtractor.selectedPawn.Drawer.renderer.RenderPawnAt(building_GeneExtractor.DrawPos + building_GeneExtractor.PawnDrawOffset, null, neverAimWeapon: true);
    //            building_GeneExtractor.selectedPawn.holdingOwner = oldHoldingOwner;
    //        }
    //    }

    //    public static bool ContainsCorpseOrPawn(ThingOwner innerContainer, Pawn selectedPawn)
    //    {
    //        foreach (var thing in innerContainer)
    //        {
    //            if (thing is Corpse corpse && corpse.InnerPawn == selectedPawn)
    //            {
    //                return true;
    //            }
    //            else if (thing is Pawn pawn && pawn == selectedPawn)
    //            {
    //                return true;
    //            }
    //        }
    //        return false;
    //    }
    //}

    
}
