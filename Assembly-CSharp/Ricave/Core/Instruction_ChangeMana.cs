using System;

namespace Ricave.Core
{
    public class Instruction_ChangeMana : Instruction
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

        protected Instruction_ChangeMana()
        {
        }

        public Instruction_ChangeMana(Actor actor, int offset)
        {
            this.actor = actor;
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            this.actor.Mana += this.offset;
        }

        protected override void UndoImpl()
        {
            this.actor.Mana -= this.offset;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private int offset;
    }
}