using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Action_ReorderSpell : Action
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

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, int, int, int>(this.actor.MyStableHash, this.spell.MyStableHash, this.destinationIndex, 613280623);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.actor, this.spell);
            }
        }

        protected Action_ReorderSpell()
        {
        }

        public Action_ReorderSpell(ActionSpec spec, Actor actor, Spell spell, int destinationIndex)
            : base(spec)
        {
            this.actor = actor;
            this.spell = spell;
            this.destinationIndex = destinationIndex;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            if (this.spell.Parent == null)
            {
                return false;
            }
            if (!ignoreActorState)
            {
                if (!this.actor.Spawned)
                {
                    return false;
                }
                if (!this.actor.Spells.Contains(this.spell))
                {
                    return false;
                }
                if (this.actor.Spells.All.IndexOf(this.spell) == this.destinationIndex)
                {
                    return false;
                }
                if (this.destinationIndex < 0 || this.destinationIndex >= this.actor.Spells.Count)
                {
                    return false;
                }
            }
            return true;
        }

        protected override bool CalculateConcernsPlayer()
        {
            return ActionUtility.TargetConcernsPlayer(this.actor) || ActionUtility.UsableConcernsPlayer(this.spell);
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            yield return new Instruction_ReorderSpell(this.actor, this.spell, this.destinationIndex);
            yield break;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private Spell spell;

        [Saved]
        private int destinationIndex;
    }
}