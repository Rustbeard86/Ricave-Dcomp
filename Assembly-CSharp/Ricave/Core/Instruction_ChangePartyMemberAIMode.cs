using System;

namespace Ricave.Core
{
    public class Instruction_ChangePartyMemberAIMode : Instruction
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        public PartyMemberAIMode Mode
        {
            get
            {
                return this.mode;
            }
        }

        protected Instruction_ChangePartyMemberAIMode()
        {
        }

        public Instruction_ChangePartyMemberAIMode(Actor actor, PartyMemberAIMode mode)
        {
            this.actor = actor;
            this.mode = mode;
        }

        protected override void DoImpl()
        {
            this.prevMode = this.actor.PartyMemberAIMode;
            this.actor.PartyMemberAIMode = this.mode;
        }

        protected override void UndoImpl()
        {
            this.actor.PartyMemberAIMode = this.prevMode;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private PartyMemberAIMode mode;

        [Saved]
        private PartyMemberAIMode prevMode;
    }
}