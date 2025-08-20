using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Action_SwitchCurrentPartyMember : Action
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
                return Calc.CombineHashes<int, int>(this.actor.MyStableHash, 546323260);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.actor);
            }
        }

        protected Action_SwitchCurrentPartyMember()
        {
        }

        public Action_SwitchCurrentPartyMember(ActionSpec spec, Actor actor)
            : base(spec)
        {
            this.actor = actor;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            return this.actor.Spawned && this.actor.IsPlayerParty && !this.actor.IsNowControlledActor;
        }

        protected override bool CalculateConcernsPlayer()
        {
            return true;
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            if (Get.NowControlledActor != null && Get.NowControlledActor.Spawned)
            {
                yield return new Instruction_TradeSequenceablePlaces(Get.NowControlledActor, this.actor);
            }
            foreach (Instruction instruction in InstructionSets_Actor.SwitchNowControlledActor(this.actor))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            if (this.actor.Conditions.AnyOfSpec(Get.Condition_Sleeping))
            {
                foreach (Instruction instruction2 in InstructionSets_Actor.WakeUp(this.actor, null))
                {
                    yield return instruction2;
                }
                enumerator = null;
            }
            yield break;
            yield break;
        }

        [Saved]
        private Actor actor;
    }
}