using System.Collections.Generic;
using System.Linq;
using PipeSystem;

namespace MedievalBiotech
{
    public class CompPowerPlantNeedResource_Steam : CompPowerPlantNeedResource
    {
        private List<CompResourceTrader> compResourceTraders;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            this.compResourceTraders = this.parent.GetComps<CompResourceTrader>().ToList<CompResourceTrader>();
            base.PostSpawnSetup(respawningAfterLoad);
        }

        protected override float DesiredPowerOutput
        {
            get
            {
                for (int index = 0; index < this.compResourceTraders.Count; ++index)
                {
                    if (!this.compResourceTraders[index].ResourceOn)
                        return 0.0f;
                }
                return base.DesiredPowerOutput;
            }
        }

        public override string CompInspectStringExtra()
        {
            return null;
            //string text;
            //if (this.powerLastOutputted && !base.Props.alwaysDisplayAsUsingPower)
            //{
            //    text = "PowerOutput".Translate() + ": " + this.PowerOutput.ToString("#####0") + " W";
            //}
            //else
            //{
            //    text = "PowerNeeded".Translate() + ": " + (-this.PowerOutput).ToString("#####0") + " W";
            //}
            //if (base.Props.idlePowerDraw > 0f || base.Props.alwaysDisplayAsUsingPower)
            //{
            //    text += " (" + "PowerActiveNeeded".Translate(base.Props.PowerConsumption.ToString("#####0")) + ")";
            //}
            //return text + "\n" + base.CompInspectStringExtra();
        }
    }
}
