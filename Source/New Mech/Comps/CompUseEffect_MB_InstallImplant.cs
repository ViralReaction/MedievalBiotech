using System;
using Verse;
using RimWorld;
namespace MedievalBiotech
{
    public class CompUseEffect_MB_InstallImplant : CompUseEffect_InstallImplant
    {
        public new CompProperties_MB_InstallImplant Props
        {
            get
            {
                return (CompProperties_MB_InstallImplant)this.props;
            }
        }
        public override AcceptanceReport CanBeUsedBy(Pawn p)
        {
            if (!ModLister.CheckBiotech("install implant mechlink"))
            {
                return false;
            }
            return CheckUsedBy(p);
        }

        public override TaggedString ConfirmMessage(Pawn p)
        {
            if (p.WorkTypeIsDisabled(WorkTypeDefOf.Smithing))
            {
                return "DankPyon_ConfirmInstallImplant_Smithing".Translate().Replace("{SUMMONER}", this.Props.summonerType);
            }
            if (p.WorkTagIsDisabled(WorkTags.Intellectual))
            {

                return "DankPyon_ConfirmInstallImplant_Intellectual".Translate().Replace("{SUMMONER}", this.Props.summonerType);
            }
            return null;
        }
        private AcceptanceReport CheckUsedBy(Pawn p)
        {
            if (!p.IsFreeColonist && !this.Props.allowNonColonists)
            {
                return "InstallImplantNotAllowedForNonColonists".Translate();
            }
            if (p.RaceProps.body.GetPartsWithDef(this.Props.bodyPart).FirstOrFallback(null) == null)
            {
                return "InstallImplantNoBodyPart".Translate() + ": " + this.Props.bodyPart.LabelShort;
            }
            if (this.Props.requiresPsychicallySensitive && p.psychicEntropy != null && !p.psychicEntropy.IsPsychicallySensitive)
            {
                return "InstallImplantPsychicallyDeaf".Translate();
            }
            Hediff existingImplant = this.GetExistingImplant(p);
            if (this.Props.requiresExistingHediff && existingImplant == null)
            {
                return "InstallImplantHediffRequired".Translate(this.Props.hediffDef.label);
            }
            if (existingImplant != null)
            {
                if (!this.Props.canUpgrade)
                {
                    return "InstallImplantAlreadyInstalled".Translate();
                }
                Hediff_Level hediff_Level = (Hediff_Level)existingImplant;
                if ((float)hediff_Level.level >= hediff_Level.def.maxSeverity)
                {
                    return "InstallImplantAlreadyMaxLevel".Translate();
                }
                if (this.Props.maxSeverity <= (float)hediff_Level.level)
                {
                    return "InstallImplantAlreadyMaxLevel".Translate() + " " + this.Props.maxSeverity;
                }
                if (this.Props.minSeverity > (float)hediff_Level.level)
                {
                    return "InstallImplantMinLevel".Translate(this.Props.minSeverity);
                }
            }
            if (p.genes.GetGene(this.Props.requiredGeneDef) == null && this.Props.requiredGeneDef != null)
            {
                return "DankPyon_InstallBloodChaliceHemogenRequired".Translate() + ": " + this.Props.requiredGeneDef.label;
            }
            return true;
        }
    }
}
