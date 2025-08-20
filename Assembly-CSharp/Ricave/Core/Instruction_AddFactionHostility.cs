using System;

namespace Ricave.Core
{
    public class Instruction_AddFactionHostility : Instruction
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

        protected Instruction_AddFactionHostility()
        {
        }

        public Instruction_AddFactionHostility(Faction faction1, Faction faction2)
        {
            this.faction1 = faction1;
            this.faction2 = faction2;
        }

        protected override void DoImpl()
        {
            Get.FactionManager.AddHostility(this.faction1, this.faction2, -1);
        }

        protected override void UndoImpl()
        {
            Get.FactionManager.RemoveHostility(this.faction1, this.faction2);
        }

        [Saved]
        private Faction faction1;

        [Saved]
        private Faction faction2;
    }
}