using System;
using UnityEngine;

namespace Ricave.Core
{
    public class Instruction_SetWanderDest : Instruction
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        public Vector3Int? WanderDest
        {
            get
            {
                return this.wanderDest;
            }
        }

        protected Instruction_SetWanderDest()
        {
        }

        public Instruction_SetWanderDest(Actor actor, Vector3Int? wanderDest)
        {
            this.actor = actor;
            this.wanderDest = wanderDest;
        }

        protected override void DoImpl()
        {
            this.prevWanderDest = this.actor.AIMemory.WanderDest;
            this.actor.AIMemory.WanderDest = this.wanderDest;
        }

        protected override void UndoImpl()
        {
            this.actor.AIMemory.WanderDest = this.prevWanderDest;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private Vector3Int? wanderDest;

        [Saved]
        private Vector3Int? prevWanderDest;
    }
}