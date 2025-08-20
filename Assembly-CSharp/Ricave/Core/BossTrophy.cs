using System;
using UnityEngine;

namespace Ricave.Core
{
    public class BossTrophy : Structure
    {
        public int BossIndex
        {
            get
            {
                return this.bossIndex;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.bossIndex = value;
            }
        }

        public TotalKillCounter.KilledBoss Boss
        {
            get
            {
                if (this.BossIndex != -1)
                {
                    return Get.TotalKillCounter.KilledBosses[this.BossIndex];
                }
                return null;
            }
            set
            {
                this.bossIndex = ((value == null) ? (-1) : Get.TotalKillCounter.KilledBosses.IndexOf(value));
            }
        }

        protected BossTrophy()
        {
        }

        public BossTrophy(EntitySpec spec)
            : base(spec)
        {
        }

        public BossTrophy(string specID, int instanceID, int stableID, Vector3Int pos, Quaternion rot, Vector3 scale)
            : base(specID, instanceID, stableID, pos, rot, scale)
        {
        }

        [Saved(-1, false)]
        private int bossIndex = -1;
    }
}