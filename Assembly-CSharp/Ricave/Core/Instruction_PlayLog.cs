using System;

namespace Ricave.Core
{
    public class Instruction_PlayLog : Instruction
    {
        public string Text
        {
            get
            {
                return this.text;
            }
        }

        protected Instruction_PlayLog()
        {
        }

        public Instruction_PlayLog(string text)
        {
            this.text = text;
        }

        protected override void DoImpl()
        {
            this.playLogEntryID = Get.PlayLog.Add(this.text);
        }

        protected override void UndoImpl()
        {
            Get.PlayLog.TryRemove(this.playLogEntryID);
        }

        [Saved]
        private string text;

        [Saved]
        private int playLogEntryID;
    }
}