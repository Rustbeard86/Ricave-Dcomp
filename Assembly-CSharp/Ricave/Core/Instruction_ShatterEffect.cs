using System;
using UnityEngine;

namespace Ricave.Core
{
    public class Instruction_ShatterEffect : Instruction
    {
        public Entity Entity
        {
            get
            {
                return this.entity;
            }
        }

        public Vector3Int? ImpactSource
        {
            get
            {
                return this.impactSource;
            }
        }

        protected Instruction_ShatterEffect()
        {
        }

        public Instruction_ShatterEffect(Entity entity, Vector3Int? impactSource)
        {
            this.entity = entity;
            this.impactSource = impactSource;
        }

        protected override void DoImpl()
        {
            if (this.entity.EntityGOC != null && this.entity.EntityGOC.gameObject.activeInHierarchy)
            {
                Actor actor = this.entity as Actor;
                float num = ((actor != null) ? actor.Spec.Actor.ShatterForceMultiplier : 1f);
                Actor actor2 = this.entity as Actor;
                if ((actor2 != null && actor2.Spec.Actor.Texture != null) || this.entity is Item)
                {
                    Get.ShatterManager.Shatter(this.entity.EntityGOC, this.impactSource, num);
                    return;
                }
                if (!(this.entity is Ethereal))
                {
                    Get.VolumeShatterManager.Shatter(this.entity.EntityGOC, this.impactSource, num);
                }
            }
        }

        protected override void UndoImpl()
        {
            if (this.entity is Actor || this.entity is Item)
            {
                Get.ShatterManager.RemoveAll();
                return;
            }
            if (this.entity is Structure)
            {
                Get.VolumeShatterManager.RemoveAll();
            }
        }

        [Saved]
        private Entity entity;

        [Saved]
        private Vector3Int? impactSource;
    }
}