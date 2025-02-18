using Verse;
using RimWorld;

namespace MedievalBiotech.New_Mech
{
    [DefOf]
    public static class MedievalBiotechDefOf
    {
        public static ThingDef MB_BasicRecharger;

        static MedievalBiotechDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(MedievalBiotechDefOf));
    }
}
