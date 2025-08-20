using System;

namespace Ricave.Core
{
    public class Instruction_SetChargedAttack : Instruction
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        public int Value
        {
            get
            {
                return this.value;
            }
        }

        protected Instruction_SetChargedAttack()
        {
        }

        public Instruction_SetChargedAttack(Actor actor, int value)
        {
            this.actor = actor;
            this.value = value;
        }

        protected override void DoImpl()
        {
            this.prevChargedAttack = this.actor.ChargedAttack;
            this.actor.ChargedAttack = this.value;
            if (this.actor.IsNowControlledActor)
            {
                Get.Player.LastChargedAttackChangeTime = Clock.UnscaledTime;
            }
        }

        protected override void UndoImpl()
        {
            this.actor.ChargedAttack = this.prevChargedAttack;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private int value;

        [Saved]
        private int prevChargedAttack;
    }
}