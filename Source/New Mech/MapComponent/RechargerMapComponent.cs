using System.Collections.Generic;
using Verse;

namespace MedievalBiotech
{
    public class RechargerMapComponent : MapComponent
    {

        public RechargerMapComponent(Map map) : base(map)
        {

        }

        public override void MapGenerated()
        {
            base.MapGenerated();
            GetChargerMap();
        }

        public void GetChargerMap()
        {
            foreach (var thing in this.map.listerBuildings.allBuildingsColonist)
            {
                if (thing is Building_SteamCharger steamCharger)
                {
                    RegisterCharger(steamCharger);
                }
            }
        }

        public void RegisterCharger(Building_SteamCharger thing)
        {
            allChargers.Add(thing);
        }

        public void RemoveCharger(Building_SteamCharger thing)
        {
            allChargers.Remove(thing);
        }


        public List<Building_SteamCharger> allChargers = [];
    }

}
