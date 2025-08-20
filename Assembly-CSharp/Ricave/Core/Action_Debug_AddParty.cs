using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Action_Debug_AddParty : Action
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, int>(this.actor.MyStableHash, 632134245);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.actor);
            }
        }

        protected Action_Debug_AddParty()
        {
        }

        public Action_Debug_AddParty(ActionSpec spec, Actor actor)
            : base(spec)
        {
            this.actor = actor;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            return true;
        }

        protected override bool CalculateConcernsPlayer()
        {
            return true;
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            if (!this.actor.IsPlayerParty)
            {
                yield return new Instruction_AddPartyMember(this.actor);
            }
            yield break;
        }

        [Saved]
        private Actor actor;
    }
}