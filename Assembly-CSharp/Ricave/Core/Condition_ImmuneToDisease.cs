using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_ImmuneToDisease : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Good;
            }
        }

        public override bool ImmuneToDisease
        {
            get
            {
                return true;
            }
        }

        protected Condition_ImmuneToDisease()
        {
        }

        public Condition_ImmuneToDisease(ConditionSpec spec)
            : base(spec)
        {
        }

        public Condition_ImmuneToDisease(ConditionSpec spec, int turnsLeft)
            : base(spec, turnsLeft)
        {
        }

        public override void CopyFieldsTo(Condition clone)
        {
        }

        public override IEnumerable<Instruction> MakeResolveConditionInstructions()
        {
            return Enumerable.Empty<Instruction>();
        }
    }
}