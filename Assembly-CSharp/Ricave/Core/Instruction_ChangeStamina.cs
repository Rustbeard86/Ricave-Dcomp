using System;

namespace Ricave.Core
{
    public class Instruction_ChangeStamina : Instruction
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ChangeStamina()
        {
        }

        public Instruction_ChangeStamina(Actor actor, int offset)
        {
            this.actor = actor;
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            this.actor.Stamina += this.offset;
        }

        protected override void UndoImpl()
        {
            this.actor.Stamina -= this.offset;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private int offset;
    }
}