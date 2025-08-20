using System;

namespace Ricave.Core
{
    public class AINode_ShouldBackOff : AINode_Conditional
    {
        protected override bool ConditionMet(Actor actor)
        {
            if (actor.AIMemory.LastUseSequence != null)
            {
                int? lastUseSequence = actor.AIMemory.LastUseSequence;
                int num = actor.Sequence - actor.SequencePerTurn;
                if ((lastUseSequence.GetValueOrDefault() >= num) & (lastUseSequence != null))
                {
                    return AIUtility.AnyAdjacentHostileActorCanTouchMeAndCanAttackMeBeforeMyTurnAt(actor.Position, actor);
                }
            }
            return false;
        }
    }
}