using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public abstract class AINode
    {
        public abstract IEnumerable<Instruction> MakeThinkInstructions(Actor actor);

        public abstract Action GetNextAction(Actor actor);
    }
}