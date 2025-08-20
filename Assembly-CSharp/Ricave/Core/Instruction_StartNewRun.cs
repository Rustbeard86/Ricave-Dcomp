using System;

namespace Ricave.Core
{
    public class Instruction_StartNewRun : Instruction
    {
        public RunConfig Config
        {
            get
            {
                return this.config;
            }
        }

        protected Instruction_StartNewRun()
        {
        }

        public Instruction_StartNewRun(RunConfig config)
        {
            this.config = config;
        }

        protected override void DoImpl()
        {
            Get.ScreenFader.FadeOutAndExecute(delegate
            {
                Root.LoadPlayScene(RootOnSceneChanged.StartNewRun(this.config));
            }, new float?(2.5f), false, true, false);
        }

        protected override void UndoImpl()
        {
            Log.Error("Tried to undo start new run instruction.", false);
        }

        [Saved]
        private RunConfig config;
    }
}