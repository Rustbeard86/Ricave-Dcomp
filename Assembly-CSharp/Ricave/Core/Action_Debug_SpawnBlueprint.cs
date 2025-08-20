using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class Action_Debug_SpawnBlueprint : Action
    {
        public BlueprintSpec BlueprintSpec
        {
            get
            {
                return this.blueprintSpec;
            }
        }

        public Vector3Int StartPosition
        {
            get
            {
                return this.startPos;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, Vector3Int, int>(this.blueprintSpec.MyStableHash, this.startPos, 712143770);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.blueprintSpec.SpecID);
            }
        }

        protected Action_Debug_SpawnBlueprint()
        {
        }

        public Action_Debug_SpawnBlueprint(ActionSpec spec, BlueprintSpec blueprintSpec, Vector3Int startPos)
            : base(spec)
        {
            this.blueprintSpec = blueprintSpec;
            this.startPos = startPos;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            BlueprintSpec blueprintSpec = this.blueprintSpec;
            return ((blueprintSpec != null) ? blueprintSpec.Blueprint : null) != null;
        }

        protected override bool CalculateConcernsPlayer()
        {
            return true;
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            foreach (Instruction instruction in this.blueprintSpec.Blueprint.SpawnInstructions(this.startPos))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        [Saved]
        private BlueprintSpec blueprintSpec;

        [Saved]
        private Vector3Int startPos;
    }
}