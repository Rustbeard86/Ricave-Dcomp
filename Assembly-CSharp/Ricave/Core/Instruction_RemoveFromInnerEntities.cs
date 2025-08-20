using System;

namespace Ricave.Core
{
    public class Instruction_RemoveFromInnerEntities : Instruction
    {
        public Entity ToRemove
        {
            get
            {
                return this.toRemove;
            }
        }

        public Structure From
        {
            get
            {
                return this.from;
            }
        }

        protected Instruction_RemoveFromInnerEntities()
        {
        }

        public Instruction_RemoveFromInnerEntities(Entity toRemove, Structure from)
        {
            this.toRemove = toRemove;
            this.from = from;
        }

        protected override void DoImpl()
        {
            this.removedFromIndex = this.from.InnerEntities.IndexOf(this.toRemove);
            this.from.InnerEntities.Remove(this.toRemove);
        }

        protected override void UndoImpl()
        {
            this.from.InnerEntities.Insert(this.removedFromIndex, this.toRemove);
        }

        [Saved]
        private Entity toRemove;

        [Saved]
        private Structure from;

        [Saved]
        private int removedFromIndex;
    }
}