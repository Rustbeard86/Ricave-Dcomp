using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Dialogue
    {
        public DialogueSpec Spec
        {
            get
            {
                return this.spec;
            }
        }

        public List<DialogueEntry> History
        {
            get
            {
                return this.history;
            }
        }

        public int CurNodeIndex
        {
            get
            {
                return this.curNodeIndex;
            }
        }

        public DialogueNode CurNode
        {
            get
            {
                if (!this.Ended)
                {
                    return this.spec.Nodes[this.curNodeIndex];
                }
                return null;
            }
        }

        public bool IsInLastNode
        {
            get
            {
                return !this.Ended && this.curNodeIndex == this.spec.Nodes.Count - 1 && this.CurNode.Next.NullOrEmpty();
            }
        }

        public bool Ended
        {
            get
            {
                return this.curNodeIndex >= this.spec.Nodes.Count;
            }
        }

        protected Dialogue()
        {
        }

        public Dialogue(DialogueSpec spec)
        {
            this.spec = spec;
        }

        public void OnContinuedDialogue()
        {
            if (this.Ended)
            {
                Log.Error("Called OnContinuedDialogue() but the dialogue has already ended.", false);
                return;
            }
            if (this.CurNode.HasResponses)
            {
                Log.Error("Called OnContinuedDialogue() but there's a response expected. Call OnResponseChosen() instead.", false);
                return;
            }
            this.history.Add(new DialogueEntry(this.CurNode.Speaker, this.CurNode.Text));
            this.ProceedToNextNode();
        }

        public void OnResponseChosen(int index)
        {
            if (this.Ended)
            {
                Log.Error("Called OnResponseChosen() but the dialogue has already ended.", false);
                return;
            }
            if (!this.CurNode.HasResponses)
            {
                Log.Error("Called OnResponseChosen() but there are no responses available. Call OnContinuedDialogue() instead.", false);
                return;
            }
            if (index < 0 || index > this.CurNode.Responses.Count - 1)
            {
                Log.Error("Called OnResponseChosen() with response index out of bounds.", false);
                return;
            }
            this.history.Add(new DialogueEntry(this.CurNode.Speaker, this.CurNode.Text));
            DialogueNode.Response response = this.CurNode.Responses[index];
            this.history.Add(new DialogueEntry(Get.Speaker_Player, response.Text));
            if (response.JumpToNode.NullOrEmpty())
            {
                this.ProceedToNextNode();
                return;
            }
            int? num = this.FindNodeWithTagIndex(response.JumpToNode);
            if (num != null)
            {
                this.curNodeIndex = num.Value;
                return;
            }
            Log.Error("Could not find dialogue node with tag \"" + response.JumpToNode + "\". Proceeding to the next node instead.", false);
            this.ProceedToNextNode();
        }

        private void ProceedToNextNode()
        {
            if (this.Ended)
            {
                Log.Error("Called ProceedToNextNode() but the dialogue has already ended.", false);
                return;
            }
            if (this.CurNode.Next.NullOrEmpty())
            {
                this.curNodeIndex++;
                return;
            }
            int? num = this.FindNodeWithTagIndex(this.CurNode.Next);
            if (num != null)
            {
                this.curNodeIndex = num.Value;
                return;
            }
            Log.Error("Could not find dialogue node with tag \"" + this.CurNode.Next + "\". Incrementing the node index instead.", false);
            this.curNodeIndex++;
        }

        private int? FindNodeWithTagIndex(string tag)
        {
            List<DialogueNode> nodes = this.spec.Nodes;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].Tag == tag)
                {
                    return new int?(i);
                }
            }
            return null;
        }

        [Saved]
        private DialogueSpec spec;

        [Saved(Default.New, true)]
        private List<DialogueEntry> history = new List<DialogueEntry>();

        [Saved]
        private int curNodeIndex;
    }
}