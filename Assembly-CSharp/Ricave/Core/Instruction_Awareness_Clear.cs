using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Instruction_Awareness_Clear : Instruction
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        protected Instruction_Awareness_Clear()
        {
        }

        public Instruction_Awareness_Clear(Actor actor)
        {
            this.actor = actor;
        }

        protected override void DoImpl()
        {
            this.prevAwareOf.Clear();
            this.prevAwareOf.AddRange(this.actor.AIMemory.AwareOf);
            this.actor.AIMemory.AwareOf.Clear();
            this.prevAwareOfPre.Clear();
            this.prevAwareOfPre.AddRange(this.actor.AIMemory.AwareOfPre);
            this.actor.AIMemory.AwareOfPre.Clear();
            this.prevAwareOfPost.Clear();
            this.prevAwareOfPost.AddRange(this.actor.AIMemory.AwareOfPost);
            this.actor.AIMemory.AwareOfPost.Clear();
            this.prevLastSeenPlayerSequence = this.actor.AIMemory.LastSeenPlayerSequence;
            this.actor.AIMemory.LastSeenPlayerSequence = null;
        }

        protected override void UndoImpl()
        {
            this.actor.AIMemory.AwareOf.Clear();
            this.actor.AIMemory.AwareOf.AddRange(this.prevAwareOf);
            this.actor.AIMemory.AwareOfPre.Clear();
            this.actor.AIMemory.AwareOfPre.AddRange(this.prevAwareOfPre);
            this.actor.AIMemory.AwareOfPost.Clear();
            this.actor.AIMemory.AwareOfPost.AddRange(this.prevAwareOfPost);
            this.actor.AIMemory.LastSeenPlayerSequence = this.prevLastSeenPlayerSequence;
        }

        [Saved]
        private Actor actor;

        [Saved(Default.New, true)]
        private List<Actor> prevAwareOf = new List<Actor>();

        [Saved(Default.New, true)]
        private List<Actor> prevAwareOfPre = new List<Actor>();

        [Saved(Default.New, true)]
        private List<Actor> prevAwareOfPost = new List<Actor>();

        [Saved]
        private int? prevLastSeenPlayerSequence;
    }
}