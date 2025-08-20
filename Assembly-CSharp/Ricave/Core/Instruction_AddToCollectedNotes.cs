using System;

namespace Ricave.Core
{
    public class Instruction_AddToCollectedNotes : Instruction
    {
        public NoteSpec NoteSpec
        {
            get
            {
                return this.noteSpec;
            }
        }

        protected Instruction_AddToCollectedNotes()
        {
        }

        public Instruction_AddToCollectedNotes(NoteSpec noteSpec)
        {
            this.noteSpec = noteSpec;
        }

        protected override void DoImpl()
        {
            Get.Player.CollectedNoteSpecs.Add(this.noteSpec);
        }

        protected override void UndoImpl()
        {
            Get.Player.CollectedNoteSpecs.Remove(this.noteSpec);
        }

        [Saved]
        private NoteSpec noteSpec;
    }
}