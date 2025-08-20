using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Action_PulledByChain : Action
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        public ChainPost ChainPost
        {
            get
            {
                return this.chainPost;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, int, int>(this.actor.MyStableHash, this.chainPost.MyStableHash, 821574907);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.actor, this.chainPost);
            }
        }

        protected Action_PulledByChain()
        {
        }

        public Action_PulledByChain(ActionSpec spec, Actor actor, ChainPost chainPost)
            : base(spec)
        {
            this.actor = actor;
            this.chainPost = chainPost;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            return ignoreActorState || (this.actor.Spawned && this.actor.AttachedToChainPost == this.chainPost && !this.actor.Position.IsAdjacentOrInside(this.chainPost.Position));
        }

        protected override bool CalculateConcernsPlayer()
        {
            return ActionUtility.TargetConcernsPlayer(this.actor) || ActionUtility.TargetConcernsPlayer(this.chainPost);
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            int sequencePerTurn = this.actor.SequencePerTurn;
            yield return new Instruction_Awareness_PreAction(this.actor);
            foreach (Instruction instruction in this.PullInstructions())
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            if (this.actor.ChargedAttack != 0)
            {
                yield return new Instruction_SetChargedAttack(this.actor, 0);
            }
            yield return new Instruction_Awareness_PostAction(this.actor);
            if (sequencePerTurn != 0)
            {
                yield return new Instruction_AddSequence(this.actor, sequencePerTurn);
            }
            yield break;
            yield break;
        }

        private IEnumerable<Instruction> PullInstructions()
        {
            foreach (Instruction instruction in InstructionSets_Entity.Push(this.actor, this.chainPost.Position - this.actor.Position, 1, true, true, true))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private ChainPost chainPost;
    }
}