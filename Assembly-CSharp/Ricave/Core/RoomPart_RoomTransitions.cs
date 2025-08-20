using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_RoomTransitions : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            foreach (Room room2 in room.AdjacentRooms)
            {
                this.DoBetween(room, room2, memory);
            }
        }

        private void DoBetween(Room room, Room adj, WorldGenMemory memory)
        {
            Room.TransitionType transitionType = room.GetTransitionType(adj);
            if (transitionType == Room.TransitionType.None)
            {
                return;
            }
            if (transitionType == Room.TransitionType.SameLevel)
            {
                if (!room.ShouldHaveMissingWallBetween(adj))
                {
                    if (RoomPart_RoomTransitions.GetExistingDoor(room, adj, memory) != null)
                    {
                        return;
                    }
                    Vector3Int vector3Int;
                    Vector3Int vector3Int2;
                    if (RoomPart_RoomTransitions.TryFindGoodDoorPosition(room, adj, memory, out vector3Int, out vector3Int2))
                    {
                        Maker.Make(RoomPart_RoomTransitions.GetDoorSpec(room, adj), null, false, false, true).Spawn(vector3Int, vector3Int2);
                        return;
                    }
                }
            }
            else if (transitionType == Room.TransitionType.Stairs)
            {
                if (RoomPart_RoomTransitions.GetExistingStairs(room, adj, memory) != null)
                {
                    return;
                }
                Vector3Int vector3Int3;
                Vector3Int vector3Int4;
                if (RoomPart_RoomTransitions.TryFindGoodStairsPosition(room, adj, memory, out vector3Int3, out vector3Int4))
                {
                    Maker.Make(Get.Entity_Stairs, null, false, false, true).Spawn(vector3Int3, vector3Int4);
                    EntitySpec doorSpec = RoomPart_RoomTransitions.GetDoorSpec(room, adj);
                    if (doorSpec == Get.Entity_Gate || doorSpec == Get.Entity_GateReinforced)
                    {
                        Maker.Make(doorSpec, null, false, false, true).Spawn(vector3Int3, vector3Int4.RightDir());
                        Maker.Make(doorSpec, null, false, false, true).Spawn(vector3Int3.Above(), vector3Int4.RightDir());
                        return;
                    }
                }
            }
            else if (transitionType == Room.TransitionType.Shaft)
            {
                Rand.PushState(Calc.CombineHashes<int, int, int, int>(memory.config.WorldSeed, Math.Min(room.Index, adj.Index), Math.Max(room.Index, adj.Index), 90016418));
                Vector2Int vector2Int;
                bool flag = room.GetIntersectionNoCorners(adj).TryGetRandomElement<Vector2Int>(out vector2Int);
                if (flag)
                {
                    vector2Int = room.GetIntersectionNoCorners(adj).Concat<Vector2Int>(room.GetIntersectionNoCorners(adj)).ElementAt<Vector2Int>(room.GetIntersectionNoCorners(adj).IndexOf(vector2Int) + Math.Min(room.Storey.Index, adj.Storey.Index) % room.GetIntersectionNoCorners(adj).Area);
                }
                Rand.PopState();
                if (flag)
                {
                    Room room2 = ((adj.StartY > room.StartY) ? room : adj);
                    Room room3 = ((adj.StartY > room.StartY) ? adj : room);
                    CellCuboid cellCuboid = new CellCuboid(vector2Int.x, Math.Min(room.StartY + 1, adj.StartY + 1), vector2Int.y, 1, Math.Abs(room.StartY - adj.StartY) + 1, 1);
                    if (Get.CellsInfo.AnyLadderAt(cellCuboid.Position))
                    {
                        return;
                    }
                    EntitySpec entity_HangingRope = Get.Entity_HangingRope;
                    EntitySpec doorSpec2 = RoomPart_RoomTransitions.GetDoorSpec(room, adj);
                    bool flag2 = doorSpec2 == Get.Entity_Gate || doorSpec2 == Get.Entity_GateReinforced;
                    foreach (Vector3Int vector3Int5 in cellCuboid)
                    {
                        foreach (Vector3Int vector3Int6 in vector3Int5.AdjacentCardinalCells())
                        {
                            if (!room.Shape.Contains(vector3Int6) && !adj.Shape.Contains(vector3Int6) && (room2.RoomAbove == null || !room2.RoomAbove.Shape.Contains(vector3Int6)) && (room3.RoomBelow == null || !room3.RoomBelow.Shape.Contains(vector3Int6)) && !Get.World.AnyEntityAt(vector3Int6))
                            {
                                Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int6);
                            }
                        }
                        if (!Get.World.AnyEntityAt(vector3Int5))
                        {
                            if (entity_HangingRope != Get.Entity_Ladder || vector3Int5.y != cellCuboid.yMax)
                            {
                                Maker.Make(entity_HangingRope, null, false, false, true).Spawn(vector3Int5);
                            }
                            if (flag2)
                            {
                                Maker.Make(doorSpec2, null, false, false, true).Spawn(vector3Int5, room.GetAdjacentRoomDir(adj).RightDir());
                            }
                        }
                    }
                    if (entity_HangingRope == Get.Entity_HangingRope)
                    {
                        for (int i = cellCuboid.yMax + 1; i <= room3.Shape.yMax; i++)
                        {
                            Vector3Int vector3Int7 = cellCuboid.Position.WithY(i);
                            if (!Get.World.AnyEntityAt(vector3Int7))
                            {
                                if (vector3Int7.y == room3.Shape.yMax)
                                {
                                    if (i != cellCuboid.yMax + 1)
                                    {
                                        Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int7);
                                    }
                                    else
                                    {
                                        Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int7);
                                    }
                                }
                                else
                                {
                                    Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int7);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static Vector3Int? GetExistingDoor(Room room, Room adj, WorldGenMemory memory)
        {
            if (room.StartY != adj.StartY)
            {
                return null;
            }
            World world = Get.World;
            foreach (Vector2Int vector2Int in adj.GetIntersectionNoCorners(room))
            {
                Vector3Int vector3Int = new Vector3Int(vector2Int.x, room.StartY + 1, vector2Int.y);
                if (world.CellsInfo.AnyDoorAt(vector3Int))
                {
                    return new Vector3Int?(vector3Int);
                }
            }
            return null;
        }

        public static Vector3Int? GetExistingStairs(Room room, Room adj, WorldGenMemory memory)
        {
            if (room.StartY == adj.StartY)
            {
                return null;
            }
            World world = Get.World;
            CellRect intersectionNoCorners = adj.GetIntersectionNoCorners(room);
            int num = Math.Max(adj.StartY, room.StartY);
            foreach (Vector2Int vector2Int in intersectionNoCorners)
            {
                Vector3Int vector3Int = new Vector3Int(vector2Int.x, num, vector2Int.y);
                if (world.CellsInfo.AnyStairsAt(vector3Int))
                {
                    return new Vector3Int?(vector3Int);
                }
            }
            return null;
        }

        private static bool TryFindGoodDoorPosition(Room room, Room adj, WorldGenMemory memory, out Vector3Int pos, out Vector3Int dir)
        {
            World world = Get.World;
            CellRect intersectionNoCorners = adj.GetIntersectionNoCorners(room);
            dir = room.GetAdjacentRoomDir(adj).RightDir();
            Func<Vector3Int, bool> <> 9__4;
            Func<Vector3Int, bool> <> 9__5;
            return intersectionNoCorners.Select<Vector2Int, Vector3Int>((Vector2Int x) => new Vector3Int(x.x, room.StartY + 1, x.y)).Where<Vector3Int>(delegate (Vector3Int x)
            {
                if (world.AnyEntityAt(x))
                {
                    return false;
                }
                IEnumerable<Vector3Int> enumerable = x.AdjacentCardinalCellsXZ();
                Func<Vector3Int, bool> func;
                if ((func = <> 9__4) == null)
                {
                    func = (<> 9__4 = (Vector3Int y) => room.Shape.Contains(y) && !room.Shape.IsOnEdgeXZ(y));
                }
                Vector3Int vector3Int = enumerable.First<Vector3Int>(func);
                if (!world.CellsInfo.CanPassThroughNoActors(vector3Int))
                {
                    return false;
                }
                IEnumerable<Vector3Int> enumerable2 = x.AdjacentCardinalCellsXZ();
                Func<Vector3Int, bool> func2;
                if ((func2 = <> 9__5) == null)
                {
                    func2 = (<> 9__5 = (Vector3Int y) => adj.Shape.Contains(y) && !adj.Shape.IsOnEdgeXZ(y));
                }
                Vector3Int vector3Int2 = enumerable2.First<Vector3Int>(func2);
                return world.CellsInfo.CanPassThroughNoActors(vector3Int2);
            }).TryGetRandomElement<Vector3Int>(out pos) || (from x in intersectionNoCorners
                                                            select new Vector3Int(x.x, room.StartY + 1, x.y) into x
                                                            where !world.AnyEntityAt(x)
                                                            select x).TryGetRandomElement<Vector3Int>(out pos);
        }

        private static bool TryFindGoodStairsPosition(Room room, Room adj, WorldGenMemory memory, out Vector3Int pos, out Vector3Int dir)
        {
            int stairsY = Math.Max(adj.StartY, room.StartY);
            World world = Get.World;
            CellRect intersectionNoCorners = adj.GetIntersectionNoCorners(room);
            dir = ((room.StartY < adj.StartY) ? room.GetAdjacentRoomDir(adj) : adj.GetAdjacentRoomDir(room));
            Func<Vector3Int, bool> <> 9__4;
            Func<Vector3Int, bool> <> 9__5;
            return intersectionNoCorners.Select<Vector2Int, Vector3Int>((Vector2Int x) => new Vector3Int(x.x, stairsY, x.y)).Where<Vector3Int>(delegate (Vector3Int x)
            {
                if (world.AnyEntityAt(x))
                {
                    return false;
                }
                IEnumerable<Vector3Int> enumerable = x.AdjacentCardinalCellsXZ();
                Func<Vector3Int, bool> func;
                if ((func = <> 9__4) == null)
                {
                    func = (<> 9__4 = (Vector3Int y) => room.Shape.Contains(y) && !room.Shape.IsOnEdgeXZ(y));
                }
                Vector3Int vector3Int = enumerable.First<Vector3Int>(func);
                vector3Int.y = room.StartY + 1;
                if (!world.CellsInfo.CanPassThroughNoActors(vector3Int))
                {
                    return false;
                }
                IEnumerable<Vector3Int> enumerable2 = x.AdjacentCardinalCellsXZ();
                Func<Vector3Int, bool> func2;
                if ((func2 = <> 9__5) == null)
                {
                    func2 = (<> 9__5 = (Vector3Int y) => adj.Shape.Contains(y) && !adj.Shape.IsOnEdgeXZ(y));
                }
                Vector3Int vector3Int2 = enumerable2.First<Vector3Int>(func2);
                vector3Int2.y = adj.StartY + 1;
                return world.CellsInfo.CanPassThroughNoActors(vector3Int2);
            }).TryGetRandomElement<Vector3Int>(out pos) || (from x in intersectionNoCorners
                                                            select new Vector3Int(x.x, stairsY, x.y) into x
                                                            where !world.AnyEntityAt(x)
                                                            select x).TryGetRandomElement<Vector3Int>(out pos);
        }

        private static EntitySpec GetDoorSpec(Room room, Room otherRoom)
        {
            if (room.Role == Room.LayoutRole.Secret || otherRoom.Role == Room.LayoutRole.Secret)
            {
                return Get.Entity_SecretPassage;
            }
            if (room.Role == Room.LayoutRole.LockedBehindSilverDoor || otherRoom.Role == Room.LayoutRole.LockedBehindSilverDoor)
            {
                return Get.Entity_SilverDoor;
            }
            if (room.Role == Room.LayoutRole.OptionalChallenge || otherRoom.Role == Room.LayoutRole.OptionalChallenge)
            {
                return Get.Entity_DangerDoor;
            }
            if (room.Role == Room.LayoutRole.End || otherRoom.Role == Room.LayoutRole.End)
            {
                if (!WorldGenUtility.ShouldUseReinforcedGates)
                {
                    return Get.Entity_Gate;
                }
                return Get.Entity_GateReinforced;
            }
            else
            {
                if (room.Role == Room.LayoutRole.LockedBehindGate || otherRoom.Role == Room.LayoutRole.LockedBehindGate)
                {
                    return Get.Entity_Gate;
                }
                return Get.Entity_Door;
            }
        }
    }
}