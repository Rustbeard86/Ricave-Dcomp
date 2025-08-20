using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Action_ChangePartyMemberAIMode : Action
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        public PartyMemberAIMode Mode
        {
            get
            {
                return this.mode;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, int>(this.actor.MyStableHash, 782134589);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.actor);
            }
        }

        protected Action_ChangePartyMemberAIMode()
        {
        }

        public Action_ChangePartyMemberAIMode(ActionSpec spec, Actor actor, PartyMemberAIMode mode)
            : base(spec)
        {
            this.actor = actor;
            this.mode = mode;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            if (!ignoreActorState)
            {
                if (!this.actor.IsPlayerParty)
                {
                    return false;
                }
                if (this.actor.PartyMemberAIMode == this.mode)
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
            yield return new Instruction_ChangePartyMemberAIMode(this.actor, this.mode);
            yield break;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private PartyMemberAIMode mode;
    }
}