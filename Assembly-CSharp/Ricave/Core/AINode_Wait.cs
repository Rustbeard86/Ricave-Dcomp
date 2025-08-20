using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class AINode_Wait : AINode
    {
        public override IEnumerable<Instruction> MakeThinkInstructions(Actor actor)
        {
            return Enumerable.Empty<Instruction>();
        }

        public override Action GetNextAction(Actor actor)
        {
            return new Action_Wait(Get.Action_Wait, actor, null, null);
        }
    }
}