using System;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_TimeMachine : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            if (memory.AllRooms.Any<Room>((Room x) => x.Role == Room.LayoutRole.LockedBehindSilverDoor))
            {
                return false;
            }
            Vector3Int vector3Int;
            if (!room.FreeOnFloorNonBlocking.TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            Maker.Make(Get.Entity_TimeMachine, null, false, false, true).Spawn(vector3Int);
            return true;
        }
    }
}