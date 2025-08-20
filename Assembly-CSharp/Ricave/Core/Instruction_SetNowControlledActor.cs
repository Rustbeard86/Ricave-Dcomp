using System;

namespace Ricave.Core
{
    public class Instruction_SetNowControlledActor : Instruction
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        protected Instruction_SetNowControlledActor()
        {
        }

        public Instruction_SetNowControlledActor(Actor actor)
        {
            this.actor = actor;
        }

        protected override void DoImpl()
        {
            this.prevActor = Get.Player.NowControlledActor;
            Get.Player.NowControlledActor = this.actor;
            if (this.prevActor != Get.NowControlledActor)
            {
                Get.Player.OnSwitchedNowControlledActor(this.prevActor);
            }
        }

        protected override void UndoImpl()
        {
            Actor nowControlledActor = Get.Player.NowControlledActor;
            Get.Player.NowControlledActor = this.prevActor;
            if (nowControlledActor != Get.NowControlledActor)
            {
                Get.Player.OnSwitchedNowControlledActor(nowControlledActor);
            }
        }

        [Saved]
        private Actor actor;

        [Saved]
        private Actor prevActor;
    }
}