using System;

namespace Ricave.Core
{
    public class Instruction_AddSpell : Instruction
    {
        public Spell Spell
        {
            get
            {
                return this.spell;
            }
        }

        public Spells To
        {
            get
            {
                return this.to;
            }
        }

        protected Instruction_AddSpell()
        {
        }

        public Instruction_AddSpell(Spell spell, Spells to)
        {
            this.spell = spell;
            this.to = to;
        }

        protected override void DoImpl()
        {
            this.to.AddSpell(this.spell, -1);
        }

        protected override void UndoImpl()
        {
            this.to.RemoveSpell(this.spell);
        }

        [Saved]
        private Spell spell;

        [Saved]
        private Spells to;
    }
}