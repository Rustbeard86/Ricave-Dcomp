using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Action_Debug_AttachToChainPost : Action
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
                return Calc.CombineHashes<int, int, int>(this.actor.MyStableHash, this.chainPost.MyStableHash, 436090014);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.actor, this.chainPost);
            }
        }

        protected Action_Debug_AttachToChainPost()
        {
        }

        public Action_Debug_AttachToChainPost(ActionSpec spec, Actor actor, ChainPost chainPost)
            : base(spec)
        {
            this.actor = actor;
            this.chainPost = chainPost;
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
            yield return new Instruction_AttachToChainPost(this.actor, this.chainPost);
            yield break;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private ChainPost chainPost;
    }
}