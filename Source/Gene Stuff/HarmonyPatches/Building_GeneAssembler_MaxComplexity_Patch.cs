﻿using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MedievalBiotech
{
    [HarmonyPatch(typeof(Building_GeneAssembler), "MaxComplexity")]
    public static class Building_GeneAssembler_MaxComplexity_Patch
    {
        public static bool Prefix(Building_GeneAssembler __instance, ref int __result)
        {
            __result = MaxComplexity(__instance);
            return false;
        }

        public static int MaxComplexity(Building_GeneAssembler __instance)
        {
            int num = 6;
            foreach (Thing item in __instance.ConnectedFacilities)
            {
                CompRefuelable compRefuelable = item.TryGetComp<CompRefuelable>();
                if (compRefuelable == null || compRefuelable.HasFuel)
                {
                    num += (int)item.GetStatValue(StatDefOf.GeneticComplexityIncrease);
                }
            }
            return num;
        }
    }
}
