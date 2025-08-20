using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Action_Debug_SetRampUp : Action
    {
        public Entity Entity
        {
            get
            {
                return this.entity;
            }
        }

        public int NewRampUp
        {
            get
            {
                return this.newRampUp;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, int, int>(this.entity.MyStableHash, this.newRampUp, 821324140);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.entity);
            }
        }

        protected Action_Debug_SetRampUp()
        {
        }

        public Action_Debug_SetRampUp(ActionSpec spec, Entity entity, int newRampUp)
            : base(spec)
        {
            this.entity = entity;
            this.newRampUp = newRampUp;
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
            yield return new Instruction_SetRampUp(this.entity, this.newRampUp);
            yield break;
        }

        [Saved]
        private Entity entity;

        [Saved]
        private int newRampUp;
    }
}