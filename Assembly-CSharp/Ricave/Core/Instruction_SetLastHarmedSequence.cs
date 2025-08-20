using System;

namespace Ricave.Core
{
    public class Instruction_SetLastHarmedSequence : Instruction
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        public int LastHarmedSequence
        {
            get
            {
                return this.lastHarmedSequence;
            }
        }

        protected Instruction_SetLastHarmedSequence()
        {
        }

        public Instruction_SetLastHarmedSequence(Actor actor, int lastHarmedSequence)
        {
            this.actor = actor;
            this.lastHarmedSequence = lastHarmedSequence;
        }

        protected override void DoImpl()
        {
            this.prevLastHarmedSequence = this.actor.AIMemory.LastHarmedSequence;
            this.actor.AIMemory.LastHarmedSequence = new int?(this.lastHarmedSequence);
        }

        protected override void UndoImpl()
        {
            this.actor.AIMemory.LastHarmedSequence = this.prevLastHarmedSequence;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private int lastHarmedSequence;

        [Saved]
        private int? prevLastHarmedSequence;
    }
}