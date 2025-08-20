using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Action_Debug_RemoveWorldSituation : Action
    {
        public WorldSituation WorldSituation
        {
            get
            {
                return this.worldSituation;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, int>(this.worldSituation.MyStableHash, 712132464);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.worldSituation);
            }
        }

        protected Action_Debug_RemoveWorldSituation()
        {
        }

        public Action_Debug_RemoveWorldSituation(ActionSpec spec, WorldSituation worldSituation)
            : base(spec)
        {
            this.worldSituation = worldSituation;
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
            foreach (Instruction instruction in InstructionSets_Misc.RemoveWorldSituation(this.worldSituation))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        [Saved]
        private WorldSituation worldSituation;
    }
}