using System;

namespace Ricave.Core
{
    public class Instruction_SetFaction : Instruction
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        public Faction Faction
        {
            get
            {
                return this.faction;
            }
        }

        protected Instruction_SetFaction()
        {
        }

        public Instruction_SetFaction(Actor actor, Faction faction)
        {
            this.actor = actor;
            this.faction = faction;
        }

        protected override void DoImpl()
        {
            this.prevFaction = this.actor.Faction;
            this.actor.Faction = this.faction;
        }

        protected override void UndoImpl()
        {
            this.actor.Faction = this.prevFaction;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private Faction faction;

        [Saved]
        private Faction prevFaction;
    }
}