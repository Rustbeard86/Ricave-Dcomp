using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_Walls : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            World world = Get.World;
            int height = room.Height;
            CellRect surface = room.Surface;
            bool flag = false;
            foreach (Vector2Int vector2Int in surface.EdgeCells)
            {
                bool flag2 = false;
                bool flag3 = false;
                RoomPart_Walls.WindowsWallType windowsWallType = RoomPart_Walls.WindowsWallType.None;
                Room adjacentRoomAtNoCorners = room.GetAdjacentRoomAtNoCorners(vector2Int);
                if (adjacentRoomAtNoCorners != null)
                {
                    if (room.ShouldHaveMissingWallBetween(adjacentRoomAtNoCorners))
                    {
                        flag3 = true;
                    }
                    else
                    {
                        windowsWallType = this.GetWindowsWallType(memory, room, adjacentRoomAtNoCorners);
                        bool flag4 = room.Role == Room.LayoutRole.End || adjacentRoomAtNoCorners.Role == Room.LayoutRole.End;
                        if (adjacentRoomAtNoCorners.StartY == room.StartY && room.Role != Room.LayoutRole.LockedBehindSilverDoor && adjacentRoomAtNoCorners.Role != Room.LayoutRole.LockedBehindSilverDoor && room.Role != Room.LayoutRole.OptionalChallenge && adjacentRoomAtNoCorners.Role != Room.LayoutRole.OptionalChallenge && room.Role != Room.LayoutRole.Secret && adjacentRoomAtNoCorners.Role != Room.LayoutRole.Secret && (!flag4 || !WorldGenUtility.ShouldUseReinforcedGates))
                        {
                            Vector3Int vector3Int = new Vector3Int(vector2Int.x, room.StartY + 1, vector2Int.y);
                            if (flag4)
                            {
                                if (!world.AnyEntityAt(vector3Int))
                                {
                                    Maker.Make(Get.Entity_WoodenWall, null, false, false, true).Spawn(vector3Int);
                                }
                                flag2 = true;
                            }
                            else if (windowsWallType == RoomPart_Walls.WindowsWallType.FirstRow)
                            {
                                if (!world.AnyEntityAt(vector3Int))
                                {
                                    Maker.Make(Get.Entity_Window, null, false, false, true).Spawn(vector3Int, room.GetAdjacentRoomDir(adjacentRoomAtNoCorners).RightDir());
                                    flag = true;
                                }
                                flag2 = true;
                            }
                            else
                            {
                                if (!world.AnyEntityAt(vector3Int))
                                {
                                    Maker.Make(Get.Entity_BrickWall, null, false, false, true).Spawn(vector3Int);
                                }
                                flag2 = true;
                            }
                        }
                    }
                }
                int num = ((room.RoomAbove != null || (((adjacentRoomAtNoCorners != null) ? adjacentRoomAtNoCorners.RoomAbove : null) != null && adjacentRoomAtNoCorners.RoomAbove.Shape.Contains(new Vector3Int(vector2Int.x, room.Shape.yMax, vector2Int.y)))) ? (height - 1) : height);
                EntitySpec entitySpec = ((adjacentRoomAtNoCorners != null) ? this.GetSpecialWallBetween(room, adjacentRoomAtNoCorners) : null);
                for (int i = 0; i < num; i++)
                {
                    Vector3Int vector3Int2 = new Vector3Int(vector2Int.x, room.StartY + i, vector2Int.y);
                    if ((!flag2 || i != 1) && (!flag3 || !room.IsWallBetween(adjacentRoomAtNoCorners, vector3Int2)) && (adjacentRoomAtNoCorners == null || room.StartY + i != Math.Max(room.StartY, adjacentRoomAtNoCorners.StartY) || (!flag3 && !world.AnyEntityOfSpecAt(vector3Int2.Above(), Get.Entity_Window) && (windowsWallType != RoomPart_Walls.WindowsWallType.All || !room.IsWallBetween(adjacentRoomAtNoCorners, vector3Int2.Above())))) && (adjacentRoomAtNoCorners == null || room.StartY + i != Math.Min(room.Shape.yMax, adjacentRoomAtNoCorners.Shape.yMax) || (!flag3 && world.CellsInfo.IsFilled(vector3Int2.Below()))))
                    {
                        Vector3Int vector3Int3 = vector3Int2.Below();
                        Vector3Int vector3Int4 = vector3Int2.Above();
                        if (!world.AnyEntityAt(vector3Int2) && !world.CellsInfo.AnyStairsAt(vector3Int3) && !world.CellsInfo.AnyLadderAt(vector3Int3) && !world.CellsInfo.AnyDoorAt(vector3Int4) && !world.CellsInfo.AnyStairsAt(vector3Int4) && !world.CellsInfo.AnyLadderAt(vector3Int4) && (i != height - 1 || !world.CellsInfo.AnyDoorAt(vector3Int3)))
                        {
                            if (windowsWallType == RoomPart_Walls.WindowsWallType.All && room.IsWallBetween(adjacentRoomAtNoCorners, vector3Int2))
                            {
                                Maker.Make(Get.Entity_Window, null, false, false, true).Spawn(vector3Int2, room.GetAdjacentRoomDir(adjacentRoomAtNoCorners).RightDir());
                                flag = true;
                            }
                            else if (entitySpec != null && room.IsWallBetween(adjacentRoomAtNoCorners, vector3Int2))
                            {
                                if (entitySpec == Get.Entity_Window)
                                {
                                    Maker.Make(entitySpec, null, false, false, true).Spawn(vector3Int2, room.GetAdjacentRoomDir(adjacentRoomAtNoCorners).RightDir());
                                }
                                else
                                {
                                    Maker.Make(entitySpec, null, false, false, true).Spawn(vector3Int2);
                                }
                            }
                            else
                            {
                                Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int2);
                            }
                        }
                    }
                }
            }
            if (flag)
            {
                memory.generatedWindowsWall = true;
            }
            using (List<Room>.Enumerator enumerator2 = room.AdjacentRooms.GetEnumerator())
            {
                Func<Vector3Int, bool> <> 9__2;
                while (enumerator2.MoveNext())
                {
                    Room adjacentRoom = enumerator2.Current;
                    if (room.ShouldHaveMissingWallBetween(adjacentRoom) && room.Index <= adjacentRoom.Index && Rand.Bool)
                    {
                        Room.TransitionType transitionType = room.GetTransitionType(adjacentRoom);
                        if (transitionType == Room.TransitionType.SameLevel || transitionType == Room.TransitionType.Stairs)
                        {
                            CellCuboid wallBetween = room.GetWallBetween(adjacentRoom);
                            IEnumerable<Vector3Int> enumerable = wallBetween.Where<Vector3Int>((Vector3Int x) => x.y == wallBetween.y);
                            Vector3Int dir = room.GetAdjacentRoomDir(adjacentRoom);
                            Vector3Int vector3Int5 = dir.RightDir();
                            IEnumerable<Vector3Int> enumerable2 = enumerable.Where<Vector3Int>((Vector3Int x) => world.CellsInfo.CanPassThroughNoActors(x) && world.CellsInfo.CanPassThroughNoActors(x + dir) && world.CellsInfo.CanPassThroughNoActors(x - dir) && (!adjacentRoom.Generated || world.CellsInfo.IsFloorUnderNoActors(x + dir) || world.CellsInfo.AnyLadderAt(x.Below()) || world.CellsInfo.AnyStairsAt(x.Below())));
                            IEnumerable<Vector3Int> enumerable3 = enumerable2;
                            Func<Vector3Int, bool> func;
                            if ((func = <> 9__2) == null)
                            {
                                func = (<> 9__2 = (Vector3Int x) => world.CellsInfo.AnyLadderAt(x.Below()) || world.CellsInfo.AnyStairsAt(x.Below()));
                            }
                            Vector3Int vector3Int6;
                            if (!enumerable3.Where<Vector3Int>(func).TryGetRandomElement<Vector3Int>(out vector3Int6) && !enumerable2.TryGetRandomElement<Vector3Int>(out vector3Int6) && !enumerable.Where<Vector3Int>((Vector3Int x) => world.CellsInfo.CanPassThroughNoActors(x) && world.CellsInfo.CanPassThroughNoActors(x + dir) && world.CellsInfo.CanPassThroughNoActors(x - dir)).TryGetRandomElement<Vector3Int>(out vector3Int6) && !enumerable.Where<Vector3Int>((Vector3Int x) => world.CellsInfo.CanPassThroughNoActors(x) && (world.CellsInfo.CanPassThroughNoActors(x + dir) || world.CellsInfo.CanPassThroughNoActors(x - dir))).TryGetRandomElement<Vector3Int>(out vector3Int6))
                            {
                                vector3Int6 = enumerable.FirstOrDefault<Vector3Int>();
                            }
                            foreach (Vector3Int vector3Int7 in enumerable)
                            {
                                if (vector3Int7 != vector3Int6 && !world.CellsInfo.AnyLadderAt(vector3Int7.Below()) && !world.CellsInfo.AnyStairsAt(vector3Int7.Below()) && !world.AnyEntityAt(vector3Int7) && (!world.AnyEntityAt(vector3Int7.Below()) || world.CellsInfo.AnyFilledImpassableAt(vector3Int7.Below())))
                                {
                                    Maker.Make(Get.Entity_Fence, null, false, false, true).Spawn(vector3Int7, vector3Int5);
                                }
                            }
                        }
                    }
                }
            }
        }

        private RoomPart_Walls.WindowsWallType GetWindowsWallType(WorldGenMemory memory, Room a, Room b)
        {
            if (memory.generatedWindowsWall)
            {
                return RoomPart_Walls.WindowsWallType.None;
            }
            if (a.Role == Room.LayoutRole.LockedBehindSilverDoor || b.Role == Room.LayoutRole.LockedBehindSilverDoor || a.Role == Room.LayoutRole.OptionalChallenge || b.Role == Room.LayoutRole.OptionalChallenge || a.Role == Room.LayoutRole.Secret || b.Role == Room.LayoutRole.Secret)
            {
                return RoomPart_Walls.WindowsWallType.None;
            }
            Rand.PushState(Calc.CombineHashes<int, int, int, int, int>(memory.config.WorldSeed, Math.Min(a.Index, b.Index), Math.Max(a.Index, b.Index), Math.Min(a.Storey.Index, b.Storey.Index), 357536599));
            RoomPart_Walls.WindowsWallType windowsWallType;
            if (Rand.Chance(0.05f))
            {
                windowsWallType = (Rand.Bool ? RoomPart_Walls.WindowsWallType.FirstRow : RoomPart_Walls.WindowsWallType.All);
            }
            else
            {
                windowsWallType = RoomPart_Walls.WindowsWallType.None;
            }
            Rand.PopState();
            return windowsWallType;
        }

        private EntitySpec GetSpecialWallBetween(Room room, Room adjacentRoom)
        {
            if (room.Role == Room.LayoutRole.OptionalChallenge || adjacentRoom.Role == Room.LayoutRole.OptionalChallenge)
            {
                return Get.Entity_Window;
            }
            return null;
        }

        private enum WindowsWallType
        {
            None,

            FirstRow,

            All
        }
    }
}