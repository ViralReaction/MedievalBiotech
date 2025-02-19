using RimWorld;
using Verse.Sound;
using Verse;
using UnityEngine;
using PipeSystem;
using System.Collections.Generic;
using System;
using Verse.AI.Group;

namespace MedievalBiotech
{
    public class Building_SteamGestator : Building_MechGestator
    {
        public CompResourceTrader SteamPower => this.TryGetComp<CompResourceTrader>();

        public override void Tick()
        {
            this.innerContainer.ThingOwnerTick(true);
            bool activeBill = false;
            var state = this.ActiveBill.State;
            if (this.ActiveBill != null && this.SteamPower.ResourceOn && this.BoundPawnStateAllowsForming)
            {
                this.activeBill.BillTick();
                ThingDef thingDef = null;
                
                if (state == FormingState.Forming)
                {
                    activeBill = true;
                    thingDef = this.def.building.gestatorFormingMote.GetForRotation(base.Rotation);
                }
                else if (state == FormingState.Preparing && this.ActiveMechBill.GestationCyclesCompleted > 0)
                {

                    thingDef = this.def.building.gestatorCycleCompleteMote.GetForRotation(base.Rotation);
                }
                else if (state == FormingState.Formed)
                {
                    this.SteamPower.UsedLastTick = false;
                    thingDef = this.def.building.gestatorFormedMote.GetForRotation(base.Rotation);
                }
                if (thingDef != null)
                {
                    if (this.workingMote == null || this.workingMote.Destroyed || this.workingMote.def != thingDef)
                    {
                        this.workingMote = MoteMaker.MakeAttachedOverlay(this, thingDef, Vector3.zero, 1f, -1f);
                    }
                    this.workingMote.Maintain();
                }

            }
            if (activeBill)
            {
                this.SteamPower.Notify_UsedThisTick();
            }
            if (this.activeBill != null && this.SteamPower.ResourceOn && state != FormingState.Gathering)
            {
                if (this.workingSound == null || this.workingSound.Ended)
                {
                    this.workingSound = SoundDefOf.MechGestator_Ambience.TrySpawnSustainer(this);
                }
                this.workingSound.Maintain();
                return;
            }
            if (this.workingSound != null)
            {
                this.workingSound.End();
                this.workingSound = null;
            }
        }

        public override string GetInspectStringExtra()
        {
            Bill_Mech bill_Mech = this.activeBill as Bill_Mech;
            if (bill_Mech == null || bill_Mech.State != FormingState.Forming)
            {
                return null;
            }
            return string.Format("{0}: {1}", "GestatingInspect".Translate(), Mathf.CeilToInt(bill_Mech.formingTicks * 1f / bill_Mech.WorkSpeedMultiplier).ToStringTicksToPeriod(true, false, true, true, false));
        }

        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            base.DrawAt(drawLoc, flip);
            if (this.activeBill != null && this.activeBill.State != FormingState.Gathering && this.def.building.formingGraphicData != null)
            {
                Vector3 loc = drawLoc + this.def.building.formingMechPerRotationOffset[base.Rotation.AsInt];
                loc.y += 0.01923077f;
                loc.z += Mathf.PingPong((float)Find.TickManager.TicksGame * this.def.building.formingMechBobSpeed, this.def.building.formingMechYBobDistance);
                Graphic graphic;
                if (this.TryGetMechFormingGraphic(out graphic))
                {
                    graphic.Draw(loc, Rot4.South, this, 0f);
                }
                else
                {
                    this.def.building.formingGraphicData.Graphic.Draw(loc, Rot4.North, this, 0f);
                }
            }
            GenDraw.FillableBarRequest barDrawData = base.BarDrawData;
            barDrawData.center = drawLoc;
            barDrawData.fillPercent = base.CurrentBillFormingPercent;
            barDrawData.filledMat = Building_MechGestator.FormingCycleBarFilledMat;
            barDrawData.unfilledMat = Building_MechGestator.FormingCycleUnfilledMat;
            barDrawData.rotation = base.Rotation;
            GenDraw.DrawFillableBar(barDrawData);
            if (this.topGraphic == null)
            {
                this.topGraphic = this.def.building.mechGestatorTopGraphic.GraphicColoredFor(this);
            }
            if (this.cylinderGraphic == null)
            {
                this.cylinderGraphic = this.def.building.mechGestatorCylinderGraphic.GraphicColoredFor(this);
            }
            Vector3 loc2 = new Vector3(drawLoc.x, AltitudeLayer.BuildingBelowTop.AltitudeFor(), drawLoc.z);
            this.cylinderGraphic.Draw(loc2, base.Rotation, this, 0f);
            Vector3 loc3 = new Vector3(drawLoc.x, AltitudeLayer.BuildingOnTop.AltitudeFor(), drawLoc.z);
            this.topGraphic.Draw(loc3, base.Rotation, this, 0f);
        }


        public override void Notify_FormingCompleted()
        {
            Pawn pawn = this.activeBill.CreateProducts() as Pawn;
            Messages.Message("GestationComplete".Translate() + ": " + pawn.kindDef.LabelCap, this, MessageTypeDefOf.PositiveEvent, true);
            this.innerContainer.ClearAndDestroyContents(DestroyMode.Vanish);
            this.innerContainer.TryAdd(pawn, true);
            SoundDefOf.MechGestatorBill_Completed.PlayOneShot(this);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (this.comps != null)
            {
                for (int i = 0; i < this.comps.Count; i++)
                {
                    foreach (Gizmo gizmo2 in this.comps[i].CompGetGizmosExtra())
                    {
                        yield return gizmo2;
                    }
 
                }
            }
            if (((this.def.BuildableByPlayer && this.def.passability != Traversability.Impassable && !this.def.IsDoor) || this.def.building.forceShowRoomStats) && Gizmo_RoomStats.GetRoomToShowStatsFor(this) != null && Find.Selector.SingleSelectedObject == this)
            {
                yield return new Gizmo_RoomStats(this);
            }
            Gizmo selectMonumentMarkerGizmo = QuestUtility.GetSelectMonumentMarkerGizmo(this);
            if (selectMonumentMarkerGizmo != null)
            {
                yield return selectMonumentMarkerGizmo;
            }
            if (this.def.Minifiable && base.Faction == Faction.OfPlayer)
            {
                yield return InstallationDesignatorDatabase.DesignatorFor(this.def);
            }
            ColorInt? glowerColorOverride = null;
            CompGlower comp;
            if ((comp = base.GetComp<CompGlower>()) != null && comp.HasGlowColorOverride)
            {
                glowerColorOverride = new ColorInt?(comp.GlowColor);
            }
            if (!this.def.building.neverBuildable)
            {
                Command command = BuildCopyCommandUtility.BuildCopyCommand(this.def, base.Stuff, base.StyleSourcePrecept as Precept_Building, this.StyleDef, true, glowerColorOverride);
                if (command != null)
                {
                    yield return command;
                }
            }
            if (base.Faction == Faction.OfPlayer || this.def.building.alwaysShowRelatedBuildCommands)
            {
                foreach (Command command2 in BuildRelatedCommandUtility.RelatedBuildCommands(this.def))
                {
                    yield return command2;
                }
            }
            Lord lord;
            if ((lord = this.GetLord()) != null && lord.CurLordToil != null)
            {
                foreach (Gizmo gizmo2 in lord.CurLordToil.GetBuildingGizmos(this))
                {
                    yield return gizmo2;
                }
            }

            if (DebugSettings.ShowDevGizmos)
            {
                if (this.ActiveMechBill != null && this.ActiveMechBill.State != FormingState.Gathering && this.ActiveMechBill.State != FormingState.Formed)
                {
                    yield return new Command_Action
                    {
                        action = delegate ()
                        {
                            this.ActiveBill.formingTicks -= (float)this.ActiveBill.recipe.formingTicks * 0.25f;
                        },
                        defaultLabel = "DEV: Forming cycle +25%"
                    };
                    yield return new Command_Action
                    {
                        action = delegate ()
                        {
                            this.ActiveBill.formingTicks = 0f;
                        },
                        defaultLabel = "DEV: Complete cycle"
                    };
                    yield return new Command_Action
                    {
                        action = new Action(this.ActiveMechBill.ForceCompleteAllCycles),
                        defaultLabel = "DEV: Complete all cycles"
                    };
                }
            }
            yield break;
        }
    }
}
