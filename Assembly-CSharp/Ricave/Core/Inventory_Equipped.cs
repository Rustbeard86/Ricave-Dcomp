using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Inventory_Equipped : ISaveableEventsReceiver
    {
        public bool Any
        {
            get
            {
                return this.equipped.Count != 0;
            }
        }

        public List<Item> Items
        {
            get
            {
                return this.equipped;
            }
        }

        protected Inventory_Equipped()
        {
        }

        public Inventory_Equipped(Actor owner)
        {
            this.owner = owner;
        }

        public Item GetEquippedItemWithSlot(ItemSlotSpec itemSlot)
        {
            for (int i = 0; i < this.equipped.Count; i++)
            {
                if (this.equipped[i].Spec.Item.ItemSlot == itemSlot)
                {
                    return this.equipped[i];
                }
            }
            return null;
        }

        public bool IsEquipped(Item item)
        {
            return item != null && this.equipped.Contains(item);
        }

        public Item GetFirstItemOfSpec(EntitySpec itemSpec)
        {
            for (int i = 0; i < this.equipped.Count; i++)
            {
                if (this.equipped[i].Spec == itemSpec)
                {
                    return this.equipped[i];
                }
            }
            return null;
        }

        public Item GetFirstItemOfSpecWithTitle(EntitySpec itemSpec, TitleSpec titleSpec)
        {
            for (int i = 0; i < this.equipped.Count; i++)
            {
                if (this.equipped[i].Spec == itemSpec && this.equipped[i].Title == titleSpec)
                {
                    return this.equipped[i];
                }
            }
            return null;
        }

        public int GetCount(EntitySpec itemSpec, bool allowCursed = true)
        {
            int num = 0;
            for (int i = 0; i < this.equipped.Count; i++)
            {
                if (this.equipped[i].Spec == itemSpec && (allowCursed || !this.equipped[i].Cursed))
                {
                    num += this.equipped[i].StackCount;
                }
            }
            return num;
        }

        public bool AnyEquippedItemCollidesWith(Item item)
        {
            return this.GetEquippedItemCollidingWith(item) != null;
        }

        public bool AnyEquippedItemCollidesWith(EntitySpec itemSpec)
        {
            return this.GetEquippedItemCollidingWith(itemSpec) != null;
        }

        public Item GetEquippedItemCollidingWith(Item item)
        {
            return this.GetEquippedItemCollidingWith(item.Spec);
        }

        public Item GetEquippedItemCollidingWith(EntitySpec itemSpec)
        {
            if (itemSpec == null)
            {
                return null;
            }
            if (!itemSpec.IsItem || !itemSpec.Item.IsEquippable)
            {
                return null;
            }
            return this.GetEquippedItemWithSlot(itemSpec.Item.ItemSlot);
        }

        public void Equip(Item item, int? insertAt = null)
        {
            Instruction.ThrowIfNotExecuting();
            if (item == null)
            {
                Log.Error("Tried to equip null item.", false);
                return;
            }
            if (this.IsEquipped(item))
            {
                Log.Error("Tried to equip item which is already equipped.", false);
                return;
            }
            if (item.Spawned)
            {
                Log.Error("Tried to equip spawned item.", false);
                return;
            }
            if (!item.Spec.Item.IsEquippable)
            {
                Log.Error("Tried to equip non-equippable item.", false);
                return;
            }
            if (this.AnyEquippedItemCollidesWith(item))
            {
                Log.Error("Tried to equip item but this slot is already occupied by something else.", false);
                return;
            }
            if (item.ParentInventory != null && item.ParentInventory == this.owner.Inventory)
            {
                Log.Error("Tried to equip item but it already has a ParentInventory (same as this one).", false);
                return;
            }
            if (item.ParentInventory != null)
            {
                Log.Error("Tried to equip item but it already has a ParentInventory.", false);
                return;
            }
            int seeRange = this.owner.SeeRange;
            if (insertAt != null)
            {
                int? num = insertAt;
                int count = this.equipped.Count;
                if ((num.GetValueOrDefault() > count) & (num != null))
                {
                    string[] array = new string[5];
                    array[0] = "Tried to insert equipped item at index ";
                    int num2 = 1;
                    num = insertAt;
                    array[num2] = num.ToString();
                    array[2] = ", but equipped list count is only ";
                    array[3] = this.equipped.Count.ToString();
                    array[4] = ".";
                    Log.Error(string.Concat(array), false);
                    return;
                }
                this.equipped.Insert(insertAt.Value, item);
            }
            else
            {
                this.equipped.Add(item);
            }
            item.ParentInventory = this.owner.Inventory;
            item.OnAddedToInventory();
            ConditionsAccumulated.SetCachedConditionsDirty();
            if (seeRange != this.owner.SeeRange)
            {
                Get.VisibilityCache.OnSeeRangeChanged(this.owner);
            }
        }

        public int? Unequip(Item item)
        {
            Instruction.ThrowIfNotExecuting();
            if (item == null)
            {
                Log.Error("Tried to unequip null item.", false);
                return null;
            }
            if (!this.IsEquipped(item))
            {
                Log.Error("Tried to unequip item but it's not equipped.", false);
                return null;
            }
            int seeRange = this.owner.SeeRange;
            if (item.ParentInventory == this.owner.Inventory)
            {
                item.ParentInventory = null;
            }
            int num = this.equipped.IndexOf(item);
            this.equipped.Remove(item);
            item.OnRemovedFromInventory(this.owner.Inventory);
            ConditionsAccumulated.SetCachedConditionsDirty();
            if (seeRange != this.owner.SeeRange)
            {
                Get.VisibilityCache.OnSeeRangeChanged(this.owner);
            }
            return new int?(num);
        }

        public void OnSaved()
        {
        }

        public void OnLoaded()
        {
            if (this.equipped.RemoveAll((Item x) => x.Spec == null) != 0)
            {
                Log.Error("Removed some equipped items with null spec.", false);
            }
        }

        [Saved]
        private Actor owner;

        [Saved(Default.New, true)]
        private List<Item> equipped = new List<Item>();
    }
}