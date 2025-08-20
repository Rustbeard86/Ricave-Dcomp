using System;

namespace Ricave.Core
{
    public class Instruction_ChangeQuestState : Instruction
    {
        public string Key
        {
            get
            {
                return this.key;
            }
        }

        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ChangeQuestState()
        {
        }

        public Instruction_ChangeQuestState(string key, int offset)
        {
            this.key = key;
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            if (Get.InLobby)
            {
                Get.TotalQuestsState.Change(this.key, this.offset);
                return;
            }
            Get.ThisRunQuestsState.Change(this.key, this.offset);
        }

        protected override void UndoImpl()
        {
            if (Get.InLobby)
            {
                Get.TotalQuestsState.Change(this.key, -this.offset);
                return;
            }
            Get.ThisRunQuestsState.Change(this.key, -this.offset);
        }

        [Saved]
        private string key;

        [Saved]
        private int offset;
    }
}