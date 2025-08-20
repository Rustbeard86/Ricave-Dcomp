using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_CritChanceFactor : Condition
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

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(this.factor.ToStringFactor());
            }
        }

        public override float CritChanceFactor
        {
            get
            {
                return this.factor;
            }
        }

        protected Condition_CritChanceFactor()
        {
        }

        public Condition_CritChanceFactor(ConditionSpec spec)
            : base(spec)
        {
        }

        public Condition_CritChanceFactor(ConditionSpec spec, float factor)
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
            ((Condition_CritChanceFactor)clone).factor = this.factor;
        }

        [Saved(1f, false)]
        private float factor = 1f;
    }
}