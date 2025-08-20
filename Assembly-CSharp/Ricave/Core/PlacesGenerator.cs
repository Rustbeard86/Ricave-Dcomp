using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ricave.Core
{
    public static class PlacesGenerator
    {
        public static void GeneratePlaces()
        {
            if (Get.RunSpec.PlaceCounts == null)
            {
                return;
            }
            Rand.PushState(Calc.CombineHashes<int, int>(Get.RunConfig.RunSeed, 934214660));
            try
            {
                PlacesGenerator.GeneratePlaceLayout();
                PlacesGenerator.DecideEnemies();
                PlacesGenerator.AddShelters();
                PlacesGenerator.DistributeSpirits();
            }
            finally
            {
                Rand.PopState();
            }
        }

        private static void GeneratePlaceLayout()
        {
            List<int> placeCounts = Get.RunSpec.PlaceCounts;
            List<Place>[] array = new List<Place>[placeCounts.Count];
            int num = 4;
            for (int i = 0; i < placeCounts.Count; i++)
            {
                array[i] = new List<Place>();
                float num2 = (float)i;
                float num3 = (float)(num - placeCounts[i]) / 2f;
                for (int j = 0; j < placeCounts[i]; j++)
                {
                    PlaceSpec placeSpec;
                    if (i == 4)
                    {
                        placeSpec = Get.Place_GoogonsLair;
                    }
                    else if (i == 8)
                    {
                        placeSpec = Get.Place_DemonsLair;
                    }
                    else
                    {
                        placeSpec = Get.Place_Normal;
                    }
                    Place place = new Place(placeSpec, new Vector2(num3, num2), i + 1, PlacesGenerator.GeneratePlaceName(placeSpec));
                    array[i].Add(place);
                    new Instruction_AddPlace(place).Do();
                    if (i != 0)
                    {
                        int num4 = placeCounts[i - 1];
                        if (num4 == 1)
                        {
                            new Instruction_AddPlaceLink(new PlaceLink(array[i - 1][0], place)).Do();
                        }
                        else if (placeCounts[i] == 1)
                        {
                            for (int k = 0; k < num4; k++)
                            {
                                new Instruction_AddPlaceLink(new PlaceLink(array[i - 1][k], place)).Do();
                            }
                        }
                        else
                        {
                            int num5;
                            int num6;
                            if (placeCounts[i] > num4)
                            {
                                num5 = j - 1;
                                num6 = j;
                            }
                            else
                            {
                                num5 = j;
                                num6 = j + 1;
                            }
                            if (num5 >= 0 && num5 < num4)
                            {
                                new Instruction_AddPlaceLink(new PlaceLink(array[i - 1][num5], place)).Do();
                            }
                            if (num6 >= 0 && num6 < num4)
                            {
                                new Instruction_AddPlaceLink(new PlaceLink(array[i - 1][num6], place)).Do();
                            }
                        }
                    }
                    num3 += 1f;
                }
            }
        }

        private static void DecideEnemies()
        {
            if (Get.RunSpec.Enemies.NullOrEmpty<EntitySpec>())
            {
                return;
            }
            if (Get.RunConfig.ProgressDisabled && !Get.RunConfig.IsDailyChallenge)
            {
                return;
            }
            List<Place> list = Get.PlaceManager.Places.Where<Place>((Place x) => !Get.PlaceManager.GetPreviousPlaces(x).Any<Place>()).ToList<Place>();
            Dictionary<Place, int> dictionary = new Dictionary<Place, int>();
            for (int i = 0; i < list.Count; i++)
            {
                PlacesGenerator.< DecideEnemies > g__AssignEnemies | 5_1(list[i], null);
            }
            BFS<Place>.TraverseAll(list, new Func<Place, IEnumerable<Place>>(Get.PlaceManager.GetNextPlaces), dictionary, delegate (Place x)
            {
                List<Place> list2 = Get.PlaceManager.GetNextPlaces(x).ToList<Place>();
                if (list2.Count == 0)
                {
                    return;
                }
                for (int j = 0; j < list2.Count; j++)
                {
                    PlacesGenerator.< DecideEnemies > g__AssignEnemies | 5_1(list2[j], list2);
                }
            });
        }

        private static void DistributeModifiers()
        {
            List<Place> places = Get.PlaceManager.Places;
            List<Place> list = places.Where<Place>((Place x) => !Get.PlaceManager.GetPreviousPlaces(x).Any<Place>()).ToList<Place>();
            Dictionary<Place, int> dictionary = new Dictionary<Place, int>();
            Dictionary<Place, int> assignedDanger = new Dictionary<Place, int>();
            foreach (Place place in list)
            {
                assignedDanger.Add(place, 0);
            }
            BFS<Place>.TraverseAll(list, new Func<Place, IEnumerable<Place>>(Get.PlaceManager.GetNextPlaces), dictionary, delegate (Place x)
            {
                List<Place> list2 = Get.PlaceManager.GetNextPlaces(x).ToList<Place>();
                if (list2.Count == 0)
                {
                    return;
                }
                if (list2.Count == 1)
                {
                    if (!assignedDanger.ContainsKey(list2[0]))
                    {
                        if (PlacesGenerator.IsBoss(list2[0].Spec))
                        {
                            assignedDanger.Add(list2[0], 0);
                            return;
                        }
                        assignedDanger.Add(list2[0], Rand.RangeInclusive(-1, 1));
                    }
                    return;
                }
                bool flag = false;
                bool flag2 = false;
                foreach (Place place3 in list2)
                {
                    int num2;
                    if (assignedDanger.TryGetValue(place3, out num2))
                    {
                        if (num2 <= 0)
                        {
                            flag2 = true;
                        }
                        else
                        {
                            flag = true;
                        }
                    }
                }
                foreach (Place place4 in list2.InRandomOrder<Place>())
                {
                    if (!assignedDanger.ContainsKey(place4))
                    {
                        if (PlacesGenerator.IsBoss(place4.Spec))
                        {
                            assignedDanger.Add(place4, 0);
                            flag2 = true;
                        }
                        else if (!flag)
                        {
                            assignedDanger.Add(place4, 1);
                            flag = true;
                        }
                        else if (!flag2)
                        {
                            assignedDanger.Add(place4, Rand.RangeInclusive(-1, 0));
                            flag2 = true;
                        }
                        else
                        {
                            assignedDanger.Add(place4, Rand.RangeInclusive(-1, 1));
                        }
                    }
                }
            });
            foreach (Place place2 in places)
            {
                int num = assignedDanger[place2];
                PlacesGenerator.AssignDanger(place2, num);
            }
        }

        private static void AssignDanger(Place place, int danger)
        {
            float num;
            if (PlacesGenerator.IsBoss(place.Spec) || !Get.PlaceManager.GetPreviousPlaces(place).Any<Place>())
            {
                num = 1f;
            }
            else
            {
                int num2 = Rand.RangeInclusive(-1, 1);
                if (num2 == -1)
                {
                    num = 0.85f;
                }
                else if (num2 == 0)
                {
                    num = 1f;
                }
                else
                {
                    num = 1.15f;
                }
            }
            Place.CrystalType? crystalType;
            if (!PlacesGenerator.IsBoss(place.Spec))
            {
                crystalType = new Place.CrystalType?(Rand.Element<Place.CrystalType>(Place.CrystalType.Red, Place.CrystalType.Green, Place.CrystalType.Blue));
            }
            else
            {
                crystalType = null;
            }
            if (danger == -1)
            {
                place.PlaceGen_AssignDanger(0.85f, num, crystalType);
                return;
            }
            if (danger == 0)
            {
                place.PlaceGen_AssignDanger(1f, num, crystalType);
                return;
            }
            if (danger == 1)
            {
                place.PlaceGen_AssignDanger(1.15f, num, crystalType);
            }
        }

        private static void DistributeLinkRewards()
        {
            foreach (PlaceLink placeLink in Get.PlaceManager.Links)
            {
                PlacesGenerator.AssignLinkReward(placeLink);
            }
        }

        private static void AssignLinkReward(PlaceLink link)
        {
            Place to = link.To;
            int num = Rand.RangeInclusive(0, 2);
            while (Get.RunConfig.ProgressDisabled && num == 1)
            {
                num = Rand.RangeInclusive(0, 2);
            }
            int num2 = 0;
            if (to.EnemyCountFactor > 1f && Rand.Chance(0.5f))
            {
                num2 = 5;
            }
            if (num == 0)
            {
                int num3 = Calc.RoundToIntHalfUp((float)Rand.RangeInclusive(20, 40) * to.EnemyCountFactor);
                if (Get.UnlockableManager.IsUnlocked(Get.Unlockable_GoldBoost, null))
                {
                    num3 = Calc.RoundToIntHalfUp((float)num3 * 1.25f);
                }
                link.PlaceGen_AssignReward(new EntitySpecWithCount?(new EntitySpecWithCount(Get.Entity_Gold, num3)), num2, 0);
                return;
            }
            if (num == 1)
            {
                int num4 = Calc.RoundToIntHalfUp((float)Rand.RangeInclusive(10, 20) * to.EnemyCountFactor * Calc.Clamp(Calc.Pow(1.15f, (float)(to.Floor - 2)), 1f, 10f));
                if (Get.UnlockableManager.IsUnlocked(Get.Unlockable_StardustBoost, null))
                {
                    num4 = Calc.RoundToIntHalfUp((float)num4 * 1.5f);
                }
                link.PlaceGen_AssignReward(new EntitySpecWithCount?(new EntitySpecWithCount(Get.Entity_Stardust, num4)), num2, 0);
                return;
            }
            EntitySpec entitySpec = Rand.Element<EntitySpec>(Get.Entity_Sword, Get.Entity_Mace, Get.Entity_Axe);
            int desiredRampUpOnFloorWithRandomVariation = RampUpUtility.GetDesiredRampUpOnFloorWithRandomVariation(link.From.Floor);
            link.PlaceGen_AssignReward(new EntitySpecWithCount?(new EntitySpecWithCount(entitySpec, 1)), num2, desiredRampUpOnFloorWithRandomVariation);
        }

        private static void AddShelters()
        {
            if (!Get.RunSpec.HasShelters || Get.DungeonModifier_NoShelters.IsActiveAndAppliesToCurrentRun())
            {
                return;
            }
            PlacesGenerator.<> c__DisplayClass10_0 CS$<> 8__locals1;
            CS$<> 8__locals1.placesCopy = Get.PlaceManager.Places.ToList<Place>();
            foreach (Place place in CS$<> 8__locals1.placesCopy)
			{
                if (!Get.PlaceManager.GetNextPlaces(place).Any<Place>((Place x) => PlacesGenerator.IsBoss(x.Spec)) && place.Spec != Get.Place_DemonsLair && Get.PlaceManager.GetNextPlaces(place).Any<Place>())
                {
                    Vector2 vector = new Vector2(place.PositionOnMap.x, place.PositionOnMap.y + 1f);
                    if (PlacesGenerator.< AddShelters > g__CollidesWithAnyPlaceVisually | 10_0(vector, ref CS$<> 8__locals1))
                    {
                        foreach (Place place2 in CS$<> 8__locals1.placesCopy)
						{
                            if (place2.PositionOnMap.y >= vector.y - 0.4999f)
                            {
                                place2.PlaceGen_ChangePosition(place2.PositionOnMap.WithAddedY(1f));
                            }
                        }
                    }
                    Place place3 = new Place(Get.Place_Shelter, vector, place.Floor, null);
                    new Instruction_AddPlace(place3).Do();
                    foreach (PlaceLink placeLink in Get.PlaceManager.GetLinksFrom(place).ToList<PlaceLink>())
                    {
                        placeLink.PlaceGen_RelinkFrom(place3);
                    }
                    new Instruction_AddPlaceLink(new PlaceLink(place, place3)).Do();
                    ValueTuple<EntitySpec, int, int> valueTuple = PlacesGenerator.GenerateShelterFreeItem(place3);
                    place3.PlaceGen_AssignShelterReward(valueTuple.Item1, valueTuple.Item2, valueTuple.Item3);
                }
            }
        }

        private static ValueTuple<EntitySpec, int, int> GenerateShelterFreeItem(Place shelter)
        {
            int num = Rand.RangeInclusive(0, 2);
            while (Get.RunConfig.ProgressDisabled && num == 1)
            {
                num = Rand.RangeInclusive(0, 2);
            }
            if (num == 0)
            {
                int num2 = Calc.RoundToIntHalfUp((float)Rand.RangeInclusive(20, 40));
                if (Get.UnlockableManager.IsUnlocked(Get.Unlockable_GoldBoost, null))
                {
                    num2 = Calc.RoundToIntHalfUp((float)num2 * 1.25f);
                }
                return new ValueTuple<EntitySpec, int, int>(Get.Entity_Gold, num2, 0);
            }
            if (num == 1)
            {
                int num3 = Calc.RoundToIntHalfUp((float)Rand.RangeInclusive(10, 20) * Calc.Clamp(Calc.Pow(1.15f, (float)(shelter.Floor - 1)), 1f, 10f));
                if (Get.UnlockableManager.IsUnlocked(Get.Unlockable_StardustBoost, null))
                {
                    num3 = Calc.RoundToIntHalfUp((float)num3 * 1.5f);
                }
                return new ValueTuple<EntitySpec, int, int>(Get.Entity_Stardust, num3, 0);
            }
            EntitySpec entitySpec = Rand.Element<EntitySpec>(Get.Entity_Sword, Get.Entity_Mace, Get.Entity_Axe);
            int desiredRampUpOnFloorWithRandomVariation = RampUpUtility.GetDesiredRampUpOnFloorWithRandomVariation(shelter.Floor);
            return new ValueTuple<EntitySpec, int, int>(entitySpec, 1, desiredRampUpOnFloorWithRandomVariation);
        }

        public static void GenerateNextPlaceIfNone()
        {
            if (Get.PlaceManager.Places.Count == 0 || Get.Place == null)
            {
                return;
            }
            if (Get.PlaceManager.GetNextPlaces(Get.Place).Any<Place>())
            {
                return;
            }
            int? floorCount = Get.RunSpec.FloorCount;
            int floor = Get.Floor;
            if ((floorCount.GetValueOrDefault() == floor) & (floorCount != null))
            {
                return;
            }
            List<int> placeCounts = Get.RunSpec.PlaceCounts;
            int num = 4;
            Place place = new Place(Get.Place_Normal, new Vector2((float)(num - 1) / 2f, Get.Place.PositionOnMap.y + 1f), Get.Place.Floor + 1, PlacesGenerator.GeneratePlaceName(Get.Place_Normal));
            new Instruction_AddPlace(place).Do();
            new Instruction_AddPlaceLink(new PlaceLink(Get.Place, place)).Do();
            PlacesGenerator.AssignDanger(place, Rand.RangeInclusive(-1, 1));
        }

        private static void DistributeSpirits()
        {
            if (Get.PlaceManager.Places.Count == 0 || Get.RunSpec.SpiritsCount <= 0)
            {
                return;
            }
            Dictionary<Place, int> distances = new Dictionary<Place, int>();
            int num = Get.PlaceManager.Places.Max<Place>((Place x) => x.Floor);
            bool anyShelter = Get.PlaceManager.Places.Any<Place>((Place x) => x.Spec == Get.Place_Shelter);
            List<int> list = (from x in Enumerable.Range(1, num)
                              where Get.PlaceManager.Places.Any<Place>((Place y) => y.Floor == x && y.Spec == Get.Place_Normal)
                              select x).Where<int>(delegate (int x)
                          {
                              if (!anyShelter)
                              {
                                  return true;
                              }
                              IEnumerable<Place> enumerable = Get.PlaceManager.Places.Where<Place>((Place y) => y.Floor == x && y.Spec == Get.Place_Normal);
                              distances.Clear();
                              BFS<Place>.TraverseAll(enumerable, new Func<Place, IEnumerable<Place>>(Get.PlaceManager.GetNextPlaces), distances, null);
                              return distances.Any<KeyValuePair<Place, int>>((KeyValuePair<Place, int> y) => y.Key.Spec == Get.Place_Shelter);
                          }).InRandomOrder<int>().ToList<int>();
            if (list.Count < Get.RunSpec.SpiritsCount)
            {
                Log.Warning("Not enough floors to distribute all spirits.", false);
            }
            foreach (int num2 in list.Take<int>(Get.RunSpec.SpiritsCount))
            {
                foreach (Place place in Get.PlaceManager.Places)
                {
                    if (place.Floor == num2 && place.Spec == Get.Place_Normal)
                    {
                        place.PlaceGen_SetHasSpirit();
                    }
                }
            }
        }

        private static bool IsBoss(PlaceSpec spec)
        {
            return spec == Get.Place_GoogonsLair || spec == Get.Place_DemonsLair;
        }

        private static string GeneratePlaceName(PlaceSpec spec)
        {
            List<Place> places = Get.PlaceManager.Places;
            if (spec.PossibleNames.NullOrEmpty<string>())
            {
                return null;
            }
            string text;
            if (spec.PossibleNames.TryGetRandomElementWhere<string>((string x) => !places.Any<Place>((Place y) => y.Name == x), out text))
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
        internal static void <DecideEnemies>g__AssignEnemies|5_1(Place toPlace, List<Place> siblings)
		{
			if (!toPlace.Enemies.NullOrEmpty<EntitySpec>())
			{
				return;
			}
			if (toPlace.Spec == Get.Place_Shelter || PlacesGenerator.IsBoss(toPlace.Spec))
			{
				return;
			}
			if (Get.RunSpec == Get.Run_Main3 && toPlace.Floor == 1 && toPlace.Spec == Get.Place_Normal)
			{
				toPlace.PlaceGen_AssignEnemies(new List<EntitySpec> { Get.Entity_Tangler
    });
				return;
			}
if (Get.RunSpec == Get.Run_Main4 && toPlace.Floor == 1 && toPlace.Spec == Get.Place_Normal)
{
    toPlace.PlaceGen_AssignEnemies(new List<EntitySpec>
                {
                    Get.Entity_Skeleton,
                    Get.Entity_Trickster
                });
    return;
}
List<EntitySpec> chosen = new List<EntitySpec>();
List<EntitySpec> possibleEnemies = Get.RunSpec.Enemies.Where<EntitySpec>((EntitySpec x) => toPlace.Floor >= x.Actor.GenerateMinFloor).ToList<EntitySpec>();
int i = 0;
Func<EntitySpec, bool> <> 9__6;
Func<EntitySpec, int> <> 9__4;
Func<Place, bool> <> 9__5;
while (i < 10)
{
    chosen.Clear();
    for (int j = 0; j < 2; j++)
    {
        IEnumerable<EntitySpec> possibleEnemies2 = possibleEnemies;
        Func<EntitySpec, bool> func;
        if ((func = <> 9__6) == null)
        {
            func = (<> 9__6 = (EntitySpec x) => !chosen.Contains(x));
        }
        EntitySpec entitySpec;
        if (!possibleEnemies2.Where<EntitySpec>(func).TryGetRandomElement<EntitySpec>(out entitySpec, (EntitySpec x) => x.Actor.GenerateSelectionWeight))
        {
            break;
        }
        chosen.Add(entitySpec);
    }
    List<EntitySpec> chosen2 = chosen;
    Func<EntitySpec, int> func2;
    if ((func2 = <> 9__4) == null)
    {
        func2 = (<> 9__4 = (EntitySpec x) => possibleEnemies.IndexOf(x));
    }
    chosen2.StableSort<EntitySpec, int>(func2);
    if (i != 9 && siblings != null)
    {
        Func<Place, bool> func3;
        if ((func3 = <> 9__5) == null)
        {
            func3 = (<> 9__5 = (Place x) => x.Enemies != null && x.Enemies.SequenceEqual<EntitySpec>(chosen));
        }
        if (siblings.Any<Place>(func3))
        {
            i++;
            continue;
        }
    }
    toPlace.PlaceGen_AssignEnemies(chosen);
    return;
}
		}

		[CompilerGenerated]
internal static bool < AddShelters > g__CollidesWithAnyPlaceVisually | 10_0(Vector2 pos, ref PlacesGenerator.<> c__DisplayClass10_0 A_1)

        {
    using (List<Place>.Enumerator enumerator = A_1.placesCopy.GetEnumerator())
    {
        while (enumerator.MoveNext())
        {
            if (RectUtility.CenteredAt(enumerator.Current.PositionOnMap, 0.999f).Overlaps(RectUtility.CenteredAt(pos, 0.999f)))
            {
                return true;
            }
        }
    }
    return false;
}

private const int AssumedMaxPlacesPerLine = 4;

public const int GoogonFloor = 5;

public const int DemonFloor = 9;
	}
}