using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class AINode_Flee : AINode
    {
        public override IEnumerable<Instruction> MakeThinkInstructions(Actor actor)
        {
            return Enumerable.Empty<Instruction>();
        }

        public override Action GetNextAction(Actor actor)
        {
            return AIUtility_Flee.TryGetFleeFromAllHostilesAction(actor, this.canMoveToEquallyBadPositions);
        }

        [Saved(true, false)]
        private bool canMoveToEquallyBadPositions = true;
    }
}