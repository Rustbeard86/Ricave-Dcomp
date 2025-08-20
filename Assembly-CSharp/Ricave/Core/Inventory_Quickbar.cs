using System;

namespace Ricave.Core
{
    public class Inventory_Quickbar : ISaveableEventsReceiver
    {
        public Item[] Items
        {
            get
            {
                return this.items;
            }
        }

        public int FirstFreeSlot
        {
            get
            {
                for (int i = 0; i < this.items.Length; i++)
                {
                    if (this.items[i] == null)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }

        public int FreeSlotsCount
        {
            get
            {
                int num = 0;
                for (int i = 0; i < this.items.Length; i++)
                {
                    if (this.items[i] == null)
                    {
                        num++;
                    }
                }
                return num;
            }
        }

        public bool Any
        {
            get
            {
                for (int i = 0; i < this.items.Length; i++)
                {
                    if (this.items[i] != null)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        protected Inventory_Quickbar()
        {
        }

        public Inventory_Quickbar(Actor owner)
        {
            this.owner = owner;
        }

        public bool Contains(Item item)
        {
            return this.GetCurrentIndexOf(item) != null;
        }

        public int? GetCurrentIndexOf(Item item)
        {
            if (item == null)
            {
                return null;
            }
            for (int i = 0; i < this.items.Length; i++)
            {
                if (this.items[i] == item)
                {
                    return new int?(i);
                }
            }
            return null;
        }

        public void Add(Item item, int? insertAt = null)
        {
            Instruction.ThrowIfNotExecuting();
            if (item == null)
            {
                Log.Error("Tried to add null item to quickbar.", false);
                return;
            }
            if (this.Contains(item))
            {
                Log.Error("Tried to add the same item twice to quickbar.", false);
                return;
            }
            if (item.Spawned)
            {
                Log.Error("Tried to add spawned item to quickbar.", false);
                return;
            }
            if (item.ParentInventory != null && item.ParentInventory == this.owner.Inventory)
            {
                Log.Error("Tried to add item to quickbar but it already has a ParentInventory (same as this one).", false);
                return;
            }
            if (item.ParentInventory != null)
            {
                Log.Error("Tried to add item to quickbar but it already has a ParentInventory.", false);
                return;
            }
            if (insertAt != null)
            {
                int value = insertAt.Value;
                if (this.items[value] != null)
                {
                    Log.Error("Tried to insert item into quickbar, but it's already occupied.", false);
                    return;
                }
                this.items[value] = item;
            }
            else
            {
                int firstFreeSlot = this.FirstFreeSlot;
                if (firstFreeSlot == -1)
                {
                    Log.Error("Tried to add item to quickbar but there's no space.", false);
                    return;
                }
                this.items[firstFreeSlot] = item;
            }
            item.ParentInventory = this.owner.Inventory;
            item.OnAddedToInventory();
        }

        public int? Remove(Item item)
        {
            Instruction.ThrowIfNotExecuting();
            if (item == null)
            {
                Log.Error("Tried to remove null item from quickbar.", false);
                return null;
            }
            if (!this.Contains(item))
            {
                Log.Error("Tried to remove item from quickbar but it's not here.", false);
                return null;
            }
            if (item.ParentInventory == this.owner.Inventory)
            {
                item.ParentInventory = null;
            }
            for (int i = 0; i < this.items.Length; i++)
            {
                if (this.items[i] == item)
                {
                    this.items[i] = null;
                    item.OnRemovedFromInventory(this.owner.Inventory);
                    return new int?(i);
                }
            }
            Log.Error("Contains() returned true, but couldn't find the item anywhere.", false);
            return null;
        }

        public Item GetFirstItemOfSpec(EntitySpec itemSpec)
        {
            for (int i = 0; i < this.items.Length; i++)
            {
                if (this.items[i] != null && this.items[i].Spec == itemSpec)
                {
                    return this.items[i];
                }
            }
            return null;
        }

        public int GetCount(EntitySpec itemSpec)
        {
            int num = 0;
            for (int i = 0; i < this.items.Length; i++)
            {
                if (this.items[i] != null && this.items[i].Spec == itemSpec)
                {
                    num += this.items[i].StackCount;
                }
            }
            return num;
        }

        public void OnSaved()
        {
        }

        public void OnLoaded()
        {
            for (int i = 0; i < this.items.Length; i++)
            {
                if (this.items[i] != null && this.items[i].Spec == null)
                {
                    this.items[i] = null;
                    Log.Error("Removed some items with null spec from quickbar.", false);
                }
            }
        }

        [Saved]
        private Actor owner;

        [Saved]
        private Item[] items = new Item[10];

        public const int Size = 10;
    }
}