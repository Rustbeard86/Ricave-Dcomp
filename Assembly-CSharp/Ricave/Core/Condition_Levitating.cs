using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_Levitating : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Good;
            }
        }

        public bool CanBeDisabledByBodyParts
        {
            get
            {
                return this.canBeDisabledByBodyParts;
            }
        }

        public override bool CanFly
        {
            get
            {
                if (this.canBeDisabledByBodyParts)
                {
                    Actor affectedActor = base.AffectedActor;
                    if (affectedActor != null && affectedActor.ConditionsAccumulated.LevitatingDisabledByBodyParts)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        protected Condition_Levitating()
        {
        }

        public Condition_Levitating(ConditionSpec spec)
            : base(spec)
        {
        }

        public Condition_Levitating(ConditionSpec spec, int turnsLeft)
            : base(spec, turnsLeft)
        {
        }

        public override void CopyFieldsTo(Condition clone)
        {
            ((Condition_Levitating)clone).canBeDisabledByBodyParts = this.canBeDisabledByBodyParts;
        }

        public override IEnumerable<Instruction> MakeResolveConditionInstructions()
        {
            return Enumerable.Empty<Instruction>();
        }

        [Saved]
        private bool canBeDisabledByBodyParts;
    }
}