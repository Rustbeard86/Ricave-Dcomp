using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class Action_Sleep : Action
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
                return Calc.CombineHashes<int, int>(this.actor.MyStableHash, 712169043);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.actor);
            }
        }

        protected Action_Sleep()
        {
        }

        public Action_Sleep(ActionSpec spec, Actor actor)
            : base(spec)
        {
            this.actor = actor;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            return ignoreActorState || this.actor.Spawned;
        }

        protected override bool CalculateConcernsPlayer()
        {
            return ActionUtility.TargetConcernsPlayer(this.actor);
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            int sequencePerTurn = this.actor.SequencePerTurn;
            if (ActionUtility.TargetConcernsPlayer(this.actor) && !this.actor.IsNowControlledActor)
            {
                yield return new Instruction_AddFloatingText(this.actor, "Zzz".Translate(), new Color(0.8f, 0.8f, 0.8f), 0.4f, 0f, 0f, null);
            }
            yield return new Instruction_AddSequence(this.actor, sequencePerTurn);
            yield break;
        }

        [Saved]
        private Actor actor;
    }
}