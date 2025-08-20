using System;

namespace Ricave.Core
{
    public class Instruction_CompleteAchievement : Instruction
    {
        public AchievementSpec AchievementSpec
        {
            get
            {
                return this.achievementSpec;
            }
        }

        protected Instruction_CompleteAchievement()
        {
        }

        public Instruction_CompleteAchievement(AchievementSpec achievementSpec)
        {
            this.achievementSpec = achievementSpec;
        }

        protected override void DoImpl()
        {
            if (!this.achievementSpec.IsCompleted)
            {
                this.achievementSpec.TryComplete();
            }
        }

        protected override void UndoImpl()
        {
        }

        [Saved]
        private AchievementSpec achievementSpec;
    }
}