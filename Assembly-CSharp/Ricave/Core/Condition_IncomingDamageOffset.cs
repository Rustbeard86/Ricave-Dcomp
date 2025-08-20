using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_IncomingDamageOffset : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                if (this.OffsetFrom > 0 || this.OffsetTo > 0)
                {
                    return GoodBadNeutral.Bad;
                }
                if (this.OffsetFrom >= 0 && this.OffsetTo >= 0)
                {
                    return GoodBadNeutral.Neutral;
                }
                return GoodBadNeutral.Good;
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

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(StringUtility.RangeToString(this.OffsetFrom, this.OffsetTo), this.damageType.Adjective);
            }
        }

        public override ValueTuple<IntRange, DamageTypeSpec> IncomingDamageOffset
        {
            get
            {
                return new ValueTuple<IntRange, DamageTypeSpec>(new IntRange(this.OffsetFrom, this.OffsetTo), this.damageType);
            }
        }

        protected Condition_IncomingDamageOffset()
        {
        }

        public Condition_IncomingDamageOffset(ConditionSpec spec)
            : base(spec)
        {
        }

        public override IEnumerable<Instruction> MakeResolveConditionInstructions()
        {
            return Enumerable.Empty<Instruction>();
        }

        public override void CopyFieldsTo(Condition clone)
        {
            Condition_IncomingDamageOffset condition_IncomingDamageOffset = (Condition_IncomingDamageOffset)clone;
            condition_IncomingDamageOffset.offsetFrom = this.offsetFrom;
            condition_IncomingDamageOffset.offsetTo = this.offsetTo;
            condition_IncomingDamageOffset.damageType = this.damageType;
            condition_IncomingDamageOffset.offsetUsesRampUp = this.offsetUsesRampUp;
        }

        [Saved]
        private int offsetFrom;

        [Saved]
        private int offsetTo;

        [Saved]
        private DamageTypeSpec damageType;

        [Saved]
        private bool offsetUsesRampUp;
    }
}