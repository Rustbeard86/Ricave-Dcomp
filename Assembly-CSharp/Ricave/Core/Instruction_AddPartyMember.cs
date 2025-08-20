using System;

namespace Ricave.Core
{
    public class Instruction_AddPartyMember : Instruction
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        protected Instruction_AddPartyMember()
        {
        }

        public Instruction_AddPartyMember(Actor actor)
        {
            this.actor = actor;
        }

        protected override void DoImpl()
        {
            Get.Player.OtherPartyMembers.Add(this.actor);
        }

        protected override void UndoImpl()
        {
            Get.Player.OtherPartyMembers.RemoveLast(this.actor);
        }

        [Saved]
        private Actor actor;
    }
}