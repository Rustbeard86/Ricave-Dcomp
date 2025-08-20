using System;

namespace Ricave.Core
{
    public class AINode_WasHarmedRecently : AINode_Conditional
    {
        protected override bool ConditionMet(Actor actor)
        {
            return AINode_WasHarmedRecently.WasHarmedRecently(actor, this.turns);
        }

        public static bool WasHarmedRecently(Actor actor, int turns)
        {
            if (actor.AIMemory.LastHarmedSequence != null)
            {
                int? lastHarmedSequence = actor.AIMemory.LastHarmedSequence;
                int num = Get.TurnManager.CurrentSequence - Math.Max(12, actor.SequencePerTurn) * turns;
                return (lastHarmedSequence.GetValueOrDefault() >= num) & (lastHarmedSequence != null);
            }
            return false;
        }

        [Saved(1, false)]
        private int turns = 1;
    }
}