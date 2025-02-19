using RimWorld;
using System.Text;
using Verse;
using PipeSystem;

namespace MedievalBiotech
{
    public class CompResourceTrader_Steam : CompResourceTrader
    {
        public override string CompInspectStringExtra()
        {
            StringBuilder sb = new StringBuilder();
            if (this.Consumption >= 0f)
            {
                sb.AppendFormat("{0} {1:##0} {2}/d", "PipeSystem_ResourceNeeded".Translate(this.Resource.name), (this.Consumption / 100.0 * 60000.0), this.Resource.unit);
            }
            else
            {
                sb.AppendFormat("{0} {1:##0} {2}/d", "PipeSystem_ResourceOutput".Translate(this.Resource.name), this.ResourceOn ? -(this.Consumption / 100.0 * 60000.0) : 0.0, this.Resource.unit);
            }
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
