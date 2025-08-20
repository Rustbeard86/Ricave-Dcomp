using System;

namespace Ricave.Core
{
    public class Instruction_ChangePlayerSkillPoints : Instruction
    {
        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ChangePlayerSkillPoints()
        {
        }

        public Instruction_ChangePlayerSkillPoints(int offset)
        {
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            Get.Player.SkillPoints += this.offset;
        }

        protected override void UndoImpl()
        {
            Get.Player.SkillPoints -= this.offset;
        }

        [Saved]
        private int offset;
    }
}