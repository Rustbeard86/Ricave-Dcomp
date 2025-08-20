using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class Action_Debug_Spawn : Action
    {
        public Entity Entity
        {
            get
            {
                return this.entity;
            }
        }

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
                return Calc.CombineHashes<int, Vector3Int, int>(this.entity.MyStableHash, this.pos, 712437590);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.entity);
            }
        }

        protected Action_Debug_Spawn()
        {
        }

        public Action_Debug_Spawn(ActionSpec spec, Entity entity, Vector3Int pos)
            : base(spec)
        {
            this.entity = entity;
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
            foreach (Instruction instruction in InstructionSets_Entity.Spawn(this.entity, this.pos, null))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        [Saved]
        private Entity entity;

        [Saved]
        private Vector3Int pos;
    }
}