using System;

namespace Ricave.Core
{
    public class Instruction_RemoveFromBurrowedActors : Instruction
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        protected Instruction_RemoveFromBurrowedActors()
        {
        }

        public Instruction_RemoveFromBurrowedActors(Actor actor)
        {
            this.actor = actor;
        }

        protected override void DoImpl()
        {
            ValueTuple<BurrowManager.BurrowedActor, int> valueTuple = Get.BurrowManager.RemoveBurrowedActor(this.actor);
            this.removedActor = valueTuple.Item1;
            this.removedFromIndex = valueTuple.Item2;
        }

        protected override void UndoImpl()
        {
            Get.BurrowManager.InsertBurrowedActor(this.removedActor, this.removedFromIndex);
        }

        [Saved]
        private Actor actor;

        [Saved]
        private BurrowManager.BurrowedActor removedActor;

        [Saved]
        private int removedFromIndex;
    }
}