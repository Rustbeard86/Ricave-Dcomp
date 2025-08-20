using System;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_SupportForCeilingAttachedStructuresBelow : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            Room roomBelow = room.RoomBelow;
            if (roomBelow == null)
            {
                return;
            }
            World world = Get.World;
            foreach (Vector3Int vector3Int in roomBelow.Shape.InnerCuboid(1).TopSurfaceCuboid)
            {
                if (world.GetEntitiesAt(vector3Int).Any<Entity>((Entity x) => x is Structure && x.Spec.Structure.AttachesToCeiling))
                {
                    for (int i = 0; i < room.Height - 1; i++)
                    {
                        Vector3Int vector3Int2 = vector3Int.WithAddedY(i + 1);
                        if (world.AnyEntityAt(vector3Int2))
                        {
                            break;
                        }
                        Maker.Make(Get.Entity_WoodenWall, null, false, false, true).Spawn(vector3Int2);
                    }
                }
            }
        }
    }
}