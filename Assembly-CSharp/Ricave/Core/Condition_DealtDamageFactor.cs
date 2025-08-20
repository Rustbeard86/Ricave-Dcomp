using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_DealtDamageFactor : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                if (this.factor > 1f)
                {
                    return GoodBadNeutral.Good;
                }
                if (this.factor >= 1f)
                {
                    return GoodBadNeutral.Neutral;
                }
                return GoodBadNeutral.Bad;
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

        public override ValueTuple<float, DamageTypeSpec> DealtDamageFactor
        {
            get
            {
                return new ValueTuple<float, DamageTypeSpec>(this.factor, this.damageType);
            }
        }

        public override float ActorScaleFactor
        {
            get
            {
                return this.actorScaleFactor;
            }
        }

        protected Condition_DealtDamageFactor()
        {
        }

        public Condition_DealtDamageFactor(ConditionSpec spec)
            : base(spec)
        {
        }

        public Condition_DealtDamageFactor(ConditionSpec spec, float factor, DamageTypeSpec damageType, int turnsLeft = 0)
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
            Condition_DealtDamageFactor condition_DealtDamageFactor = (Condition_DealtDamageFactor)clone;
            condition_DealtDamageFactor.factor = this.factor;
            condition_DealtDamageFactor.damageType = this.damageType;
            condition_DealtDamageFactor.actorScaleFactor = this.actorScaleFactor;
        }

        [Saved(1f, false)]
        private float factor = 1f;

        [Saved]
        private DamageTypeSpec damageType;

        [Saved(1f, false)]
        private float actorScaleFactor = 1f;
    }
}