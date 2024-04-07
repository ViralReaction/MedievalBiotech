using System;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;

namespace MedievalBiotech
{
    public class CompDeathrestBindable_Soul : ThingComp
    {
        public CompProperties_DeathrestBindable_Soul Props
        {
            get
            {
                return (CompProperties_DeathrestBindable_Soul)this.props;
            }
        }

        public Pawn BoundPawn
        {
            get
            {
                return this.boundPawn;
            }
        }

        public bool CanIncreasePresence
        {
            get
            {
                if (this.PowerTraderComp != null && !this.PowerTraderComp.PowerOn)
                {
                    return false;
                }
                if (this.RefuelableComp != null && !this.RefuelableComp.HasFuel)
                {
                    return false;
                }
                if (!this.boundPawn.InBed())
                {
                    return false;
                }
                Building_Bed building_Bed = this.boundPawn.CurrentBed();
                if (building_Bed == this.parent)
                {
                    return true;
                }
                CompDeathrestBindable_Soul compDeathrestBindable = building_Bed.TryGetComp<CompDeathrestBindable_Soul>();
                return compDeathrestBindable != null && compDeathrestBindable.BoundPawn == this.boundPawn;
            }
        }

        private CompPowerTrader PowerTraderComp
        {
            get
            {
                if (this.cachedPowerComp == null)
                {
                    this.cachedPowerComp = this.parent.TryGetComp<CompPowerTrader>();
                }
                return this.cachedPowerComp;
            }
        }

        private CompRefuelable RefuelableComp
        {
            get
            {
                if (this.cachedRefuelableComp == null)
                {
                    this.cachedRefuelableComp = this.parent.TryGetComp<CompRefuelable>();
                }
                return this.cachedRefuelableComp;
            }
        }

        private Gene_Deathrest DeathrestGene
        {
            get
            {
                if (this.cachedDeathrestGene == null)
                {
                    Pawn pawn = this.boundPawn;
                    Gene_Deathrest gene_Deathrest;
                    if (pawn == null)
                    {
                        gene_Deathrest = null;
                    }
                    else
                    {
                        Pawn_GeneTracker genes = pawn.genes;
                        gene_Deathrest = ((genes != null) ? genes.GetFirstGeneOfType<Gene_Deathrest>() : null);
                    }
                    this.cachedDeathrestGene = gene_Deathrest;
                }
                return this.cachedDeathrestGene;
            }
        }

        private Material HoseMat
        {
            get
            {
                if (this.cachedHoseMat == null)
                {
                    this.cachedHoseMat = MaterialPool.MatFrom("Other/DeathrestBuildingConnection");
                }
                return this.cachedHoseMat;
            }
        }

        public override void PostPostMake()
        {
            if (!ModLister.CheckBiotech("Deathrest binding"))
            {
                this.parent.Destroy(DestroyMode.Vanish);
                return;
            }
            base.PostPostMake();
        }

        public bool CanBindTo(Pawn pawn)
        {
            return (!this.Props.mustBeLayingInToBind || pawn.CurrentBed() == this.parent) && (this.boundPawn == null || this.boundPawn == pawn);
        }

        public void BindTo(Pawn pawn)
        {
            this.boundPawn = pawn;
        }

        public void Notify_DeathrestGeneRemoved()
        {
            this.cachedDeathrestGene = null;
            this.boundPawn = null;
            this.presenceTicks = 0;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (!respawningAfterLoad)
            {
                this.presenceTicks = 0;
                foreach (Pawn pawn in this.parent.Map.mapPawns.PawnsInFaction(this.parent.Faction))
                {
                    if (pawn.Deathresting)
                    {
                        Pawn_GeneTracker genes = pawn.genes;
                        Gene_Deathrest gene_Deathrest = (genes != null) ? genes.GetFirstGeneOfType<Gene_Deathrest>() : null;
                        if (gene_Deathrest != null && gene_Deathrest.CanBindToBindable(this))
                        {
                            gene_Deathrest.BindTo(this);
                        }
                    }
                }
            }
        }

        public override void PostDeSpawn(Map map)
        {
            this.presenceTicks = 0;
            if (this.boundPawn != null)
            {
                Pawn_GeneTracker genes = this.boundPawn.genes;
                Gene_Deathrest gene_Deathrest = (genes != null) ? genes.GetFirstGeneOfType<Gene_Deathrest>() : null;
                if (gene_Deathrest != null)
                {
                    gene_Deathrest.Notify_BoundBuildingDeSpawned(this.parent);
                }
            }
            if (this.sustainer != null && !this.sustainer.Ended)
            {
                this.sustainer.End();
            }
            this.sustainer = null;
        }

        public override void CompTick()
        {
            if (this.parent.IsHashIntervalTick(250))
            {
                this.CompTickRare();
            }
            if (this.presenceTicks > 0)
            {
                if (!this.Props.soundWorking.NullOrUndefined())
                {
                    if (this.sustainer == null || this.sustainer.Ended)
                    {
                        this.sustainer = this.Props.soundWorking.TrySpawnSustainer(SoundInfo.InMap(this.parent, MaintenanceType.None));
                    }
                    this.sustainer.Maintain();
                }
                CompRefuelable refuelableComp = this.RefuelableComp;
                if (refuelableComp == null)
                {
                    return;
                }
                refuelableComp.Notify_UsedThisTick();
            }
        }

        public override void CompTickRare()
        {
            if (this.PowerTraderComp != null)
            {
                this.PowerTraderComp.PowerOutput = ((this.presenceTicks > 0) ? (-this.PowerTraderComp.Props.PowerConsumption) : (-this.PowerTraderComp.Props.idlePowerDraw));
            }
        }

        public override void PostDraw()
        {
            if (!this.Props.mustBeLayingInToBind && this.boundPawn != null && this.boundPawn.Map == this.parent.Map && this.boundPawn.Deathresting && this.DeathrestGene != null && this.DeathrestGene.BoundComps.Contains(this) && this.CanIncreasePresence)
            {
                Vector3 a = this.boundPawn.CurrentBed().TrueCenter();
                Vector3 b = this.parent.TrueCenter();
                a.y = (b.y = AltitudeLayer.SmallWire.AltitudeFor());
                Matrix4x4 identity = Matrix4x4.identity;
                identity.SetTRS((a + b) / 2f, Quaternion.Euler(0f, a.AngleToFlat(b) + 90f, 0f), new Vector3(1f, 1f, (a - b).MagnitudeHorizontal()));
                Graphics.DrawMesh(MeshPool.plane10, identity, this.HoseMat, 0);
            }
        }

        public void Apply()
        {
            if (this.boundPawn != null)
            {
                if (this.Props.hediffToApply != null)
                {
                    this.boundPawn.health.AddHediff(this.Props.hediffToApply, null, null, null);
                }
                if (this.Props.hemogenLimitOffset > 0f)
                {
                    Pawn_GeneTracker genes = this.boundPawn.genes;
                    Gene_Soul gene_Hemogen = (genes != null) ? genes.GetFirstGeneOfType<Gene_Soul>() : null;
                    if (gene_Hemogen != null)
                    {
                        gene_Hemogen.SetMax(gene_Hemogen.Max + this.Props.hemogenLimitOffset);
                    }
                }
            }
        }

        public void TryIncreasePresence()
        {
            if (this.CanIncreasePresence)
            {
                if (this.presenceTicks <= 0)
                {
                    SoundInfo info = SoundInfo.InMap(this.parent, MaintenanceType.None);
                    if (!this.Props.soundWorking.NullOrUndefined() && (this.sustainer == null || this.sustainer.Ended))
                    {
                        this.sustainer = this.Props.soundWorking.TrySpawnSustainer(info);
                    }
                    if (!this.Props.soundStart.NullOrUndefined())
                    {
                        this.Props.soundStart.PlayOneShot(info);
                    }
                }
                this.presenceTicks++;
            }
        }

        public void Notify_DeathrestEnded()
        {
            this.presenceTicks = 0;
            if (this.parent.Spawned)
            {
                if (this.sustainer != null && !this.sustainer.Ended)
                {
                    this.sustainer.End();
                }
                if (!this.Props.soundEnd.NullOrUndefined())
                {
                    this.Props.soundEnd.PlayOneShot(SoundInfo.InMap(this.parent, MaintenanceType.None));
                }
            }
        }

        public override string CompInspectStringExtra()
        {
            string text = null;
            if (this.boundPawn != null && this.DeathrestGene != null)
            {
                text = text + ("BoundTo".Translate() + ": " + this.boundPawn.NameShortColored).Resolve() + string.Format(" ({0}/{1} {2})", this.DeathrestGene.CurrentCapacity, this.DeathrestGene.DeathrestCapacity, "DeathrestCapacity".Translate());
                if (this.Props.displayTimeActive && this.presenceTicks > 0 && this.DeathrestGene.deathrestTicks > 0)
                {
                    float f = Mathf.Clamp01((float)this.presenceTicks / (float)this.DeathrestGene.deathrestTicks);
                    text += string.Format("\n{0}: {1} / {2} ({3})\n{4}", new object[]
                    {
                        "TimeActiveThisDeathrest".Translate(),
                        this.presenceTicks.ToStringTicksToPeriod(true, true, true, true, false),
                        this.DeathrestGene.deathrestTicks.ToStringTicksToPeriod(true, true, true, true, false),
                        f.ToStringPercent(),
                        "MinimumNeededToApply".Translate(0.75f.ToStringPercent())
                    });
                }
            }
            else
            {
                text += "WillBindOnFirstUse".Translate();
            }
            return text;
        }

        public override void PostExposeData()
        {
            Scribe_References.Look<Pawn>(ref this.boundPawn, "boundPawn", false);
            Scribe_Values.Look<int>(ref this.presenceTicks, "presenceTicks", 0, false);
        }

        private Pawn boundPawn;
        public int presenceTicks;
        [Unsaved(false)]
        private CompPowerTrader cachedPowerComp;
        [Unsaved(false)]
        private CompRefuelable cachedRefuelableComp;
        [Unsaved(false)]
        private Gene_Deathrest cachedDeathrestGene;
        [Unsaved(false)]
        private Material cachedHoseMat;
        [Unsaved(false)]
        private Sustainer sustainer;
    }
}
