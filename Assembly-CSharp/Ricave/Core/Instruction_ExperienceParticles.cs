using System;
using UnityEngine;

namespace Ricave.Core
{
    public class Instruction_ExperienceParticles : Instruction
    {
        public Vector3 Position
        {
            get
            {
                return this.position;
            }
        }

        public int Amount
        {
            get
            {
                return this.amount;
            }
        }

        protected Instruction_ExperienceParticles()
        {
        }

        public Instruction_ExperienceParticles(Vector3 position, int amount)
        {
            this.position = position;
            this.amount = amount;
        }

        protected override void DoImpl()
        {
            Quaternion quaternion = Quaternion.LookRotation(Get.VisualEffectsManager.ParticlesFlyingTowardsCameraDestination - this.position);
            for (int i = 0; i < this.amount; i++)
            {
                Vector3 vector = Rand.UnitVector3 * 0.14f;
                Get.VisualEffectsManager.DoOneShot(Get.VisualEffect_ExperienceParticle, this.position + vector, quaternion, Vector3.one, null, null, null);
            }
        }

        protected override void UndoImpl()
        {
            Get.VisualEffectsManager.StopAllOfSpec(Get.VisualEffect_ExperienceParticle);
        }

        [Saved]
        private Vector3 position;

        [Saved]
        private int amount;

        private const float MaxOffset = 0.14f;
    }
}