using System;
using UnityEngine;

namespace Ricave.Core
{
    public class Instruction_BodyPartShatterEffect : Instruction
    {
        public BodyPart BodyPart
        {
            get
            {
                return this.bodyPart;
            }
        }

        public Vector3Int? ImpactSource
        {
            get
            {
                return this.impactSource;
            }
        }

        protected Instruction_BodyPartShatterEffect()
        {
        }

        public Instruction_BodyPartShatterEffect(BodyPart bodyPart, Vector3Int? impactSource)
        {
            this.bodyPart = bodyPart;
            this.impactSource = impactSource;
        }

        protected override void DoImpl()
        {
            if (this.bodyPart.Actor.EntityGOC != null && this.bodyPart.Actor.EntityGOC.gameObject.activeInHierarchy)
            {
                if (this.bodyPart.Actor.Spec.Actor.Texture != null)
                {
                    Get.ShatterManager.Shatter(this.bodyPart, this.impactSource, this.bodyPart.Actor.Spec.Actor.ShatterForceMultiplier);
                    return;
                }
                Get.VolumeShatterManager.Shatter(this.bodyPart, this.impactSource, this.bodyPart.Actor.Spec.Actor.ShatterForceMultiplier);
            }
        }

        protected override void UndoImpl()
        {
            Get.ShatterManager.RemoveAll();
        }

        [Saved]
        private BodyPart bodyPart;

        [Saved]
        private Vector3Int? impactSource;
    }
}