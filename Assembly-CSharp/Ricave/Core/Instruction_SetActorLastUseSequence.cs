using System;

namespace Ricave.Core
{
    public class Instruction_SetActorLastUseSequence : Instruction
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

        protected Instruction_SetActorLastUseSequence()
        {
        }

        public Instruction_SetActorLastUseSequence(Actor actor, int lastUseSequence)
        {
            this.actor = actor;
            this.lastUseSequence = lastUseSequence;
        }

        protected override void DoImpl()
        {
            this.prevLastUseSequence = this.actor.AIMemory.LastUseSequence;
            this.actor.AIMemory.LastUseSequence = new int?(this.lastUseSequence);
        }

        protected override void UndoImpl()
        {
            this.actor.AIMemory.LastUseSequence = this.prevLastUseSequence;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private int lastUseSequence;

        [Saved]
        private int? prevLastUseSequence;
    }
}