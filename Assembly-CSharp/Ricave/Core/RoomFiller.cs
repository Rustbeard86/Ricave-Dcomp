using System;

namespace Ricave.Core
{
    public class RoomFiller
    {
        public RoomSpec Spec
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

        public virtual float GetSelectionWeight(Room room, WorldGenMemory memory)
        {
            return this.spec.SelectionWeight;
        }

        public virtual void FillRoom(Room room, WorldGenMemory memory)
        {
            foreach (RoomPartSpec roomPartSpec in this.spec.PartsInOrder)
            {
                if (memory.debugStopFillingRooms)
                {
                    break;
                }
                try
                {
                    roomPartSpec.RoomPart.Generate(room, memory);
                }
                catch (Exception ex)
                {
                    Log.Error("Error while generating room part.", ex);
                }
            }
        }

        public virtual string GetRoomName(Room room)
        {
            if (room.Role == Room.LayoutRole.End)
            {
                return "Room_End".Translate();
            }
            if (room.Role == Room.LayoutRole.LeverRoom)
            {
                return "Room_Lever".Translate();
            }
            if (room.Role == Room.LayoutRole.Start)
            {
                return "Room_Start".Translate();
            }
            if (!room.AssignedRoomSpec.LabelCap.NullOrEmpty())
            {
                return room.AssignedRoomSpec.LabelCap;
            }
            if (room.IsBossRoom)
            {
                return "Room_BossRoom".Translate();
            }
            if (room.Role == Room.LayoutRole.StoreysTransition)
            {
                return "Room_StoreysTransition".Translate();
            }
            return null;
        }

        public void OnRoomSpecLoaded(RoomSpec spec)
        {
            this.spec = spec;
        }

        private RoomSpec spec;
    }
}