using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public static class MediaCreatorHelper
    {
        public static bool Running
        {
            get
            {
                return MediaCreatorHelper.running;
            }
        }

        public static void RandomizeEverything()
        {
            MediaCreatorHelper.extraInstructions.Clear();
            MediaCreatorHelper.running = true;
            try
            {
                new Instruction_Immediate(delegate
                {
                    MediaCreatorHelper.SetPlayerStats();
                    MediaCreatorHelper.DestroyInventory();
                    MediaCreatorHelper.AddWearables();
                    MediaCreatorHelper.AddWeapon();
                    MediaCreatorHelper.AddQuickbarItems();
                    MediaCreatorHelper.AddInventory();
                    MediaCreatorHelper.AddSpells();
                    MediaCreatorHelper.RandomizeTime();
                }).Do();
                foreach (Instruction instruction in MediaCreatorHelper.extraInstructions)
                {
                    instruction.Do();
                }
                MediaCreatorHelper.extraInstructions.Clear();
                foreach (Instruction instruction2 in MediaCreatorHelper.AddConditions())
                {
                    instruction2.Do();
                }
            }
            finally
            {
                MediaCreatorHelper.running = false;
            }
        }

        private static void SetPlayerStats()
        {
            int num = Rand.RangeInclusive(3, 15);
            Get.Player.Experience = LevelUtility.GetTotalExperienceRequired(num);
            float num2 = Rand.Range(0.2f, 0.8f);
            Get.Player.Experience += Calc.RoundToInt((float)Get.Player.ExperienceToNextLevel * num2);
            float num3 = Rand.Range(0.6f, 1f);
            Get.MainActor.HP = Calc.RoundToInt((float)Get.MainActor.MaxHP * num3);
            float num4 = Rand.Range(0.2f, 1f);
            Get.MainActor.Mana = Calc.RoundToInt((float)Get.MainActor.MaxMana * num4);
            float num5 = Rand.Range(0.7f, 1f);
            Get.MainActor.Stamina = Calc.RoundToInt((float)Get.MainActor.MaxStamina * num5);
            float num6 = Rand.Range(0.05f, 1f);
            Get.Player.Satiation = Calc.RoundToInt(600f * num6);
            Get.Player.TurnsCanRewind = Rand.RangeInclusive(1, 8);
            Get.Player.Gold = Rand.RangeInclusive(100, 2000);
        }

        private static void DestroyInventory()
        {
            foreach (Item item in Get.MainActor.Inventory.AllItems)
            {
                Get.MainActor.Inventory.Remove(item);
            }
        }

        private static void AddWearables()
        {
            int num = 1;
            if (Rand.Chance(0.5f))
            {
                num = 2;
            }
            for (int i = 0; i < num; i++)
            {
                EntitySpec entitySpec;
                if ((from x in Get.Specs.GetAll<EntitySpec>()
                     where x.IsItem && x.Item.IsEquippableNonWeapon
                     select x).TryGetRandomElement<EntitySpec>(out entitySpec) && !Get.MainActor.Inventory.Equipped.AnyEquippedItemCollidesWith(entitySpec))
                {
                    Item item = Maker.Make<Item>(entitySpec, null, false, false, true);
                    item.TurnsLeftToIdentify = 0;
                    item.RampUp = RampUpUtility.ResolveNonRepeatedRampUpFor(item, Rand.RangeInclusive(0, 7));
                    if (Rand.Bool)
                    {
                        MediaCreatorHelper.extraInstructions.AddRange(UseEffect_EnchantChosenItem.EnchantWearable(item, Get.MainActor));
                    }
                    Get.MainActor.Inventory.Equipped.Equip(item, null);
                }
            }
        }

        private static void AddWeapon()
        {
            EntitySpec entitySpec;
            if ((from x in Get.Specs.GetAll<EntitySpec>()
                 where x.IsItem && x.Item.IsEquippableWeapon && !x.Item.Generator_IsWand
                 select x).TryGetRandomElement<EntitySpec>(out entitySpec))
            {
                Item item = Maker.Make<Item>(entitySpec, null, false, false, true);
                item.TurnsLeftToIdentify = 0;
                item.RampUp = RampUpUtility.ResolveNonRepeatedRampUpFor(item, Rand.RangeInclusive(0, 7));
                if (Rand.Bool)
                {
                    MediaCreatorHelper.extraInstructions.AddRange(UseEffect_EnchantChosenItem.EnchantWeapon(item, Get.MainActor));
                }
                Get.MainActor.Inventory.Equipped.Equip(item, null);
            }
        }

        private static void AddQuickbarItems()
        {
            int num = Rand.RangeInclusive(4, 5);
            for (int i = 0; i < num; i++)
            {
                Item item = MediaCreatorHelper.GenerateRandomItem();
                Get.MainActor.Inventory.Add(item, new ValueTuple<Vector2Int?, int?, int?>(null, new int?(MediaCreatorHelper.GetRandomFreeQuickbarSlot()), null));
            }
        }

        private static void AddInventory()
        {
            int num = Rand.RangeInclusive(11, 18);
            for (int i = 0; i < num; i++)
            {
                int num2 = i % Get.MainActor.Inventory.Backpack.Width;
                int num3 = i / Get.MainActor.Inventory.Backpack.Height;
                Item item;
                if (i == num - 2 && !Get.MainActor.Inventory.HasAnyItemOfSpec(Get.Entity_Shield))
                {
                    item = Maker.Make<Item>(Get.Entity_Shield, null, false, false, true);
                }
                else if (i == num - 1 && !Get.MainActor.Inventory.HasAnyItemOfSpec(Get.Entity_Watch))
                {
                    item = Maker.Make<Item>(Get.Entity_Watch, null, false, false, true);
                }
                else
                {
                    item = MediaCreatorHelper.GenerateRandomItem();
                }
                Get.MainActor.Inventory.Add(item, new ValueTuple<Vector2Int?, int?, int?>(new Vector2Int?(new Vector2Int(num2, num3)), null, null));
            }
        }

        private static void AddSpells()
        {
            foreach (Spell spell in Get.MainActor.Spells.All.ToTemporaryList<Spell>())
            {
                Get.MainActor.Spells.RemoveSpell(spell);
            }
            int num = Rand.RangeInclusive(2, 5);
            for (int i = 0; i < num; i++)
            {
                SpellSpec spellSpec;
                if (!(from x in Get.Specs.GetAll<SpellSpec>()
                      where x.CanAppearAsChoiceForPlayer && !Get.MainActor.Spells.AnyOfSpec(x)
                      select x).TryGetRandomElement<SpellSpec>(out spellSpec))
                {
                    break;
                }
                Get.MainActor.Spells.AddSpell(new Spell(spellSpec), -1);
            }
        }

        private static void RandomizeTime()
        {
            int num = Rand.RangeInclusive(0, 1320) * 12;
            foreach (ISequenceable sequenceable in Get.TurnManager.SequenceablesInOrder)
            {
                sequenceable.Sequence += num;
            }
        }

        private static IEnumerable<Instruction> AddConditions()
        {
            foreach (Condition condition in Get.MainActor.Conditions.All.ToTemporaryList<Condition>())
            {
                if (condition.Spec != Get.Condition_Hunger)
                {
                    foreach (Instruction instruction in InstructionSets_Misc.RemoveCondition(condition, false, false))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator2 = null;
                }
            }
            List<Condition>.Enumerator enumerator = default(List<Condition>.Enumerator);
            int count = Rand.RangeInclusive(1, 2);
            int num;
            for (int i = 0; i < count; i = num + 1)
            {
                Func<Condition> func;
                if (!MediaCreatorHelper.ConditionPossibilities.Where<Func<Condition>>((Func<Condition> x) => !Get.MainActor.Conditions.AnyOfSpec(x().Spec)).TryGetRandomElement<Func<Condition>>(out func))
                {
                    break;
                }
                foreach (Instruction instruction2 in InstructionSets_Misc.AddCondition(func(), Get.MainActor.Conditions, false, false))
                {
                    yield return instruction2;
                }
                IEnumerator<Instruction> enumerator2 = null;
                num = i;
            }
            if (Get.Player.Satiation <= 250)
            {
                foreach (Instruction instruction3 in InstructionSets_Misc.AddCondition(Maker.Make(Get.Condition_Hungry), Get.MainActor.Conditions, false, false))
                {
                    yield return instruction3;
                }
                IEnumerator<Instruction> enumerator2 = null;
            }
            yield break;
            yield break;
        }

        private static Item GenerateRandomItem()
        {
            EntitySpec entitySpec;
            (from x in Get.Specs.GetAll<EntitySpec>()
             where x.IsItem && !x.Item.LobbyItem && x != Get.Entity_UnlockableAsItem && x != Get.Entity_PrivateRoomStructureAsItem && x != Get.Entity_Gold
             select x).TryGetRandomElement<EntitySpec>(out entitySpec);
            Item item = Maker.Make<Item>(entitySpec, null, false, false, true);
            item.RampUp = RampUpUtility.ResolveNonRepeatedRampUpFor(item, Rand.RangeInclusive(0, 7));
            if (Rand.Chance(0.5f))
            {
                item.TurnsLeftToIdentify = 0;
            }
            else
            {
                item.TurnsLeftToIdentify = Calc.RoundToInt((float)item.TurnsLeftToIdentify * Rand.Range(0.2f, 1f));
            }
            return item;
        }

        private static int GetRandomFreeQuickbarSlot()
        {
            int num;
            (from x in Enumerable.Range(0, 10)
             where Get.MainActor.Inventory.QuickbarItems[x] == null
             select x).TryGetRandomElement<int>(out num);
            return num;
        }

        private static List<Instruction> extraInstructions = new List<Instruction>();

        private static bool running;

        private static Func<Condition>[] ConditionPossibilities = new Func<Condition>[]
        {
            () => new Condition_Bleeding(Get.Condition_Bleeding, Rand.RangeInclusive(10, 30)),
            () => new Condition_BrokenBone(Get.Condition_BrokenBone),
            () => new Condition_MaxHPOffset(Get.Condition_MaxHPOffset, 1, Rand.RangeInclusive(10, 30)),
            () => new Condition_MaxHPOffset(Get.Condition_MaxHPOffset, -1, Rand.RangeInclusive(10, 30)),
            () => new Condition_ArmMissing(Get.Condition_ArmMissing),
            () => new Condition_LegMissing(Get.Condition_LegMissing),
            () => new Condition_Disease(Get.Condition_Disease),
            () => new Condition_SequencePerTurnMultiplier(Get.Condition_SequencePerTurnMultiplier, 2, false, Rand.RangeInclusive(10, 30))
        };
    }
}