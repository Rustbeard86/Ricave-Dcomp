using System;
using UnityEngine;

namespace Ricave.Core
{
    public class ChainPost : Structure
    {
        public Actor AttachedActorDirect
        {
            get
            {
                return this.attachedActor;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.attachedActor = value;
            }
        }

        public Actor AttachedActor
        {
            get
            {
                if (this.attachedActor == null || !this.attachedActor.Spawned || this.attachedActor.AttachedToChainPostDirect != this || !base.Spawned)
                {
                    return null;
                }
                return this.attachedActor;
            }
        }

        protected ChainPost()
        {
        }

        public ChainPost(EntitySpec spec)
            : base(spec)
        {
        }

        public ChainPost(string specID, int instanceID, int stableID, Vector3Int pos, Quaternion rot, Vector3 scale)
            : base(specID, instanceID, stableID, pos, rot, scale)
        {
        }

        [Saved]
        private Actor attachedActor;
    }
}