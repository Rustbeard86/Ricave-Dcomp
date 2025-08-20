using System;

namespace Ricave.Core
{
    public class Instruction_UnlockSkill : Instruction
    {
        public SkillSpec SkillSpec
        {
            get
            {
                return this.skillSpec;
            }
        }

        protected Instruction_UnlockSkill()
        {
        }

        public Instruction_UnlockSkill(SkillSpec skillSpec)
        {
            this.skillSpec = skillSpec;
        }

        protected override void DoImpl()
        {
            Get.SkillManager.Unlock(this.skillSpec);
        }

        protected override void UndoImpl()
        {
            Get.SkillManager.UndoUnlock(this.skillSpec);
        }

        [Saved]
        private SkillSpec skillSpec;
    }
}