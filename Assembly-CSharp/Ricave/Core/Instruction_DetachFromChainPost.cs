using System;

namespace Ricave.Core
{
    public class Instruction_DetachFromChainPost : Instruction
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        protected Instruction_DetachFromChainPost()
        {
        }

        public Instruction_DetachFromChainPost(Actor actor)
        {
            this.actor = actor;
        }

        protected override void DoImpl()
        {
            this.prevChainPost = this.actor.AttachedToChainPostDirect;
            ChainPost chainPost = this.prevChainPost;
            this.prevChainPostAttachedActorDirect = ((chainPost != null) ? chainPost.AttachedActorDirect : null);
            this.actor.AttachedToChainPostDirect = null;
            if (this.prevChainPost != null && this.prevChainPost.AttachedActorDirect == this.actor)
            {
                this.prevChainPost.AttachedActorDirect = null;
            }
        }

        protected override void UndoImpl()
        {
            this.actor.AttachedToChainPostDirect = this.prevChainPost;
            if (this.prevChainPost != null)
            {
                this.prevChainPost.AttachedActorDirect = this.prevChainPostAttachedActorDirect;
            }
        }

        [Saved]
        private Actor actor;

        [Saved]
        private ChainPost prevChainPost;

        [Saved]
        private Actor prevChainPostAttachedActorDirect;
    }
}