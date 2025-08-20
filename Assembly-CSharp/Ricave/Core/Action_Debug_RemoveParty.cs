using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Action_Debug_RemoveParty : Action
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
                return Calc.CombineHashes<int, int>(this.actor.MyStableHash, 896743425);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.actor);
            }
        }

        protected Action_Debug_RemoveParty()
        {
        }

        public Action_Debug_RemoveParty(ActionSpec spec, Actor actor)
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
            if (this.actor.IsPlayerParty)
            {
                yield return new Instruction_RemovePartyMember(this.actor);
            }
            yield break;
        }

        [Saved]
        private Actor actor;
    }
}