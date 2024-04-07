using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedievalBiotech
{
    public class AbilitySoul : Ability
    {
        public float SoulCost()
        {
            if (this.comps != null)
            {
                using (List<AbilityComp>.Enumerator enumerator = this.comps.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        CompAbilityEffect_SoulCost compAbilityEffect_HemogenCost;
                        if ((compAbilityEffect_HemogenCost = (enumerator.Current as CompAbilityEffect_SoulCost)) != null)
                        {
                            return compAbilityEffect_HemogenCost.Props.soulCost;
                        }
                    }
                }
            }
            return 0f;
        }
    }
}
