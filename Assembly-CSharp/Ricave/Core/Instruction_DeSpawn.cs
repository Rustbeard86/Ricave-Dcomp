using System;
using UnityEngine;

namespace Ricave.Core
{
    public class Instruction_DeSpawn : Instruction
    {
        public Entity Entity
        {
            get
            {
                return this.entity;
            }
        }

        public bool FadeOut
        {
            get
            {
                return this.fadeOut;
            }
        }

        protected Instruction_DeSpawn()
        {
        }

        public Instruction_DeSpawn(Entity entity, bool fadeOut = false)
        {
            this.entity = entity;
            this.fadeOut = fadeOut;
        }

        protected override void DoImpl()
        {
            this.deSpawnedAt = this.entity.Position;
            this.prevRot = this.entity.Rotation;
            this.prevScale = this.entity.Scale;
            this.entity.DeSpawn(this.fadeOut);
        }

        protected override void UndoImpl()
        {
            this.entity.Spawn(this.deSpawnedAt, new Quaternion?(this.prevRot), this.prevScale);
        }

        [Saved]
        private Entity entity;

        [Saved]
        private bool fadeOut;

        [Saved]
        private Vector3Int deSpawnedAt;

        [Saved]
        private Quaternion prevRot;

        [Saved]
        private Vector3 prevScale;
    }
}