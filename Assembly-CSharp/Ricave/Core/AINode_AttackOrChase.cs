using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class AINode_AttackOrChase : AINode
    {
        public override IEnumerable<Instruction> MakeThinkInstructions(Actor actor)
        {
            IUsable weaponToUse = AIUtility_Attack.GetWeaponToUse(actor, true);
            if (weaponToUse == null)
            {
                if (actor.AIMemory.AttackTarget != null)
                {
                    yield return new Instruction_SetAttackTarget(actor, null);
                }
                yield break;
            }
            Actor attackTarget = actor.AIMemory.AttackTarget;
            Vector3Int? lastKnownAttackTargetPos = actor.AIMemory.LastKnownAttackTargetPos;
            if (attackTarget == null || !AIUtility_Attack.IsAttackTargetValidAndReachableFor(attackTarget, actor, weaponToUse, lastKnownAttackTargetPos, this.keepDistance))
            {
                Actor newBestAttackTargetFor = AIUtility_Attack.GetNewBestAttackTargetFor(actor);
                if (newBestAttackTargetFor != null || attackTarget != null)
                {
                    yield return new Instruction_SetAttackTarget(actor, newBestAttackTargetFor);
                }
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
            Action action = AIUtility_Attack.TryGetAttackAnyFromCurrentPosAction(actor, weaponToUse);
            if (action != null)
            {
                return action;
            }
            if (actor.AIMemory.AttackTarget == null)
            {
                return null;
            }
            return AIUtility_Attack.TryGetAttackOrApproachAction(actor, weaponToUse, actor.AIMemory.AttackTarget, actor.AIMemory.LastKnownAttackTargetPos, this.keepDistance);
        }

        [Saved]
        private bool keepDistance;
    }
}