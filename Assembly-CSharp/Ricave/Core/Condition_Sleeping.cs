using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_Sleeping : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Neutral;
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

        public override bool Lying
        {
            get
            {
                return true;
            }
        }

        protected Condition_Sleeping()
        {
        }

        public Condition_Sleeping(ConditionSpec spec)
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

        public override Action GetForcedAction(Actor actor)
        {
            return new Action_Sleep(Get.Action_Sleep, actor);
        }
    }
}