using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace MedievalBiotech
{
    public class CompAbilityEffect_CreateArcanaStone : CompAbilityEffect
    {
        public new CompProperties_CreateArcanaStone Props
        {
            get
            {
                return (CompProperties_CreateArcanaStone)this.props;
            }
        }
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            Pawn pawn = this.parent.pawn;
            Thing thing = ThingMaker.MakeThing(this.Props.stoneCreated, null);
            if (pawn.inventory.innerContainer != null)
            {
                pawn.inventory.innerContainer.TryAdd(thing);
            }
            //if (!GenPlace.TryPlaceThing(ThingMaker.MakeThing(this.Props.stoneCreated, null), pawn.PositionHeld, pawn.MapHeld, ThingPlaceMode.Near, null, null, default(Rot4)))
            //{
            //    Log.Error("Could not drop arcana stone near " + pawn.PositionHeld);
            //}
        }
}
}
