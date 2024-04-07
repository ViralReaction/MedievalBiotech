using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;

namespace MedievalBiotech
{
    // Token: 0x02001244 RID: 4676
    public class Verb_CastAbilitySoul : Verb_CastAbility
    {
        // Token: 0x170012E3 RID: 4835
        // (get) Token: 0x06006E7E RID: 28286 RVA: 0x00254AE8 File Offset: 0x00252CE8
        // (set) Token: 0x06006E7F RID: 28287 RVA: 0x00254AF0 File Offset: 0x00252CF0
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

        // Token: 0x170012E4 RID: 4836
        // (get) Token: 0x06006E80 RID: 28288 RVA: 0x00254AF9 File Offset: 0x00252CF9
        public static Color RadiusHighlightColor
        {
            get
            {
                return new Color(0.3f, 0.8f, 1f);
            }
        }

        //// Token: 0x170012E5 RID: 4837
        //// (get) Token: 0x06006E81 RID: 28289 RVA: 0x00254B0F File Offset: 0x00252D0F
        //public override string ReportLabel
        //{
        //    get
        //    {
        //        return this.ability.def.label;
        //    }
        //}

        //// Token: 0x170012E6 RID: 4838
        //// (get) Token: 0x06006E82 RID: 28290 RVA: 0x00002739 File Offset: 0x00000939
        //public override bool MultiSelect
        //{
        //    get
        //    {
        //        return true;
        //    }
        //}

        // Token: 0x170012E7 RID: 4839
        // (get) Token: 0x06006E83 RID: 28291 RVA: 0x00254B24 File Offset: 0x00252D24
        //public override bool HidePawnTooltips
        //{
        //    get
        //    {
        //        using (List<CompAbilityEffect>.Enumerator enumerator = this.ability.EffectComps.GetEnumerator())
        //        {
        //            while (enumerator.MoveNext())
        //            {
        //                if (enumerator.Current.HideTargetPawnTooltip)
        //                {
        //                    return true;
        //                }
        //            }
        //        }
        //        return false;
        //    }
        //}

        // Token: 0x170012E8 RID: 4840
        // (get) Token: 0x06006E84 RID: 28292 RVA: 0x00254B84 File Offset: 0x00252D84
        //public override ITargetingSource DestinationSelector
        //{
        //    get
        //    {
        //        CompAbilityEffect_WithDest compAbilityEffect_WithDest = this.ability.CompOfType<CompAbilityEffect_WithDest>();
        //        if (compAbilityEffect_WithDest != null && compAbilityEffect_WithDest.Props.destination == AbilityEffectDestination.Selected)
        //        {
        //            return compAbilityEffect_WithDest;
        //        }
        //        return null;
        //    }
        //}
        //public override bool TryCastShot()
        //{
        //    return this.ability.Activate(this.currentTarget, this.currentDestination);
        //}

        //// Token: 0x06006E86 RID: 28294 RVA: 0x00254BCC File Offset: 0x00252DCC
        //public override void OrderForceTarget(LocalTargetInfo target)
        //{
        //    CompAbilityEffect_WithDest compAbilityEffect_WithDest = this.ability.CompOfType<CompAbilityEffect_WithDest>();
        //    if (compAbilityEffect_WithDest != null && compAbilityEffect_WithDest.Props.destination == AbilityEffectDestination.Selected)
        //    {
        //        compAbilityEffect_WithDest.SetTarget(target);
        //        return;
        //    }
        //    this.ability.QueueCastingJob(target, null);
        //}

        //// Token: 0x06006E87 RID: 28295 RVA: 0x00002739 File Offset: 0x00000939
        //public virtual bool IsApplicableTo(LocalTargetInfo target, bool showMessages = false)
        //{
        //    return true;
        //}

        //// Token: 0x06006E88 RID: 28296 RVA: 0x00254C10 File Offset: 0x00252E10
        //public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        //{
        //    if (this.verbProps.range > 0f)
        //    {
        //        if (!this.CanHitTarget(target))
        //        {
        //            if (target.IsValid && showMessages)
        //            {
        //                string t = "CannotUseAbility".Translate(this.ability.def.label) + ": ";
        //                if (this.ability.verb.OutOfRange(this.ability.pawn.Position, target, target.HasThing ? target.Thing.OccupiedRect() : CellRect.SingleCell(target.Cell)))
        //                {
        //                    Messages.Message(t + "AbilityOutOfRange".Translate(), new LookTargets(new TargetInfo[]
        //                    {
        //                        this.ability.pawn,
        //                        target.ToTargetInfo(this.ability.pawn.Map)
        //                    }), MessageTypeDefOf.RejectInput, false);
        //                }
        //                else if (this.ability.pawn.Spawned)
        //                {
        //                    Messages.Message(t + "AbilityCannotHitTarget".Translate(), new LookTargets(new TargetInfo[]
        //                    {
        //                        this.ability.pawn,
        //                        target.ToTargetInfo(this.ability.pawn.Map)
        //                    }), MessageTypeDefOf.RejectInput, false);
        //                }
        //            }
        //            return false;
        //        }
        //    }
        //    else if (!this.ability.pawn.CanReach(target, PathEndMode.Touch, this.ability.pawn.NormalMaxDanger(), false, false, TraverseMode.ByPawn))
        //    {
        //        if (target.IsValid && showMessages)
        //        {
        //            Messages.Message("CannotUseAbility".Translate(this.ability.def.label) + ": " + "AbilityCannotReachTarget".Translate(), new LookTargets(new TargetInfo[]
        //            {
        //                this.ability.pawn,
        //                target.ToTargetInfo(this.ability.pawn.Map)
        //            }), MessageTypeDefOf.RejectInput, false);
        //        }
        //        return false;
        //    }
        //    if (!this.IsApplicableTo(target, showMessages))
        //    {
        //        return false;
        //    }
        //    for (int i = 0; i < this.ability.EffectComps.Count; i++)
        //    {
        //        if (!this.ability.EffectComps[i].Valid(target, showMessages))
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        //// Token: 0x06006E89 RID: 28297 RVA: 0x00254E9D File Offset: 0x0025309D
        //public override bool CanHitTarget(LocalTargetInfo targ)
        //{
        //    return this.verbProps.range <= 0f || base.CanHitTarget(targ);
        //}

        //// Token: 0x06006E8A RID: 28298 RVA: 0x00254EBA File Offset: 0x002530BA
        //public override void OnGUI(LocalTargetInfo target)
        //{
        //    if (this.CanHitTarget(target) && this.IsApplicableTo(target, false) && this.ValidateTarget(target, false))
        //    {
        //        base.OnGUI(target);
        //    }
        //    else
        //    {
        //        GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
        //    }
        //    this.DrawAttachmentExtraLabel(target);
        //}

        //// Token: 0x06006E8B RID: 28299 RVA: 0x00254EF4 File Offset: 0x002530F4
        //protected void DrawAttachmentExtraLabel(LocalTargetInfo target)
        //{
        //    foreach (CompAbilityEffect compAbilityEffect in this.ability.EffectComps)
        //    {
        //        string text = compAbilityEffect.ExtraLabelMouseAttachment(target);
        //        if (!text.NullOrEmpty())
        //        {
        //            Widgets.MouseAttachedLabel(text, 0f, 0f);
        //            break;
        //        }
        //    }
        //}

        //// Token: 0x06006E8C RID: 28300 RVA: 0x00254F68 File Offset: 0x00253168
        //public override bool TryStartCastOn(LocalTargetInfo castTarg, LocalTargetInfo destTarg, bool surpriseAttack = false, bool canHitNonTargetPawns = true, bool preventFriendlyFire = false, bool nonInterruptingSelfCast = false)
        //{
        //    bool flag = base.TryStartCastOn(castTarg, destTarg, surpriseAttack, canHitNonTargetPawns, preventFriendlyFire, nonInterruptingSelfCast);
        //    Pawn pawn;
        //    if (flag && this.ability.def.stunTargetWhileCasting && this.ability.def.verbProperties.warmupTime > 0f && (pawn = (castTarg.Thing as Pawn)) != null && pawn != this.ability.pawn)
        //    {
        //        pawn.stances.stunner.StunFor(this.ability.def.verbProperties.warmupTime.SecondsToTicks(), this.ability.pawn, false, false, false);
        //        if (!pawn.Awake())
        //        {
        //            RestUtility.WakeUp(pawn, true);
        //        }
        //    }
        //    return flag;
        //}

        //// Token: 0x06006E8D RID: 28301 RVA: 0x00255020 File Offset: 0x00253220
        //public override void DrawHighlight(LocalTargetInfo target)
        //{
        //    AbilityDef def = this.ability.def;
        //    if (this.verbProps.range > 0f)
        //    {
        //        this.verbProps.DrawRadiusRing_NewTemp(this.caster.Position, this);
        //    }
        //    if (this.CanHitTarget(target) && this.IsApplicableTo(target, false))
        //    {
        //        if (def.HasAreaOfEffect)
        //        {
        //            if (target.IsValid)
        //            {
        //                GenDraw.DrawTargetHighlightWithLayer(target.CenterVector3, AltitudeLayer.MetaOverlays);
        //                GenDraw.DrawRadiusRing(target.Cell, def.EffectRadius, Verb_CastAbility.RadiusHighlightColor, null);
        //            }
        //        }
        //        else
        //        {
        //            GenDraw.DrawTargetHighlightWithLayer(target.CenterVector3, AltitudeLayer.MetaOverlays);
        //        }
        //    }
        //    if (target.IsValid)
        //    {
        //        this.ability.DrawEffectPreviews(target);
        //    }
        //}

        //// Token: 0x170012E9 RID: 4841
        //// (get) Token: 0x06006E8E RID: 28302 RVA: 0x002550D1 File Offset: 0x002532D1
        //public override Texture2D UIIcon
        //{
        //    get
        //    {
        //        return this.ability.def.uiIcon;
        //    }
        //}

        // Token: 0x06006E8F RID: 28303 RVA: 0x002550E3 File Offset: 0x002532E3
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<AbilitySoul>(ref this.ability, "ability", false);
        }

        // Token: 0x04004264 RID: 16996
        public AbilitySoul ability;
    }
}
