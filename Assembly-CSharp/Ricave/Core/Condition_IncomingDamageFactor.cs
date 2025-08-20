using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_IncomingDamageFactor : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                if (this.factor > 1f)
                {
                    return GoodBadNeutral.Bad;
                }
                if (this.factor >= 1f)
                {
                    return GoodBadNeutral.Neutral;
                }
                return GoodBadNeutral.Good;
            }
        }

        public float Factor
        {
            get
            {
                return this.factor;
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
                return base.Spec.LabelFormat.Formatted(this.factor.ToStringFactor(), this.damageType.Adjective);
            }
        }

        public override ValueTuple<float, DamageTypeSpec> IncomingDamageFactor
        {
            get
            {
                return new ValueTuple<float, DamageTypeSpec>(this.factor, this.damageType);
            }
        }

        protected Condition_IncomingDamageFactor()
        {
        }

        public Condition_IncomingDamageFactor(ConditionSpec spec)
            : base(spec)
        {
        }

        public Condition_IncomingDamageFactor(ConditionSpec spec, float factor, DamageTypeSpec damageType, int turnsLeft = 0)
            : base(spec, turnsLeft)
        {
            this.factor = factor;
            this.damageType = damageType;
        }

        public override IEnumerable<Instruction> MakeResolveConditionInstructions()
        {
            return Enumerable.Empty<Instruction>();
        }

        public override void CopyFieldsTo(Condition clone)
        {
            Condition_IncomingDamageFactor condition_IncomingDamageFactor = (Condition_IncomingDamageFactor)clone;
            condition_IncomingDamageFactor.factor = this.factor;
            condition_IncomingDamageFactor.damageType = this.damageType;
        }

        [Saved(1f, false)]
        private float factor = 1f;

        [Saved]
        private DamageTypeSpec damageType;
    }
}