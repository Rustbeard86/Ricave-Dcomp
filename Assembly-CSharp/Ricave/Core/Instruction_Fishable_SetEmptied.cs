using System;

namespace Ricave.Core
{
    public class Instruction_Fishable_SetEmptied : Instruction
    {
        public FishableComp Fishable
        {
            get
            {
                return this.fishable;
            }
        }

        public bool Emptied
        {
            get
            {
                return this.emptied;
            }
        }

        protected Instruction_Fishable_SetEmptied()
        {
        }

        public Instruction_Fishable_SetEmptied(FishableComp fishable, bool emptied)
        {
            this.fishable = fishable;
            this.emptied = emptied;
        }

        protected override void DoImpl()
        {
            this.prevEmptied = this.fishable.Emptied;
            this.fishable.Emptied = this.emptied;
        }

        protected override void UndoImpl()
        {
            this.fishable.Emptied = this.prevEmptied;
        }

        [Saved]
        private FishableComp fishable;

        [Saved]
        private bool emptied;

        [Saved]
        private bool prevEmptied;
    }
}