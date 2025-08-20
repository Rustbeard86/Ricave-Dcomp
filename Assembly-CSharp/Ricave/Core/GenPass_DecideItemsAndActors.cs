using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class GenPass_DecideItemsAndActors : GenPass
    {
        public override int SeedPart
        {
            get
            {
                return 386970255;
            }
        }

        public override void DoPass(WorldGenMemory memory)
        {
            bool flag = memory.AllRooms.Any<Room>((Room x) => x.IsBossRoom);
            this.DecideItems(memory);
            this.DecideActors(memory, flag);
            this.GiveRandomLoot(memory);
            if (flag)
            {
                this.AddBoss(memory, false);
                if (Get.DungeonModifier_ExtraMiniBoss.IsActiveAndAppliesToCurrentRun() && memory.config.Floor != 1)
                {
                    this.AddBoss(memory, true);
                }
            }
            this.DoErrorChecks(memory);
        }

        private void DecideItems(WorldGenMemory memory)
        {
            memory.baseItems = new List<Item>();
            if (memory.AllRooms.Any<Room>((Room x) => x.Role == Room.LayoutRole.LockedBehindSilverDoor))
            {
                memory.baseItems.Add(Maker.Make<Item>(Get.Entity_SilverKey, null, false, false, true));
            }
            List<Item> list = new List<Item>();
            list.Add(ItemGenerator.CommonWeapon(false));
            list.Add(ItemGenerator.CommonWearable(false));
            list.Add(Rand.Bool ? ItemGenerator.CommonWeapon(false) : ItemGenerator.CommonWearable(false));
            this.DistributeCurse(list);
            memory.baseItems.AddRange(list);
            memory.baseItems.Add(Maker.Make<Item>(Get.Entity_Bandage, delegate (Item x)
            {
                x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
            }, false, false, true));
            memory.baseItems.Add(Maker.Make<Item>(Get.Entity_Goo, delegate (Item x)
            {
                x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
            }, false, false, true));
            int num = Math.Max(Get.Difficulty.UpToHealthPotionsPerFloor - Get.Player.GetPartyInventoryCount(Get.Entity_Potion_Health), 0);
            for (int i = 0; i < num; i++)
            {
                memory.baseItems.Add(Maker.Make<Item>(Get.Entity_Potion_Health, delegate (Item x)
                {
                    x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                }, false, false, true));
            }
            for (int j = 0; j < Get.RunSpec.PotionsCount; j++)
            {
                memory.baseItems.Add(ItemGenerator.Potion(false));
            }
            for (int k = 0; k < Get.RunSpec.ScrollsCount; k++)
            {
                memory.baseItems.Add(ItemGenerator.Scroll(false));
            }
            memory.baseItems.Add(Maker.Make<Item>(Get.Entity_Scroll_Identification, delegate (Item x)
            {
                x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
            }, false, false, true));
            for (int l = 0; l < Get.Difficulty.BreadCountPerFloor; l++)
            {
                memory.baseItems.Add(Maker.Make<Item>(Get.Entity_Bread, null, false, false, true));
            }
            if (this.ShouldSpawnCurseRemovalScroll())
            {
                memory.baseItems.Add(Maker.Make<Item>(Get.Entity_Scroll_CurseRemoval, delegate (Item x)
                {
                    x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                }, false, false, true));
            }
            memory.baseItems.Add(ItemGenerator.Trap());
            if (Get.Skill_MoreTraps.IsUnlocked())
            {
                memory.baseItems.Add(ItemGenerator.Trap());
            }
            if (memory.config.Floor >= 2)
            {
                memory.baseItems.Add(ItemGenerator.Bomb());
            }
            if (memory.config.Floor >= 3 && memory.config.Floor % 2 == 1)
            {
                memory.baseItems.Add(Maker.Make<Item>(Get.Entity_EmptyVial, null, false, false, true));
            }
            if (memory.config.Floor >= 2)
            {
                memory.baseItems.Add(Maker.Make<Item>(Get.Entity_ThrowingKnife, delegate (Item x)
                {
                    x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                }, false, false, true));
            }
            if (Rand.Chance(0.5f))
            {
                memory.baseItems.Add(ItemGenerator.Ring(false));
            }
            List<EntitySpec> list2 = new List<EntitySpec>();
            if (!Get.Player.PartyHasAnyItemOfSpec(Get.Entity_Pickaxe))
            {
                list2.Add(Get.Entity_Pickaxe);
            }
            if (!Get.Player.PartyHasAnyItemOfSpec(Get.Entity_Shovel))
            {
                list2.Add(Get.Entity_Shovel);
            }
            EntitySpec entitySpec;
            if (list2.TryGetRandomElement<EntitySpec>(out entitySpec))
            {
                memory.baseItems.Add(Maker.Make<Item>(entitySpec, null, false, false, true));
            }
            if (memory.config.Floor % 2 == 0 || Get.Skill_MoreRawMeat.IsUnlocked())
            {
                memory.baseItems.Add(Maker.Make<Item>(Get.Entity_RawMeat, null, false, false, true));
            }
            if (!Get.RunConfig.ProgressDisabled)
            {
                int count = Get.TotalLobbyItems.GetCount(Get.Entity_PuzzlePiece);
                if (count < 10 && memory.config.Floor == count + 1 && Get.ThisRunLobbyItems.GetCount(Get.Entity_PuzzlePiece) <= 0)
                {
                    memory.baseItems.Add(Maker.Make<Item>(Get.Entity_PuzzlePiece, null, false, false, true));
                }
                if ((memory.config.Floor != 3 || !Get.Quest_MemoryPiece1.IsActive()) && (memory.config.Floor != 4 || !Get.Quest_MemoryPiece2.IsActive()) && (memory.config.Floor != 6 || !Get.Quest_MemoryPiece3.IsActive()) && (memory.config.Floor != 8 || !Get.Quest_MemoryPiece4.IsActive()))
                {
                    if (Rand.Chance(this.GetNoteSpawnChance()))
                    {
                        NoteSpec noteSpec;
                        if ((from x in Get.Specs.GetAll<NoteSpec>()
                             where !Get.Player.CollectedNoteSpecs.Contains(x) && !Get.Progress.CollectedNoteSpecs.Contains(x)
                             select x).TryGetRandomElement<NoteSpec>(out noteSpec))
                        {
                            memory.baseItems.Add(Maker.Make<Item>(Get.Entity_Note, delegate (Item x)
                            {
                                ((Note)x).NoteSpec = noteSpec;
                            }, false, false, true));
                        }
                    }
                }
                if (Get.RunSpec != Get.Run_Main1 && Get.RunSpec.IsMain && (memory.config.Floor == 2 || memory.config.Floor == 6))
                {
                    memory.baseItems.Add(Maker.Make<Item>(Get.Entity_PetRatCheese, null, false, false, true));
                }
            }
            if (WorldGenUtility.MiniChallengeForCurrentWorld == MiniChallenge.AncientMechanisms)
            {
                for (int m = 0; m < 3; m++)
                {
                    memory.baseItems.Add(Maker.Make<Item>(Get.Entity_AncientMechanism, null, false, false, true));
                }
            }
            if (memory.config.Floor == 2 && Get.Quest_MoonlightMedallions.IsActive())
            {
                for (int n = 0; n < 4; n++)
                {
                    Item item = Maker.Make<Item>(Get.Entity_MoonlightMedallion, null, false, false, true);
                    if (n == 0)
                    {
                        item.Title = Get.Title_Childhood;
                    }
                    else if (n == 1)
                    {
                        item.Title = Get.Title_Adolescence;
                    }
                    else if (n == 2)
                    {
                        item.Title = Get.Title_Adulthood;
                    }
                    else if (n == 3)
                    {
                        item.Title = Get.Title_Elderhood;
                    }
                    memory.baseItems.Add(item);
                }
            }
            if (memory.config.Floor == 4 && Get.Quest_FindRelic.IsActive())
            {
                PrivateRoomStructureAsItem privateRoomStructureAsItem = Maker.Make<PrivateRoomStructureAsItem>(Get.Entity_PrivateRoomStructureAsItem, null, false, false, true);
                privateRoomStructureAsItem.PrivateRoomStructure = Get.Entity_Relic;
                memory.baseItems.Add(privateRoomStructureAsItem);
            }
            if (memory.config.Floor == 6 && Get.Quest_MemoryPiece3.IsActive())
            {
                memory.baseItems.Add(Maker.Make<Item>(Get.Entity_Rope, null, false, false, true));
            }
            memory.unusedBaseItems = new List<Item>(memory.baseItems);
        }

        private void DecideActors(WorldGenMemory memory, bool hasBoss)
        {
            GenPass_DecideItemsAndActors.<> c__DisplayClass9_0 CS$<> 8__locals1 = new GenPass_DecideItemsAndActors.<> c__DisplayClass9_0();
            CS$<> 8__locals1.memory = memory;
            CS$<> 8__locals1.memory.baseActors = new List<Actor>();
            CS$<> 8__locals1.expLeft = Get.RunSpec.TotalEnemiesExpPerFloor;
            GenPass_DecideItemsAndActors.<> c__DisplayClass9_0 CS$<> 8__locals2 = CS$<> 8__locals1;
            float num = (float)CS$<> 8__locals1.expLeft* Get.Difficulty.EnemyCountPerFloorMultiplier * (GenPass_RoomLayout.HasUpperStorey(CS$<> 8__locals1.memory) ? 1.25f : 1f);
            Place place = CS$<> 8__locals1.memory.config.Place;
            CS$<> 8__locals2.expLeft = Calc.RoundToIntHalfUp(num * ((place != null) ? place.EnemyCountFactor : 1f) * (Get.DungeonModifier_MoreEnemies.IsActiveAndAppliesToCurrentRun() ? 1.2f : 1f) * (Get.DungeonModifier_FewerEnemies.IsActiveAndAppliesToCurrentRun() ? 0.8f : 1f) * (Get.RunConfig.HasPartyMember ? 1.1f : 1f));
            if (hasBoss)
            {
                CS$<> 8__locals1.expLeft -= 4;
            }
            this.AddSpecialHardcodedEnemies(CS$<> 8__locals1.memory, ref CS$<> 8__locals1.expLeft);
            Place place2 = Get.Place;
            List<EntitySpec> list = (((place2 != null) ? place2.Enemies : null).NullOrEmpty<EntitySpec>() ? (from x in Get.Specs.GetAll<EntitySpec>()
                                                                                                             where x.IsActor && x.Actor.KilledExperience > 0 && CS$<> 8__locals1.memory.config.Floor >= x.Actor.GenerateMinFloor && x.Actor.GenerateSelectionWeight > 0f && !x.Actor.AlwaysBoss

                select x).ToList<EntitySpec>() : Get.Place.Enemies);
            while (CS$<> 8__locals1.expLeft > 0)
			{
                IEnumerable<EntitySpec> enumerable = list;
                Func<EntitySpec, bool> func;
                if ((func = CS$<> 8__locals1.<> 9__1) == null)
				{
                    func = (CS$<> 8__locals1.<> 9__1 = (EntitySpec x) => x.Actor.KilledExperience <= CS$<> 8__locals1.expLeft && (x.Actor.MaxPerMap == -1 || CS$<> 8__locals1.memory.baseActors.Count<Actor>((Actor y) => y.Spec == x) < x.Actor.MaxPerMap));
                }
                EntitySpec entitySpec;
                if (!enumerable.Where<EntitySpec>(func).TryGetRandomElement<EntitySpec>(out entitySpec, (EntitySpec x) => x.Actor.GenerateSelectionWeight))
                {
                    break;
                }
                Actor actor = Maker.Make<Actor>(entitySpec, delegate (Actor x)
                {
                    x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                }, false, false, true);
                DifficultyUtility.AddConditionsForDifficulty(actor);
                actor.CalculateInitialHPManaAndStamina();
                actor.Conditions.AddCondition(new Condition_Sleeping(Get.Condition_Sleeping), -1);
                CS$<> 8__locals1.memory.baseActors.Add(actor);
                CS$<> 8__locals1.expLeft -= Math.Max(entitySpec.Actor.KilledExperience, 1);
            }
            int num2 = 1 + Get.Difficulty.ExtraBabies;
            for (int i = 0; i < num2; i++)
            {
                Actor actor2 = BabyGenerator.Baby();
                if (actor2 != null)
                {
                    actor2.Conditions.AddCondition(new Condition_Sleeping(Get.Condition_Sleeping), -1);
                    CS$<> 8__locals1.memory.baseActors.Add(actor2);
                }
            }
            string text;
            if (CS$<> 8__locals1.memory.config.Floor % 2 == 0 && NameGenerator.TryGenerateGhostName(out text))
			{
                Actor actor3 = Maker.Make<Actor>(Get.Entity_Ghost, delegate (Actor x)
                {
                    x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                }, false, false, true);
                DifficultyUtility.AddConditionsForDifficulty(actor3);
                actor3.CalculateInitialHPManaAndStamina();
                actor3.Name = text;
                if (!Get.Trait_Scary.IsChosen())
                {
                    actor3.Inventory.Add(ItemGenerator.Wand(), default(ValueTuple<Vector2Int?, int?, int?>));
                    if (!Get.RunConfig.ProgressDisabled)
                    {
                        actor3.Inventory.Add(ItemGenerator.Stardust(10), default(ValueTuple<Vector2Int?, int?, int?>));
                    }
                }
                CS$<> 8__locals1.memory.baseActors.Add(actor3);
            }
            if (CS$<> 8__locals1.memory.config.Floor == 2 && Get.Quest_RatInfestation.IsActive())
			{
                for (int j = 0; j < 5; j++)
                {
                    Actor actor4 = Maker.Make<Actor>(Get.Entity_Rat, delegate (Actor x)
                    {
                        x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                    }, false, false, true);
                    DifficultyUtility.AddConditionsForDifficulty(actor4);
                    actor4.CalculateInitialHPManaAndStamina();
                    CS$<> 8__locals1.memory.baseActors.Add(actor4);
                }
            }
            if (CS$<> 8__locals1.memory.config.Floor == 3 && Get.Quest_PhantomSwarm.IsActive())
			{
                for (int k = 0; k < 3; k++)
                {
                    Actor actor5 = Maker.Make<Actor>(Get.Entity_Phantom, delegate (Actor x)
                    {
                        x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                    }, false, false, true);
                    DifficultyUtility.AddConditionsForDifficulty(actor5);
                    actor5.CalculateInitialHPManaAndStamina();
                    CS$<> 8__locals1.memory.baseActors.Add(actor5);
                }
            }
            CS$<> 8__locals1.memory.unusedBaseActors = new List<Actor>(CS$<> 8__locals1.memory.baseActors);
        }

        private void DistributeCurse(List<Item> gear)
        {
            List<Item> list = gear.Where<Item>((Item x) => x.Spec.CanBeCursed).ToList<Item>();
            int num = list.Count / 2;
            if (list.Count % 2 == 1 && Rand.Bool)
            {
                num++;
            }
            for (int i = 0; i < num; i++)
            {
                Item item;
                if (!list.Where<Item>((Item x) => !x.Cursed).TryGetRandomElement<Item>(out item))
                {
                    break;
                }
                item.Cursed = true;
            }
            for (int j = 0; j < list.Count; j++)
            {
                if (Rand.Chance(0.3f))
                {
                    ItemGenerator.AddEnchantmentOrPenalty(list[j]);
                }
            }
        }

        private bool ShouldSpawnCurseRemovalScroll()
        {
            if (Get.Player.PartyHasAnyItemOfSpec(Get.Entity_Scroll_CurseRemoval))
            {
                return false;
            }
            foreach (Actor actor in Get.PlayerParty)
            {
                using (List<Item>.Enumerator enumerator2 = actor.Inventory.EquippedItems.GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        if (enumerator2.Current.Cursed)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void GiveRandomLoot(WorldGenMemory memory)
        {
            List<Actor> list = memory.baseActors.Where<Actor>((Actor x) => !x.IsBaby && !x.IsBoss && x.Spec != Get.Entity_Ghost && x.Spec != Get.Entity_Gorhorn && x.Spec != Get.Entity_Thief).ToList<Actor>();
            List<Item> list2 = memory.unusedBaseItems.Where<Item>((Item x) => x.Spec == Get.Entity_Bread).ToList<Item>();
            foreach (Actor actor in list.InRandomOrder<Actor>())
            {
                Item item;
                if (list2.TryGetRandomElement<Item>(out item))
                {
                    memory.unusedBaseItems.Remove(item);
                    list2.Remove(item);
                    actor.Inventory.Add(item, default(ValueTuple<Vector2Int?, int?, int?>));
                }
                else
                {
                    memory.goldWanters.Add(actor);
                }
            }
            if (memory.goldWanters.Count == 0)
            {
                memory.goldWanters.AddRange(list);
            }
            if (WorldGenUtility.MiniChallengeForCurrentWorld == MiniChallenge.GreenKeyFragments)
            {
                int i = 0;
                while (i < 3)
                {
                    Actor actor2;
                    if (list.Where<Actor>((Actor x) => !x.Inventory.Any).TryGetRandomElement<Actor>(out actor2))
                    {
                        goto IL_0161;
                    }
                    if (list.Where<Actor>((Actor x) => !x.Inventory.HasAnyItemOfSpec(Get.Entity_GreenKeyFragment)).TryGetRandomElement<Actor>(out actor2) || list.TryGetRandomElement<Actor>(out actor2))
                    {
                        goto IL_0161;
                    }
                IL_0185:
                    i++;
                    continue;
                IL_0161:
                    actor2.Inventory.Add(Maker.Make<Item>(Get.Entity_GreenKeyFragment, null, false, false, true), default(ValueTuple<Vector2Int?, int?, int?>));
                    goto IL_0185;
                }
            }
        }

        private void AddBoss(WorldGenMemory memory, bool secondary = false)
        {
            Actor actor = BossGenerator.Boss(!secondary);
            if (actor == null)
            {
                return;
            }
            if (!Get.Trait_BossHunter.IsChosen())
            {
                actor.Conditions.AddCondition(new Condition_Sleeping(Get.Condition_Sleeping), -1);
            }
            memory.baseActors.Add(actor);
            if (!secondary && Get.Quest_MemoryPiece2.IsActive() && memory.config.Floor == 4)
            {
                actor.Inventory.Add(Maker.Make<Item>(Get.Entity_MemoryPiece2, null, false, false, true), default(ValueTuple<Vector2Int?, int?, int?>));
            }
            actor.Inventory.Add(ItemGenerator.GoodReward(), default(ValueTuple<Vector2Int?, int?, int?>));
        }

        private void AddSpecialHardcodedEnemies(WorldGenMemory memory, ref int expLeft)
        {
            if (!Get.RunSpec.AllowSpecialEnemies)
            {
                return;
            }
            if (!Rand.Chance(0.2f))
            {
                return;
            }
            List<EntitySpec> list = new List<EntitySpec>();
            if (memory.config.Floor >= Get.Entity_Gorhorn.Actor.GenerateMinFloor)
            {
                list.Add(Get.Entity_Gorhorn);
            }
            if (memory.config.Floor >= Get.Entity_Thief.Actor.GenerateMinFloor)
            {
                list.Add(Get.Entity_Thief);
            }
            EntitySpec entitySpec;
            if (!list.TryGetRandomElement<EntitySpec>(out entitySpec))
            {
                return;
            }
            Actor actor = Maker.Make<Actor>(entitySpec, delegate (Actor x)
            {
                x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
            }, false, false, true);
            DifficultyUtility.AddConditionsForDifficulty(actor);
            actor.CalculateInitialHPManaAndStamina();
            if (entitySpec != Get.Entity_Thief)
            {
                actor.Conditions.AddCondition(new Condition_Sleeping(Get.Condition_Sleeping), -1);
            }
            memory.baseActors.Add(actor);
            expLeft -= entitySpec.Actor.KilledExperience;
        }

        private void DoErrorChecks(WorldGenMemory memory)
        {
            if (memory.baseItems.ContainsDuplicates<Item>())
            {
                Log.Error("memory.baseItems contains duplicates.", false);
            }
            if (memory.unusedBaseItems.ContainsDuplicates<Item>())
            {
                Log.Error("memory.unusedBaseItems contains duplicates.", false);
            }
            if (memory.baseActors.ContainsDuplicates<Actor>())
            {
                Log.Error("memory.baseActors contains duplicates.", false);
            }
            if (memory.unusedBaseActors.ContainsDuplicates<Actor>())
            {
                Log.Error("memory.unusedBaseActors contains duplicates.", false);
            }
            if (memory.unusedBaseItems.Any<Item>((Item x) => !memory.baseItems.Contains(x)))
            {
                Log.Error("memory.unusedBaseItems contains items which are not in memory.baseItems.", false);
            }
            if (memory.unusedBaseActors.Any<Actor>((Actor x) => !memory.baseActors.Contains(x)))
            {
                Log.Error("memory.unusedBaseActors contains actors which are not in memory.baseActors.", false);
            }
            if (memory.unusedBaseItems.Any<Item>((Item x) => memory.baseActors.Any<Actor>((Actor y) => y.Inventory.Contains(x))))
            {
                Log.Error("memory.unusedBaseItems contains items which are already in someone's inventory.", false);
            }
        }

        private float GetNoteSpawnChance()
        {
            int count = Get.Player.CollectedNoteSpecs.Count;
            int num = count + Get.Progress.CollectedNoteSpecs.Count;
            if (num >= Get.Specs.GetAll<NoteSpec>().Count)
            {
                return 0f;
            }
            if (num == 0)
            {
                return 1f;
            }
            float num2;
            if ((float)num >= (float)Get.Specs.GetAll<NoteSpec>().Count / 2f)
            {
                num2 = 0.5f;
            }
            else
            {
                num2 = 0.8f;
            }
            return num2 * Calc.Pow(0.5f, (float)count);
        }

        private const float ExpPerFloorMultiStoreyFactor = 1.25f;

        public const int AncientMechanismsCount = 3;

        public const int GreenKeyFragmentsCount = 3;

        public const float GoldBoostFactor = 1.25f;

        public const float EnemyCountFactorFromHavingParty = 1.1f;
    }
}