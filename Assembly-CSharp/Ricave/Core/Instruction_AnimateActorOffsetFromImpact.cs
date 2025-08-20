using System;
using UnityEngine;

namespace Ricave.Core
{
    public class Instruction_AnimateActorOffsetFromImpact : Instruction
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        public Vector3Int ImpactSource
        {
            get
            {
                return this.impactSource;
            }
        }

        public bool Powerful
        {
            get
            {
                return this.powerful;
            }
        }

        protected Instruction_AnimateActorOffsetFromImpact()
        {
        }

        public Instruction_AnimateActorOffsetFromImpact(Actor actor, Vector3Int impactSource, bool powerful)
        {
            this.actor = actor;
            this.impactSource = impactSource;
            this.powerful = powerful;
        }

        protected override void DoImpl()
        {
            if (this.actor.ActorGOC != null)
            {
                this.actor.ActorGOC.AddOffsetFromImpact(this.impactSource, this.powerful);
                if (Get.NowControlledActor.Spawned && this.impactSource == Get.NowControlledActor.Position && !this.actor.IsNowControlledActor)
                {
                    Get.CameraEffects.OnActorHitByPlayerWithImpact(this.actor);
                }
            }
        }

        protected override void UndoImpl()
        {
        }

        [Saved]
        private Actor actor;

        [Saved]
        private Vector3Int impactSource;

        [Saved]
        private bool powerful;
    }
}