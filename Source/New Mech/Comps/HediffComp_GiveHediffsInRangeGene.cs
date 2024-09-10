using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace MedievalBiotech
{
    public class HediffComp_GiveHediffsInRangeUndead : HediffComp
    {
        public HediffCompProperties_GiveHediffsInRangeUndead Props
        {
            get
            {
                return (HediffCompProperties_GiveHediffsInRangeUndead)this.props;
            }
        }
        public override void CompPostTick(ref float severityAdjustment)
        {
            if (!this.parent.pawn.Awake() || this.parent.pawn.health == null || this.parent.pawn.health.InPainShock || !this.parent.pawn.Spawned)
            {
                return;
            }
            if (!this.Props.hideMoteWhenNotDrafted || this.parent.pawn.Drafted)
            {
                if (this.Props.mote != null && (this.mote == null || this.mote.Destroyed))
                {
                    this.mote = MoteMaker.MakeAttachedOverlay(this.parent.pawn, this.Props.mote, Vector3.zero, 1f, -1f);
                }
                if (this.mote != null)
                {
                    this.mote.Maintain();
                }
            }
            IReadOnlyList<Pawn> readOnlyList;
            if (this.Props.onlyPawnsInSameFaction && this.parent.pawn.Faction != null)
            {
                readOnlyList = this.parent.pawn.Map.mapPawns.SpawnedPawnsInFaction(this.parent.pawn.Faction);
            }
            else
            {
                readOnlyList = this.parent.pawn.Map.mapPawns.AllPawnsSpawned;
            }
            foreach (Pawn pawn in readOnlyList)
            {
                var extension = pawn.def.GetModExtension<Custom_Mech>()?.UndeadMech;
                if ((pawn.RaceProps.Humanlike && pawn.genes.HasGene(Props.geneDef) || (pawn.RaceProps.IsMechanoid && (extension == true))) && !pawn.Dead && pawn.health != null && pawn != this.parent.pawn && pawn.Position.DistanceTo(this.parent.pawn.Position) <= this.Props.range && this.Props.targetingParameters.CanTarget(pawn, null))
                {
                    Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(this.Props.hediff, false);
                    if (hediff == null)
                    {
                        hediff = pawn.health.AddHediff(this.Props.hediff, pawn.health.hediffSet.GetBrain(), null, null);
                        hediff.Severity = this.Props.initialSeverity;
                        HediffComp_Link hediffComp_Link = hediff.TryGetComp<HediffComp_Link>();
                        if (hediffComp_Link != null)
                        {
                            hediffComp_Link.drawConnection = true;
                            hediffComp_Link.other = this.parent.pawn;
                        }
                    }
                    HediffComp_Disappears hediffComp_Disappears = hediff.TryGetComp<HediffComp_Disappears>();
                    if (hediffComp_Disappears == null)
                    {
                        Log.Error("HediffComp_GiveHediffsInRange has a hediff in props which does not have a HediffComp_Disappears");
                    }
                    else
                    {
                        hediffComp_Disappears.ticksToDisappear = 5;
                    }
                }

            }
        }

        private Mote mote;
    }
}
