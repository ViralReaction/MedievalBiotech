using RimWorld;
using Verse;

namespace MedievalBiotech
{
    public class Hediff_SaguineMage : HediffWithComps
    {
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            if (!ModLister.CheckBiotech("MB_BloodChaliceImplant"))
            {
                this.pawn.health.RemoveHediff(this);
                return;
            }
            PawnComponentsUtility.AddAndRemoveDynamicComponents(this.pawn, false);
            if (this.pawn.Spawned)
            {
                Find.LetterStack.ReceiveLetter("LetterLabelBloodChaliceInstalled".Translate() + ": " + this.pawn.LabelShortCap, "LetterBloodChaliceInstalled".Translate(this.pawn.Named("PAWN")), LetterDefOf.PositiveEvent, this.pawn, null, null, null, null, 0, true);
            }
        }
        public override void PostRemoved()
        {
            base.PostRemoved();
            Pawn_MechanitorTracker mechanitor = this.pawn.mechanitor;
            if (mechanitor == null)
            {
                return;
            }
            mechanitor.Notify_MechlinkRemoved();
        }
        public override void PostTick()
        {
            base.PostTick();
            if (this.pawn.Spawned && this.pawn.IsHashIntervalTick(300))
            {
                LessonAutoActivator.TeachOpportunity(ConceptDefOf.Mechanitors, OpportunityType.Important);
            }
        }


        private const int LearningOpportunityCheckInterval = 300;
    }
}