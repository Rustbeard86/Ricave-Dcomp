using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class Instruction_Awareness_PostAction : Instruction
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        protected Instruction_Awareness_PostAction()
        {
        }

        public Instruction_Awareness_PostAction(Actor actor)
        {
            this.actor = actor;
        }

        protected override void DoImpl()
        {
            this.prevAwareOf.Clear();
            this.prevAwareOf.AddRange(this.actor.AIMemory.AwareOf);
            this.prevAwareOfPre.Clear();
            this.prevAwareOfPre.AddRange(this.actor.AIMemory.AwareOfPre);
            this.prevAwareOfPost.Clear();
            this.prevAwareOfPost.AddRange(this.actor.AIMemory.AwareOfPost);
            List<Actor> awareOf = this.actor.AIMemory.AwareOf;
            for (int i = awareOf.Count - 1; i >= 0; i--)
            {
                if (!this.actor.Sees(awareOf[i], null) && !this.actor.AIMemory.AwareOfPre.Contains(awareOf[i]))
                {
                    awareOf.RemoveAt(i);
                }
            }
            this.actor.AIMemory.AwareOfPre.Clear();
            this.actor.AIMemory.AwareOfPost.Clear();
            this.prevLastSeenPlayerSequence = this.actor.AIMemory.LastSeenPlayerSequence;
            List<Actor> actors = Get.World.Actors;
            for (int j = 0; j < actors.Count; j++)
            {
                if (actors[j] != this.actor && this.actor.Sees(actors[j], null))
                {
                    this.actor.AIMemory.AwareOfPost.Add(actors[j]);
                    if (actors[j].IsMainActor)
                    {
                        this.actor.AIMemory.LastSeenPlayerSequence = new int?(this.actor.Sequence);
                    }
                }
            }
            this.prevLastKnownAttackTargetPos = this.actor.AIMemory.LastKnownAttackTargetPos;
            if (this.actor.AIMemory.AttackTarget != null && this.actor.Sees(this.actor.AIMemory.AttackTarget, null))
            {
                this.actor.AIMemory.LastKnownAttackTargetPos = new Vector3Int?(this.actor.AIMemory.AttackTarget.Position);
            }
        }

        protected override void UndoImpl()
        {
            this.actor.AIMemory.AwareOf.Clear();
            this.actor.AIMemory.AwareOf.AddRange(this.prevAwareOf);
            this.actor.AIMemory.AwareOfPre.Clear();
            this.actor.AIMemory.AwareOfPre.AddRange(this.prevAwareOfPre);
            this.actor.AIMemory.AwareOfPost.Clear();
            this.actor.AIMemory.AwareOfPost.AddRange(this.prevAwareOfPost);
            this.actor.AIMemory.LastKnownAttackTargetPos = this.prevLastKnownAttackTargetPos;
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
        private Vector3Int? prevLastKnownAttackTargetPos;

        [Saved]
        private int? prevLastSeenPlayerSequence;
    }
}