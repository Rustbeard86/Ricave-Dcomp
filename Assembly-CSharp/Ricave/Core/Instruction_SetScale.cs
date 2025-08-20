using System;
using UnityEngine;

namespace Ricave.Core
{
    public class Instruction_SetScale : Instruction
    {
        public Entity Entity
        {
            get
            {
                return this.entity;
            }
        }

        public Vector3 NewScale
        {
            get
            {
                return this.newScale;
            }
        }

        protected Instruction_SetScale()
        {
        }

        public Instruction_SetScale(Entity entity, Vector3 newScale)
        {
            this.entity = entity;
            this.newScale = newScale;
        }

        protected override void DoImpl()
        {
            this.prevScale = this.entity.Scale;
            this.entity.Scale = this.newScale;
        }

        protected override void UndoImpl()
        {
            this.entity.Scale = this.prevScale;
        }

        [Saved]
        private Entity entity;

        [Saved]
        private Vector3 newScale;

        [Saved]
        private Vector3 prevScale;
    }
}