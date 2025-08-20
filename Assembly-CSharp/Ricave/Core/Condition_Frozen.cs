using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_Frozen : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Bad;
            }
        }

        public override bool AnyForcedAction
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

        public override bool DisableFlying
        {
            get
            {
                return true;
            }
        }

        protected Condition_Frozen()
        {
        }

        public Condition_Frozen(ConditionSpec spec)
            : base(spec)
        {
        }

        public Condition_Frozen(ConditionSpec spec, int turnsLeft)
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

        public override Action GetForcedAction(Actor actor)
        {
            return new Action_Wait(Get.Action_Wait, actor, actor.IsNowControlledActor ? new float?(0.5f) : null, "WaitsReason".Translate(this.LabelBase));
        }
    }
}