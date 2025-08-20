using System;

namespace Ricave.Core
{
    public class Instruction_SetRampUp : Instruction
    {
        public Entity Entity
        {
            get
            {
                return this.entity;
            }
        }

        public int NewRampUp
        {
            get
            {
                return this.newRampUp;
            }
        }

        protected Instruction_SetRampUp()
        {
        }

        public Instruction_SetRampUp(Entity entity, int newRampUp)
        {
            this.entity = entity;
            this.newRampUp = newRampUp;
        }

        protected override void DoImpl()
        {
            Item item = this.entity as Item;
            if (item != null)
            {
                this.prevRampUp = item.RampUp;
                item.RampUp = this.newRampUp;
                return;
            }
            Actor actor = this.entity as Actor;
            if (actor != null)
            {
                this.prevRampUp = actor.RampUp;
                actor.RampUp = this.newRampUp;
            }
        }

        protected override void UndoImpl()
        {
            Item item = this.entity as Item;
            if (item != null)
            {
                item.RampUp = this.prevRampUp;
                return;
            }
            Actor actor = this.entity as Actor;
            if (actor != null)
            {
                actor.RampUp = this.prevRampUp;
            }
        }

        [Saved]
        private Entity entity;

        [Saved]
        private int newRampUp;

        [Saved]
        private int prevRampUp;
    }
}