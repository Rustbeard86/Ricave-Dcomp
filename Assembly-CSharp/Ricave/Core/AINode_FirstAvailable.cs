using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class AINode_FirstAvailable : AINode
    {
        public List<AINode> Nodes
        {
            get
            {
                return this.nodes;
            }
        }

        public override IEnumerable<Instruction> MakeThinkInstructions(Actor actor)
        {
            int num;
            for (int i = 0; i < this.nodes.Count; i = num + 1)
            {
                foreach (Instruction instruction in this.nodes[i].MakeThinkInstructions(actor))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
                num = i;
            }
            yield break;
            yield break;
        }

        public override Action GetNextAction(Actor actor)
        {
            for (int i = 0; i < this.nodes.Count; i++)
            {
                Action nextAction = this.nodes[i].GetNextAction(actor);
                if (nextAction != null)
                {
                    return nextAction;
                }
            }
            return null;
        }

        [Saved(Default.New, true)]
        private List<AINode> nodes = new List<AINode>();
    }
}