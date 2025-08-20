using System;

namespace Ricave.Core
{
    public class Instruction_ChangeBodyPartHP : Instruction
    {
        public BodyPart BodyPart
        {
            get
            {
                return this.bodyPart;
            }
        }

        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ChangeBodyPartHP()
        {
        }

        public Instruction_ChangeBodyPartHP(BodyPart bodyPart, int offset)
        {
            this.bodyPart = bodyPart;
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            this.bodyPart.HP += this.offset;
        }

        protected override void UndoImpl()
        {
            this.bodyPart.HP -= this.offset;
        }

        [Saved]
        private BodyPart bodyPart;

        [Saved]
        private int offset;
    }
}