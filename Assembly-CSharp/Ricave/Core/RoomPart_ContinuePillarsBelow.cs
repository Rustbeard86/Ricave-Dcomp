using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_ContinuePillarsBelow : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            World world = Get.World;
            Room roomBelow = room.RoomBelow;
            if (roomBelow == null)
            {
                return;
            }
            foreach (Vector3Int vector3Int in roomBelow.Shape.InnerCuboid(1).TopSurfaceCuboid)
            {
                if (world.AnyEntityOfSpecAt(vector3Int, Get.Entity_Pillar))
                {
                    for (int i = 0; i < room.Height - 1; i++)
                    {
                        Vector3Int vector3Int2 = vector3Int.WithAddedY(i + 1);
                        if (world.AnyEntityAt(vector3Int2))
                        {
                            break;
                        }
                        Maker.Make(Get.Entity_Pillar, null, false, false, true).Spawn(vector3Int2);
                    }
                }
            }
        }
    }
}