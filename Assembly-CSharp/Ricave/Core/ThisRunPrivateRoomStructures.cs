using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class ThisRunPrivateRoomStructures
    {
        public Dictionary<EntitySpec, int> CollectedStructures
        {
            get
            {
                return this.collectedStructures;
            }
        }

        public void ChangeCount(EntitySpec structureSpec, int offset)
        {
            Instruction.ThrowIfNotExecuting();
            if (structureSpec == null)
            {
                Log.Error("Tried to set null collected private room structure.", false);
                return;
            }
            this.collectedStructures.SetOrIncrement(structureSpec, offset);
        }

        public int GetCount(EntitySpec structureSpec)
        {
            if (structureSpec == null)
            {
                return 0;
            }
            return this.collectedStructures.GetOrDefault(structureSpec, 0);
        }

        [Saved(Default.New, false)]
        private Dictionary<EntitySpec, int> collectedStructures = new Dictionary<EntitySpec, int>();
    }
}