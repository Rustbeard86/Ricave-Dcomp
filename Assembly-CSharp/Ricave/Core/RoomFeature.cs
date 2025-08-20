using System;

namespace Ricave.Core
{
    public abstract class RoomFeature
    {
        public RoomFeatureSpec Spec
        {
            get
            {
                return this.spec;
            }
        }

        public virtual float Priority
        {
            get
            {
                return this.spec.Priority;
            }
        }

        public virtual float SelectionWeight
        {
            get
            {
                return this.spec.SelectionWeight;
            }
        }

        public abstract bool TryGenerate(Room room, WorldGenMemory memory);

        public void OnRoomFeatureSpecLoaded(RoomFeatureSpec spec)
        {
            this.spec = spec;
        }

        private RoomFeatureSpec spec;
    }
}