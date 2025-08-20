using System;

namespace Ricave.Core
{
    public class WorldSequenceable : ISequenceable
    {
        public int Sequence
        {
            get
            {
                return this.sequence;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.sequence = value;
                Get.TurnManager.OnSequenceChanged();
            }
        }

        protected WorldSequenceable()
        {
        }

        public WorldSequenceable(World world)
        {
            this.world = world;
        }

        void ISequenceable.TakeTurn()
        {
            new Action_ResolveWorld(Get.Action_ResolveWorld).Do(false);
        }

        [Saved]
        private World world;

        [Saved]
        private int sequence;
    }
}