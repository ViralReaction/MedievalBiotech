using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;
using System.Reflection;
using Mono.Unix.Native;

namespace MedievalBiotech
{
    [StaticConstructorOnStartup]
    public class Building_SteamCharger : Building_MechCharger
    {
        //private new Pawn currentlyChargingMech;

        //private new float wasteProduced;

        private new int wireExtensionTicks = 70;

        private new CompWasteProducer wasteProducer;

        private new CompThingContainer container;

        private new Sustainer sustainerCharging;

        private new Mote moteCharging;

        private new Mote moteCablePulse;

        public new const float ChargePerDay = 50f;

        private new const float ChargePerTick = 0.00083333335f;

        private new static readonly Material WasteBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.9f, 0.85f, 0.2f));

        private new static readonly Material WasteBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f, 1f));

        private new const int TicksToExtendWire = 70;

        private new const float MinWireExtension = 0.32f;

        private new Material wireMaterial;

        public new CompPowerTrader Power => this.TryGetComp<CompPowerTrader>();

        public new bool IsPowered => Power.PowerOn;

        public new bool IsFullOfWaste
        {
            get
            {
                if (wasteProduced >= (float)WasteProducedPerChargingCycle)
                {
                    return Container.innerContainer.Any;
                }
                return false;
            }
        }

        private new CompWasteProducer WasteProducer
        {
            get
            {
                return null;
            }
        }

        public new CompThingContainer Container
        {
            get
            {
                if (container == null)
                {
                    container = GetComp<CompThingContainer>();
                }
                return container;
            }
        }

        public new GenDraw.FillableBarRequest BarDrawData => def.building.BarDrawDataFor(base.Rotation);

        private new Material WireMaterial
        {
            get
            {
                if (wireMaterial == null)
                {
                    wireMaterial = MaterialPool.MatFrom("Other/BundledWires", ShaderDatabase.Transparent, Color.white);
                }
                return wireMaterial;
            }
        }

        private new bool IsAttachedToMech
        {
            get
            {
                if (currentlyChargingMech != null)
                {
                    return wireExtensionTicks >= 70;
                }
                return false;
            }
        }

        private new int WasteProducedPerChargingCycle => Container.Props.stackLimit;

        private new float WasteProducedPercentFull => wasteProduced / (float)WasteProducedPerChargingCycle;

        private new float WasteProducedPerTick => currentlyChargingMech.GetStatValue(StatDefOf.WastepacksPerRecharge) * (0.00083333335f / currentlyChargingMech.needs.energy.MaxLevel);

        public override void PostPostMake()
        {
            if (!ModLister.CheckBiotech("Mech recharger"))
            {
                Destroy();
            }
            else
            {
                base.PostPostMake();
            }
        }

        public new bool CanPawnChargeCurrently(Pawn pawn)
        {
            if (Power.PowerNet == null)
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



        public new  bool IsCompatibleWithCharger(PawnKindDef kindDef)
        {
            if (kindDef.race.GetModExtension<Custom_Mech> == null)
            {
                return false;
            }
            return IsCompatibleWithCharger(def, kindDef);
        }

        public new static bool IsCompatibleWithCharger(ThingDef chargerDef, PawnKindDef kindDef)
        {
            return IsCompatibleWithCharger(chargerDef, kindDef.race);
        }

        public new static bool IsCompatibleWithCharger(ThingDef chargerDef, ThingDef mechRace)
        {
            if (mechRace.race.IsMechanoid && mechRace.GetCompProperties<CompProperties_OverseerSubject>() != null)
            {
                return chargerDef.building.requiredMechWeightClasses.NotNullAndContains(mechRace.race.mechWeightClass);
            }
            return false;
        }

        public override void Tick()
        {
            base.Tick();
            if (currentlyChargingMech != null && (currentlyChargingMech.CurJobDef != JobDefOf.MechCharge || currentlyChargingMech.CurJob.targetA.Thing != this))
            {
                Log.Warning("Mech did not clean up his charging job properly");
                StopCharging();
            }
            if (currentlyChargingMech != null && Power.PowerOn)
            {
                currentlyChargingMech.needs.energy.CurLevel += 0.00083333335f;
                if (moteCablePulse == null || moteCablePulse.Destroyed)
                {
                    moteCablePulse = MoteMaker.MakeInteractionOverlay(ThingDefOf.Mote_ChargingCablesPulse, this, new TargetInfo(InteractionCell, base.Map));
                }
                moteCablePulse?.Maintain();
            }
            if (currentlyChargingMech != null && Power.PowerOn && IsAttachedToMech)
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
            else if (sustainerCharging != null && (currentlyChargingMech == null || !Power.PowerOn))
            {
                sustainerCharging.End();
                sustainerCharging = null;
            }
            if (wireExtensionTicks < 70)
            {
                wireExtensionTicks++;
            }
        }

        public new void GenerateWastePack()
        {
            
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

        public new void StartCharging(Pawn mech)
        {
            if (ModLister.CheckBiotech("Mech charging"))
            {
                if (currentlyChargingMech != null)
                {
                    Log.Error("Tried charging on already charging mech charger!");
                    return;
                }
                currentlyChargingMech = mech;
                mech.needs.energy.currentCharger = this;
                wireExtensionTicks = 0;
                SoundDefOf.MechChargerStart.PlayOneShot(this);
            }
        }

        public new void StopCharging()
        {
            if (currentlyChargingMech == null)
            {
                Log.Error("Tried stopping charging on currently not charging mech charger!");
                return;
            }
            if (currentlyChargingMech.needs.energy != null)
            {
                currentlyChargingMech.needs.energy.currentCharger = null;
            }
            currentlyChargingMech = null;
            wireExtensionTicks = 0;
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

        private new float EaseInOutQuart(float val)
        {
            if (!((double)val < 0.5))
            {
                return 1f - Mathf.Pow(-2f * val + 2f, 4f) / 2f;
            }
            return 8f * val * val * val * val;
        }

        public override string GetInspectString()
        {
           // StringBuilder stringBuilder = new StringBuilder();
            //stringBuilder.Append(base.GetInspectString());
            return base.GetInspectString();
        }

        public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
        {
            IEnumerable<PawnKindDef> source = DefDatabase<PawnKindDef>.AllDefs.Where((PawnKindDef pk) => IsCompatibleWithCharger(pk));
            string text = source.Select((PawnKindDef pk) => pk.LabelCap.Resolve()).ToLineList(" - ");
            yield return new StatDrawEntry(StatCategoryDefOf.Basics, "StatsReport_RechargerWeightClass".Translate(), def.building.requiredMechWeightClasses.Select((MechWeightClass w) => w.ToStringHuman()).ToCommaList().CapitalizeFirst(), "StatsReport_RechargerWeightClass_Desc".Translate() + ": " + "\n\n" + text, 99999, null, source.Select((PawnKindDef pk) => new Dialog_InfoCard.Hyperlink(pk.race)));
        }

    }
}