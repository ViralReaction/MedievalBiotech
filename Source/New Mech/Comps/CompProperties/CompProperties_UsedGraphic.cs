using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace MedievalBiotech
{
    public class CompProperties_UsedGraphic : CompProperties
    {
        public GraphicData graphicData;

        public CompProperties_UsedGraphic() => this.compClass = typeof(CompUsedGraphic);
    }
}
