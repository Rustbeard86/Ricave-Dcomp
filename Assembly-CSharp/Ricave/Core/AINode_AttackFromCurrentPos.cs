using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class AINode_AttackFromCurrentPos : AINode
    {
        public override IEnumerable<Instruction> MakeThinkInstructions(Actor actor)
        {
            IUsable weaponToUse = AIUtility_Attack.GetWeaponToUse(actor, true);
            if (weaponToUse != null)
            {
                Actor actor2 = AIUtility_Attack.TryGetActorToAttackFromCurrentPos(actor, weaponToUse);
                if (actor2 != null || actor.AIMemory.AttackTarget != null)
                {
                    yield return new Instruction_SetAttackTarget(actor, actor2);
                }
            }
            else if (actor.AIMemory.AttackTarget != null)
            {
                yield return new Instruction_SetAttackTarget(actor, null);
            }
            yield break;
        }

        public override Action GetNextAction(Actor actor)
        {
            IUsable weaponToUse = AIUtility_Attack.GetWeaponToUse(actor, true);
            if (weaponToUse == null)
            {
                return null;
            }
            return AIUtility_Attack.TryGetAttackAnyFromCurrentPosAction(actor, weaponToUse);
        }
    }
}