using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_Entangled : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Bad;
            }
        }

        public override bool MovingDisallowed
        {
            get
            {
                return true;
            }
        }

        public override bool StopDanceAnimation
        {
            get
            {
                return true;
            }
        }

        protected Condition_Entangled()
        {
        }

        public Condition_Entangled(ConditionSpec spec)
            : base(spec)
        {
        }

        public Condition_Entangled(ConditionSpec spec, int turnsLeft)
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