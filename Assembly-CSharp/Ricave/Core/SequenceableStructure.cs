using System;
using UnityEngine;

namespace Ricave.Core
{
    public class SequenceableStructure : Structure, ISequenceable
    {
        public int Sequence
        {
            get
            {
                return this.sequence;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.sequence = value;
                Get.TurnManager.OnSequenceChanged();
            }
        }

        protected SequenceableStructure()
        {
        }

        public SequenceableStructure(EntitySpec spec)
            : base(spec)
        {
        }

        public SequenceableStructure(string specID, int instanceID, int stableID, Vector3Int pos, Quaternion rot, Vector3 scale)
            : base(specID, instanceID, stableID, pos, rot, scale)
        {
        }

        void ISequenceable.TakeTurn()
        {
            if (!base.Spawned)
            {
                Log.Error("Called TakeTurn() on unspawned structure. Only spawned entities can ever take turns.", false);
                return;
            }
            new Action_ResolveStructure(Get.Action_ResolveStructure, this).Do(false);
        }

        [Saved]
        private int sequence;
    }
}