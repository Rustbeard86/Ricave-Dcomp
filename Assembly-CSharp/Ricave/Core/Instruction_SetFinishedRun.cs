using System;

namespace Ricave.Core
{
    public class Instruction_SetFinishedRun : Instruction
    {
        public bool Finished
        {
            get
            {
                return this.finished;
            }
        }

        protected Instruction_SetFinishedRun()
        {
        }

        public Instruction_SetFinishedRun(bool finished)
        {
            this.finished = finished;
        }

        protected override void DoImpl()
        {
            this.prevFinished = Get.RunInfo.FinishedRun;
            Get.RunInfo.FinishedRun = this.finished;
        }

        protected override void UndoImpl()
        {
            Get.RunInfo.FinishedRun = this.prevFinished;
        }

        [Saved]
        private bool finished;

        [Saved]
        private bool prevFinished;
    }
}