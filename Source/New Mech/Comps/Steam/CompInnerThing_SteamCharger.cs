using RimWorld;
using Verse;

namespace MedievalBiotech
{
    public class CompInnerThing_SteamCharger : CompThingContainer
    {

        public override string CompInspectStringExtra()
        {
            return null;
        }

        public override void PostDeSpawn(Map map)
        {
            this.innerContainer.TryDropAll(this.parent.Position, map, ThingPlaceMode.Near, null, null, true);
        }
    }
}
