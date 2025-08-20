using System;
using UnityEngine;

namespace Ricave.Core
{
    public class Instruction_ChangePosition : Instruction
    {
        public Entity Entity
        {
            get
            {
                return this.entity;
            }
        }

        public Vector3Int To
        {
            get
            {
                return this.to;
            }
        }

        protected Instruction_ChangePosition()
        {
        }

        public Instruction_ChangePosition(Entity entity, Vector3Int to)
        {
            this.entity = entity;
            this.to = to;
        }

        protected override void DoImpl()
        {
            this.prevPos = this.entity.Position;
            this.entity.Position = this.to;
        }

        protected override void UndoImpl()
        {
            this.entity.Position = this.prevPos;
        }

        [Saved]
        private Entity entity;

        [Saved]
        private Vector3Int to;

        [Saved]
        private Vector3Int prevPos;
    }
}