using System;
using System.Linq;

namespace Ricave.Core
{
    public class RoomFiller_Ledge : RoomFiller
    {
        public override float GetSelectionWeight(Room room, WorldGenMemory memory)
        {
            if (room.Shape.width <= 4 || room.Shape.depth <= 4)
            {
                return 0f;
            }
            if (room.RoomBelow == null)
            {
                return 0f;
            }
            if (room.RoomBelow.Role == Room.LayoutRole.LockedBehindGate || room.RoomBelow.Role == Room.LayoutRole.LockedBehindSilverDoor || room.RoomBelow.Role == Room.LayoutRole.OptionalChallenge || room.RoomBelow.Role == Room.LayoutRole.Secret)
            {
                return 0f;
            }
            if (room.Storey.Rooms.Count<Room>((Room x) => x.AssignedRoomSpec == Get.Room_Ledge) >= room.Storey.Rooms.Count<Room>() - 1)
            {
                return 0f;
            }
            return base.GetSelectionWeight(room, memory);
        }
    }
}