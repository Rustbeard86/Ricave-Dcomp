using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class Action_MoveSelf : Action
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        public Vector3Int From
        {
            get
            {
                return this.from;
            }
        }

        public Vector3Int To
        {
            get
            {
                return this.to;
            }
        }

        public float? CustomOptionalDelay
        {
            get
            {
                return this.customOptionalDelay;
            }
        }

        public bool DrunkEffect
        {
            get
            {
                return this.drunkEffect;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, Vector3Int, Vector3Int, int>(this.actor.MyStableHash, this.from, this.to, 745319079);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.actor, (this.to - this.from).DirectionToString());
            }
        }

        protected Action_MoveSelf()
        {
        }

        public Action_MoveSelf(ActionSpec spec, Actor actor, Vector3Int from, Vector3Int to, float? customOptionalDelay = null, bool drunkEffect = false)
            : base(spec)
        {
            this.actor = actor;
            this.from = from;
            this.to = to;
            this.customOptionalDelay = customOptionalDelay;
            this.drunkEffect = drunkEffect;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            return ignoreActorState || (this.actor.Spawned && this.actor.Position == this.from && Get.World.CanMoveFromTo(this.from, this.to, this.actor));
        }

        protected override bool CalculateConcernsPlayer()
        {
            return ActionUtility.TargetConcernsPlayer(this.actor);
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            int sequencePerMove = this.actor.SequencePerMove;
            yield return new Instruction_Awareness_PreAction(this.actor);
            foreach (Instruction instruction in InstructionSets_Entity.Move(this.actor, this.to, true, true))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            if (ActionUtility.TargetConcernsPlayer(this.actor))
            {
                yield return new Instruction_OptionalDelay(this.customOptionalDelay ?? 0.1f);
            }
            if (this.drunkEffect && this.actor.IsNowControlledActor)
            {
                yield return new Instruction_VisualEffect(Get.VisualEffect_PlayerDrunk, this.actor.Position);
            }
            if (this.actor.ChargedAttack != 0)
            {
                yield return new Instruction_SetChargedAttack(this.actor, 0);
            }
            yield return new Instruction_Awareness_PostAction(this.actor);
            yield return new Instruction_AddSequence(this.actor, sequencePerMove);
            yield break;
            yield break;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private Vector3Int from;

        [Saved]
        private Vector3Int to;

        [Saved]
        private float? customOptionalDelay;

        [Saved]
        private bool drunkEffect;
    }
}