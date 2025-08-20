using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class AIMemory
    {
        public List<Actor> AwareOf
        {
            get
            {
                return this.awareOf;
            }
        }

        public List<Actor> AwareOfPre
        {
            get
            {
                return this.awareOfPre;
            }
        }

        public List<Actor> AwareOfPost
        {
            get
            {
                return this.awareOfPost;
            }
        }

        public Vector3Int? WanderDest
        {
            get
            {
                return this.wanderDest;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.wanderDest = value;
            }
        }

        public Actor AttackTarget
        {
            get
            {
                return this.attackTarget;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.attackTarget = value;
            }
        }

        public Vector3Int? LastKnownAttackTargetPos
        {
            get
            {
                return this.lastKnownAttackTargetPos;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.lastKnownAttackTargetPos = value;
            }
        }

        public int? LastUseSequence
        {
            get
            {
                return this.lastUseSequence;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.lastUseSequence = value;
            }
        }

        public int? LastUseOnHostileSequence
        {
            get
            {
                return this.lastUseOnHostileSequence;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.lastUseOnHostileSequence = value;
            }
        }

        public int? LastSeenPlayerSequence
        {
            get
            {
                return this.lastSeenPlayerSequence;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.lastSeenPlayerSequence = value;
            }
        }

        public int? LastHarmedSequence
        {
            get
            {
                return this.lastHarmedSequence;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.lastHarmedSequence = value;
            }
        }

        public Vector3Int? LastDestroyedBlockerAt
        {
            get
            {
                return this.lastDestroyedBlockerAt;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.lastDestroyedBlockerAt = value;
            }
        }

        public Actor LastViolentlyAttackedBy
        {
            get
            {
                return this.lastViolentlyAttackedBy;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.lastViolentlyAttackedBy = value;
            }
        }

        public override string ToString()
        {
            string[] array = new string[24];
            array[0] = "awareOf: ";
            array[1] = this.awareOf.ElementsToString<Actor>();
            array[2] = "\nawareOfPre: ";
            array[3] = this.awareOfPre.ElementsToString<Actor>();
            array[4] = "\nawareOfPost: ";
            array[5] = this.awareOfPost.ElementsToString<Actor>();
            array[6] = "\nwanderDest: ";
            int num = 7;
            Vector3Int? vector3Int = this.wanderDest;
            array[num] = vector3Int.ToString();
            array[8] = "\nattackTarget: ";
            int num2 = 9;
            Actor actor = this.attackTarget;
            array[num2] = ((actor != null) ? actor.ToString() : null);
            array[10] = "\nlastKnownAttackTargetPos: ";
            int num3 = 11;
            vector3Int = this.lastKnownAttackTargetPos;
            array[num3] = vector3Int.ToString();
            array[12] = "\nlastUseSequence: ";
            int num4 = 13;
            int? num5 = this.lastUseSequence;
            array[num4] = num5.ToString();
            array[14] = "\nlastUseOnHostileSequence: ";
            int num6 = 15;
            num5 = this.lastUseOnHostileSequence;
            array[num6] = num5.ToString();
            array[16] = "\nlastSeenPlayerSequence: ";
            int num7 = 17;
            num5 = this.lastSeenPlayerSequence;
            array[num7] = num5.ToString();
            array[18] = "\nlastHarmedSequence: ";
            int num8 = 19;
            num5 = this.lastHarmedSequence;
            array[num8] = num5.ToString();
            array[20] = "\nlastDestroyedBlockerAt: ";
            int num9 = 21;
            vector3Int = this.lastDestroyedBlockerAt;
            array[num9] = vector3Int.ToString();
            array[22] = "\nlastViolentlyAttackedBy: ";
            int num10 = 23;
            Actor actor2 = this.lastViolentlyAttackedBy;
            array[num10] = ((actor2 != null) ? actor2.ToString() : null);
            return string.Concat(array);
        }

        [Saved(Default.New, true)]
        private List<Actor> awareOf = new List<Actor>();

        [Saved(Default.New, true)]
        private List<Actor> awareOfPre = new List<Actor>();

        [Saved(Default.New, true)]
        private List<Actor> awareOfPost = new List<Actor>();

        [Saved]
        private Vector3Int? wanderDest;

        [Saved]
        private Actor attackTarget;

        [Saved]
        private Vector3Int? lastKnownAttackTargetPos;

        [Saved]
        private int? lastUseSequence;

        [Saved]
        private int? lastUseOnHostileSequence;

        [Saved]
        private int? lastSeenPlayerSequence;

        [Saved]
        private int? lastHarmedSequence;

        [Saved]
        private Vector3Int? lastDestroyedBlockerAt;

        [Saved]
        private Actor lastViolentlyAttackedBy;
    }
}