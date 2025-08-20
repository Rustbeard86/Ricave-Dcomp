using System;

namespace Ricave.Core
{
    public class Instruction_ChangeTrialOfSwiftKillerEnemiesKilled : Instruction
    {
        public Condition_TrialOfSwiftKiller Condition
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

        protected Instruction_ChangeTrialOfSwiftKillerEnemiesKilled()
        {
        }

        public Instruction_ChangeTrialOfSwiftKillerEnemiesKilled(Condition_TrialOfSwiftKiller condition, int offset)
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
        private Condition_TrialOfSwiftKiller condition;

        [Saved]
        private int offset;
    }
}