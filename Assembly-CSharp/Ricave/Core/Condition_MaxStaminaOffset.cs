using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_MaxStaminaOffset : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                if (this.Offset > 0)
                {
                    return GoodBadNeutral.Good;
                }
                if (this.Offset >= 0)
                {
                    return GoodBadNeutral.Neutral;
                }
                return GoodBadNeutral.Bad;
            }
        }

        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(this.offset.ToStringOffset(true));
            }
        }

        public override int MaxStaminaOffset
        {
            get
            {
                return this.offset;
            }
        }

        protected Condition_MaxStaminaOffset()
        {
        }

        public Condition_MaxStaminaOffset(ConditionSpec spec)
            : base(spec)
        {
        }

        public Condition_MaxStaminaOffset(ConditionSpec spec, int offset, int turnsLeft = 0)
            : base(spec, turnsLeft)
        {
            this.offset = offset;
        }

        public override IEnumerable<Instruction> MakeResolveConditionInstructions()
        {
            return Enumerable.Empty<Instruction>();
        }

        public override IEnumerable<Instruction> MakePostAddInstructions()
        {
            foreach (Instruction instruction in base.MakePostAddInstructions())
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            Actor affectedActor = base.AffectedActor;
            if (affectedActor != null && this.offset > 0)
            {
                yield return new Instruction_ChangeStamina(affectedActor, this.offset);
            }
            yield break;
            yield break;
        }

        public override void CopyFieldsTo(Condition clone)
        {
            ((Condition_MaxStaminaOffset)clone).offset = this.offset;
        }

        [Saved]
        private int offset;
    }
}