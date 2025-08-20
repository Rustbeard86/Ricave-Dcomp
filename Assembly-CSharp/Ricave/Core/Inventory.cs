using System;
using System.Collections.Generic;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class Inventory
    {
        public Actor Owner
        {
            get
            {
                return this.owner;
            }
        }

        public Inventory_Backpack Backpack
        {
            get
            {
                return this.backpack;
            }
        }

        public Item[,] BackpackItems
        {
            get
            {
                return this.backpack.Items;
            }
        }

        public Inventory_Quickbar Quickbar
        {
            get
            {
                return this.quickbar;
            }
        }

        public Item[] QuickbarItems
        {
            get
            {
                return this.quickbar.Items;
            }
        }

        public Inventory_Equipped Equipped
        {
            get
            {
                return this.equipped;
            }
        }

        public List<Item> EquippedItems
        {
            get
            {
                return this.equipped.Items;
            }
        }

        public Item EquippedWeapon
        {
            get
            {
                return this.equipped.GetEquippedItemWithSlot(Get.ItemSlot_Weapon);
            }
        }

        public bool BackpackAndQuickbarFull
        {
            get
            {
                return this.backpack.FirstFreeSlot == null && this.quickbar.FirstFreeSlot == -1;
            }
        }

        public int FreeBackpackAndQuickbarSlotsCount
        {
            get
            {
                return this.backpack.FreeSlotsCount + this.quickbar.FreeSlotsCount;
            }
        }

        public List<Item> AllItems
        {
            get
            {
                this.tmpAllItems.Clear();
                this.tmpAllItems.AddRange(this.EquippedItems);
                Item[] quickbarItems = this.QuickbarItems;
                for (int i = 0; i < quickbarItems.Length; i++)
                {
                    if (quickbarItems[i] != null)
                    {
                        this.tmpAllItems.Add(quickbarItems[i]);
                    }
                }
                Item[,] backpackItems = this.BackpackItems;
                int length = backpackItems.GetLength(0);
                int length2 = backpackItems.GetLength(1);
                for (int j = 0; j < length2; j++)
                {
                    for (int k = 0; k < length; k++)
                    {
                        if (backpackItems[k, j] != null)
                        {
                            this.tmpAllItems.Add(backpackItems[k, j]);
                        }
                    }
                }
                return this.tmpAllItems;
            }
        }

        public List<Item> UnidentifiedItemsInIdentifyOrder
        {
            get
            {
                this.tmpUnidentifiedItemsInIdentifyOrder.Clear();
                List<Item> allItems = this.AllItems;
                for (int i = 0; i < allItems.Count; i++)
                {
                    if (!allItems[i].Identified)
                    {
                        this.tmpUnidentifiedItemsInIdentifyOrder.Add(allItems[i]);
                    }
                }
                if (this.tmpUnidentifiedItemsInIdentifyOrder.Count >= 2)
                {
                    this.tmpUnidentifiedItemsInIdentifyOrder.Sort(Inventory.ByIdentifyOrder);
                }
                return this.tmpUnidentifiedItemsInIdentifyOrder;
            }
        }

        public List<Item> AllNonEquippedItems
        {
            get
            {
                this.tmpAllNonEquippedItems.Clear();
                List<Item> allItems = this.AllItems;
                for (int i = 0; i < allItems.Count; i++)
                {
                    if (!this.equipped.IsEquipped(allItems[i]))
                    {
                        this.tmpAllNonEquippedItems.Add(allItems[i]);
                    }
                }
                return this.tmpAllNonEquippedItems;
            }
        }

        public bool HasWatch
        {
            get
            {
                return this.HasAnyItemOfSpec(Get.Entity_Watch) || this.HasSuperwatch;
            }
        }

        public bool HasSuperwatch
        {
            get
            {
                return this.HasAnyItemOfSpec(Get.Entity_Superwatch);
            }
        }

        public bool AnyEquippedItemHealsWearerOnKilled
        {
            get
            {
                using (List<Item>.Enumerator enumerator = this.EquippedItems.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.Spec.Item.HealWearerOnKilled)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public bool Any
        {
            get
            {
                return this.equipped.Any || this.quickbar.Any || this.backpack.Any;
            }
        }

        public bool AnyInBackpackOrQuickbar
        {
            get
            {
                return this.quickbar.Any || this.backpack.Any;
            }
        }

        protected Inventory()
        {
        }

        public Inventory(Actor owner)
        {
            this.owner = owner;
            this.backpack = new Inventory_Backpack(owner);
            this.quickbar = new Inventory_Quickbar(owner);
            this.equipped = new Inventory_Equipped(owner);
        }

        public bool Contains(Item item)
        {
            return item != null && (this.equipped.IsEquipped(item) || this.backpack.Contains(item) || this.quickbar.Contains(item));
        }

        public void Add(Item item, ValueTuple<Vector2Int?, int?, int?> insertAt = default(ValueTuple<Vector2Int?, int?, int?>))
        {
            Instruction.ThrowIfNotExecuting();
            if (insertAt.Item1 != null)
            {
                this.backpack.Add(item, new Vector2Int?(insertAt.Item1.Value));
                return;
            }
            if (insertAt.Item2 != null)
            {
                this.quickbar.Add(item, new int?(insertAt.Item2.Value));
                return;
            }
            if (insertAt.Item3 != null)
            {
                this.equipped.Equip(item, new int?(insertAt.Item3.Value));
                return;
            }
            if (this.quickbar.FirstFreeSlot != -1)
            {
                this.quickbar.Add(item, null);
                return;
            }
            this.backpack.Add(item, null);
        }

        public ValueTuple<Vector2Int?, int?, int?> Remove(Item item)
        {
            Instruction.ThrowIfNotExecuting();
            if (this.equipped.IsEquipped(item))
            {
                return new ValueTuple<Vector2Int?, int?, int?>(null, null, this.equipped.Unequip(item));
            }
            if (this.quickbar.Contains(item))
            {
                return new ValueTuple<Vector2Int?, int?, int?>(null, this.quickbar.Remove(item), null);
            }
            return new ValueTuple<Vector2Int?, int?, int?>(this.backpack.Remove(item), null, null);
        }

        public bool HasAnyItemOfSpec(EntitySpec itemSpec)
        {
            return this.GetFirstItemOfSpec(itemSpec) != null;
        }

        public Item GetFirstItemOfSpec(EntitySpec itemSpec)
        {
            Item item;
            if ((item = this.equipped.GetFirstItemOfSpec(itemSpec)) == null)
            {
                item = this.quickbar.GetFirstItemOfSpec(itemSpec) ?? this.backpack.GetFirstItemOfSpec(itemSpec);
            }
            return item;
        }

        public bool HasItems(EntitySpec itemSpec, int count, bool allowEquippedCursed = true)
        {
            return count <= 0 || this.GetCount(itemSpec, allowEquippedCursed) >= count;
        }

        public int GetCount(EntitySpec itemSpec, bool allowEquippedCursed = true)
        {
            return this.equipped.GetCount(itemSpec, allowEquippedCursed) + this.quickbar.GetCount(itemSpec) + this.backpack.GetCount(itemSpec);
        }

        public Item GetItemToStackWith(Item item)
        {
            if (!item.Spec.Item.Stackable)
            {
                return null;
            }
            foreach (Item item2 in this.AllItems)
            {
                if (item2.CanStackWith(item))
                {
                    return item2;
                }
            }
            return null;
        }

        public List<ValueTuple<string, Action, string>> GetPossibleInteractions(Item item, out bool canAutoUseOnlyAvailableOption, bool skipNonImportant = false)
        {
            List<ValueTuple<string, Action, string>> list = new List<ValueTuple<string, Action, string>>();
            canAutoUseOnlyAvailableOption = false;
            if (item == null)
            {
                Log.Error("Tried to get possible interactions for a null item.", false);
                return list;
            }
            if (!this.Contains(item))
            {
                Log.Error("Tried to get possible interactions for an item which is not here.", false);
                return list;
            }
            bool flag = item.Cursed || (!item.Identified && item.Spec.CanBeCursed);
            if (this.equipped.IsEquipped(item))
            {
                string text = (item.Spec.Item.IsEquippableWeapon ? "Unequip".Translate() : "Unwear".Translate());
                if (item.Cursed)
                {
                    list.Add(new ValueTuple<string, Action, string>(text, null, "Cursed".Translate()));
                }
                else if (this.BackpackAndQuickbarFull)
                {
                    list.Add(new ValueTuple<string, Action, string>(text, null, "NoSpace".Translate()));
                }
                else
                {
                    Func<Action> <> 9__6;
                    list.Add(new ValueTuple<string, Action, string>(text, delegate
                    {
                        Func<Action> func;
                        if ((func = <> 9__6) == null)
                        {
                            func = (<> 9__6 = () => new Action_Unequip(Get.Action_Unequip, this.owner, item));
                        }
                        ActionViaInterfaceHelper.TryDo(func);
                    }, null));
                    canAutoUseOnlyAvailableOption = true;
                }
            }
            else if (item.Spec.Item.IsEquippable)
            {
                string text2 = (item.Spec.Item.IsEquippableWeapon ? "Equip".Translate() : "Wear".Translate());
                Item equippedItemCollidingWith = this.equipped.GetEquippedItemCollidingWith(item);
                if (equippedItemCollidingWith != null && equippedItemCollidingWith.Cursed)
                {
                    list.Add(new ValueTuple<string, Action, string>(text2, null, "CurrentlyEquippedItemIsCursed".Translate()));
                }
                else
                {
                    Func<Action> <> 9__8;
                    list.Add(new ValueTuple<string, Action, string>(text2, delegate
                    {
                        Func<Action> func2;
                        if ((func2 = <> 9__8) == null)
                        {
                            func2 = (<> 9__8 = () => new Action_Equip(Get.Action_Equip, this.owner, item));
                        }
                        ActionViaInterfaceHelper.TryDo(func2);
                    }, null));
                    canAutoUseOnlyAvailableOption = !flag;
                }
            }
            if (!item.Spec.Item.IsEquippableWeapon && item.UseEffects.Any)
            {
                if (item.Spec.Item.AllowUseOnSelfViaInterface && item.UseFilter.Allows(this.owner, this.owner))
                {
                    this.tmpFailReason.Clear();
                    Actor actor = this.owner;
                    IUsable item2 = item;
                    Target target = this.owner;
                    StringSlot stringSlot = this.tmpFailReason;
                    if (actor.CanUseOn(item2, target, null, false, stringSlot))
                    {
                        list.Add(new ValueTuple<string, Action, string>(item.UseLabel_Self, delegate
                        {
                            Inventory.UseOnSelfOrShowUsePrompt(item);
                        }, null));
                    }
                    else
                    {
                        list.Add(new ValueTuple<string, Action, string>(item.UseLabel_Self, null, this.tmpFailReason.String.NullOrEmpty() ? "Disabled".Translate() : this.tmpFailReason.String));
                    }
                }
                if (item.UseRange > 0 && Get.UseOnTargetUI.TargetingUsable != item)
                {
                    list.Add(new ValueTuple<string, Action, string>(item.UseLabel_Other, delegate
                    {
                        Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                        Get.UseOnTargetUI.BeginTargeting(item, true);
                    }, null));
                    canAutoUseOnlyAvailableOption = true;
                }
            }
            foreach (ValueTuple<string, Action, string> valueTuple in InventoryExtraPossibleInteractions.GetFor(item))
            {
                list.Add(valueTuple);
            }
            if (Get.UseOnTargetUI.TargetingUsable == item)
            {
                list.Add(new ValueTuple<string, Action, string>("StopTargeting".Translate(), delegate
                {
                    Get.Sound_CloseWindow.PlayOneShot(null, 1f, 1f);
                    Get.UseOnTargetUI.StopTargeting();
                }, null));
                canAutoUseOnlyAvailableOption = true;
            }
            if (!this.equipped.IsEquipped(item) || !item.Cursed)
            {
                if (!skipNonImportant || flag || (list.Count <= 1 && !canAutoUseOnlyAvailableOption))
                {
                    Func<Action> <> 9__9;
                    list.Add(new ValueTuple<string, Action, string>("Drop".Translate(), delegate
                    {
                        Func<Action> func3;
                        if ((func3 = <> 9__9) == null)
                        {
                            func3 = (<> 9__9 = () => new Action_DropItem(Get.Action_DropItem, this.owner, item));
                        }
                        ActionViaInterfaceHelper.TryDo(func3);
                    }, null));
                    canAutoUseOnlyAvailableOption = false;
                }
                if (!skipNonImportant)
                {
                    Func<Action> <> 9__10;
                    list.Add(new ValueTuple<string, Action, string>("Destroy".Translate(), delegate
                    {
                        Func<Action> func4;
                        if ((func4 = <> 9__10) == null)
                        {
                            func4 = (<> 9__10 = () => new Action_DestroyItemInInventory(Get.Action_DestroyItemInInventory, this.owner, item));
                        }
                        ActionViaInterfaceHelper.TryDo(func4);
                    }, null));
                    canAutoUseOnlyAvailableOption = false;
                }
            }
            return list;
        }

        public static void UseOnSelfOrShowUsePrompt(IUsable usable)
        {
            UsePrompt usePrompt = usable.UsePrompt;
            if (usePrompt != null)
            {
                usePrompt.ShowUsePrompt(new Action_Use(Get.Action_Use, Get.NowControlledActor, usable, Get.NowControlledActor, null));
                return;
            }
            ActionViaInterfaceHelper.TryDo(() => new Action_Use(Get.Action_Use, Get.NowControlledActor, usable, Get.NowControlledActor, null));
        }

        [Saved]
        private Actor owner;

        [Saved]
        private Inventory_Backpack backpack;

        [Saved]
        private Inventory_Quickbar quickbar;

        [Saved]
        private Inventory_Equipped equipped;

        private static readonly Comparison<Item> ByIdentifyOrder = delegate (Item a, Item b)
        {
            int num = a.IdentifyOrder.CompareTo(b.IdentifyOrder);
            if (num != 0)
            {
                return num;
            }
            return a.MyStableHash.CompareTo(b.MyStableHash);
        };

        private List<Item> tmpAllItems = new List<Item>();

        private List<Item> tmpUnidentifiedItemsInIdentifyOrder = new List<Item>();

        private List<Item> tmpAllNonEquippedItems = new List<Item>();

        private StringSlot tmpFailReason = new StringSlot();
    }
}