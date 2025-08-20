using System;

namespace Ricave.Core
{
    public class Instruction_SetLastViolentlyAttackedBy : Instruction
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        public Actor AttackedBy
        {
            get
            {
                return this.attackedBy;
            }
        }

        protected Instruction_SetLastViolentlyAttackedBy()
        {
        }

        public Instruction_SetLastViolentlyAttackedBy(Actor actor, Actor attackedBy)
        {
            this.actor = actor;
            this.attackedBy = attackedBy;
        }

        protected override void DoImpl()
        {
            this.prevAttackedBy = this.actor.AIMemory.LastViolentlyAttackedBy;
            this.actor.AIMemory.LastViolentlyAttackedBy = this.attackedBy;
        }

        protected override void UndoImpl()
        {
            this.actor.AIMemory.LastViolentlyAttackedBy = this.prevAttackedBy;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private Actor attackedBy;

        [Saved]
        private Actor prevAttackedBy;
    }
}