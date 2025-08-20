using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_SequencePerTurnMultiplier : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                if (this.multiplier == 1)
                {
                    return GoodBadNeutral.Neutral;
                }
                if (!this.invert)
                {
                    return GoodBadNeutral.Bad;
                }
                return GoodBadNeutral.Good;
            }
        }

        public override int SequencePerTurnMultiplier
        {
            get
            {
                return this.multiplier;
            }
        }

        public override bool SequencePerTurnMultiplierInvert
        {
            get
            {
                return this.invert;
            }
        }

        public override string LabelBase
        {
            get
            {
                if (this.invert)
                {
                    if (this.multiplier == 2)
                    {
                        return "ActsTwicePerTurn".Translate();
                    }
                    return "ActsTimesPerTurn".Translate(this.multiplier);
                }
                else
                {
                    if (this.multiplier == 2)
                    {
                        return "ActsEverySecondTurn".Translate();
                    }
                    return "ActsOnceEveryTurns".Translate(this.multiplier);
                }
            }
        }

        protected Condition_SequencePerTurnMultiplier()
        {
        }

        public Condition_SequencePerTurnMultiplier(ConditionSpec spec)
            : base(spec)
        {
        }

        public Condition_SequencePerTurnMultiplier(ConditionSpec spec, int multiplier, bool invert, int turnsLeft)
            : base(spec, turnsLeft)
        {
            this.multiplier = multiplier;
            this.invert = invert;
        }

        public override IEnumerable<Instruction> MakeResolveConditionInstructions()
        {
            return Enumerable.Empty<Instruction>();
        }

        public override void CopyFieldsTo(Condition clone)
        {
            Condition_SequencePerTurnMultiplier condition_SequencePerTurnMultiplier = (Condition_SequencePerTurnMultiplier)clone;
            condition_SequencePerTurnMultiplier.multiplier = this.multiplier;
            condition_SequencePerTurnMultiplier.invert = this.invert;
        }

        [Saved(1, false)]
        private int multiplier = 1;

        [Saved]
        private bool invert;
    }
}