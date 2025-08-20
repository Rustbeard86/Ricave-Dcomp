using System;
using UnityEngine;

namespace Ricave.Core
{
    public class TemporarilyOpenedDoor : Structure
    {
        public Actor OpenedBy
        {
            get
            {
                return this.openedBy;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.openedBy = value;
            }
        }

        protected TemporarilyOpenedDoor()
        {
        }

        public TemporarilyOpenedDoor(EntitySpec spec)
            : base(spec)
        {
        }

        public TemporarilyOpenedDoor(string specID, int instanceID, int stableID, Vector3Int pos, Quaternion rot, Vector3 scale)
            : base(specID, instanceID, stableID, pos, rot, scale)
        {
        }

        [Saved]
        private Actor openedBy;
    }
}