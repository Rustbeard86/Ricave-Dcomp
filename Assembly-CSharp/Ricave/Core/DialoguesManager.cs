using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class DialoguesManager
    {
        public Dialogue GetDialogue(DialogueSpec spec)
        {
            if (spec == null)
            {
                return null;
            }
            for (int i = 0; i < this.dialogues.Count; i++)
            {
                if (this.dialogues[i].Spec == spec)
                {
                    return this.dialogues[i];
                }
            }
            return null;
        }

        public Dialogue GetOrCreateDialogue(DialogueSpec spec)
        {
            if (spec == null)
            {
                return null;
            }
            Dialogue dialogue = this.GetDialogue(spec);
            if (dialogue != null)
            {
                return dialogue;
            }
            Dialogue dialogue2 = new Dialogue(spec);
            this.dialogues.Add(dialogue2);
            return dialogue2;
        }

        [Saved(Default.New, true)]
        private List<Dialogue> dialogues = new List<Dialogue>();
    }
}