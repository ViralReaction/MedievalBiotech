using System;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using RimWorld;

namespace MedievalBiotech
{
    [StaticConstructorOnStartup]
    public class CompNecrarchMeetingFire : CompFireOverlayBase
    {

        public new CompProperties_SanguophageMeetingFire Props
        {
            get
            {
                return (CompProperties_SanguophageMeetingFire)this.props;
            }
        }

        public override void PostDraw()
        {
            base.PostDraw();
            CompGlower compGlower = this.parent.TryGetComp<CompGlower>();
            if (compGlower != null && !compGlower.Glows)
            {
                return;
            }
            Vector3 drawPos = this.parent.DrawPos;
            drawPos.y += 0.03846154f;
            CompSanguophageMeetingFire.RedlightGraphic.Draw(drawPos + this.Props.offset, Rot4.North, this.parent, 0f);
        }

        public override bool CompPreventClaimingBy(Faction faction)
        {
            Lord lord = ((Building)this.parent).GetLord();
            return ((lord != null) ? lord.CurLordToil : null) is LordToil_SanguophageMeeting;
        }

        public static readonly Graphic RedlightGraphic = GraphicDatabase.Get<Graphic_Flicker>("Things/Special/Redlight", ShaderDatabase.TransparentPostLight, Vector2.one, Color.white);
    }
}
