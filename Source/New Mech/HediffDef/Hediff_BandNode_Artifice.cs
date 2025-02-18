using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MedievalBiotech
{
    public class Hediff_BandNode_Artifice : Hediff
    {
        public int AdditionalBandwidth
        {
            get
            {
                return this.cachedTunedBandNodesCount;
            }
        }

        public override bool ShouldRemove
        {
            get
            {
                return this.cachedTunedBandNodesCount == 0;
            }
        }

        public override HediffStage CurStage
        {
            get
            {
                if (this.curStage == null && this.cachedTunedBandNodesCount > 0)
                {
                    StatModifier statModifier = new StatModifier();
                    statModifier.stat = StatDefOf.MechBandwidth;
                    statModifier.value = (float)this.cachedTunedBandNodesCount;
                    this.curStage = new HediffStage();
                    this.curStage.statOffsets = new List<StatModifier>
                    {
                        statModifier
                    };
                }
                return this.curStage;
            }
        }

        public override void PostTick()
        {
            base.PostTick();
            if (this.pawn.IsHashIntervalTick(60))
            {
                this.RecacheBandNodes();
            }
        }

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            this.RecacheBandNodes();
        }

        public void RecacheBandNodes()
        {
            int num = this.cachedTunedBandNodesCount;
            this.cachedTunedBandNodesCount = 0;
            List<Map> maps = Find.Maps;
            for (int i = 0; i < maps.Count; i++)
            {
                foreach (Building thing in maps[i].listerBuildings.AllBuildingsColonistOfDef(MB_DefOf.MB_BandNode))
                {
                    if (thing.TryGetComp<CompBandNode_Steam>().tunedTo == this.pawn && thing.TryGetComp<CompResourceTrader_Steam>().ResourceOn)
                    {
                        this.cachedTunedBandNodesCount++;
                    }
                }
            }
            if (num != this.cachedTunedBandNodesCount)
            {
                this.curStage = null;
                Pawn_MechanitorTracker mechanitor = this.pawn.mechanitor;
                if (mechanitor == null)
                {
                    return;
                }
                mechanitor.Notify_BandwidthChanged();
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.cachedTunedBandNodesCount, "cachedTunedBandNodesCount", 0, false);
        }

        private const int BandNodeCheckInterval = 60;

        private int cachedTunedBandNodesCount;

        private HediffStage curStage;
    }
}
