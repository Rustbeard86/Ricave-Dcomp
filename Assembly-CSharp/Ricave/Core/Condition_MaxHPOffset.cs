using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_MaxHPOffset : Condition
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

        public int BaseOffset
        {
            get
            {
                return this.offset;
            }
        }

        public int Offset
        {
            get
            {
                return this.offset + RampUpUtility.GetOffsetFromRampUp(this.offset, base.InheritedRampUp, this.OffsetRampUpFactor);
            }
        }

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(this.Offset.ToStringOffset(true));
            }
        }

        public override int MaxHPOffset
        {
            get
            {
                return this.Offset;
            }
        }

        public float OffsetRampUpFactor
        {
            get
            {
                if (!this.offsetUsesRampUp)
                {
                    return 1f;
                }
                return 1.2500001f;
            }
        }

        public override float ActorScaleFactor
        {
            get
            {
                return this.actorScaleFactor;
            }
        }

        protected Condition_MaxHPOffset()
        {
        }

        public Condition_MaxHPOffset(ConditionSpec spec)
            : base(spec)
        {
        }

        public Condition_MaxHPOffset(ConditionSpec spec, int offset, int turnsLeft = 0)
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
            if (affectedActor != null && this.Offset > 0 && affectedActor.HP > 0)
            {
                yield return new Instruction_ChangeHP(affectedActor, this.Offset);
            }
            yield break;
            yield break;
        }

        public override void CopyFieldsTo(Condition clone)
        {
            Condition_MaxHPOffset condition_MaxHPOffset = (Condition_MaxHPOffset)clone;
            condition_MaxHPOffset.offset = this.offset;
            condition_MaxHPOffset.offsetUsesRampUp = this.offsetUsesRampUp;
            condition_MaxHPOffset.actorScaleFactor = this.actorScaleFactor;
        }

        [Saved]
        private int offset;

        [Saved]
        private bool offsetUsesRampUp;

        [Saved(1f, false)]
        private float actorScaleFactor = 1f;
    }
}