using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class Action_Wait : Action
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        public float? CustomOptionalDelay
        {
            get
            {
                return this.customOptionalDelay;
            }
        }

        public string CustomWaitsText
        {
            get
            {
                return this.customWaitsText;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, int>(this.actor.MyStableHash, 65773109);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.actor);
            }
        }

        protected Action_Wait()
        {
        }

        public Action_Wait(ActionSpec spec, Actor actor, float? customOptionalDelay = null, string customWaitsText = null)
            : base(spec)
        {
            this.actor = actor;
            this.customOptionalDelay = customOptionalDelay;
            this.customWaitsText = customWaitsText;
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
            yield return new Instruction_Awareness_PreAction(this.actor);
            if (ActionUtility.TargetConcernsPlayer(this.actor))
            {
                yield return new Instruction_OptionalDelay(this.customOptionalDelay ?? 0.1f);
                if (!this.actor.IsNowControlledActor && !Get.InLobby)
                {
                    yield return new Instruction_AddFloatingText(this.actor, this.customWaitsText ?? "Waits".Translate(), new Color(0.8f, 0.8f, 0.8f), 0.4f, 0f, 0f, null);
                }
            }
            if (this.actor.IsPlayerParty && this.actor.ChargedAttack < 2 && !Get.InLobby)
            {
                yield return new Instruction_SetChargedAttack(this.actor, this.actor.ChargedAttack + 1);
            }
            yield return new Instruction_Awareness_PostAction(this.actor);
            yield return new Instruction_AddSequence(this.actor, sequencePerTurn);
            yield break;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private float? customOptionalDelay;

        [Saved]
        private string customWaitsText;
    }
}