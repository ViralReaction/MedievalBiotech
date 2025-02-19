using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;
using PipeSystem;
using System.Text;

namespace MedievalBiotech
{
    [StaticConstructorOnStartup]
    public class Building_SteamCharger : Building_MechCharger
    {
        public new CompResourceTrader Power => this.TryGetComp<CompResourceTrader>();

        public new bool IsPowered => Power.ResourceOn;

        private new CompWasteProducer WasteProducer => null;

        public RechargerMapComponent chargerMapComp;



        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            chargerMapComp = this.Map.GetComponent<RechargerMapComponent>();
            chargerMapComp.RegisterCharger(this);
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            chargerMapComp.RemoveCharger(this);
            base.DeSpawn(mode);
        }

        public override void Tick()
        {
            // Tick all components
            if (this.comps != null)
            {
                for (int i = 0, count = this.comps.Count; i < count; i++)
                {
                    this.comps[i].CompTick();
                }
            }
            bool hasChargingMech = currentlyChargingMech != null;
            bool powerOn = Power.ResourceOn;

            if (hasChargingMech && (currentlyChargingMech.CurJobDef != JobDefOf.MechCharge || currentlyChargingMech.CurJob.targetA.Thing != this))
            {
                Log.Warning("Mech did not clean up its charging job properly");
                StopCharging();
            }
            if (hasChargingMech && powerOn)
            {
                Power.UsedLastTick = true;
                currentlyChargingMech.needs.energy.CurLevel += 0.00083333335f;

                if (moteCablePulse == null || moteCablePulse.Destroyed)
                {
                    moteCablePulse = MoteMaker.MakeInteractionOverlay(ThingDefOf.Mote_ChargingCablesPulse, this, new TargetInfo(InteractionCell, base.Map));
                }
                moteCablePulse?.Maintain();

                if (IsAttachedToMech)
                {
                    if (sustainerCharging == null)
                    {
                        sustainerCharging = SoundDefOf.MechChargerCharging.TrySpawnSustainer(SoundInfo.InMap(this));
                    }
                    sustainerCharging.Maintain();

                    if (moteCharging == null || moteCharging.Destroyed)
                    {
                        moteCharging = MoteMaker.MakeAttachedOverlay(currentlyChargingMech, ThingDefOf.Mote_MechCharging, Vector3.zero);
                    }
                    moteCharging?.Maintain();
                }
            }
            else
            {
                Power.UsedLastTick = false;
                if (sustainerCharging != null)
                {
                    sustainerCharging.End();
                    sustainerCharging = null;
                }
            }
            if (wireExtensionTicks < 70)
            {
                wireExtensionTicks++;
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            if (!DebugSettings.ShowDevGizmos)
            {
                yield break;
            }
            if (currentlyChargingMech != null)
            {
                Command_Action command_Action5 = new Command_Action();
                command_Action5.action = delegate
                {
                    currentlyChargingMech.needs.TryGetNeed<Need_MechEnergy>().CurLevelPercentage = 1f;
                };
                command_Action5.defaultLabel = "DEV: Charge 100%";
                yield return command_Action5;
            }
        }
        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            base.DrawAt(drawLoc, flip);
            GenDraw.FillableBarRequest barDrawData = BarDrawData;
            barDrawData.center = DrawPos + Vector3.up * 0.1f;
            barDrawData.fillPercent = WasteProducedPercentFull;
            barDrawData.filledMat = WasteBarFilledMat;
            barDrawData.unfilledMat = WasteBarUnfilledMat;
            barDrawData.rotation = base.Rotation;
            GenDraw.DrawFillableBar(barDrawData);
            Vector3 a = drawLoc;
            float num = EaseInOutQuart((float)wireExtensionTicks / 70f);
            if (currentlyChargingMech == null)
            {
                num = 1f - num;
            }
            num = Mathf.Max(num, 0.32f);
            Vector3 b = Vector3.Lerp(drawLoc, InteractionCell.ToVector3Shifted(), num);
            b.y = AltitudeLayer.BuildingOnTop.AltitudeFor();
            a.y = AltitudeLayer.BuildingOnTop.AltitudeFor();
            GenDraw.DrawLineBetween(a, b, WireMaterial, 1f);
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (this.currentlyChargingMech != null)
            {
                stringBuilder.Append("MB_SteamCharger_CurrentlyCharging".Translate(this.currentlyChargingMech.Label));
            }
            string text = this.InspectStringPartsFromComps();
            if (!text.NullOrEmpty())
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.AppendLine();
                }
                stringBuilder.Append(text);
            }
            return stringBuilder.ToString();
        }

        public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
        {
            IEnumerable<PawnKindDef> source = DefDatabase<PawnKindDef>.AllDefs.Where((PawnKindDef pk) => IsCompatibleWithCharger(pk));
            string text = source.Select((PawnKindDef pk) => pk.LabelCap.Resolve()).ToLineList(" - ");
            yield return new StatDrawEntry(StatCategoryDefOf.Basics, "StatsReport_RechargerWeightClass".Translate(), def.building.requiredMechWeightClasses.Select((MechWeightClass w) => w.ToStringHuman()).ToCommaList().CapitalizeFirst(), "StatsReport_RechargerWeightClass_Desc".Translate() + ": " + "\n\n" + text, 99999, null, source.Select((PawnKindDef pk) => new Dialog_InfoCard.Hyperlink(pk.race)));
        }

        public bool CanPawnChargeCurrentlySteam(Pawn pawn)
        {
            if (pawn == null)
            {
                return false;
            }


            if (!IsCompatibleWithCharger(pawn.kindDef))
            {
                return false;
            }

            if (IsPowered)
            {

                if (currentlyChargingMech == null)
                {
                    return true;
                }

                if (currentlyChargingMech == pawn)
                {
                    return true;
                }

            }

            return false;
        }

        public bool IsCompatibleWithSteamCharger(PawnKindDef kindDef)
        {
            var extension = kindDef.race.GetModExtension<Custom_Mech>();
            if (extension == null)
            {
                return false;
            }

            ThingDef mechRace = kindDef.race;
            return mechRace.race.IsMechanoid
                && mechRace.GetCompProperties<CompProperties_OverseerSubject>() != null
                && this.def.building.requiredMechWeightClasses.NotNullAndContains(mechRace.race.mechWeightClass);
        }

        

    }
}