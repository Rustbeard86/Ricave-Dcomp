using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Action_Debug_AddUseEffect : Action
    {
        public UseEffects UseEffects
        {
            get
            {
                return this.useEffects;
            }
        }

        public UseEffectSpec UseEffectSpec
        {
            get
            {
                return this.useEffectSpec;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, int, int>(this.useEffectSpec.MyStableHash, this.useEffects.MyStableHash, 87865091);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.useEffectSpec, this.useEffects);
            }
        }

        protected Action_Debug_AddUseEffect()
        {
        }

        public Action_Debug_AddUseEffect(ActionSpec spec, UseEffects useEffects, UseEffectSpec useEffectSpec)
            : base(spec)
        {
            this.useEffects = useEffects;
            this.useEffectSpec = useEffectSpec;
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
            UseEffect useEffect = Maker.Make(this.useEffectSpec);
            foreach (Instruction instruction in InstructionSets_Misc.AddUseEffect(useEffect, this.useEffects, true))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        [Saved]
        private UseEffects useEffects;

        [Saved]
        private UseEffectSpec useEffectSpec;
    }
}