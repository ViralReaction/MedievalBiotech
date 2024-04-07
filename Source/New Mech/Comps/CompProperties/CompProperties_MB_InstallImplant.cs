// Token: 0x0200189B RID: 6299
using RimWorld;
using Verse;

namespace MedievalBiotech
{
    public class CompProperties_MB_InstallImplant : CompProperties_UseEffectInstallImplant
    {
        public CompProperties_MB_InstallImplant()
        {
            this.compClass = typeof(CompUseEffect_MB_InstallImplant);
        }

        public GeneDef requiredGeneDef;
        public string summonerType;
    }
}