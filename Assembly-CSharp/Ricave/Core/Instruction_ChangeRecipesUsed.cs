using System;

namespace Ricave.Core
{
    public class Instruction_ChangeRecipesUsed : Instruction
    {
        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ChangeRecipesUsed()
        {
        }

        public Instruction_ChangeRecipesUsed(int offset)
        {
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            Get.Player.RecipesUsed += this.offset;
        }

        protected override void UndoImpl()
        {
            Get.Player.RecipesUsed -= this.offset;
        }

        [Saved]
        private int offset;
    }
}