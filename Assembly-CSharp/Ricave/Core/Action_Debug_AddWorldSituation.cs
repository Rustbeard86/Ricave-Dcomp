using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Action_Debug_AddWorldSituation : Action
    {
        public WorldSituationSpec WorldSituationSpec
        {
            get
            {
                return this.worldSituationSpec;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, int>(this.worldSituationSpec.MyStableHash, 931547442);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.worldSituationSpec);
            }
        }

        protected Action_Debug_AddWorldSituation()
        {
        }

        public Action_Debug_AddWorldSituation(ActionSpec spec, WorldSituationSpec worldSituationSpec)
            : base(spec)
        {
            this.worldSituationSpec = worldSituationSpec;
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
            WorldSituation worldSituation = Maker.Make(this.worldSituationSpec);
            foreach (Instruction instruction in InstructionSets_Misc.AddWorldSituation(worldSituation))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        [Saved]
        private WorldSituationSpec worldSituationSpec;
    }
}