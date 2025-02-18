using RimWorld;
using System.Text;
using Verse;
using PipeSystem;

namespace MedievalBiotech
{
    public class CompResourceTrader_Steam : CompResourceTrader
    {
        public CompProperties_ResourceTrader Props => (CompProperties_ResourceTrader)this.props;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder sb = new StringBuilder();
            if ((double)this.Consumption >= 0.0)
                sb.Append(string.Format("{0} {1:##0} {2}/d", (object)"PipeSystem_ResourceNeeded".Translate((NamedArgument)this.Resource.name), (object)(float)((double)this.Consumption / 100.0 * 60000.0), (object)this.Resource.unit));
            else
                sb.Append(string.Format("{0} {1:##0} {2}/d", (object)"PipeSystem_ResourceOutput".Translate((NamedArgument)this.Resource.name), (object)(float)(this.ResourceOn ? -(double)this.Consumption / 100.0 * 60000.0 : 0.0), (object)this.Resource.unit));
            if (!ResourceOn)
            {
                sb.AppendInNewLine("MB_PipeSystem_NotConnected".Translate());
            }
            if (DebugSettings.ShowDevGizmos)
                sb.AppendInNewLine(this.DebugString);
            return sb.ToString().Trim();
        }
    }
}
