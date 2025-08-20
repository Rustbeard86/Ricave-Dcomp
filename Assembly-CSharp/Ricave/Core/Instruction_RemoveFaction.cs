using System;

namespace Ricave.Core
{
    public class Instruction_RemoveFaction : Instruction
    {
        public Faction Faction
        {
            get
            {
                return this.faction;
            }
        }

        protected Instruction_RemoveFaction()
        {
        }

        public Instruction_RemoveFaction(Faction faction)
        {
            this.faction = faction;
        }

        protected override void DoImpl()
        {
            this.removedFromIndex = Get.FactionManager.RemoveFaction(this.faction);
        }

        protected override void UndoImpl()
        {
            Get.FactionManager.AddFaction(this.faction, this.removedFromIndex);
        }

        [Saved]
        private Faction faction;

        [Saved]
        private int removedFromIndex;
    }
}