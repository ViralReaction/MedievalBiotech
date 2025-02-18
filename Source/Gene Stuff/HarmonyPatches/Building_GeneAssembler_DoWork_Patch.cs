using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MedievalBiotech
{
    [HarmonyPatch(typeof(Building_GeneAssembler), "DoWork")]
    public static class Building_GeneAssembler_DoWork_Patch
    {
        public static Dictionary<Building_GeneAssembler, IntermittentSteamSprayer> sprayers = new Dictionary<Building_GeneAssembler, IntermittentSteamSprayer>();
        public static void Prefix(Building_GeneAssembler __instance)
        {
            if (!sprayers.TryGetValue(__instance, out var sprayer))
            {
                sprayers[__instance] = sprayer = new IntermittentSteamSprayer(__instance);
            }
            sprayer.SteamSprayerTick();
        }
    }

    public class IntermittentSteamSprayer
    {
        private Thing parent;

        private int ticksUntilSpray = 500;

        private int sprayTicksLeft;
        public IntermittentSteamSprayer(Thing parent)
        {
            this.parent = parent;
        }

        public void SteamSprayerTick()
        {
            if (sprayTicksLeft > 0)
            {
                sprayTicksLeft--;
                if (Rand.Value < 0.6f)
                {
                    FleckMaker.ThrowAirPuffUp(parent.TrueCenter() + new UnityEngine.Vector3(0, 0, 1), parent.Map);
                }
                if (Find.TickManager.TicksGame % 20 == 0)
                {
                    GenTemperature.PushHeat(parent, 40f);
                }
                if (sprayTicksLeft <= 0)
                {
                    ticksUntilSpray = Rand.RangeInclusive(500, 2000);
                }
                return;
            }
            ticksUntilSpray--;
            if (ticksUntilSpray <= 0)
            {
                sprayTicksLeft = Rand.RangeInclusive(200, 500);
            }
        }
    }

    public class SteamSprayer : ThingComp
    {
        public IntermittentSteamSprayer steamSprayer;
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
        }
    }
}
