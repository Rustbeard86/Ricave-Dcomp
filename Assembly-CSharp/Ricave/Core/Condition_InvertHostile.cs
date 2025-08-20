using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_InvertHostile : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Bad;
            }
        }

        public override bool InvertHostile
        {
            get
            {
                return true;
            }
        }

        protected Condition_InvertHostile()
        {
        }

        public Condition_InvertHostile(ConditionSpec spec)
            : base(spec)
        {
        }

        public Condition_InvertHostile(ConditionSpec spec, int turnsLeft)
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