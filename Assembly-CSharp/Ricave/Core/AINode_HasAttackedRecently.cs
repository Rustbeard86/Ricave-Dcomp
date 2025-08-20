using System;

namespace Ricave.Core
{
    public class AINode_HasAttackedRecently : AINode_Conditional
    {
        protected override bool ConditionMet(Actor actor)
        {
            return AINode_HasAttackedRecently.HasAttackedRecently(actor, this.turns);
        }

        public static bool HasAttackedRecently(Actor actor, int turns)
        {
            if (actor.AIMemory.LastUseSequence != null)
            {
                int? lastUseSequence = actor.AIMemory.LastUseSequence;
                int num = actor.Sequence - 12 * turns;
                return (lastUseSequence.GetValueOrDefault() >= num) & (lastUseSequence != null);
            }
            return false;
        }

        [Saved(1, false)]
        private int turns = 1;
    }
}