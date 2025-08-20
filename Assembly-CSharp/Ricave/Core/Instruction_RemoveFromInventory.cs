using System;
using UnityEngine;

namespace Ricave.Core
{
    public class Instruction_RemoveFromInventory : Instruction
    {
        public Item Item
        {
            get
            {
                return this.item;
            }
        }

        protected Instruction_RemoveFromInventory()
        {
        }

        public Instruction_RemoveFromInventory(Item item)
        {
            this.item = item;
        }

        protected override void DoImpl()
        {
            this.removedFromActor = this.item.ParentInventory.Owner;
            ValueTuple<Vector2Int?, int?, int?> valueTuple = this.item.ParentInventory.Remove(this.item);
            this.removedFromSlot = valueTuple.Item1;
            this.removedFromQuickbar = valueTuple.Item2;
            this.removedFromEquipped = valueTuple.Item3;
        }

        protected override void UndoImpl()
        {
            this.removedFromActor.Inventory.Add(this.item, new ValueTuple<Vector2Int?, int?, int?>(this.removedFromSlot, this.removedFromQuickbar, this.removedFromEquipped));
        }

        [Saved]
        private Item item;

        [Saved]
        private Actor removedFromActor;

        [Saved]
        private Vector2Int? removedFromSlot;

        [Saved]
        private int? removedFromQuickbar;

        [Saved]
        private int? removedFromEquipped;
    }
}