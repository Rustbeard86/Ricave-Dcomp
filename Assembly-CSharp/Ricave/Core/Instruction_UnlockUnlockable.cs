using System;

namespace Ricave.Core
{
    public class Instruction_UnlockUnlockable : Instruction
    {
        public UnlockableSpec UnlockableSpec
        {
            get
            {
                return this.unlockableSpec;
            }
        }

        protected Instruction_UnlockUnlockable()
        {
        }

        public Instruction_UnlockUnlockable(UnlockableSpec unlockableSpec)
        {
            this.unlockableSpec = unlockableSpec;
        }

        protected override void DoImpl()
        {
            Get.UnlockableManager.Unlock(this.unlockableSpec);
        }

        protected override void UndoImpl()
        {
            Get.UnlockableManager.RemoveFromDirectlyUnlocked(this.unlockableSpec);
        }

        [Saved]
        private UnlockableSpec unlockableSpec;
    }
}