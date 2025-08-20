using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class Action_ResolveVelocity : Action
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, int>(this.actor.MyStableHash, 850907561);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.actor);
            }
        }

        protected Action_ResolveVelocity()
        {
        }

        public Action_ResolveVelocity(ActionSpec spec, Actor actor)
            : base(spec)
        {
            this.actor = actor;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            return ignoreActorState || (this.actor.Spawned && this.actor.Velocity != 0 && !Get.CellsInfo.IsFallingAt(this.actor.Position, this.actor, false));
        }

        protected override bool CalculateConcernsPlayer()
        {
            return ActionUtility.TargetConcernsPlayer(this.actor);
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            foreach (Instruction instruction in Action_ResolveVelocity.ResolveVelocityInstructions(this.actor))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> ResolveVelocityInstructions(Actor actor)
        {
            if (!actor.CanFly && !Get.CellsInfo.AnyWaterAt(actor.Position))
            {
                int num;
                if (actor.IsMainActor && Get.Skill_NoFallDamage.IsUnlocked())
                {
                    num = 0;
                }
                else if (actor.IsMainActor && Get.TraitManager.FallIncomingDamageFactor == 0f)
                {
                    num = 0;
                }
                else if (actor.Spec.Actor.DisableFallDamage)
                {
                    num = 0;
                }
                else if (actor.Velocity == 1)
                {
                    num = 0;
                }
                else
                {
                    num = actor.Velocity;
                }
                if (num > 0)
                {
                    num = DamageUtility.ApplyDamageProtectionAndClamp(actor, num, Get.DamageType_Fall);
                    foreach (Instruction instruction in InstructionSets_Entity.Damage(actor, num, Get.DamageType_Fall, null, new Vector3Int?(actor.Position - actor.Gravity), false, false, null, null, false, true))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator = null;
                }
            }
            yield return new Instruction_ChangeVelocity(actor, -actor.Velocity);
            if (!actor.CanFly)
            {
                if (actor.IsNowControlledActor)
                {
                    if (Get.CellsInfo.AnyWaterAt(actor.Position))
                    {
                        yield return new Instruction_VisualEffect(Get.VisualEffect_PlayerHitsWater, actor.Position);
                    }
                    else
                    {
                        yield return new Instruction_VisualEffect(Get.VisualEffect_PlayerHitsGround, actor.Position);
                    }
                }
                if (!Get.CellsInfo.AnyWaterAt(actor.Position))
                {
                    yield return new Instruction_VisualEffect(Get.VisualEffect_ActorHitsGround, actor.Position);
                }
            }
            yield break;
            yield break;
        }

        [Saved]
        private Actor actor;
    }
}