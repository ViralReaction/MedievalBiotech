using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace MedievalBiotech
{
    public class CompUsedGraphic : ThingComp
    {
        private CompProperties_UsedGraphic Props => (CompProperties_UsedGraphic)this.props;

        public bool ParentIsFull
        {
            get
            {
                if (this.parent is Building_Casket parent && parent.HasAnyContents)
                    return true;
                CompPawnSpawnOnWakeup comp = this.parent.TryGetComp<CompPawnSpawnOnWakeup>();
                return comp != null && !comp.CanSpawn;
            }
        }
        public override void PostDraw()
        {
            base.PostDraw();
            if (!this.ParentIsFull)
                return;
            Mesh mesh = this.Props.graphicData.Graphic.MeshAt(this.parent.Rotation);
            Vector3 drawPos = this.parent.DrawPos;
            drawPos.y = AltitudeLayer.BuildingOnTop.AltitudeFor();
            //Vector3 vector 3 = Vector3.
            //Vector3 vector3 = Vector3.(drawPos, this.Props.graphicData.drawOffset.RotatedBy(this.parent.Rotation));
            Quaternion identity = Quaternion.identity;
            Material material = this.Props.graphicData.Graphic.MatAt(this.parent.Rotation);
            Graphics.DrawMesh(mesh, drawPos, identity, material, 0);
        }
    }
}