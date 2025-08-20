using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Action_Debug_Destroy : Action
    {
        public Entity Entity
        {
            get
            {
                return this.entity;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, int>(this.entity.MyStableHash, 524643212);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.entity);
            }
        }

        protected Action_Debug_Destroy()
        {
        }

        public Action_Debug_Destroy(ActionSpec spec, Entity entity)
            : base(spec)
        {
            this.entity = entity;
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
            foreach (Instruction instruction in InstructionSets_Entity.Destroy(this.entity, null, null))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        [Saved]
        private Entity entity;
    }
}