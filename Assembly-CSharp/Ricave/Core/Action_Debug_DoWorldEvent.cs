using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Action_Debug_DoWorldEvent : Action
    {
        public WorldEventSpec WorldEventSpec
        {
            get
            {
                return this.worldEventSpec;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, int>(this.worldEventSpec.MyStableHash, 951213664);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.worldEventSpec);
            }
        }

        protected Action_Debug_DoWorldEvent()
        {
        }

        public Action_Debug_DoWorldEvent(ActionSpec spec, WorldEventSpec worldEventSpec)
            : base(spec)
        {
            this.worldEventSpec = worldEventSpec;
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
            foreach (Instruction instruction in InstructionSets_Misc.DoWorldEvent(this.worldEventSpec))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        [Saved]
        private WorldEventSpec worldEventSpec;
    }
}