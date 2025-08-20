using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Action_Debug_RemoveUseEffect : Action
    {
        public UseEffect UseEffect
        {
            get
            {
                return this.useEffect;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, int>(this.useEffect.MyStableHash, 365121980);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.useEffect, this.removedFrom);
            }
        }

        protected Action_Debug_RemoveUseEffect()
        {
        }

        public Action_Debug_RemoveUseEffect(ActionSpec spec, UseEffect useEffect)
            : base(spec)
        {
            this.useEffect = useEffect;
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
            this.removedFrom = this.useEffect.Parent;
            foreach (Instruction instruction in InstructionSets_Misc.RemoveUseEffect(this.useEffect, true, true))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        [Saved]
        private UseEffect useEffect;

        [Saved]
        private UseEffects removedFrom;
    }
}