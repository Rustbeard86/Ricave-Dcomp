using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_DealtDamageOffset : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                if (this.OffsetFrom > 0 || this.OffsetTo > 0)
                {
                    return GoodBadNeutral.Good;
                }
                if (this.OffsetFrom >= 0 && this.OffsetTo >= 0)
                {
                    return GoodBadNeutral.Neutral;
                }
                return GoodBadNeutral.Bad;
            }
        }

        public int BaseOffsetFrom
        {
            get
            {
                return this.offsetFrom;
            }
        }

        public int BaseOffsetTo
        {
            get
            {
                return this.offsetTo;
            }
        }

        public int OffsetFrom
        {
            get
            {
                return this.offsetFrom + RampUpUtility.GetOffsetFromRampUp(this.offsetFrom, base.InheritedRampUp, this.OffsetRampUpFactor);
            }
        }

        public int OffsetTo
        {
            get
            {
                return this.offsetTo + RampUpUtility.GetOffsetFromRampUp(this.offsetTo, base.InheritedRampUp, this.OffsetRampUpFactor);
            }
        }

        public DamageTypeSpec DamageType
        {
            get
            {
                return this.damageType;
            }
        }

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(StringUtility.RangeToStringOffset(this.OffsetFrom, this.OffsetTo), this.damageType.Adjective);
            }
        }

        public override ValueTuple<IntRange, DamageTypeSpec> DealtDamageOffset
        {
            get
            {
                return new ValueTuple<IntRange, DamageTypeSpec>(new IntRange(this.OffsetFrom, this.OffsetTo), this.damageType);
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

        protected Condition_DealtDamageOffset()
        {
        }

        public Condition_DealtDamageOffset(ConditionSpec spec)
            : base(spec)
        {
        }

        public Condition_DealtDamageOffset(ConditionSpec spec, int offsetFrom, int offsetTo, DamageTypeSpec damageType, int turnsLeft = 0)
            : base(spec, turnsLeft)
        {
            this.offsetFrom = offsetFrom;
            this.offsetTo = offsetTo;
            this.damageType = damageType;
        }

        public override IEnumerable<Instruction> MakeResolveConditionInstructions()
        {
            return Enumerable.Empty<Instruction>();
        }

        public override void CopyFieldsTo(Condition clone)
        {
            Condition_DealtDamageOffset condition_DealtDamageOffset = (Condition_DealtDamageOffset)clone;
            condition_DealtDamageOffset.offsetFrom = this.offsetFrom;
            condition_DealtDamageOffset.offsetTo = this.offsetTo;
            condition_DealtDamageOffset.damageType = this.damageType;
            condition_DealtDamageOffset.offsetUsesRampUp = this.offsetUsesRampUp;
            condition_DealtDamageOffset.actorScaleFactor = this.actorScaleFactor;
        }

        [Saved]
        private int offsetFrom;

        [Saved]
        private int offsetTo;

        [Saved]
        private DamageTypeSpec damageType;

        [Saved]
        private bool offsetUsesRampUp;

        [Saved(1f, false)]
        private float actorScaleFactor = 1f;
    }
}