using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class Condition_Shield : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Bad;
            }
        }

        public override Color IconColor
        {
            get
            {
                return Color.white;
            }
        }

        public override bool DisableDamageDealtToPlayer
        {
            get
            {
                return true;
            }
        }

        protected Condition_Shield()
        {
        }

        public Condition_Shield(ConditionSpec spec)
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