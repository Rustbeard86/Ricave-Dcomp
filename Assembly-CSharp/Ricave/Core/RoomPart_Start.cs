using System;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_Start : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            if (room.Role != Room.LayoutRole.Start)
            {
                return;
            }
            Vector3Int center = room.Shape.InnerCuboid(1).BottomSurfaceCuboid.Center;
            memory.playerStartPos = new Vector3Int(room.Shape.Center.x, room.StartY + 1, room.Shape.z + 1);
            Maker.Make(Get.Entity_Placeholder, null, false, false, true).Spawn(memory.playerStartPos);
            Vector3Int vector3Int = memory.playerStartPos + Vector3Int.forward;
            if (vector3Int != center)
            {
                Maker.Make(Get.Entity_Placeholder, null, false, false, true).Spawn(vector3Int);
            }
            Maker.Make(Get.Entity_Sign, null, false, false, true).Spawn(center);
            this.DoHatch(room, memory);
        }

        private void DoHatch(Room room, WorldGenMemory memory)
        {
            if (!Get.World.AnyEntityAt(memory.playerStartPos) && Get.CellsInfo.IsFloorUnderNoActors(memory.playerStartPos))
            {
                if (memory.playerStartPos.AdjacentCardinalCellsXZ().Any<Vector3Int>(delegate (Vector3Int x)
                {
                    if (x.InBounds() && Get.CellsInfo.AnyPermanentFilledImpassableAt(x))
                    {
                        return !Get.World.GetEntitiesAt(x).Any<Entity>(delegate (Entity y)
                        {
                            Structure structure = y as Structure;
                            return structure != null && structure.UseEffects != null && structure.UseEffects.Any;
                        });
                    }
                    return false;
                }))
                {
                    Maker.Make(Get.Entity_Hatch, null, false, false, true).Spawn(memory.playerStartPos);
                    return;
                }
            }
            Vector3Int vector3Int;
            if (room.Shape.BottomSurfaceCuboid.WithAddedY(1).EdgeCellsXZNoCorners.Where<Vector3Int>(delegate (Vector3Int x)
            {
                if (!Get.CellsInfo.AnyPermanentFilledImpassableAt(x))
                {
                    return false;
                }
                if (Get.World.GetEntitiesAt(x).Any<Entity>(delegate (Entity y)
                {
                    Structure structure2 = y as Structure;
                    return structure2 != null && structure2.UseEffects != null && structure2.UseEffects.Any;
                }))
                {
                    return false;
                }
                Vector3Int vector3Int3 = x - room.Shape.GetEdge(x);
                return !Get.World.AnyEntityAt(vector3Int3) && Get.CellsInfo.IsFloorUnderNoActors(vector3Int3);
            }).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                Vector3Int vector3Int2 = vector3Int - room.Shape.GetEdge(vector3Int);
                Maker.Make(Get.Entity_Hatch, null, false, false, true).Spawn(vector3Int2);
            }
        }
    }
}