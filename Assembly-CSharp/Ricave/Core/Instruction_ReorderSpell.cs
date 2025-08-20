using System;

namespace Ricave.Core
{
    public class Instruction_ReorderSpell : Instruction
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        public Spell Spell
        {
            get
            {
                return this.spell;
            }
        }

        public int DestinationIndex
        {
            get
            {
                return this.destinationIndex;
            }
        }

        protected Instruction_ReorderSpell()
        {
        }

        public Instruction_ReorderSpell(Actor actor, Spell spell, int destinationIndex)
        {
            this.actor = actor;
            this.spell = spell;
            this.destinationIndex = destinationIndex;
        }

        protected override void DoImpl()
        {
            this.movedFromIndex = this.actor.Spells.All.IndexOf(this.spell);
            this.actor.Spells.Reorder(this.movedFromIndex, this.destinationIndex);
        }

        protected override void UndoImpl()
        {
            this.actor.Spells.Reorder(this.destinationIndex, this.movedFromIndex);
        }

        [Saved]
        private Actor actor;

        [Saved]
        private Spell spell;

        [Saved]
        private int destinationIndex;

        [Saved]
        private int movedFromIndex;
    }
}