using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Action_Debug_AddCondition : Action
    {
        public Conditions Conditions
        {
            get
            {
                return this.conditions;
            }
        }

        public ConditionSpec ConditionSpec
        {
            get
            {
                return this.conditionSpec;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, int, int>(this.conditionSpec.MyStableHash, this.conditions.MyStableHash, 712843640);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.conditionSpec, this.conditions);
            }
        }

        protected Action_Debug_AddCondition()
        {
        }

        public Action_Debug_AddCondition(ActionSpec spec, Conditions conditions, ConditionSpec conditionSpec)
            : base(spec)
        {
            this.conditions = conditions;
            this.conditionSpec = conditionSpec;
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
            Condition condition = Maker.Make(this.conditionSpec);
            foreach (Instruction instruction in InstructionSets_Misc.AddCondition(condition, this.conditions, true, true))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        [Saved]
        private Conditions conditions;

        [Saved]
        private ConditionSpec conditionSpec;
    }
}