using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_ImmuneToFire : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Good;
            }
        }

        public override bool ImmuneToFire
        {
            get
            {
                return true;
            }
        }

        protected Condition_ImmuneToFire()
        {
        }

        public Condition_ImmuneToFire(ConditionSpec spec)
            : base(spec)
        {
        }

        public Condition_ImmuneToFire(ConditionSpec spec, int turnsLeft)
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