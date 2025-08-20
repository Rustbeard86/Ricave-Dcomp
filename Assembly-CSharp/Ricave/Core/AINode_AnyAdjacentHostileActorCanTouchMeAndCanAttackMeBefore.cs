using System;

namespace Ricave.Core
{
    public class AINode_AnyAdjacentHostileActorCanTouchMeAndCanAttackMeBeforeMyTurn : AINode_Conditional
    {
        protected override bool ConditionMet(Actor actor)
        {
            return AIUtility.AnyAdjacentHostileActorCanTouchMeAndCanAttackMeBeforeMyTurnAt(actor.Position, actor);
        }
    }
}