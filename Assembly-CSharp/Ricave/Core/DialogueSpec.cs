using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class DialogueSpec : Spec
    {
        public List<DialogueNode> Nodes
        {
            get
            {
                return this.nodes;
            }
        }

        public bool Ended
        {
            get
            {
                Dialogue dialogue = Get.DialoguesManager.GetDialogue(this);
                return dialogue != null && dialogue.Ended;
            }
        }

        public List<SpeakerSpec> AllSpeakers
        {
            get
            {
                if (this.allSpeakersCached == null)
                {
                    this.allSpeakersCached = new List<SpeakerSpec>();
                    foreach (DialogueNode dialogueNode in this.nodes)
                    {
                        if (dialogueNode.Speaker != null && dialogueNode.Speaker != Get.Speaker_Player && !this.allSpeakersCached.Contains(dialogueNode.Speaker))
                        {
                            this.allSpeakersCached.Add(dialogueNode.Speaker);
                        }
                    }
                }
                return this.allSpeakersCached;
            }
        }

        [Saved(Default.New, false)]
        private List<DialogueNode> nodes = new List<DialogueNode>();

        private List<SpeakerSpec> allSpeakersCached;
    }
}