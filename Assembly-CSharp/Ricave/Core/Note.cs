using System;

namespace Ricave.Core
{
    public class Note : Item
    {
        public NoteSpec NoteSpec
        {
            get
            {
                return this.noteSpec;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.noteSpec = value;
            }
        }

        public override string Label
        {
            get
            {
                NoteSpec noteSpec = this.noteSpec;
                return ((noteSpec != null) ? noteSpec.Label : null) ?? base.Label;
            }
        }

        protected Note()
        {
        }

        public Note(EntitySpec spec)
            : base(spec)
        {
        }

        [Saved]
        private NoteSpec noteSpec;
    }
}