using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_VisualEffect : UseEffect
    {
        public VisualEffectSpec VisualEffectSpec
        {
            get
            {
                return this.visualEffectSpec;
            }
        }

        public bool AtTarget
        {
            get
            {
                return this.atTarget;
            }
        }

        public bool AtSource
        {
            get
            {
                return this.atSource;
            }
        }

        public bool RotateLikeTargetActorMovement
        {
            get
            {
                return this.rotateLikeTargetActorMovement;
            }
        }

        protected UseEffect_VisualEffect()
        {
        }

        public UseEffect_VisualEffect(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            UseEffect_VisualEffect useEffect_VisualEffect = (UseEffect_VisualEffect)clone;
            useEffect_VisualEffect.visualEffectSpec = this.visualEffectSpec;
            useEffect_VisualEffect.atTarget = this.atTarget;
            useEffect_VisualEffect.atSource = this.atSource;
            useEffect_VisualEffect.betweenTargetAndOriginalTarget = this.betweenTargetAndOriginalTarget;
            useEffect_VisualEffect.betweenTargetAndSource = this.betweenTargetAndSource;
            useEffect_VisualEffect.rotateLikeTargetActorMovement = this.rotateLikeTargetActorMovement;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (this.betweenTargetAndOriginalTarget || this.betweenTargetAndSource)
            {
                Target target2 = (this.betweenTargetAndSource ? UseEffect_Sound.GetSourcePos(user, usable, target) : originalTarget);
                Vector3 vector = (target2.Position + target.Position) / 2f;
                yield return new Instruction_VisualEffect(this.visualEffectSpec, vector, (target2.Position == target.Position) ? Quaternion.identity : Quaternion.LookRotation((target.Position - target2.Position).normalized));
            }
            else
            {
                Quaternion quaternion;
                if (this.rotateLikeTargetActorMovement)
                {
                    Actor actor = target.Entity as Actor;
                    if (actor != null && actor.Spawned)
                    {
                        quaternion = Quaternion.LookRotation(actor.ActorGOC.CurrentVisualMovementDirection);
                        goto IL_0144;
                    }
                }
                quaternion = Quaternion.identity;
            IL_0144:
                Quaternion quaternion2 = quaternion;
                if (this.atSource)
                {
                    yield return new Instruction_VisualEffect(this.visualEffectSpec, UseEffect_Sound.GetSourcePos(user, usable, target), quaternion2);
                }
                else
                {
                    yield return new Instruction_VisualEffect(this.visualEffectSpec, target.Position, quaternion2);
                }
            }
            yield break;
        }

        [Saved]
        private VisualEffectSpec visualEffectSpec;

        [Saved]
        private bool atTarget;

        [Saved]
        private bool atSource;

        [Saved]
        private bool betweenTargetAndOriginalTarget;

        [Saved]
        private bool betweenTargetAndSource;

        [Saved]
        private bool rotateLikeTargetActorMovement;
    }
}