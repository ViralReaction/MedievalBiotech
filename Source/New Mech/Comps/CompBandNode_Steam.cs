using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;
using PipeSystem;

namespace MedievalBiotech
{
    [StaticConstructorOnStartup]
    public class CompBandNode_Steam : ThingComp
    {
        private CompResourceTrader_Steam SteamTrader
        {
            get
            {
                return this.parent.TryGetComp<CompResourceTrader_Steam>();
            }
        }

        public CompProperties_BandNode Props
        {
            get
            {
                return (CompProperties_BandNode)this.props;
            }
        }

        private int RetuneTimeTicks
        {
            get
            {
                return (int)(60000f * this.Props.retuneDays);
            }
        }

        private int TuningTimeTicks
        {
            get
            {
                return (int)(this.Props.tuneSeconds * 60f);
            }
        }

        private BandNodeState State
        {
            get
            {
                if (this.tunedTo != null && this.tuningTo != null)
                {
                    return BandNodeState.Retuning;
                }
                if (this.tuningTo != null)
                {
                    return BandNodeState.Tuning;
                }
                if (this.tunedTo != null)
                {
                    return BandNodeState.Tuned;
                }
                return BandNodeState.Untuned;
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (!ModLister.CheckBiotech("Band node"))
            {
                this.parent.Destroy(DestroyMode.Vanish);
                return;
            }
            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override void PostExposeData()
        {
            Scribe_References.Look<Pawn>(ref this.tunedTo, "tunedTo", false);
            Scribe_References.Look<Pawn>(ref this.tuningTo, "tuningTo", false);
            Scribe_Values.Look<int>(ref this.tuningTimeLeft, "tuningTimeLeft", 0, false);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            Command_Action command_Action = new Command_Action
            {
                defaultLabel = (this.tunedTo == null) ? "BandNodeTuneTo".Translate() + "..." : "BandNodeRetuneTo".Translate() + "...",
                defaultDesc = (this.tunedTo == null)
                    ? "BandNodeTuningDesc".Translate("PeriodSeconds".Translate(this.Props.tuneSeconds))
                    : "BandNodeRetuningDesc".Translate(this.Props.retuneDays + " " + "Days".Translate()),
                icon = ContentFinder<Texture2D>.Get("UI/Gizmos/BandNodeTuning", true)
            };

            command_Action.onHover += delegate ()
            {
                Pawn pawn2 = (this.tuningTo != null) ? this.tuningTo : this.tunedTo;
                if (pawn2 != null)
                {
                    GenDraw.DrawLineBetween(this.parent.DrawPos, pawn2.DrawPos);
                }
            };

            bool flag = false;
            foreach (Pawn pawn in this.parent.Map.mapPawns.AllPawnsSpawned)
            {
                if (MechanitorUtility.IsMechanitor(pawn) && this.tunedTo != pawn && this.tuningTo != pawn)
                {
                    flag = true;
                    break;
                }
            }
            command_Action.Disabled = !flag;

            command_Action.action += delegate ()
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();

                foreach (Pawn pawn in this.parent.Map.mapPawns.AllPawnsSpawned)
                {
                    if (MechanitorUtility.IsMechanitor(pawn))
                    {
                        List<CompBandNode_Steam> bandNodes = new List<CompBandNode_Steam>();

                        foreach (object obj in Find.Selector.SelectedObjects)
                        {
                            Thing thing = obj as Thing;
                            if (thing != null)
                            {
                                CompBandNode_Steam bandNode = thing.TryGetComp<CompBandNode_Steam>();
                                if (bandNode != null && bandNode.tunedTo != pawn && bandNode.tuningTo != pawn)
                                {
                                    bandNodes.Add(bandNode);
                                }
                            }
                        }

                        if (bandNodes.Count > 0)
                        {
                            string text = pawn.Name.ToStringFull;
                            bool allUntuned = true;
                            bool allTuned = true;

                            foreach (CompBandNode_Steam b in bandNodes)
                            {
                                if (b.tunedTo != null)
                                {
                                    allUntuned = false;
                                }
                                else
                                {
                                    allTuned = false;
                                }
                            }

                            if (!allUntuned && !allTuned)
                            {
                                continue;
                            }

                            if (this.tunedTo == null)
                            {
                                text += " (" + this.Props.tuneSeconds + " " + "SecondsLower".Translate() + ")";
                            }
                            else
                            {
                                text += " (" + this.RetuneTimeTicks.ToStringTicksToPeriod(true, false, true, true, false) + ")";
                            }

                            Pawn localPawn = pawn;
                            list.Add(new FloatMenuOption(text, delegate ()
                            {
                                foreach (CompBandNode_Steam compBandNode in bandNodes)
                                {
                                    compBandNode.TuneTo(localPawn);
                                }
                            }, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
                        }
                    }
                }
                Find.WindowStack.Add(new FloatMenu(list));
            };

            yield return command_Action;

            if (DebugSettings.ShowDevGizmos)
            {
                yield return new Command_Action
                {
                    defaultLabel = "DEV: complete tuning",
                    action = delegate ()
                    {
                        this.tuningTimeLeft = 0;
                    }
                };
            }
        }


        public void TuneTo(Pawn pawn)
        {
            this.tuningTimeLeft = ((this.tunedTo == null) ? this.TuningTimeTicks : this.RetuneTimeTicks);
            this.tuningTo = pawn;
        }

        public override void PostDraw()
        {
            base.PostDraw();
            BandNodeState state = this.State;
            Material material;
            if (state != BandNodeState.Untuned)
            {
                if (state == BandNodeState.Tuned)
                {
                    material = CompBandNode_Steam.TunedMaterial;
                }
                else
                {
                    material = CompBandNode_Steam.TuningMaterial;
                }
            }
            else
            {
                material = CompBandNode_Steam.UntunedMaterial;
            }
            Vector3 s = new Vector3(CompBandNode_Steam.TuningBarSize.x, 1f, CompBandNode_Steam.TuningBarSize.y);
            Vector3 pos = this.parent.DrawPos + new Vector3(0f, 0f, -0.4f);
            pos.y = this.parent.def.altitudeLayer.AltitudeFor() + 0.03846154f;
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(pos, this.parent.Rotation.AsQuat, s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, material, 0);
        }

        public override void CompTick()
        {
            if (this.tunedTo != null || this.tuningTo != null)
            {
                SteamTrader.UsedLastTick = true;
            }
            if (this.tunedTo != null && this.tunedTo.Dead)
            {
                this.tunedTo = null;
            }
            if (this.tuningTo != null && this.tuningTo.Dead)
            {
                this.tuningTo = null;
            }
            if (this.SteamTrader != null && !this.SteamTrader.UsedLastTick)
            {
                Effecter effecter = this.effecter;
                if (effecter != null)
                {
                    effecter.Cleanup();
                }
                this.effecter = null;
                return;
            }
            if (this.tuningTo != null)
            {
                this.tuningTimeLeft--;
                if (this.tuningTimeLeft <= 0)
                {
                    this.tunedTo = this.tuningTo;
                    this.tuningTo = null;
                    if (this.Props.tuningCompleteSound != null)
                    {
                        this.Props.tuningCompleteSound.PlayOneShot(this.parent);
                    }
                }
            }
            if (this.tuningTo == null && this.tunedTo != null && !this.tunedTo.health.hediffSet.HasHediff(this.Props.hediff, false))
            {
                this.tunedTo.health.AddHediff(this.Props.hediff, this.tunedTo.health.hediffSet.GetBrain(), null, null);
            }
            if (this.State == BandNodeState.Untuned)
            {
                if (this.effecter == null || this.effecter.def != this.Props.untunedEffect)
                {
                    Effecter effecter2 = this.effecter;
                    if (effecter2 != null)
                    {
                        effecter2.Cleanup();
                    }
                    this.effecter = this.Props.untunedEffect.Spawn();
                }
            }
            else if (this.State == BandNodeState.Tuned)
            {
                if (this.effecter == null || this.effecter.def != this.Props.tunedEffect)
                {
                    Effecter effecter3 = this.effecter;
                    if (effecter3 != null)
                    {
                        effecter3.Cleanup();
                    }
                    this.effecter = this.Props.tunedEffect.Spawn();
                }
            }
            else if (this.State == BandNodeState.Tuning)
            {
                if (this.effecter == null || this.effecter.def != this.Props.tuningEffect)
                {
                    Effecter effecter4 = this.effecter;
                    if (effecter4 != null)
                    {
                        effecter4.Cleanup();
                    }
                    this.effecter = this.Props.tuningEffect.Spawn();
                }
            }
            else if (this.State == BandNodeState.Retuning)
            {
                if (this.effecter == null || this.effecter.def != this.Props.retuningEffect)
                {
                    Effecter effecter5 = this.effecter;
                    if (effecter5 != null)
                    {
                        effecter5.Cleanup();
                    }
                    this.effecter = this.Props.retuningEffect.Spawn();
                }
            }
            else
            {
                Effecter effecter6 = this.effecter;
                if (effecter6 != null)
                {
                    effecter6.Cleanup();
                }
                this.effecter = null;
            }
            if (this.effecter != null)
            {
                this.effecter.EffectTick(this.parent, this.parent);
            }
        }

        public override string CompInspectStringExtra()
        {
            string t = null;
            if (this.tuningTo != null)
            {
                return "BandNodeTuningTo".Translate() + ": " + this.tuningTo.Name.ToStringFull + " - " + this.tuningTimeLeft.ToStringTicksToPeriod(true, false, true, true, false) + t;
            }
            return "BandNodeTunedTo".Translate() + ": " + ((this.tunedTo == null) ? "Nobody".Translate().Resolve() : this.tunedTo.Name.ToStringFull) + t;
        }

        private static readonly Material UntunedMaterial = SolidColorMaterials.SimpleSolidColorMaterial(Color.red, false);

        private static readonly Material TuningMaterial = SolidColorMaterials.SimpleSolidColorMaterial(Color.yellow, false);

        private static readonly Material TunedMaterial = SolidColorMaterials.SimpleSolidColorMaterial(Color.green, false);

        private static readonly Vector2 TuningBarSize = new Vector3(0.255f, 0.035f);

        private const float TuningBarYOffset = -0.4f;

        public Pawn tunedTo;

        public int tuningTimeLeft;

        public Pawn tuningTo;

        private Effecter effecter;
    }
}
