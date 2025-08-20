using System;

namespace Ricave.Core
{
    public class Instruction_CompleteQuest : Instruction
    {
        public QuestSpec QuestSpec
        {
            get
            {
                return this.questSpec;
            }
        }

        protected Instruction_CompleteQuest()
        {
        }

        public Instruction_CompleteQuest(QuestSpec questSpec)
        {
            this.questSpec = questSpec;
        }

        protected override void DoImpl()
        {
            if (Get.InLobby)
            {
                Get.QuestManager.Complete(this.questSpec);
                return;
            }
            Get.ThisRunCompletedQuests.MarkCompleted(this.questSpec);
            Get.QuestCompletedText.OnQuestCompleted(this.questSpec);
        }

        protected override void UndoImpl()
        {
            if (!Get.InLobby)
            {
                Get.ThisRunCompletedQuests.MarkNotCompleted(this.questSpec);
                Get.QuestCompletedText.OnUndoQuestCompleted(this.questSpec);
            }
        }

        [Saved]
        private QuestSpec questSpec;
    }
}