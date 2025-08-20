using System;

namespace Ricave.Core
{
    public class AINode_PlayerHasAttackedRecently : AINode_Conditional
    {
        protected override bool ConditionMet(Actor actor)
        {
            Actor nowControlledActor = Get.NowControlledActor;
            if (nowControlledActor.AIMemory.LastUseOnHostileSequence != null)
            {
                int? lastUseOnHostileSequence = nowControlledActor.AIMemory.LastUseOnHostileSequence;
                int num = nowControlledActor.Sequence - Math.Max(12, actor.SequencePerTurn) * this.turns;
                return (lastUseOnHostileSequence.GetValueOrDefault() >= num) & (lastUseOnHostileSequence != null);
            }
            return false;
        }

        [Saved(1, false)]
        private int turns = 1;
    }
}