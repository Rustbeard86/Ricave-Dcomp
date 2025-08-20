using System;
using UnityEngine;

namespace Ricave.Core
{
    public class Instruction_SetGravity : Instruction
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        public Vector3Int NewGravity
        {
            get
            {
                return this.newGravity;
            }
        }

        protected Instruction_SetGravity()
        {
        }

        public Instruction_SetGravity(Actor actor, Vector3Int newGravity)
        {
            this.actor = actor;
            this.newGravity = newGravity;
        }

        protected override void DoImpl()
        {
            this.prevGravity = this.actor.Gravity;
            this.actor.Gravity = this.newGravity;
        }

        protected override void UndoImpl()
        {
            this.actor.Gravity = this.prevGravity;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private Vector3Int newGravity;

        [Saved]
        private Vector3Int prevGravity;
    }
}