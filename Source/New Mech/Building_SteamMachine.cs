using Verse;
using PipeSystem;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

namespace MedievalBiotech
{
    public class Building_SteamMachine : Building
    {

        private List<CompResourceTrader> traders = new List<CompResourceTrader>();
        public CompRefuelable compRefuelable;
        private int tradersCount = 0;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.traders = this.GetComps<CompResourceTrader>().ToList<CompResourceTrader>();
            this.tradersCount = this.traders.Count;
            this.compRefuelable = this.GetComp<CompRefuelable>();
        }

        public override void Tick()
        {
            base.Tick();
            if (compRefuelable != null)
                {
                if (compRefuelable.HasFuel)
                {
                    for (int index = 0; index < this.tradersCount; ++index)
                    {
                        this.BroadcastCompSignal(traders[index].OnSignal);
                    }

                }
                else
                {
                    for (int index = 0; index < this.tradersCount; ++index)
                    {
                        this.BroadcastCompSignal(traders[index].OffSignal);
                    }
                }
            }

        }
    }
}
