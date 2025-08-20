using System;

namespace Ricave.Core
{
    public class Instruction_SetLastUseSequence : Instruction
    {
        public IUsable Usable
        {
            get
            {
                return this.usable;
            }
        }

        public int LastUseSequence
        {
            get
            {
                return this.lastUseSequence;
            }
        }

        protected Instruction_SetLastUseSequence()
        {
        }

        public Instruction_SetLastUseSequence(IUsable usable, int lastUseSequence)
        {
            this.usable = usable;
            this.lastUseSequence = lastUseSequence;
        }

        protected override void DoImpl()
        {
            this.prevLastUseSequence = this.usable.LastUseSequence;
            this.usable.LastUseSequence = new int?(this.lastUseSequence);
        }

        protected override void UndoImpl()
        {
            this.usable.LastUseSequence = this.prevLastUseSequence;
        }

        [Saved]
        private IUsable usable;

        [Saved]
        private int lastUseSequence;

        [Saved]
        private int? prevLastUseSequence;
    }
}