using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;

namespace MedievalBiotech
{
    public class Verb_CastAbilitySoul : Verb_CastAbility
    {
        public AbilitySoul Ability
        {
            get
            {
                return this.ability;
            }
            set
            {
                this.ability = value;
            }
        }
        public static Color RadiusHighlightColor
        {
            get
            {
                return new Color(0.3f, 0.8f, 1f);
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<AbilitySoul>(ref this.ability, "ability", false);
        }

        // Token: 0x04004264 RID: 16996
        public AbilitySoul ability;
    }
}
