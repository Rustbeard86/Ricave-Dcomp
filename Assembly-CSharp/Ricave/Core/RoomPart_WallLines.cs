using System;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_WallLines : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            World world = Get.World;
            if (room.ContainsAnyEntityOfSpec(Get.Entity_Shop))
            {
                return;
            }
            CellCuboid usable = room.Shape.InnerCuboid(1).BottomSurfaceCuboid;
            if (usable.width <= 2 || usable.depth <= 2)
            {
                return;
            }
            if (usable.width <= 5 && usable.depth <= 5)
            {
                if (!Rand.Chance(0.3f))
                {
                    return;
                }
            }
            else if (!Rand.Chance(0.8f))
            {
                return;
            }
            Vector3Int vector3Int;
            if (usable.EdgeCellsXZNoCorners.Where<Vector3Int>((Vector3Int x) => this.CanSpawnWallAt(x, room) && this.CanSpawnWallAt(x - usable.GetEdge(x), room)).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                EntitySpec entitySpec = ((Rand.Chance(0.2f) || !world.CellsInfo.AnyPermanentFilledImpassableAt(vector3Int.Below())) ? Get.Entity_WoodenWall : Get.Entity_Wall);
                Vector3Int vector3Int2 = -usable.GetEdge(vector3Int);
                int num = ((vector3Int.x == usable.xMin || vector3Int.x == usable.xMax) ? (usable.width - 1) : (usable.depth - 1));
                int num2 = Rand.RangeInclusive(2, num);
                for (int i = 0; i < num2; i++)
                {
                    Vector3Int vector3Int3 = vector3Int + vector3Int2 * i;
                    if (!this.CanSpawnWallAt(vector3Int3, room))
                    {
                        break;
                    }
                    Maker.Make<Structure>((entitySpec == Get.Entity_Wall) ? Rand.Element<EntitySpec>(entitySpec, Get.Entity_BrickWall) : entitySpec, null, false, false, true).Spawn(vector3Int3);
                    if (room.Height >= 4 && Rand.Chance(0.5f) && !world.AnyEntityAt(vector3Int3.Above()))
                    {
                        Maker.Make(Get.Entity_Grass, null, false, false, true).Spawn(vector3Int3.Above());
                    }
                }
            }
        }

        private bool CanSpawnWallAt(Vector3Int c, Room room)
        {
            World world = Get.World;
            return !world.AnyEntityAt(c) && world.CellsInfo.IsFloorUnderNoActors(c) && world.CellsInfo.IsFilled(c.Below()) && !world.TiledDecals.AnyForcedAt(c) && !room.WouldPotentiallyBlockPath(c, true) && !world.AnyAdjacentXZEntityOfSpecAt(c, Get.Entity_Pillar);
        }
    }
}