using System;

namespace Ricave.Core
{
    public class Instruction_Equip : Instruction
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

        protected Instruction_Equip()
        {
        }

        public Instruction_Equip(Actor actor, Item item)
        {
            this.actor = actor;
            this.item = item;
        }

        protected override void DoImpl()
        {
            this.actor.Inventory.Equipped.Equip(this.item, null);
        }

        protected override void UndoImpl()
        {
            this.actor.Inventory.Equipped.Unequip(this.item);
        }

        [Saved]
        private Actor actor;

        [Saved]
        private Item item;
    }
}