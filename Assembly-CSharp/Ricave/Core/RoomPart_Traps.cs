using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_Traps : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            if (!RoomPart_Traps.DoCannons(room, memory))
            {
                RoomPart_Traps.DoCeilingSpikes(room, memory);
            }
            if (!RoomPart_Traps.DoExplodingBarrels(room, memory))
            {
                RoomPart_Traps.DoGooPuddle(room, memory);
            }
            if (Rand.Chance(0.8f))
            {
                RoomPart_Traps.DoCeilingSupports(room, memory);
            }
            if (Rand.Chance(0.75f))
            {
                this.DoProximityBomb(room, memory);
            }
            if (Rand.Chance(0.1f) && Get.RunSpec != Get.Run_Main1 && Get.RunSpec != Get.Run_Main2)
            {
                RoomPart_Traps.DoBeerBarrel(room, memory);
            }
        }

        public static bool DoCannons(Room room, WorldGenMemory memory)
        {
            World world = Get.World;
            if (world.GetEntitiesOfSpec(Get.Entity_Cannon).Count >= 2)
            {
                return false;
            }
            if (room.IsLeaf)
            {
                return false;
            }
            int minZ;
            int maxZ;
            int minX;
            int maxX;
            if (!room.TryGetOnFloorLevelEntrancesToAvoidBlockingMinMax(out minX, out maxX, out minZ, out maxZ, false))
            {
                return false;
            }
            Func<Vector3Int, bool> <> 9__3;
            Func<Vector3Int, bool> <> 9__1;
            Func<Vector3Int, bool> <> 9__2;
            Predicate<Vector3Int> <> 9__4;
            for (int i = 0; i < 2; i++)
            {
                using (IEnumerator<Vector3Int> enumerator = Vector3IntUtility.DirectionsXZCardinal.InRandomOrder<Vector3Int>().GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Vector3Int edge = enumerator.Current;
                        if ((edge.x == 0 || room.Shape.width > 3) && (edge.z == 0 || room.Shape.depth > 3))
                        {
                            IEnumerable<Vector3Int> enumerable = room.Shape.InnerCuboid(1).BottomSurfaceCuboid.GetEdgeCells(edge).Where<Vector3Int>(delegate (Vector3Int x)
                            {
                                if (!world.AnyEntityAt(x) && world.CellsInfo.IsFilled(x + edge) && !room.WouldPotentiallyBlockPath(x, true))
                                {
                                    IEnumerable<Vector3Int> enumerable4 = x.AdjacentCardinalCellsXZ();
                                    Func<Vector3Int, bool> func3;
                                    if ((func3 = <> 9__3) == null)
                                    {
                                        func3 = (<> 9__3 = (Vector3Int y) => world.CellsInfo.IsFilled(y));
                                    }
                                    return enumerable4.Count<Vector3Int>(func3) < 3;
                                }
                                return false;
                            });
                            if (edge.x != 0)
                            {
                                IEnumerable<Vector3Int> enumerable2 = enumerable;
                                Func<Vector3Int, bool> func;
                                if ((func = <> 9__1) == null)
                                {
                                    func = (<> 9__1 = (Vector3Int x) => x.z >= minZ && x.z <= maxZ);
                                }
                                enumerable = enumerable2.Where<Vector3Int>(func);
                            }
                            else
                            {
                                IEnumerable<Vector3Int> enumerable3 = enumerable;
                                Func<Vector3Int, bool> func2;
                                if ((func2 = <> 9__2) == null)
                                {
                                    func2 = (<> 9__2 = (Vector3Int x) => x.x >= minX && x.x <= maxX);
                                }
                                enumerable = enumerable3.Where<Vector3Int>(func2);
                            }
                            foreach (Vector3Int vector3Int in enumerable.InRandomOrder<Vector3Int>())
                            {
                                Vector3Int vector3Int2 = vector3Int;
                                Vector3Int vector3Int3 = -edge;
                                Predicate<Vector3Int> predicate;
                                if ((predicate = <> 9__4) == null)
                                {
                                    predicate = (<> 9__4 = (Vector3Int x) => world.CellsInfo.CanProjectilesPassThrough(x));
                                }
                                CellCuboid cellCuboid = vector3Int2.SelectLineInBounds(vector3Int3, predicate);
                                if (i != 0)
                                {
                                    if (cellCuboid.Count<Vector3Int>() < 4)
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    int num;
                                    if (edge.x != 0)
                                    {
                                        num = room.Shape.width - 2;
                                    }
                                    else
                                    {
                                        num = room.Shape.depth - 2;
                                    }
                                    if (cellCuboid.Count<Vector3Int>() < num)
                                    {
                                        continue;
                                    }
                                }
                                Maker.Make(Get.Entity_Cannon, null, false, false, true).Spawn(vector3Int, -edge);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public static bool DoCeilingSpikes(Room room, WorldGenMemory memory)
        {
            World world = Get.World;
            if (world.GetEntitiesOfSpec(Get.Entity_CeilingSpikes).Count >= 3)
            {
                return false;
            }
            if (room.Height > 4)
            {
                return false;
            }
            if (room.IsLeaf)
            {
                return false;
            }
            int minX;
            int maxX;
            int minZ;
            int maxZ;
            if (!room.TryGetOnFloorLevelEntrancesToAvoidBlockingMinMax(out minX, out maxX, out minZ, out maxZ, false))
            {
                return false;
            }
            List<Vector3Int> doorsOrStairs = room.OnFloorLevelEntrancesNonSecretOrOptionalChallengeRoom.ToList<Vector3Int>();
            Vector3Int vector3Int;
            if (room.Shape.InnerCuboid(1).BottomSurfaceCuboid.Where<Vector3Int>((Vector3Int x) => !world.AnyEntityAt(x) && x.x >= minX && x.x <= maxX && x.z >= minZ && x.z <= maxZ && world.CellsInfo.IsFloorUnderNoActors(x) && room.AllCellsToOneBelowCeilingFree(x) && doorsOrStairs.Any<Vector3Int>((Vector3Int door) => door.x == x.x || door.z == x.z)).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                Maker.Make(Get.Entity_CeilingSpikes, null, false, false, true).Spawn(new Vector3Int(vector3Int.x, room.Shape.yMax - 1, vector3Int.z), Vector3IntUtility.Down);
                return true;
            }
            return false;
        }

        public static bool DoExplodingBarrels(Room room, WorldGenMemory memory)
        {
            World world = Get.World;
            if (world.AnyEntityOfSpec(Get.Entity_Barrel_Damage) || world.AnyEntityOfSpec(Get.Entity_Barrel_Fire) || world.AnyEntityOfSpec(Get.Entity_Barrel_Poison))
            {
                return false;
            }
            if (room.IsLeaf)
            {
                return false;
            }
            int minX;
            int maxX;
            int minZ;
            int maxZ;
            if (!room.TryGetOnFloorLevelEntrancesToAvoidBlockingMinMax(out minX, out maxX, out minZ, out maxZ, false))
            {
                return false;
            }
            int num = minX;
            minX = num - 1;
            num = maxX;
            maxX = num + 1;
            num = minZ;
            minZ = num - 1;
            num = maxZ;
            maxZ = num + 1;
            List<Vector3Int> doorsOrStairs = room.OnFloorLevelEntrancesNonSecretOrOptionalChallengeRoom.ToList<Vector3Int>();
            Vector3Int vector3Int;
            if (room.FreeOnFloorNonBlocking.Where<Vector3Int>((Vector3Int x) => x.x >= minX && x.x <= maxX && x.z >= minZ && x.z <= maxZ && doorsOrStairs.Any<Vector3Int>((Vector3Int door) => door.x == x.x || door.x - 1 == x.x || door.x + 1 == x.x || door.z == x.z || door.z - 1 == x.z || door.z + 1 == x.z)).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                Maker.Make(Rand.Element<EntitySpec>(Get.Entity_Barrel_Damage, Get.Entity_Barrel_Fire, Get.Entity_Barrel_Poison), null, false, false, true).Spawn(vector3Int);
                return true;
            }
            return false;
        }

        public static bool DoGooPuddle(Room room, WorldGenMemory memory)
        {
            if (Get.World.AnyEntityOfSpec(Get.Entity_GooPuddle))
            {
                return false;
            }
            if (room.IsLeaf)
            {
                return false;
            }
            int minX;
            int maxX;
            int minZ;
            int maxZ;
            if (!room.TryGetOnFloorLevelEntrancesToAvoidBlockingMinMax(out minX, out maxX, out minZ, out maxZ, false))
            {
                return false;
            }
            List<Vector3Int> doorsOrStairs = room.OnFloorLevelEntrancesNonSecretOrOptionalChallengeRoom.ToList<Vector3Int>();
            Vector3Int vector3Int;
            if (room.FreeOnFloor.Where<Vector3Int>((Vector3Int x) => x.x >= minX && x.x <= maxX && x.z >= minZ && x.z <= maxZ && doorsOrStairs.Any<Vector3Int>((Vector3Int door) => door.x == x.x || door.z == x.z)).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                Maker.Make(Get.Entity_GooPuddle, null, false, false, true).Spawn(vector3Int);
                return true;
            }
            return false;
        }

        public static bool DoCeilingSupports(Room room, WorldGenMemory memory)
        {
            if (room.Height <= 3)
            {
                return false;
            }
            if (room.Shape.width <= 3 || room.Shape.depth <= 3)
            {
                return false;
            }
            if (room.RoomAbove != null)
            {
                return false;
            }
            foreach (Vector3Int vector3Int in Vector3IntUtility.DirectionsXZCardinal.InRandomOrder<Vector3Int>())
            {
                if (RoomPart_Traps.DoCeilingSupports(room, vector3Int))
                {
                    memory.generatedCeilingSupports = true;
                    return true;
                }
            }
            return false;
        }

        private static bool DoCeilingSupports(Room room, Vector3Int dir)
        {
            CellCuboid edgeCells = room.Shape.InnerCuboid(1).TopSurfaceCuboid.GetEdgeCells(dir);
            foreach (Vector3Int vector3Int in edgeCells)
            {
                if (Get.World.AnyEntityAt(vector3Int) || room.IsEntranceCellToAvoidBlocking(vector3Int, true) || !ItemOrStructureFallUtility.WouldHaveSupport(Get.Entity_CeilingSupport, vector3Int, null))
                {
                    return false;
                }
            }
            foreach (Vector3Int vector3Int2 in edgeCells)
            {
                Maker.Make(Get.Entity_CeilingSupport, null, false, false, true).Spawn(vector3Int2);
            }
            return edgeCells.Any<Vector3Int>();
        }

        private bool DoProximityBomb(Room room, WorldGenMemory memory)
        {
            int num = Get.World.GetEntitiesOfSpec(Get.Entity_ProximityBomb).Count + Get.World.GetEntitiesOfSpec(Get.Entity_ProximitySmokeBomb).Count + Get.World.GetEntitiesOfSpec(Get.Entity_ProximityFireBomb).Count;
            if (Get.DungeonModifier_MoreTraps.IsActiveAndAppliesToCurrentRun())
            {
                if (num >= 2)
                {
                    return false;
                }
            }
            else if (num >= 1)
            {
                return false;
            }
            EntitySpec entitySpec = Rand.Element<EntitySpec>(Get.Entity_ProximityBomb, Get.Entity_ProximitySmokeBomb, Get.Entity_ProximityFireBomb);
            IEnumerable<Vector3Int> enumerable = room.FreeOnFloorNonBlocking;
            if (entitySpec != Get.Entity_ProximitySmokeBomb)
            {
                enumerable = enumerable.Where<Vector3Int>((Vector3Int x) => x.AdjacentCellsXZ().All<Vector3Int>((Vector3Int y) => !RoomPart_Traps.< DoProximityBomb > g__AnyImportantEntityAt | 7_0(y)));
            }
            Vector3Int vector3Int;
            if (enumerable.TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                Maker.Make(entitySpec, null, false, false, true).Spawn(vector3Int);
                return true;
            }
            return false;
        }

        private static void DoBeerBarrel(Room room, WorldGenMemory memory)
        {
            if (Get.World.AnyEntityOfSpec(Get.Entity_BeerBarrel))
            {
                return;
            }
            Vector3Int vector3Int;
            if (!room.FreeOnFloor.Where<Vector3Int>(delegate (Vector3Int x)
            {
                if (ItemOrStructureFallUtility.WouldHaveSupport(Get.Entity_BeerBarrel, x, null))
                {
                    if (x.AdjacentCardinalCellsXZ().Any<Vector3Int>((Vector3Int y) => Get.CellsInfo.AnyFilledImpassableAt(y)))
                    {
                        return Get.CellsInfo.IsFloorUnderNoActors(x);
                    }
                }
                return false;
            }).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return;
            }
            Maker.Make(Get.Entity_BeerBarrel, null, false, false, true).Spawn(vector3Int);
        }

        [CompilerGenerated]
        internal static bool <DoProximityBomb>g__AnyImportantEntityAt|7_0(Vector3Int at)
		{
			if (Get.CellsInfo.IsFilled(at))
			{
				return false;
			}
			foreach (Entity entity in Get.World.GetEntitiesAt(at))
			{
				if (entity.Spec != Get.Entity_Crates && !(entity is Item))
				{
					return true;
				}
			}
			return false;
		}
	}
}