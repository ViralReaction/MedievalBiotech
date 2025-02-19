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
                for (int index = 0; index < compResourceTraders.Count; ++index)
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
        }
    }
}
