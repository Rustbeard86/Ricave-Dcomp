using System;

namespace Ricave.Core
{
    public class Instruction_ChangeBossTrophyHasStardust : Instruction
    {
        public int BossIndex
        {
            get
            {
                return this.bossIndex;
            }
        }

        public bool NewValue
        {
            get
            {
                return this.newValue;
            }
        }

        protected Instruction_ChangeBossTrophyHasStardust()
        {
        }

        public Instruction_ChangeBossTrophyHasStardust(int bossIndex, bool newValue)
        {
            this.bossIndex = bossIndex;
            this.newValue = newValue;
        }

        protected override void DoImpl()
        {
            if (Get.InLobby)
            {
                TotalKillCounter.KilledBoss killedBoss = Get.TotalKillCounter.KilledBosses[this.bossIndex];
                this.prevValue = killedBoss.TrophyHasStardust;
                killedBoss.TrophyHasStardust = this.newValue;
                return;
            }
            Log.Error("Used Instruction_ChangeBossTrophyHasStardust outside of lobby.", false);
        }

        protected override void UndoImpl()
        {
            if (Get.InLobby)
            {
                Get.TotalKillCounter.KilledBosses[this.bossIndex].TrophyHasStardust = this.prevValue;
                return;
            }
            Log.Error("Used Instruction_ChangeBossTrophyHasStardust outside of lobby.", false);
        }

        [Saved(-1, false)]
        private int bossIndex = -1;

        [Saved]
        private bool newValue;

        [Saved]
        private bool prevValue;
    }
}