using System;

namespace Ricave.Core
{
    public class Instruction_AddFaction : Instruction
    {
        public Faction Faction
        {
            get
            {
                return this.faction;
            }
        }

        protected Instruction_AddFaction()
        {
        }

        public Instruction_AddFaction(Faction faction)
        {
            this.faction = faction;
        }

        protected override void DoImpl()
        {
            Get.FactionManager.AddFaction(this.faction, -1);
        }

        protected override void UndoImpl()
        {
            Get.FactionManager.RemoveFaction(this.faction);
        }

        [Saved]
        private Faction faction;
    }
}