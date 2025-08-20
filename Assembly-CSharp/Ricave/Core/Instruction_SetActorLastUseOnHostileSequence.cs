using System;

namespace Ricave.Core
{
    public class Instruction_SetActorLastUseOnHostileSequence : Instruction
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        public int LastUseSequence
        {
            get
            {
                return this.lastUseSequence;
            }
        }

        protected Instruction_SetActorLastUseOnHostileSequence()
        {
        }

        public Instruction_SetActorLastUseOnHostileSequence(Actor actor, int lastUseSequence)
        {
            this.actor = actor;
            this.lastUseSequence = lastUseSequence;
        }

        protected override void DoImpl()
        {
            this.prevLastUseSequence = this.actor.AIMemory.LastUseOnHostileSequence;
            this.actor.AIMemory.LastUseOnHostileSequence = new int?(this.lastUseSequence);
        }

        protected override void UndoImpl()
        {
            this.actor.AIMemory.LastUseOnHostileSequence = this.prevLastUseSequence;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private int lastUseSequence;

        [Saved]
        private int? prevLastUseSequence;
    }
}