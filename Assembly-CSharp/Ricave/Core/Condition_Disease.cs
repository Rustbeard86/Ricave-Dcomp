using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_Disease : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Bad;
            }
        }

        public override bool DisableNativeHPRegen
        {
            get
            {
                return true;
            }
        }

        public override bool DisableFlying
        {
            get
            {
                return true;
            }
        }

        protected Condition_Disease()
        {
        }

        public Condition_Disease(ConditionSpec spec)
            : base(spec)
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