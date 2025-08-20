using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Action_Debug_RewindTime : Action
    {
        public int Turns
        {
            get
            {
                return this.turns;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, int>(this.turns, 912132609);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.turns);
            }
        }

        protected Action_Debug_RewindTime()
        {
        }

        public Action_Debug_RewindTime(ActionSpec spec, int turns)
            : base(spec)
        {
            this.turns = turns;
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
            yield return new Instruction_RewindTime(this.turns);
            yield break;
        }

        [Saved]
        private int turns;
    }
}