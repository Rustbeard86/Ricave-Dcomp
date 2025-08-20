using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public static class ItemGenerator
    {
        private static IEnumerable<EntitySpec> Items
        {
            get
            {
                return from x in Get.Specs.GetAll<EntitySpec>()
                       where x.IsItem
                       select x;
            }
        }

        public static Item CommonWeapon(bool canAddCurseAndEnchantment = true)
        {
            EntitySpec entitySpec;
            ItemGenerator.Items.Where<EntitySpec>((EntitySpec x) => x.Item.IsEquippableWeapon && x.Item.Generator_IsCommonGear).TryGetRandomElement<EntitySpec>(out entitySpec);
            return Maker.Make<Item>(entitySpec, delegate (Item x)
            {
                x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                if (canAddCurseAndEnchantment)
                {
                    ItemGenerator.RandomizeCurseAndEnchantment(x);
                }
            }, false, false, true);
        }

        public static Item CommonWearable(bool canAddCurseAndEnchantment = true)
        {
            EntitySpec entitySpec;
            ItemGenerator.Items.Where<EntitySpec>((EntitySpec x) => x.Item.IsEquippableNonWeapon && x.Item.Generator_IsCommonGear).TryGetRandomElement<EntitySpec>(out entitySpec);
            return Maker.Make<Item>(entitySpec, delegate (Item x)
            {
                x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                if (canAddCurseAndEnchantment)
                {
                    ItemGenerator.RandomizeCurseAndEnchantment(x);
                }
            }, false, false, true);
        }

        private static void RandomizeCurseAndEnchantment(Item item)
        {
            if (item.Spec.CanBeCursed)
            {
                item.Cursed = Rand.Bool;
                if (Rand.Chance(0.3f))
                {
                    ItemGenerator.AddEnchantmentOrPenalty(item);
                }
            }
        }

        public static Item Potion(bool allowHealthAndMana = false)
        {
            EntitySpec entitySpec;
            ItemGenerator.Items.Where<EntitySpec>((EntitySpec x) => x.Item.Generator_IsPotion && (allowHealthAndMana || x != Get.Entity_Potion_Mana) && (allowHealthAndMana || x != Get.Entity_Potion_Health)).TryGetRandomElement<EntitySpec>(out entitySpec);
            return Maker.Make<Item>(entitySpec, delegate (Item x)
            {
                x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
            }, false, false, true);
        }

        public static Item Wand()
        {
            EntitySpec entitySpec;
            ItemGenerator.Items.Where<EntitySpec>((EntitySpec x) => x.Item.Generator_IsWand).TryGetRandomElement<EntitySpec>(out entitySpec);
            return Maker.Make<Item>(entitySpec, delegate (Item x)
            {
                x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
            }, false, false, true);
        }

        public static Item Scroll(bool allowIdentificationAndCurseRemoval = false)
        {
            EntitySpec entitySpec;
            ItemGenerator.Items.Where<EntitySpec>((EntitySpec x) => x.Item.Generator_IsScroll && (allowIdentificationAndCurseRemoval || x != Get.Entity_Scroll_Identification) && (allowIdentificationAndCurseRemoval || x != Get.Entity_Scroll_CurseRemoval)).TryGetRandomElement<EntitySpec>(out entitySpec);
            return Maker.Make<Item>(entitySpec, delegate (Item x)
            {
                x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
            }, false, false, true);
        }

        public static Item Bomb()
        {
            EntitySpec entitySpec;
            ItemGenerator.Items.Where<EntitySpec>((EntitySpec x) => x.Item.Generator_IsBomb).TryGetRandomElement<EntitySpec>(out entitySpec);
            return Maker.Make<Item>(entitySpec, delegate (Item x)
            {
                x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
            }, false, false, true);
        }

        public static Item Trap()
        {
            EntitySpec entitySpec;
            ItemGenerator.Items.Where<EntitySpec>((EntitySpec x) => x.Item.Generator_IsTrap).TryGetRandomElement<EntitySpec>(out entitySpec);
            return Maker.Make<Item>(entitySpec, delegate (Item x)
            {
                x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
            }, false, false, true);
        }

        public static Item Gold(int amount)
        {
            return Maker.Make<Item>(Get.Entity_Gold, delegate (Item x)
            {
                x.StackCount = amount;
            }, false, false, true);
        }

        public static Item Stardust(int amount)
        {
            return Maker.Make<Item>(Get.Entity_Stardust, delegate (Item x)
            {
                x.StackCount = amount;
            }, false, false, true);
        }

        public static Item Diamonds(int amount)
        {
            return Maker.Make<Item>(Get.Entity_Diamond, delegate (Item x)
            {
                x.StackCount = amount;
            }, false, false, true);
        }

        public static Item SmallReward(bool allowGold = true)
        {
            if (!allowGold || Rand.Chance(0.4f))
            {
                return Rand.Element<Func<Item>>(() => Maker.Make<Item>(Get.Entity_Rope, null, false, false, true), () => Maker.Make<Item>(Get.Entity_ThrowingKnife, delegate (Item x)
                {
                    x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                }, false, false, true), () => Maker.Make<Item>(Get.Entity_PoisonThrowingKnife, delegate (Item x)
                {
                    x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                }, false, false, true), () => Maker.Make<Item>(Get.Entity_Stone, null, false, false, true), () => Maker.Make<Item>(Get.Entity_VineSeed, null, false, false, true), () => Maker.Make<Item>(Get.Entity_StaminaBooster, null, false, false, true), () => Maker.Make<Item>(Get.Entity_ChainItem, null, false, false, true))();
            }
            return ItemGenerator.Gold(Rand.RangeInclusive(5, 10));
        }

        public static Item Reward(bool allowGold = true)
        {
            if (!allowGold || Rand.Chance(0.55f))
            {
                return Rand.Element<Func<Item>>(() => Maker.Make<Item>(Get.Entity_FishOil, null, false, false, true), () => Maker.Make<Item>(Get.Entity_PocketKnife, delegate (Item x)
                {
                    x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                }, false, false, true), () => Maker.Make<Item>(Get.Entity_Shuriken, delegate (Item x)
                {
                    x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                }, false, false, true), () => Maker.Make<Item>(Get.Entity_PoisonShuriken, delegate (Item x)
                {
                    x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                }, false, false, true), () => Maker.Make<Item>(Get.Entity_TeleportationOrb, null, false, false, true), () => ItemGenerator.Wand(), () => Maker.Make<Item>(Get.Entity_EmptyVial, null, false, false, true), () => Maker.Make<Item>(Get.Entity_Medkit, null, false, false, true), () => ItemGenerator.Trap())();
            }
            return ItemGenerator.Gold(Rand.RangeInclusive(20, 40));
        }

        public static Item GoodReward()
        {
            List<EntitySpec> list = new List<EntitySpec>();
            list.AddRange(ItemGenerator.Items.Where<EntitySpec>((EntitySpec x) => x.Item.Generator_IsGoodReward));
            list.RemoveAll((EntitySpec x) => !ItemGenerator.IsUnique(x));
            EntitySpec entitySpec;
            if (list.TryGetRandomElement<EntitySpec>(out entitySpec))
            {
                return Maker.Make<Item>(entitySpec, delegate (Item x)
                {
                    x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                    ItemGenerator.RandomizeCurseAndEnchantment(x);
                }, false, false, true);
            }
            return ItemGenerator.Gold(100);
        }

        private static bool IsUnique(EntitySpec spec)
        {
            if (spec.Item.Stackable)
            {
                return true;
            }
            if (Get.Player.PartyHasAnyItemOfSpec(spec))
            {
                return false;
            }
            if (Get.World.AnyEntityOfSpec(spec))
            {
                return false;
            }
            if (WorldGen.Working && Get.WorldGenMemory.baseItems.Any<Item>((Item x) => x.Spec == spec))
            {
                return false;
            }
            if (WorldGen.Working && Get.WorldGenMemory.baseActors.Any<Actor>((Actor x) => x.Inventory.HasAnyItemOfSpec(spec)))
            {
                return false;
            }
            Func<Entity, bool> <> 9__2;
            foreach (Structure structure in Get.World.GetEntitiesOfSpec(Get.Entity_Chest).Concat<Entity>(Get.World.GetEntitiesOfSpec(Get.Entity_SilverChest)).Cast<Structure>())
            {
                IEnumerable<Entity> innerEntities = structure.InnerEntities;
                Func<Entity, bool> func;
                if ((func = <> 9__2) == null)
                {
                    func = (<> 9__2 = (Entity x) => x.Spec == spec);
                }
                if (innerEntities.Any<Entity>(func))
                {
                    return false;
                }
            }
            foreach (Entity entity in Get.World.GetEntitiesWithComp<SacrificialAltarComp>())
            {
                Item reward = entity.GetComp<SacrificialAltarComp>().Reward;
                if (((reward != null) ? reward.Spec : null) == spec)
                {
                    return false;
                }
            }
            using (List<Actor>.Enumerator enumerator3 = Get.World.Actors.GetEnumerator())
            {
                while (enumerator3.MoveNext())
                {
                    if (enumerator3.Current.Inventory.HasAnyItemOfSpec(spec))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static void AddEnchantmentOrPenalty(Item item)
        {
            if (item.Spec.Item.IsEquippableWeapon)
            {
                ItemGenerator.AddEnchantmentOrPenalty_Weapon(item);
                return;
            }
            if (item.Spec.Item.IsEquippableNonWeapon)
            {
                ItemGenerator.AddEnchantmentOrPenalty_Wearable(item);
            }
        }

        private static void AddEnchantmentOrPenalty_Weapon(Item item)
        {
            if (item.Cursed)
            {
                int num = Rand.RangeInclusive(0, 2);
                UseEffect useEffect = null;
                TitleSpec titleSpec = null;
                switch (num)
                {
                    case 0:
                        useEffect = new UseEffect_AddCondition(Get.UseEffect_AddCondition, new Condition_MaxHPOffset(Get.Condition_MaxHPOffset, 2, 0), 0.5f, UseEffect_AddCondition.StackMode.NeverSameSpecAndOrigin, 0);
                        titleSpec = Get.Title_TargetMaxHP;
                        break;
                    case 1:
                        useEffect = new UseEffect_AddCondition(Get.UseEffect_AddCondition, new Condition_Levitating(Get.Condition_Levitating), 1f, UseEffect_AddCondition.StackMode.NeverSameSpec, 0);
                        titleSpec = Get.Title_TargetLevitation;
                        break;
                    case 2:
                        useEffect = new UseEffect_MakeNoise(Get.UseEffect_MakeNoise, NoiseType.LoudInteraction, 3, false, true, true);
                        titleSpec = Get.Title_Noisy;
                        break;
                }
                item.UseEffects.AddUseEffect(useEffect, -1);
                item.Title = titleSpec;
                return;
            }
            int num2 = Rand.RangeInclusive(0, 5);
            UseEffect useEffect2 = null;
            TitleSpec titleSpec2 = null;
            switch (num2)
            {
                case 0:
                    useEffect2 = new UseEffect_AddCondition(Get.UseEffect_AddCondition, new Condition_Frozen(Get.Condition_Frozen, 1), 0.17f, UseEffect_AddCondition.StackMode.CanStack, 0);
                    titleSpec2 = Get.Title_Freezing;
                    break;
                case 1:
                    useEffect2 = new UseEffect_AddCondition(Get.UseEffect_AddCondition, new Condition_Drunk(Get.Condition_Drunk, 3), 0.17f, UseEffect_AddCondition.StackMode.CanStack, 0);
                    titleSpec2 = Get.Title_Drunkenness;
                    break;
                case 2:
                    useEffect2 = new UseEffect_AddCondition(Get.UseEffect_AddCondition, new Condition_Poisoned(Get.Condition_Poisoned, 1, 10), 0.17f, UseEffect_AddCondition.StackMode.NeverSameSpec, 0);
                    titleSpec2 = Get.Title_Poison;
                    break;
                case 3:
                    useEffect2 = new UseEffect_TeleportRandomly(Get.UseEffect_TeleportRandomly, 1f, 6);
                    titleSpec2 = Get.Title_RandomTeleportation;
                    break;
                case 4:
                    useEffect2 = new UseEffect_Push(Get.UseEffect_Push, 2, 1f, 5);
                    titleSpec2 = Get.Title_Push;
                    break;
                case 5:
                    useEffect2 = new UseEffect_SetOnFire(Get.UseEffect_SetOnFire, 0.17f, 0);
                    titleSpec2 = Get.Title_Fire;
                    break;
            }
            item.UseEffects.AddUseEffect(useEffect2, -1);
            item.Title = titleSpec2;
        }

        private static void AddEnchantmentOrPenalty_Wearable(Item item)
        {
            if (item.Cursed)
            {
                int num = Rand.RangeInclusive(0, 3);
                Condition condition = null;
                TitleSpec titleSpec = null;
                switch (num)
                {
                    case 0:
                        condition = new Condition_MaxHPOffset(Get.Condition_MaxHPOffset, -4, 0);
                        titleSpec = Get.Title_Fragility;
                        break;
                    case 1:
                        condition = new Condition_SeeRangeOffset(Get.Condition_SeeRangeOffset, -1, 0);
                        titleSpec = Get.Title_Darkness;
                        break;
                    case 2:
                        condition = new Condition_DamageOverTime(Get.Condition_DamageOverTime, 50, 1, Get.DamageType_Decay, 0);
                        titleSpec = Get.Title_Death;
                        break;
                    case 3:
                        condition = new Condition_MaxStaminaOffset(Get.Condition_MaxStaminaOffset, -2, 0);
                        titleSpec = Get.Title_Fatigue;
                        break;
                }
                item.ConditionsEquipped.AddCondition(condition, -1);
                item.Title = titleSpec;
                return;
            }
            int num2 = Rand.RangeInclusive(0, 2);
            Condition condition2 = null;
            TitleSpec titleSpec2 = null;
            switch (num2)
            {
                case 0:
                    condition2 = new Condition_MaxHPOffset(Get.Condition_MaxHPOffset, 3, 0);
                    titleSpec2 = Get.Title_Life;
                    break;
                case 1:
                    condition2 = new Condition_SeeRangeOffset(Get.Condition_SeeRangeOffset, 1, 0);
                    titleSpec2 = Get.Title_Awareness;
                    break;
                case 2:
                    condition2 = new Condition_MaxStaminaOffset(Get.Condition_MaxStaminaOffset, 2, 0);
                    titleSpec2 = Get.Title_Endurance;
                    break;
            }
            item.ConditionsEquipped.AddCondition(condition2, -1);
            item.Title = titleSpec2;
        }

        public static Item Ring(bool onlyGood = false)
        {
            return Maker.Make<Item>(Get.Entity_Ring, delegate (Item x)
            {
                x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                x.Cursed = !onlyGood && Rand.Bool;
                if (x.Cursed)
                {
                    int num = Rand.RangeInclusive(0, 3);
                    Condition condition = null;
                    TitleSpec titleSpec = null;
                    switch (num)
                    {
                        case 0:
                            condition = new Condition_MaxHPOffset(Get.Condition_MaxHPOffset, -1, 0);
                            titleSpec = Get.Title_Fragility;
                            break;
                        case 1:
                            condition = new Condition_SeeRangeOffset(Get.Condition_SeeRangeOffset, -1, 0);
                            titleSpec = Get.Title_Darkness;
                            break;
                        case 2:
                            condition = new Condition_MaxStaminaOffset(Get.Condition_MaxStaminaOffset, -1, 0);
                            titleSpec = Get.Title_Fatigue;
                            break;
                        case 3:
                            condition = new Condition_HungerRateMultiplier(Get.Condition_HungerRateMultiplier, 2f);
                            titleSpec = Get.Title_Hunger;
                            break;
                    }
                    x.ConditionsEquipped.AddCondition(condition, -1);
                    x.Title = titleSpec;
                    return;
                }
                int num2 = Rand.RangeInclusive(0, 4);
                Condition condition2 = null;
                TitleSpec titleSpec2 = null;
                switch (num2)
                {
                    case 0:
                        condition2 = new Condition_MaxHPOffset(Get.Condition_MaxHPOffset, 1, 0);
                        titleSpec2 = Get.Title_Life;
                        break;
                    case 1:
                        condition2 = new Condition_SeeRangeOffset(Get.Condition_SeeRangeOffset, 1, 0);
                        titleSpec2 = Get.Title_Awareness;
                        break;
                    case 2:
                        condition2 = new Condition_MaxStaminaOffset(Get.Condition_MaxStaminaOffset, 1, 0);
                        titleSpec2 = Get.Title_Endurance;
                        break;
                    case 3:
                        condition2 = new Condition_IdentificationRateMultiplier(Get.Condition_IdentificationRateMultiplier, 2f);
                        titleSpec2 = Get.Title_FasterIdentification;
                        break;
                    case 4:
                        condition2 = new Condition_IncomingDamageFactor(Get.Condition_IncomingDamageFactor, 0.2f, Get.DamageType_Fire, 0);
                        titleSpec2 = Get.Title_FireResistance;
                        break;
                }
                x.ConditionsEquipped.AddCondition(condition2, -1);
                x.Title = titleSpec2;
            }, false, false, true);
        }

        public static Item Amulet(bool onlyGood = false)
        {
            return Maker.Make<Item>(Get.Entity_Amulet, delegate (Item x)
            {
                x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                x.Cursed = !onlyGood && Rand.Bool;
                if (x.Cursed)
                {
                    int num = Rand.RangeInclusive(0, 1);
                    Condition condition = null;
                    TitleSpec titleSpec = null;
                    int num2 = 0;
                    if (num != 0)
                    {
                        if (num == 1)
                        {
                            condition = new Condition_IncomingDamageFactor(Get.Condition_IncomingDamageFactor, 1.2f, Get.DamageType_Physical, 0);
                            titleSpec = Get.Title_Vulnerability;
                            num2 = 5;
                        }
                    }
                    else
                    {
                        condition = new Condition_DealtDamageFactor(Get.Condition_DealtDamageFactor, 0.8f, Get.DamageType_Physical, 0);
                        titleSpec = Get.Title_Weakness;
                        num2 = 5;
                    }
                    x.ConditionsEquipped.AddCondition(condition, -1);
                    x.Title = titleSpec;
                    x.ChargesLeft = num2;
                    return;
                }
                int num3 = Rand.RangeInclusive(0, 1);
                Condition condition2 = null;
                TitleSpec titleSpec2 = null;
                int num4 = 0;
                if (num3 != 0)
                {
                    if (num3 == 1)
                    {
                        condition2 = new Condition_IncomingDamageFactor(Get.Condition_IncomingDamageFactor, 0.8f, Get.DamageType_Physical, 0);
                        titleSpec2 = Get.Title_Protection;
                        num4 = 5;
                    }
                }
                else
                {
                    condition2 = new Condition_DealtDamageFactor(Get.Condition_DealtDamageFactor, 1.2f, Get.DamageType_Physical, 0);
                    titleSpec2 = Get.Title_Strength;
                    num4 = 5;
                }
                x.ConditionsEquipped.AddCondition(condition2, -1);
                x.Title = titleSpec2;
                x.ChargesLeft = num4;
            }, false, false, true);
        }

        public const float EnchantmentOrPenaltyChance = 0.3f;
    }
}