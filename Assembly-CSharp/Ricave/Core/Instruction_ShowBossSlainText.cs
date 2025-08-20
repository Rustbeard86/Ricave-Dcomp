using System;

namespace Ricave.Core
{
    public class Instruction_ShowBossSlainText : Instruction
    {
        public Actor Boss
        {
            get
            {
                return this.boss;
            }
        }

        protected Instruction_ShowBossSlainText()
        {
        }

        public Instruction_ShowBossSlainText(Actor boss)
        {
            this.boss = boss;
        }

        protected override void DoImpl()
        {
            Get.BossSlainText.ShowTextFor(this.boss.LabelCap);
        }

        protected override void UndoImpl()
        {
            Get.BossSlainText.StopShowingText();
        }

        [Saved]
        private Actor boss;
    }
}