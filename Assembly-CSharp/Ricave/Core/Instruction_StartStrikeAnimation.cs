using System;
using UnityEngine;

namespace Ricave.Core
{
    public class Instruction_StartStrikeAnimation : Instruction
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        public Vector3Int Target
        {
            get
            {
                return this.target;
            }
        }

        protected Instruction_StartStrikeAnimation()
        {
        }

        public Instruction_StartStrikeAnimation(Actor actor, Vector3Int target)
        {
            this.actor = actor;
            this.target = target;
        }

        protected override void DoImpl()
        {
            if (this.actor.ActorGOC != null)
            {
                this.actor.ActorGOC.StartStrikeAnimation(this.target);
            }
        }

        protected override void UndoImpl()
        {
            if (this.actor.ActorGOC != null)
            {
                this.actor.ActorGOC.StartStrikeAnimation(this.target);
            }
        }

        [Saved]
        private Actor actor;

        [Saved]
        private Vector3Int target;
    }
}