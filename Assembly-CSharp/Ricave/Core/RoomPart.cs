using System;

namespace Ricave.Core
{
    public abstract class RoomPart
    {
        public RoomPartSpec Spec
        {
            get
            {
                return this.spec;
            }
        }

        public abstract void Generate(Room room, WorldGenMemory memory);

        public void OnRoomPartSpecLoaded(RoomPartSpec spec)
        {
            this.spec = spec;
        }

        private RoomPartSpec spec;
    }
}