using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Action_Debug_RemoveCondition : Action
    {
        public Condition Condition
        {
            get
            {
                return this.condition;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, int>(this.condition.MyStableHash, 27834120);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.condition, this.removedFrom);
            }
        }

        protected Action_Debug_RemoveCondition()
        {
        }

        public Action_Debug_RemoveCondition(ActionSpec spec, Condition condition)
            : base(spec)
        {
            this.condition = condition;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            return true;
        }

        protected override bool CalculateConcernsPlayer()
        {
            return true;
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            this.removedFrom = this.condition.Parent;
            foreach (Instruction instruction in InstructionSets_Misc.RemoveCondition(this.condition, true, true))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        [Saved]
        private Condition condition;

        [Saved]
        private Conditions removedFrom;
    }
}