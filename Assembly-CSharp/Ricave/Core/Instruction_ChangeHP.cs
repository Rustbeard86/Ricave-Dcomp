using System;

namespace Ricave.Core
{
    public class Instruction_ChangeHP : Instruction
    {
        public Entity Entity
        {
            get
            {
                return this.entity;
            }
        }

        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ChangeHP()
        {
        }

        public Instruction_ChangeHP(Entity entity, int offset)
        {
            this.entity = entity;
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            this.entity.HP += this.offset;
        }

        protected override void UndoImpl()
        {
            this.entity.HP -= this.offset;
        }

        [Saved]
        private Entity entity;

        [Saved]
        private int offset;
    }
}