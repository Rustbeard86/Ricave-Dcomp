using System;

namespace Ricave.Core
{
    public class Instruction_SetLastUsedToRewindTimeSequence : Instruction
    {
        public IUsable Usable
        {
            get
            {
                return this.usable;
            }
        }

        public int NewValue
        {
            get
            {
                return this.newValue;
            }
        }

        protected Instruction_SetLastUsedToRewindTimeSequence()
        {
        }

        public Instruction_SetLastUsedToRewindTimeSequence(IUsable usable, int newValue)
        {
            this.usable = usable;
            this.newValue = newValue;
        }

        protected override void DoImpl()
        {
            this.usable.LastUsedToRewindTimeSequence = new int?(this.newValue);
        }

        protected override void UndoImpl()
        {
        }

        [Saved]
        private IUsable usable;

        [Saved]
        private int newValue;
    }
}