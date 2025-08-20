using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Ricave.Core
{
    public static class FactionUtility
    {
        public static List<Faction> DefaultHostileFactions(Faction faction)
        {
            FactionUtility.<> c__DisplayClass0_0 CS$<> 8__locals1;
            CS$<> 8__locals1.faction = faction;
            CS$<> 8__locals1.ret = new List<Faction>();
            foreach (FactionSpec factionSpec in CS$<> 8__locals1.faction.Spec.DefaultHostileTo)
			{
                FactionUtility.< DefaultHostileFactions > g__CheckAddHostilityAllOfSpec | 0_0(factionSpec, ref CS$<> 8__locals1);
            }
            if (CS$<> 8__locals1.faction.IsPlayerFactionSpec)
			{
                using (HashSet<TraitSpec>.Enumerator enumerator2 = Get.TraitManager.Chosen.GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        TraitSpec traitSpec = enumerator2.Current;
                        foreach (FactionSpec factionSpec2 in traitSpec.HostileTo)
                        {
                            FactionUtility.< DefaultHostileFactions > g__CheckAddHostilityAllOfSpec | 0_0(factionSpec2, ref CS$<> 8__locals1);
                        }
                    }
                    goto IL_0129;
                }
            }
            bool flag = false;
            using (HashSet<TraitSpec>.Enumerator enumerator2 = Get.TraitManager.Chosen.GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    if (enumerator2.Current.HostileTo.Contains(CS$<> 8__locals1.faction.Spec))
                    {
                        flag = true;
                        break;
                    }
                }
            }
            if (flag)
            {
                FactionUtility.< DefaultHostileFactions > g__CheckAddHostilityAllOfSpec | 0_0(Get.RunSpec.PlayerFaction, ref CS$<> 8__locals1);
            }
        IL_0129:
            if (CS$<> 8__locals1.faction.IsPlayerFactionSpec)
			{
                if (Get.Cosmetic_DevilHorns.IsChosen())
                {
                    FactionUtility.< DefaultHostileFactions > g__CheckAddHostilityAllOfSpec | 0_0(Get.Faction_Guardians, ref CS$<> 8__locals1);
                }
            }

            else if (Get.Cosmetic_DevilHorns.IsChosen() && CS$<> 8__locals1.faction.Spec == Get.Faction_Guardians)
			{
                FactionUtility.< DefaultHostileFactions > g__CheckAddHostilityAllOfSpec | 0_0(Get.RunSpec.PlayerFaction, ref CS$<> 8__locals1);
            }
            return CS$<> 8__locals1.ret;
        }

        public static string GenerateFactionName(FactionSpec spec)
        {
            if (spec.PossibleNames.NullOrEmpty<string>())
            {
                return null;
            }
            string text;
            if (spec.PossibleNames.TryGetRandomElementWhere<string>((string x) => !Get.FactionManager.Factions.Any<Faction>((Faction y) => y.Name == x), out text))
            {
                return text;
            }
            if (spec.PossibleNames.TryGetRandomElement<string>(out text))
            {
                return text;
            }
            return null;
        }

        [CompilerGenerated]
        internal static void <DefaultHostileFactions>g__CheckAddHostilityAllOfSpec|0_0(FactionSpec hostile, ref FactionUtility.<>c__DisplayClass0_0 A_1)
		{
			foreach (Faction faction in Get.FactionManager.Factions)
			{
				if (faction != A_1.faction && faction.Spec == hostile && !Get.FactionManager.HostilityExists(A_1.faction, faction))
				{
					if (A_1.faction.IsPlayerFactionSpec || faction.IsPlayerFactionSpec)
					{
						bool flag = false;
						foreach (TraitSpec traitSpec in Get.TraitManager.Chosen)
						{
							if ((A_1.faction.IsPlayerFactionSpec && traitSpec.NotHostileTo.Contains(faction.Spec)) || (faction.IsPlayerFactionSpec && traitSpec.NotHostileTo.Contains(A_1.faction.Spec)))
							{
								flag = true;
								break;
							}
}
if (flag)
{
    continue;
}
					}
					A_1.ret.Add(faction);
				}
			}
		}
	}
}