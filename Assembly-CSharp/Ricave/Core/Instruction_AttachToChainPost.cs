using System;

namespace Ricave.Core
{
    public class Instruction_AttachToChainPost : Instruction
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

        protected Instruction_AttachToChainPost()
        {
        }

        public Instruction_AttachToChainPost(Actor actor, ChainPost chainPost)
        {
            this.actor = actor;
            this.chainPost = chainPost;
        }

        protected override void DoImpl()
        {
            this.prevActorChainPost = this.actor.AttachedToChainPostDirect;
            this.prevChainPostActor = this.chainPost.AttachedActorDirect;
            this.actor.AttachedToChainPostDirect = this.chainPost;
            this.chainPost.AttachedActorDirect = this.actor;
        }

        protected override void UndoImpl()
        {
            this.actor.AttachedToChainPostDirect = this.prevActorChainPost;
            this.chainPost.AttachedActorDirect = this.prevChainPostActor;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private ChainPost chainPost;

        [Saved]
        private ChainPost prevActorChainPost;

        [Saved]
        private Actor prevChainPostActor;
    }
}