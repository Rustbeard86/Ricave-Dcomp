using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Action_ResolveCondition : Action
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
                return Calc.CombineHashes<int, int>(this.condition.MyStableHash, 745121749);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.condition);
            }
        }

        protected Action_ResolveCondition()
        {
        }

        public Action_ResolveCondition(ActionSpec spec, Condition condition)
            : base(spec)
        {
            this.condition = condition;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            Actor affectedActor = this.condition.AffectedActor;
            return affectedActor != null && affectedActor.Spawned;
        }

        protected override bool CalculateConcernsPlayer()
        {
            return false;
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            Actor affectedActor = this.condition.AffectedActor;
            foreach (Instruction instruction in this.condition.MakeResolveConditionInstructions())
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            if ((affectedActor == null || affectedActor.Spawned) && this.condition.TurnsLeft > 0)
            {
                bool conditionDisappears = this.condition.TurnsLeft == 1;
                yield return new Instruction_ChangeConditionTurnsLeft(this.condition, -1);
                if (conditionDisappears)
                {
                    foreach (Instruction instruction2 in InstructionSets_Misc.RemoveCondition(this.condition, false, true))
                    {
                        yield return instruction2;
                    }
                    enumerator = null;
                }
            }
            yield return new Instruction_AddSequence(this.condition, 12);
            yield break;
            yield break;
        }

        [Saved]
        private Condition condition;
    }
}