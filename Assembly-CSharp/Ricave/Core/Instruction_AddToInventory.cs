using System;
using UnityEngine;

namespace Ricave.Core
{
    public class Instruction_AddToInventory : Instruction
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        public Item Item
        {
            get
            {
                return this.item;
            }
        }

        public Vector2Int? BackpackSlot
        {
            get
            {
                return this.backpackSlot;
            }
        }

        public int? QuickbarSlot
        {
            get
            {
                return this.quickbarSlot;
            }
        }

        protected Instruction_AddToInventory()
        {
        }

        public Instruction_AddToInventory(Actor actor, Item item, Vector2Int? backpackSlot = null, int? quickbarSlot = null)
        {
            this.actor = actor;
            this.item = item;
            this.backpackSlot = backpackSlot;
            this.quickbarSlot = quickbarSlot;
        }

        protected override void DoImpl()
        {
            this.actor.Inventory.Add(this.item, new ValueTuple<Vector2Int?, int?, int?>(this.backpackSlot, this.quickbarSlot, null));
        }

        protected override void UndoImpl()
        {
            this.actor.Inventory.Remove(this.item);
        }

        [Saved]
        private Actor actor;

        [Saved]
        private Item item;

        [Saved]
        private Vector2Int? backpackSlot;

        [Saved]
        private int? quickbarSlot;
    }
}