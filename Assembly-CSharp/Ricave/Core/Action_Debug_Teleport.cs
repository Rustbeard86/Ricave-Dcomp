using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class Action_Debug_Teleport : Action
    {
        public Vector3Int Position
        {
            get
            {
                return this.pos;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<Vector3Int, int>(this.pos, 190947581);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(Get.NowControlledActor);
            }
        }

        protected Action_Debug_Teleport()
        {
        }

        public Action_Debug_Teleport(ActionSpec spec, Vector3Int pos)
            : base(spec)
        {
            this.pos = pos;
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
            foreach (Instruction instruction in InstructionSets_Entity.Move(Get.NowControlledActor, this.pos, true, true))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        [Saved]
        private Vector3Int pos;
    }
}