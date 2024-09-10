using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace MedievalBiotech
{
    public class Deathrest_SoulOffset : DefModExtension
    {
        public float soulGainOffset;
    }

    public class Custom_Mech : DefModExtension
    {
        public bool UndeadMech = false;
        public bool DemonMech = false;
        public bool ArtificeMech = false;
    }
}

