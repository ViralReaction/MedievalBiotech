using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace MedievalBiotech
{
    public class Utility
    {
        public static bool IsSanguinMage(Pawn pawn)
        {
            if (pawn.health.hediffSet.HasHediff(MB_DefOf.MB_BloodChaliceImplant))
            {
                return true;
            }
            return false;
        }

        public static bool IsNecrarch(Pawn pawn)
        {
            if (pawn.health.hediffSet.HasHediff(MB_DefOf.MB_NecronomiconImplant))
            {
                return true;
            }
            return false;
        }
    }
}
