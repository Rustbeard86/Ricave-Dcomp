using System;

namespace Ricave.Core
{
    public class Instruction_AddToBurrowedActors : Instruction
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        protected Instruction_AddToBurrowedActors()
        {
        }

        public Instruction_AddToBurrowedActors(Actor actor)
        {
            this.actor = actor;
        }

        protected override void DoImpl()
        {
            Get.BurrowManager.AddBurrowedActor(this.actor);
        }

        protected override void UndoImpl()
        {
            Get.BurrowManager.RemoveBurrowedActor(this.actor);
        }

        [Saved]
        private Actor actor;
    }
}