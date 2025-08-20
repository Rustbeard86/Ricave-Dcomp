using System;
using UnityEngine;

namespace Ricave.Core
{
    public class Inventory_Backpack : ISaveableEventsReceiver
    {
        public Item[,] Items
        {
            get
            {
                return this.items;
            }
        }

        public int Width
        {
            get
            {
                return this.items.GetLength(0);
            }
        }

        public int Height
        {
            get
            {
                return this.items.GetLength(1);
            }
        }

        public Vector2Int? FirstFreeSlot
        {
            get
            {
                int width = this.Width;
                int height = this.Height;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (this.items[j, i] == null)
                        {
                            return new Vector2Int?(new Vector2Int(j, i));
                        }
                    }
                }
                return null;
            }
        }

        public int FreeSlotsCount
        {
            get
            {
                int num = 0;
                int width = this.Width;
                int height = this.Height;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (this.items[j, i] == null)
                        {
                            num++;
                        }
                    }
                }
                return num;
            }
        }

        public bool Any
        {
            get
            {
                int width = this.Width;
                int height = this.Height;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (this.items[j, i] != null)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        protected Inventory_Backpack()
        {
        }

        public Inventory_Backpack(Actor owner)
        {
            this.owner = owner;
            if (owner.Spec == Get.RunSpec.PlayerActorSpec)
            {
                int num = Math.Max(5 + Get.TraitManager.BackpackSlotsOffset / 5, 1);
                this.items = new Item[5, num];
                return;
            }
            this.items = new Item[5, 5];
        }

        public bool Contains(Item item)
        {
            return this.GetCurrentSlotOf(item) != null;
        }

        public Vector2Int? GetCurrentSlotOf(Item item)
        {
            if (item == null)
            {
                return null;
            }
            int width = this.Width;
            int height = this.Height;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (this.items[i, j] == item)
                    {
                        return new Vector2Int?(new Vector2Int(i, j));
                    }
                }
            }
            return null;
        }

        public void Add(Item item, Vector2Int? insertAt = null)
        {
            Instruction.ThrowIfNotExecuting();
            if (item == null)
            {
                Log.Error("Tried to add null item to inventory.", false);
                return;
            }
            if (this.Contains(item))
            {
                Log.Error("Tried to add the same item twice to inventory.", false);
                return;
            }
            if (item.Spawned)
            {
                Log.Error("Tried to add spawned item to inventory.", false);
                return;
            }
            if (item.ParentInventory != null && item.ParentInventory == this.owner.Inventory)
            {
                Log.Error("Tried to add item to inventory but it already has a ParentInventory (same as this one).", false);
                return;
            }
            if (item.ParentInventory != null)
            {
                Log.Error("Tried to add item to inventory but it already has a ParentInventory.", false);
                return;
            }
            if (insertAt != null)
            {
                Vector2Int value = insertAt.Value;
                if (value.x < 0 || value.y < 0 || value.x >= this.Width || value.y >= this.Height)
                {
                    Log.Error("Tried to insert item into inventory slot, but this slot is out of bounds.", false);
                    return;
                }
                if (this.items[value.x, value.y] != null)
                {
                    Log.Error("Tried to insert item into inventory slot, but this slot is already occupied.", false);
                    return;
                }
                this.items[value.x, value.y] = item;
            }
            else
            {
                Vector2Int? firstFreeSlot = this.FirstFreeSlot;
                if (firstFreeSlot == null)
                {
                    Log.Error("Tried to add item to inventory but there's no space.", false);
                    return;
                }
                this.items[firstFreeSlot.Value.x, firstFreeSlot.Value.y] = item;
            }
            item.ParentInventory = this.owner.Inventory;
            item.OnAddedToInventory();
        }

        public Vector2Int? Remove(Item item)
        {
            Instruction.ThrowIfNotExecuting();
            if (item == null)
            {
                Log.Error("Tried to remove null item from inventory.", false);
                return null;
            }
            if (!this.Contains(item))
            {
                Log.Error("Tried to remove item from inventory but it's not here.", false);
                return null;
            }
            if (item.ParentInventory == this.owner.Inventory)
            {
                item.ParentInventory = null;
            }
            int width = this.Width;
            int height = this.Height;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (this.items[i, j] == item)
                    {
                        this.items[i, j] = null;
                        item.OnRemovedFromInventory(this.owner.Inventory);
                        return new Vector2Int?(new Vector2Int(i, j));
                    }
                }
            }
            Log.Error("Contains() returned true, but couldn't find the item anywhere.", false);
            return null;
        }

        public Item GetFirstItemOfSpec(EntitySpec itemSpec)
        {
            int width = this.Width;
            int height = this.Height;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (this.items[j, i] != null && this.items[j, i].Spec == itemSpec)
                    {
                        return this.items[j, i];
                    }
                }
            }
            return null;
        }

        public int GetCount(EntitySpec itemSpec)
        {
            int num = 0;
            int width = this.Width;
            int height = this.Height;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (this.items[j, i] != null && this.items[j, i].Spec == itemSpec)
                    {
                        num += this.items[j, i].StackCount;
                    }
                }
            }
            return num;
        }

        public void OnSaved()
        {
        }

        public void OnLoaded()
        {
            int width = this.Width;
            int height = this.Height;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (this.items[j, i] != null && this.items[j, i].Spec == null)
                    {
                        this.items[j, i] = null;
                        Log.Error("Removed some items with null spec from inventory.", false);
                    }
                }
            }
        }

        [Saved]
        private Actor owner;

        [Saved]
        private Item[,] items;

        public const int DefaultWidth = 5;

        public const int DefaultHeight = 5;
    }
}