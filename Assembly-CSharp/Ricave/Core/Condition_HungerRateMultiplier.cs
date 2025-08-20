using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_HungerRateMultiplier : Condition
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
                    return GoodBadNeutral.Bad;
                }
                return GoodBadNeutral.Good;
            }
        }

        public override float HungerRateMultiplier
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
                if (this.multiplier <= 0f)
                {
                    return "DoesntGetHungry".Translate();
                }
                if (this.multiplier == 2f)
                {
                    return "GetsHungryTwiceAsFast".Translate();
                }
                if (this.multiplier == 0.5f)
                {
                    return "GetsHungryTwiceAsSlow".Translate();
                }
                if (this.multiplier > 1f)
                {
                    return "GetsHungryTimesAsFast".Translate(this.multiplier.ToStringCached());
                }
                return "HungerRateMultiplier".Translate(this.multiplier.ToStringPercent(false));
            }
        }

        protected Condition_HungerRateMultiplier()
        {
        }

        public Condition_HungerRateMultiplier(ConditionSpec spec)
            : base(spec)
        {
        }

        public Condition_HungerRateMultiplier(ConditionSpec spec, float multiplier)
            : base(spec)
        {
            this.multiplier = multiplier;
        }

        public Condition_HungerRateMultiplier(ConditionSpec spec, float multiplier, int turnsLeft)
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
            ((Condition_HungerRateMultiplier)clone).multiplier = this.multiplier;
        }

        [Saved(1f, false)]
        private float multiplier = 1f;
    }
}