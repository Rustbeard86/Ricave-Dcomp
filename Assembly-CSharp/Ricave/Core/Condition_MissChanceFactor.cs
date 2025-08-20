using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_MissChanceFactor : Condition
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

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(this.factor.ToStringFactor());
            }
        }

        public override float MissChanceFactor
        {
            get
            {
                return this.factor;
            }
        }

        protected Condition_MissChanceFactor()
        {
        }

        public Condition_MissChanceFactor(ConditionSpec spec)
            : base(spec)
        {
        }

        public Condition_MissChanceFactor(ConditionSpec spec, float factor)
            : this(spec)
        {
            this.factor = factor;
        }

        public override IEnumerable<Instruction> MakeResolveConditionInstructions()
        {
            return Enumerable.Empty<Instruction>();
        }

        public override void CopyFieldsTo(Condition clone)
        {
            ((Condition_MissChanceFactor)clone).factor = this.factor;
        }

        [Saved(1f, false)]
        private float factor = 1f;
    }
}