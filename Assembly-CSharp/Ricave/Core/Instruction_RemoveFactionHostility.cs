using System;

namespace Ricave.Core
{
    public class Instruction_RemoveFactionHostility : Instruction
    {
        public Faction Faction1
        {
            get
            {
                return this.faction1;
            }
        }

        public Faction Faction2
        {
            get
            {
                return this.faction2;
            }
        }

        protected Instruction_RemoveFactionHostility()
        {
        }

        public Instruction_RemoveFactionHostility(Faction faction1, Faction faction2)
        {
            this.faction1 = faction1;
            this.faction2 = faction2;
        }

        protected override void DoImpl()
        {
            this.removedFromIndex = Get.FactionManager.RemoveHostility(this.faction1, this.faction2);
        }

        protected override void UndoImpl()
        {
            Get.FactionManager.AddHostility(this.faction1, this.faction2, this.removedFromIndex);
        }

        [Saved]
        private Faction faction1;

        [Saved]
        private Faction faction2;

        [Saved]
        private int removedFromIndex;
    }
}