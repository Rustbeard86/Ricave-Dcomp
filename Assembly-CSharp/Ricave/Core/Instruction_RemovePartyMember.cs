using System;

namespace Ricave.Core
{
    public class Instruction_RemovePartyMember : Instruction
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        protected Instruction_RemovePartyMember()
        {
        }

        public Instruction_RemovePartyMember(Actor actor)
        {
            this.actor = actor;
        }

        protected override void DoImpl()
        {
            this.removedFromIndex = Get.Player.OtherPartyMembers.IndexOf(this.actor);
            Get.Player.OtherPartyMembers.Remove(this.actor);
        }

        protected override void UndoImpl()
        {
            if (this.removedFromIndex != -1)
            {
                Get.Player.OtherPartyMembers.Insert(this.removedFromIndex, this.actor);
            }
        }

        [Saved]
        private Actor actor;

        [Saved]
        private int removedFromIndex;
    }
}