using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public abstract class AINode_Conditional : AINode
    {
        protected abstract bool ConditionMet(Actor actor);

        public override IEnumerable<Instruction> MakeThinkInstructions(Actor actor)
        {
            if (this.ConditionMet(actor))
            {
                if (this.yes != null)
                {
                    foreach (Instruction instruction in this.yes.MakeThinkInstructions(actor))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator = null;
                }
            }
            else if (this.no != null)
            {
                foreach (Instruction instruction2 in this.no.MakeThinkInstructions(actor))
                {
                    yield return instruction2;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            yield break;
            yield break;
        }

        public override Action GetNextAction(Actor actor)
        {
            if (this.ConditionMet(actor))
            {
                if (this.yes != null)
                {
                    return this.yes.GetNextAction(actor);
                }
            }
            else if (this.no != null)
            {
                return this.no.GetNextAction(actor);
            }
            return null;
        }

        [Saved]
        private AINode yes;

        [Saved]
        private AINode no;
    }
}