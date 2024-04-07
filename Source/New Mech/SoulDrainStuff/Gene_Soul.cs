using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace MedievalBiotech
{
    public class Gene_Soul : Gene_Resource, IGeneResourceDrain
    {
        public Gene_Resource Resource
        {
            get
            {
                return this;
            }
        }

        public Pawn Pawn
        {
            get
            {
                return this.pawn;
            }
        }

        public bool CanOffset
        {
            get
            {
                return this.Active && !this.pawn.Deathresting;
            }
        }

        public string DisplayLabel
        {
            get
            {
                return this.Label + " (" + "Gene".Translate() + ")";
            }
        }

        public float ResourceLossPerDay
        {
            get
            {
                return this.def.resourceLossPerDay;
            }
        }

        public override float InitialResourceMax
        {
            get
            {
                return 1f;
            }
        }

        public override float MinLevelForAlert
        {
            get
            {
                return 0.15f;
            }
        }

        public override float MaxLevelOffset
        {
            get
            {
                return 0.1f;
            }
        }

        public override Color BarColor
        {
            get
            {
                return new ColorInt(118, 87, 145).ToColor;
            }
        }

        public override Color BarHighlightColor
        {
            get
            {
                return new ColorInt(99, 64, 114).ToColor;
            }
        }

        public override void PostAdd()
        {
            if (!ModLister.CheckBiotech("Hemogen"))
            {
                return;
            }
            base.PostAdd();
            this.Reset();
        }

        public override void Notify_IngestedThing(Thing thing, int numTaken)
        {
            if (thing.def.IsMeat)
            {
                IngestibleProperties ingestible = thing.def.ingestible;
                bool flag;
                if (ingestible == null)
                {
                    flag = false;
                }
                else
                {
                    ThingDef sourceDef = ingestible.sourceDef;
                    bool? flag2;
                    if (sourceDef == null)
                    {
                        flag2 = null;
                    }
                    else
                    {
                        RaceProperties race = sourceDef.race;
                        flag2 = ((race != null) ? new bool?(race.Humanlike) : null);
                    }
                    bool? flag3 = flag2;
                    bool flag4 = true;
                    flag = (flag3.GetValueOrDefault() == flag4 & flag3 != null);
                }
                if (flag)
                {
                    Utility.OffsetSoul(this.pawn, 0.0375f * thing.GetStatValue(StatDefOf.Nutrition, true, -1) * (float)numTaken, true);
                }
            }
        }

        public override void Tick()
        {
            base.Tick();
            GeneResourceDrainUtility.TickResourceDrain(this);
        }

        public override void SetTargetValuePct(float val)
        {
            this.targetValue = Mathf.Clamp(val * this.Max, 0f, this.Max - this.MaxLevelOffset);
        }

        public bool ShouldConsumeSoulNow()
        {
            return this.Value < this.targetValue;
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (!this.Active)
            {
                yield break;
            }
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            IEnumerator<Gizmo> enumerator = null;
            foreach (Gizmo gizmo2 in GeneResourceDrainUtility.GetResourceDrainGizmos(this))
            {
                yield return gizmo2;
            }
            enumerator = null;
            yield break;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.soulGemsAllowed, "soulGemsAllowed", true, false);
        }

        public bool soulGemsAllowed = true;
    }
}
