using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class Action_MoveDownByGravity : Action
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        public Vector3Int From
        {
            get
            {
                return this.from;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, Vector3Int, int>(this.actor.MyStableHash, this.from, 523800631);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.actor);
            }
        }

        protected Action_MoveDownByGravity()
        {
        }

        public Action_MoveDownByGravity(ActionSpec spec, Actor actor, Vector3Int from)
            : base(spec)
        {
            this.actor = actor;
            this.from = from;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            return ignoreActorState || (this.actor.Spawned && this.actor.Position == this.from && Get.CellsInfo.IsFallingAt(this.from, this.actor, false));
        }

        protected override bool CalculateConcernsPlayer()
        {
            return ActionUtility.TargetConcernsPlayer(this.actor);
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            int sequencePerTurn = this.actor.SequencePerTurn;
            yield return new Instruction_Awareness_PreAction(this.actor);
            if (this.actor.IsMainActor && Get.Skill_NoFallDamage.IsUnlocked())
            {
                sequencePerTurn = 0;
                int iterationsGuard = 0;
                while (this.actor.Spawned && Get.CellsInfo.IsFallingAt(this.actor.Position, this.actor, false))
                {
                    if (Get.ScreenFader.AnyActionQueued)
                    {
                        break;
                    }
                    foreach (Instruction instruction in this.MoveDownByGravity())
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator = null;
                    int num = iterationsGuard;
                    iterationsGuard = num + 1;
                    if (iterationsGuard > 1000)
                    {
                        Log.Error("Too many iterations.", false);
                        break;
                    }
                }
            }
            else
            {
                foreach (Instruction instruction2 in this.MoveDownByGravity())
                {
                    yield return instruction2;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            if (this.actor.ChargedAttack != 0)
            {
                yield return new Instruction_SetChargedAttack(this.actor, 0);
            }
            yield return new Instruction_Awareness_PostAction(this.actor);
            if (sequencePerTurn != 0)
            {
                yield return new Instruction_AddSequence(this.actor, sequencePerTurn);
            }
            yield break;
            yield break;
        }

        private IEnumerable<Instruction> MoveDownByGravity()
        {
            Vector3Int vector3Int = this.actor.Position + this.actor.Gravity;
            foreach (Instruction instruction in InstructionSets_Entity.Move(this.actor, vector3Int, true, true))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield return new Instruction_ChangeVelocity(this.actor, 1);
            if (this.actor.Spawned && !Get.CellsInfo.IsFallingAt(this.actor.Position, this.actor, false))
            {
                foreach (Instruction instruction2 in Action_ResolveVelocity.ResolveVelocityInstructions(this.actor))
                {
                    yield return instruction2;
                }
                enumerator = null;
            }
            yield break;
            yield break;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private Vector3Int from;
    }
}