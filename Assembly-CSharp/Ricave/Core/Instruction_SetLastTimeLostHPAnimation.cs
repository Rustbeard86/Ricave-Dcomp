using System;

namespace Ricave.Core
{
    public class Instruction_SetLastTimeLostHPAnimation : Instruction
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        protected Instruction_SetLastTimeLostHPAnimation()
        {
        }

        public Instruction_SetLastTimeLostHPAnimation(Actor actor)
        {
            this.actor = actor;
        }

        protected override void DoImpl()
        {
            if (this.actor.ActorGOC != null)
            {
                this.actor.ActorGOC.OnLostHP();
            }
        }

        protected override void UndoImpl()
        {
            if (this.actor.ActorGOC != null)
            {
                this.actor.ActorGOC.OnLostHP();
            }
        }

        [Saved]
        private Actor actor;
    }
}