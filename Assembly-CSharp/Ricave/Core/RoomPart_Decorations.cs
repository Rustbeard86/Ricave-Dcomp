using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_Decorations : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            if (room.ContainsAnyEntityOfSpec(Get.Entity_CeilingBars) || room.ContainsAnyEntityOfSpec(Get.Entity_CeilingBarsReinforced) || !RoomPart_Decorations.DoFan(room, memory))
            {
                RoomPart_Decorations.DoWallsWithPipe(room, memory);
            }
            if (!memory.generatedChains)
            {
                RoomPart_Decorations.DoChains(room, memory);
            }
            RoomPart_Decorations.DoSoffit(room, memory);
        }

        private static bool DoFan(Room room, WorldGenMemory memory)
        {
            World world = Get.World;
            if (world.AnyEntityOfSpec(Get.Entity_Fan))
            {
                return false;
            }
            if (room.RoomAbove != null)
            {
                return false;
            }
            Func<Vector3Int, bool> <> 9__1;
            Vector3Int vector3Int;
            if (room.Shape.InnerCuboidXZ(1).TopSurfaceCuboid.Where<Vector3Int>(delegate (Vector3Int x)
            {
                if (world.AnyEntityOfSpecAt(x, Get.Entity_Floor) && !world.AnyEntityAt(x.Below()))
                {
                    IEnumerable<Vector3Int> enumerable = x.AdjacentCardinalCellsXZ();
                    Func<Vector3Int, bool> func;
                    if ((func = <> 9__1) == null)
                    {
                        func = (<> 9__1 = (Vector3Int y) => world.CellsInfo.AnyPermanentFilledImpassableAt(y));
                    }
                    return enumerable.All<Vector3Int>(func);
                }
                return false;
            }).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                world.GetFirstEntityOfSpecAt(vector3Int, Get.Entity_Floor).DeSpawn(false);
                Maker.Make(Get.Entity_Fan, null, false, false, true).Spawn(vector3Int);
                Vector3Int vector3Int2 = vector3Int.Above();
                if (vector3Int2.InBounds())
                {
                    foreach (Vector3Int vector3Int3 in vector3Int2.AdjacentCellsXZAndInside())
                    {
                        if (!Get.World.AnyEntityAt(vector3Int3))
                        {
                            Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int3);
                        }
                    }
                }
                return true;
            }
            return false;
        }

        private static bool DoWallsWithPipe(Room room, WorldGenMemory memory)
        {
            World world = Get.World;
            if (memory.typicalWallsWithPipe >= 2)
            {
                return false;
            }
            if (!room.Storey.IsMainStorey)
            {
                return false;
            }
            Vector3Int vector3Int;
            if (room.Shape.EdgeCellsXZ.Where<Vector3Int>(delegate (Vector3Int x)
            {
                if (world.AnyEntityOfSpecAt(x, Get.Entity_Wall) && x.y >= room.StartY + 2 && x.y != room.Shape.yMax && !room.Surface.IsCorner(new Vector2Int(x.x, x.z)) && world.CellsInfo.CanPassThrough(x - room.Shape.GetEdge(x)) && !world.CellsInfo.AnyLadderAt(x - room.Shape.GetEdge(x)))
                {
                    if (!world.GetEntitiesAt(x - room.Shape.GetEdge(x)).Any<Entity>((Entity y) => y is Structure && y.Spec.Structure.AttachesToBack))
                    {
                        return room.IsExclusive(x);
                    }
                }
                return false;
            }).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                world.GetFirstEntityOfSpecAt(vector3Int, Get.Entity_Wall).DeSpawn(false);
                Entity entity = Maker.Make(Get.Entity_WallWithPipe, null, false, false, true);
                entity.Spawn(vector3Int, -room.Shape.GetEdge(vector3Int));
                if (memory.typicalWallsWithPipe == 0)
                {
                    Maker.Make(Get.Entity_PipeSmoke, null, false, false, true).Spawn(vector3Int, entity.DirectionCardinal);
                }
                memory.typicalWallsWithPipe++;
                return true;
            }
            return false;
        }

        private static void DoChains(Room room, WorldGenMemory memory)
        {
            if (room.RoomAbove != null)
            {
                return;
            }
            if (room.Height <= 3)
            {
                return;
            }
            if (!Rand.Chance(0.3f))
            {
                return;
            }
            World world = Get.World;
            IEnumerable<Vector3Int> enumerable = room.FreeBelowCeiling.Where<Vector3Int>((Vector3Int x) => world.CellsInfo.AnyFilledImpassableAt(x.Above()) && !world.CellsInfo.AnyFilledImpassableAt(x.Below()));
            int num = Calc.RoundToIntHalfUp((float)enumerable.Count<Vector3Int>() * 0.35f);
            bool flag = false;
            foreach (Vector3Int vector3Int in enumerable.InRandomOrder<Vector3Int>().Take<Vector3Int>(num))
            {
                Maker.Make(Get.Entity_Chain, null, false, false, true).Spawn(vector3Int);
                flag = true;
            }
            if (flag)
            {
                memory.generatedChains = true;
            }
        }

        private static void DoSoffit(Room room, WorldGenMemory memory)
        {
            if (room.RoomAbove != null)
            {
                return;
            }
            if (room.Height <= 3)
            {
                return;
            }
            if (room.Shape.width <= 4 || room.Shape.depth <= 4)
            {
                return;
            }
            foreach (Vector3Int vector3Int in Vector3IntUtility.DirectionsXZCardinal.InRandomOrder<Vector3Int>())
            {
                if (Rand.Chance(0.6f))
                {
                    CellCuboid edgeCells = room.Shape.InnerCuboid(1).TopSurfaceCuboid.GetEdgeCells(vector3Int);
                    if (edgeCells.All<Vector3Int>((Vector3Int x) => (!Get.World.AnyEntityAt(x) || Get.World.AnyEntityOfSpecAt(x, Get.Entity_Soffit)) && Get.World.CellsInfo.AnySemiPermanentFilledImpassableAt(x.Above())))
                    {
                        foreach (Vector3Int vector3Int2 in edgeCells)
                        {
                            if (!Get.World.AnyEntityOfSpecAt(vector3Int2, Get.Entity_Soffit))
                            {
                                Maker.Make(Get.Entity_Soffit, null, false, false, true).Spawn(vector3Int2);
                            }
                        }
                    }
                }
            }
        }
    }
}