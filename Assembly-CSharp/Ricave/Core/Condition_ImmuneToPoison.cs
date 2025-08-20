using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_ImmuneToPoison : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Good;
            }
        }

        public override bool ImmuneToPoison
        {
            get
            {
                return true;
            }
        }

        protected Condition_ImmuneToPoison()
        {
        }

        public Condition_ImmuneToPoison(ConditionSpec spec)
            : base(spec)
        {
        }

        public Condition_ImmuneToPoison(ConditionSpec spec, int turnsLeft)
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