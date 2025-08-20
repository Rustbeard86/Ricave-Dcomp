using System;

namespace Ricave.Core
{
    public class Instruction_ChangeTrialOfUnblemishedEnemiesKilled : Instruction
    {
        public Condition_TrialOfUnblemished Condition
        {
            get
            {
                return this.condition;
            }
        }

        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ChangeTrialOfUnblemishedEnemiesKilled()
        {
        }

        public Instruction_ChangeTrialOfUnblemishedEnemiesKilled(Condition_TrialOfUnblemished condition, int offset)
        {
            this.condition = condition;
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            this.condition.EnemiesKilled += this.offset;
        }

        protected override void UndoImpl()
        {
            this.condition.EnemiesKilled -= this.offset;
        }

        [Saved]
        private Condition_TrialOfUnblemished condition;

        [Saved]
        private int offset;
    }
}