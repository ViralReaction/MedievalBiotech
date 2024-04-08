using System;
using System.Collections.Generic;
using RimWorld.Planet;
using Verse;
using RimWorld;

namespace MedievalBiotech
{
    public class Alert_LowSoulEnergy : Alert
    {
        public Alert_LowSoulEnergy()
        {
            this.requireBiotech = true;
            this.defaultLabel = "MB_AlertLowSoulEnergy".Translate();
        }

        public override string GetLabel()
        {
            string text = this.defaultLabel;
            if (this.targets.Count == 1)
            {
                text = text + ": " + this.targetLabels[0];
            }
            return text;
        }
        private void CalculateTargets()
        {
            this.targets.Clear();
            this.targetLabels.Clear();
            foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive)
            {
                if (pawn.genes != null && pawn.RaceProps.Humanlike && pawn.Faction == Faction.OfPlayer)
                {
                    Gene_Soul firstGeneOfType = pawn.genes.GetFirstGeneOfType<Gene_Soul>();
                    if (firstGeneOfType != null && firstGeneOfType.Value < firstGeneOfType.MinLevelForAlert)
                    {
                        this.targets.Add(pawn);
                        this.targetLabels.Add(pawn.NameShortColored.Resolve());
                    }
                }
            }
        }

        public override TaggedString GetExplanation()
        {
            return "MB_AlertLowSoulEnergyDesc".Translate() + ":\n" + this.targetLabels.ToLineList("  - ");
        }

        public override AlertReport GetReport()
        {
            this.CalculateTargets();
            return AlertReport.CulpritsAre(this.targets);
        }

        private List<GlobalTargetInfo> targets = new List<GlobalTargetInfo>();

        private List<string> targetLabels = new List<string>();
    }
}
