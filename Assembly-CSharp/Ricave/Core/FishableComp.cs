using System;

namespace Ricave.Core
{
    public class FishableComp : EntityComp
    {
        public new FishableCompProps Props
        {
            get
            {
                return (FishableCompProps)base.Props;
            }
        }

        public bool Emptied
        {
            get
            {
                return this.emptied;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.emptied = value;
            }
        }

        protected FishableComp()
        {
        }

        public FishableComp(Entity parent)
            : base(parent)
        {
        }

        [Saved]
        private bool emptied;
    }
}