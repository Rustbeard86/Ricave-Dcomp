using System;
using System.Collections.Generic;
using Ricave.UI;

namespace Ricave.Core
{
    public class Action_ChooseSpell : Action
    {
        public SpellSpec Spell
        {
            get
            {
                return this.spell;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, int>(this.spell.MyStableHash, 768715095);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.spell);
            }
        }

        protected Action_ChooseSpell()
        {
        }

        public Action_ChooseSpell(ActionSpec spec, SpellSpec spell)
            : base(spec)
        {
            this.spell = spell;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            if (!ignoreActorState)
            {
                if (!Get.MainActor.Spawned)
                {
                    return false;
                }
                if (!SpellChooserUtility.ChoicesToShow.Contains(this.spell))
                {
                    return false;
                }
            }
            return true;
        }

        protected override bool CalculateConcernsPlayer()
        {
            return true;
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            Spell spell = Maker.Make(this.spell);
            yield return new Instruction_AddSpell(spell, Get.MainActor.Spells);
            yield break;
        }

        [Saved]
        private SpellSpec spell;
    }
}