using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_IdentificationRateMultiplier : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                if (this.multiplier == 1f)
                {
                    return GoodBadNeutral.Neutral;
                }
                if (this.multiplier >= 1f)
                {
                    return GoodBadNeutral.Good;
                }
                return GoodBadNeutral.Bad;
            }
        }

        public override float IdentificationRateMultiplier
        {
            get
            {
                return this.multiplier;
            }
        }

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(this.multiplier.ToStringFactor());
            }
        }

        protected Condition_IdentificationRateMultiplier()
        {
        }

        public Condition_IdentificationRateMultiplier(ConditionSpec spec)
            : base(spec)
        {
        }

        public Condition_IdentificationRateMultiplier(ConditionSpec spec, float multiplier)
            : base(spec)
        {
            this.multiplier = multiplier;
        }

        public Condition_IdentificationRateMultiplier(ConditionSpec spec, float multiplier, int turnsLeft)
            : base(spec, turnsLeft)
        {
            this.multiplier = multiplier;
        }

        public override IEnumerable<Instruction> MakeResolveConditionInstructions()
        {
            return Enumerable.Empty<Instruction>();
        }

        public override void CopyFieldsTo(Condition clone)
        {
            ((Condition_IdentificationRateMultiplier)clone).multiplier = this.multiplier;
        }

        [Saved(1f, false)]
        private float multiplier = 1f;
    }
}