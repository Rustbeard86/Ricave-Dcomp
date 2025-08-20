using System;
using UnityEngine;

namespace Ricave.Core
{
    public class Instruction_Spawn : Instruction
    {
        public Entity Entity
        {
            get
            {
                return this.entity;
            }
        }

        public Vector3Int At
        {
            get
            {
                return this.at;
            }
        }

        public Quaternion? Rot
        {
            get
            {
                return this.rot;
            }
        }

        protected Instruction_Spawn()
        {
        }

        public Instruction_Spawn(Entity entity, Vector3Int at, Quaternion? rot)
        {
            this.entity = entity;
            this.at = at;
            this.rot = rot;
        }

        protected override void DoImpl()
        {
            this.prevPos = this.entity.Position;
            this.prevRot = this.entity.Rotation;
            this.prevScale = this.entity.Scale;
            this.entity.Spawn(this.at, this.rot, Vector3.one);
        }

        protected override void UndoImpl()
        {
            this.entity.DeSpawn(false);
            this.entity.Position = this.prevPos;
            this.entity.Rotation = this.prevRot;
            this.entity.Scale = this.prevScale;
        }

        [Saved]
        private Entity entity;

        [Saved]
        private Vector3Int at;

        [Saved]
        private Quaternion? rot;

        [Saved]
        private Vector3Int prevPos;

        [Saved]
        private Quaternion prevRot;

        [Saved]
        private Vector3 prevScale;
    }
}