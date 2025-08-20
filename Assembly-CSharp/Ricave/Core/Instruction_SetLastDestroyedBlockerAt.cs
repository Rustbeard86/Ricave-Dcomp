using System;
using UnityEngine;

namespace Ricave.Core
{
    public class Instruction_SetLastDestroyedBlockerAt : Instruction
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        public Vector3Int? LastDestroyedBlockerAt
        {
            get
            {
                return this.lastDestroyedBlockerAt;
            }
        }

        protected Instruction_SetLastDestroyedBlockerAt()
        {
        }

        public Instruction_SetLastDestroyedBlockerAt(Actor actor, Vector3Int? lastDestroyedBlockerAt)
        {
            this.actor = actor;
            this.lastDestroyedBlockerAt = lastDestroyedBlockerAt;
        }

        protected override void DoImpl()
        {
            this.prevLastDestroyedBlockerAt = this.actor.AIMemory.LastDestroyedBlockerAt;
            this.actor.AIMemory.LastDestroyedBlockerAt = this.lastDestroyedBlockerAt;
        }

        protected override void UndoImpl()
        {
            this.actor.AIMemory.LastDestroyedBlockerAt = this.prevLastDestroyedBlockerAt;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private Vector3Int? lastDestroyedBlockerAt;

        [Saved]
        private Vector3Int? prevLastDestroyedBlockerAt;
    }
}