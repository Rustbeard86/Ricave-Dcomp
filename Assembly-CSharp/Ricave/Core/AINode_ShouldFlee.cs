using System;

namespace Ricave.Core
{
    public class AINode_ShouldFlee : AINode_Conditional
    {
        public static bool CanEverFlee(Actor actor)
        {
            return actor.IsBoss && actor.Spec.Actor.CanFlee;
        }

        protected override bool ConditionMet(Actor actor)
        {
            return AINode_ShouldFlee.CanEverFlee(actor) && (float)actor.HP / (float)actor.MaxHP < 0.35f;
        }

        private const float FleePercent = 0.35f;
    }
}