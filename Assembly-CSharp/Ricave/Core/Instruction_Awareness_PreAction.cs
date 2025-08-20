using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class Instruction_Awareness_PreAction : Instruction
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        protected Instruction_Awareness_PreAction()
        {
        }

        public Instruction_Awareness_PreAction(Actor actor)
        {
            this.actor = actor;
        }

        protected override void DoImpl()
        {
            this.prevAwareOf.Clear();
            this.prevAwareOf.AddRange(this.actor.AIMemory.AwareOf);
            this.prevAwareOfPre.Clear();
            this.prevAwareOfPre.AddRange(this.actor.AIMemory.AwareOfPre);
            this.actor.AIMemory.AwareOfPre.Clear();
            this.prevLastSeenPlayerSequence = this.actor.AIMemory.LastSeenPlayerSequence;
            List<Actor> actors = Get.World.Actors;
            for (int i = 0; i < actors.Count; i++)
            {
                if (actors[i] != this.actor && this.actor.Sees(actors[i], null))
                {
                    this.actor.AIMemory.AwareOfPre.Add(actors[i]);
                    if (!this.actor.AIMemory.AwareOf.Contains(actors[i]))
                    {
                        this.actor.AIMemory.AwareOf.Add(actors[i]);
                    }
                    if (actors[i].IsMainActor)
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
            this.actor.AIMemory.LastKnownAttackTargetPos = this.prevLastKnownAttackTargetPos;
            this.actor.AIMemory.LastSeenPlayerSequence = this.prevLastSeenPlayerSequence;
        }

        [Saved]
        private Actor actor;

        [Saved(Default.New, true)]
        private List<Actor> prevAwareOf = new List<Actor>();

        [Saved(Default.New, true)]
        private List<Actor> prevAwareOfPre = new List<Actor>();

        [Saved]
        private Vector3Int? prevLastKnownAttackTargetPos;

        [Saved]
        private int? prevLastSeenPlayerSequence;
    }
}