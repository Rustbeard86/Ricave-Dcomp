using System;
using UnityEngine;

namespace Ricave.Core
{
    public class Instruction_SetAttackTarget : Instruction
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        public Actor AttackTarget
        {
            get
            {
                return this.attackTarget;
            }
        }

        public Vector3Int? SpecificLastKnownAttackTargetPos
        {
            get
            {
                return this.specificLastKnownAttackTargetPos;
            }
        }

        protected Instruction_SetAttackTarget()
        {
        }

        public Instruction_SetAttackTarget(Actor actor, Actor attackTarget)
        {
            this.actor = actor;
            this.attackTarget = attackTarget;
        }

        public Instruction_SetAttackTarget(Actor actor, Actor attackTarget, Vector3Int specificLastKnownAttackTargetPos)
        {
            this.actor = actor;
            this.attackTarget = attackTarget;
            this.specificLastKnownAttackTargetPos = new Vector3Int?(specificLastKnownAttackTargetPos);
        }

        protected override void DoImpl()
        {
            this.prevAttackTarget = this.actor.AIMemory.AttackTarget;
            this.prevLastKnownAttackTargetPos = this.actor.AIMemory.LastKnownAttackTargetPos;
            this.actor.AIMemory.AttackTarget = this.attackTarget;
            if (this.attackTarget != null)
            {
                this.actor.AIMemory.LastKnownAttackTargetPos = new Vector3Int?(this.specificLastKnownAttackTargetPos ?? this.attackTarget.Position);
                return;
            }
            this.actor.AIMemory.LastKnownAttackTargetPos = null;
        }

        protected override void UndoImpl()
        {
            this.actor.AIMemory.AttackTarget = this.prevAttackTarget;
            this.actor.AIMemory.LastKnownAttackTargetPos = this.prevLastKnownAttackTargetPos;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private Actor attackTarget;

        [Saved]
        private Vector3Int? specificLastKnownAttackTargetPos;

        [Saved]
        private Actor prevAttackTarget;

        [Saved]
        private Vector3Int? prevLastKnownAttackTargetPos;
    }
}