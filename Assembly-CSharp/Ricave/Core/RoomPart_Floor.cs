using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_Floor : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            World world = Get.World;
            bool flag = false;
            if (room.Role != Room.LayoutRole.LeverRoom && room.Role != Room.LayoutRole.Secret && room.Role != Room.LayoutRole.OptionalChallenge && room.RoomBelow != null && room.RoomBelow.Role != Room.LayoutRole.LockedBehindGate && room.RoomBelow.Role != Room.LayoutRole.LockedBehindSilverDoor && room.RoomBelow.Role != Room.LayoutRole.OptionalChallenge && room.RoomBelow.Role != Room.LayoutRole.Secret)
            {
                flag = true;
            }
            foreach (Vector2Int vector2Int in room.Surface)
            {
                Vector3Int vector3Int = new Vector3Int(vector2Int.x, room.StartY + this.yOffset, vector2Int.y);
                if (!world.AnyEntityAt(vector3Int))
                {
                    if (flag && !room.Surface.IsOnEdge(vector2Int))
                    {
                        Maker.Make(Get.Entity_WoodenFloor, null, false, false, true).Spawn(vector3Int);
                    }
                    else
                    {
                        Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int);
                    }
                }
            }
        }

        [Saved]
        private int yOffset;
    }
}