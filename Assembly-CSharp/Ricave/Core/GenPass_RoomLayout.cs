using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.Core
{
    public class GenPass_RoomLayout : GenPass
    {
        public override int SeedPart
        {
            get
            {
                return 906547771;
            }
        }

        private bool LogDebugInfo
        {
            get
            {
                return PrefsHelper.DevMode || App.IsDebugBuild;
            }
        }

        private int RoomCount_OneStorey
        {
            get
            {
                return Get.RunSpec.RoomsPerFloor;
            }
        }

        private int RoomCount_MultipleStoreys
        {
            get
            {
                return this.RoomCount_OneStorey - 1;
            }
        }

        private int MinRoomDistToEnd
        {
            get
            {
                return this.RoomCount_OneStorey / 3;
            }
        }

        private int MaxRoomDistToEnd
        {
            get
            {
                return Calc.RoundToInt((float)this.RoomCount_OneStorey * 0.7f);
            }
        }

        private int MinDistToEnd
        {
            get
            {
                return 3 + (this.MinRoomDistToEnd - 1) * 7 + 3;
            }
        }

        private int MaxDistToEnd
        {
            get
            {
                return 3 + (this.MaxRoomDistToEnd - 1) * 7 + 3 + 2;
            }
        }

        public static bool HasUpperStorey(WorldGenMemory memory)
        {
            return memory.config.Floor >= 2 && Get.RunSpec.HasUpperStorey;
        }

        public static bool UseSpecialEntranceRoom(WorldGenMemory memory)
        {
            return false;
        }

        public override void DoPass(WorldGenMemory memory)
        {
            List<CellRect> list = new List<CellRect>();
            int num = -1;
            int num2 = -1;
            int num3 = 0;
            for (int i = 0; i < 300; i++)
            {
                this.GenerateWithSmallestUnusedSpace(list, out num, memory);
                if (this.IsGoodLayout(list, num, out num2, i))
                {
                    this.MoveToPositivePositions(list);
                    memory.storeys = new List<Storey>();
                    memory.storeys.Add(this.CreateMainStorey(list, memory, num, num2));
                    if (GenPass_RoomLayout.HasUpperStorey(memory))
                    {
                        memory.storeys.Add(this.CreateUpperStorey(memory.storeys[0], memory));
                    }
                    if (!this.AssignStoreysTransitions(memory))
                    {
                        num3++;
                    }
                    else if (!this.AssignLeverRoom(memory))
                    {
                        num3++;
                    }
                    else if (!this.AssignLockedRooms(memory))
                    {
                        num3++;
                    }
                    else
                    {
                        if (Rand.Chance(0.35f) && Get.RunSpec != Get.Run_Main1)
                        {
                            this.TryMakeHighlyElevatedRoom(memory);
                        }
                        if (Rand.Chance(0.7f))
                        {
                            this.AssignMissingWallsBetweenRooms(memory);
                        }
                        if (Rand.Chance(0.3f))
                        {
                            this.TryMakeRoomWithIncreasedHeight(memory);
                        }
                        this.AssignBossRoom(memory);
                        if (this.LogDebugInfo)
                        {
                            string text = ((num3 != 0) ? (" (" + num3.ToString() + " times could not assign good room roles)") : "");
                            Log.Message(string.Concat(new string[]
                            {
                                "Generating a good map layout took ",
                                (i + 1).ToString(),
                                " tries (",
                                300.ToString(),
                                " max)",
                                text
                            }));
                            break;
                        }
                        break;
                    }
                }
                else if (i == 299)
                {
                    Log.Error("Could not generate correct room layout.", false);
                }
            }
            this.AddExtraRooms(memory);
            this.DoErrorChecks(memory);
            foreach (Storey storey in memory.storeys)
            {
                foreach (Room room in storey.Rooms)
                {
                    room.CacheAdjacentRooms();
                }
            }
        }

        private void AddExtraRooms(WorldGenMemory memory)
        {
            if (Rand.Chance(0.5f))
            {
                this.AddSecretRoom(memory);
            }
            if (Rand.Chance(0.5f))
            {
                this.AddCeilingSecretRoom(memory);
            }
            if (Rand.Chance(0.5f))
            {
                this.AddOptionalChallengeRoom(memory);
            }
            if (Rand.Chance(0.5f))
            {
                this.AddUndergroundSecretRoom(memory);
            }
            if (Rand.Chance(0.5f))
            {
                this.AddCeilingGlassRoom(memory);
            }
        }

        private void AddSecretRoom(WorldGenMemory memory)
        {
            Storey storey = memory.storeys.First<Storey>((Storey x) => x.StoreyType == Storey.Type.Main);
            Room room = this.TryAddExtraRoom(memory, new Vector3Int(5, 4, 5), Room.LayoutRole.Secret);
            if (room != null)
            {
                storey.RoomSlots.Add(room);
                for (int i = 0; i < memory.storeys.Count; i++)
                {
                    if (i != storey.Index)
                    {
                        memory.storeys[i].RoomSlots.Add(null);
                    }
                }
                this.MoveRoomsToPositivePositions(memory);
            }
        }

        private void AddCeilingSecretRoom(WorldGenMemory memory)
        {
            Storey mainStorey = memory.storeys.First<Storey>((Storey x) => x.StoreyType == Storey.Type.Main);
            Room room;
            if (!memory.AllRooms.Where<Room>((Room x) => x.Role != Room.LayoutRole.LockedBehindSilverDoor && x.Role != Room.LayoutRole.OptionalChallenge && x.Role != Room.LayoutRole.Secret && (x.Role != Room.LayoutRole.Start || !GenPass_RoomLayout.UseSpecialEntranceRoom(memory)) && x.RoomAbove == null && x.RoomAboveAbove == null && (x.Storey.IsLastStorey || mainStorey.RoomSlots[x.Index].AdjacentRooms.All<Room>((Room y) => x.Storey.StoreyAbove.RoomSlots[y.Index] == null))).TryGetRandomElement<Room>(out room))
            {
                return;
            }
            if (room.Storey.IsLastStorey)
            {
                memory.storeys.Add(this.CreateExtrasAboveStorey(memory));
            }
            room.Storey.StoreyAbove.RoomSlots[room.Index] = new Room(room.Storey.StoreyAbove, room.Index, new CellCuboid(room.Shape.x, room.Shape.yMax, room.Shape.z, room.Shape.width, 4, room.Shape.depth), Room.LayoutRole.Secret);
        }

        private void AddOptionalChallengeRoom(WorldGenMemory memory)
        {
            Storey storey = memory.storeys.First<Storey>((Storey x) => x.StoreyType == Storey.Type.Main);
            Room room = this.TryAddExtraRoom(memory, new Vector3Int(5, 4, 5), Room.LayoutRole.OptionalChallenge);
            if (room != null)
            {
                storey.RoomSlots.Add(room);
                for (int i = 0; i < memory.storeys.Count; i++)
                {
                    if (i != storey.Index)
                    {
                        memory.storeys[i].RoomSlots.Add(null);
                    }
                }
                this.MoveRoomsToPositivePositions(memory);
            }
        }

        private void AddUndergroundSecretRoom(WorldGenMemory memory)
        {
            Storey mainStorey = memory.storeys.First<Storey>((Storey x) => x.StoreyType == Storey.Type.Main);
            Room room;
            if (!mainStorey.Rooms.Where<Room>((Room x) => x.Role != Room.LayoutRole.Start && x.Role != Room.LayoutRole.LockedBehindSilverDoor && x.Role != Room.LayoutRole.OptionalChallenge && x.Role != Room.LayoutRole.Secret && x.Role != Room.LayoutRole.End && x.RoomBelow == null && x.RoomBelowBelow == null && x.AdjacentRooms.All<Room>((Room y) => Math.Abs(y.StartY - x.StartY) <= 1) && (x.Storey.IsFirstStorey || mainStorey.RoomSlots[x.Index].AdjacentRooms.All<Room>((Room y) => x.Storey.StoreyBelow.RoomSlots[y.Index] == null))).TryGetRandomElement<Room>(out room))
            {
                return;
            }
            if (room.Storey.IsFirstStorey)
            {
                foreach (Room room2 in memory.AllRooms)
                {
                    room2.Shape = room2.Shape.WithAddedY(4);
                }
                memory.storeys.Insert(0, this.CreateUndergroundStorey(memory));
            }
            room.Storey.StoreyBelow.RoomSlots[room.Index] = new Room(room.Storey.StoreyBelow, room.Index, new CellCuboid(room.Shape.x, room.Shape.yMin - 3, room.Shape.z, room.Shape.width, 4, room.Shape.depth), Room.LayoutRole.Secret);
        }

        private void AddCeilingGlassRoom(WorldGenMemory memory)
        {
            Storey mainStorey = memory.storeys.First<Storey>((Storey x) => x.StoreyType == Storey.Type.Main);
            Room room;
            if (!memory.AllRooms.Where<Room>((Room x) => x.Role != Room.LayoutRole.LockedBehindSilverDoor && x.Role != Room.LayoutRole.OptionalChallenge && x.Role != Room.LayoutRole.Secret && (x.Role != Room.LayoutRole.Start || !GenPass_RoomLayout.UseSpecialEntranceRoom(memory)) && x.RoomAbove == null && x.RoomAboveAbove == null && (x.Storey.IsLastStorey || mainStorey.RoomSlots[x.Index].AdjacentRooms.All<Room>((Room y) => x.Storey.StoreyAbove.RoomSlots[y.Index] == null))).TryGetRandomElement<Room>(out room))
            {
                return;
            }
            if (room.Storey.IsLastStorey)
            {
                memory.storeys.Add(this.CreateExtrasAboveStorey(memory));
            }
            room.Storey.StoreyAbove.RoomSlots[room.Index] = new Room(room.Storey.StoreyAbove, room.Index, new CellCuboid(room.Shape.x, room.Shape.yMax, room.Shape.z, room.Shape.width, 3, room.Shape.depth), Room.LayoutRole.OptionalChallenge);
        }

        private Room TryAddExtraRoom(WorldGenMemory memory, Vector3Int size, Room.LayoutRole layoutRole)
        {
            Storey storey = memory.storeys.First<Storey>((Storey x) => x.StoreyType == Storey.Type.Main);
            List<Room> roomSlots = storey.RoomSlots;
            List<CellRect> list = (from x in roomSlots
                                   where x != null
                                   select x.Surface).ToList<CellRect>();
            List<Room> list2 = storey.Rooms.Where<Room>((Room x) => x.Role != Room.LayoutRole.LockedBehindSilverDoor && x.Role != Room.LayoutRole.OptionalChallenge && x.Role != Room.LayoutRole.Secret && (x.Role != Room.LayoutRole.Start || !GenPass_RoomLayout.UseSpecialEntranceRoom(memory))).ToList<Room>();
            List<Room> list3 = list2.Where<Room>((Room x) => x.IsLeaf).ToList<Room>();
            for (int i = 0; i < 150; i++)
            {
                Room room;
                if (i > 130 && list3.Count != 0)
                {
                    if (!list3.TryGetRandomElement<Room>(out room))
                    {
                        break;
                    }
                }
                else if (!list2.TryGetRandomElement<Room>(out room))
                {
                    break;
                }
                this.tmpAnyRoomSlotAt.Clear();
                foreach (Room room2 in roomSlots)
                {
                    if (room2 != room)
                    {
                        foreach (Vector2Int vector2Int in room2.Surface)
                        {
                            this.tmpAnyRoomSlotAt.Add(vector2Int);
                        }
                    }
                }
                int num = Rand.RangeInclusive(0, 3);
                int num2;
                int num3;
                if (num == 0)
                {
                    num2 = Rand.RangeInclusive(room.Shape.x - size.x + 3, room.Shape.xMax - 2);
                    num3 = room.Shape.zMax;
                }
                else if (num == 1)
                {
                    num2 = room.Shape.xMax;
                    num3 = Rand.RangeInclusive(room.Shape.z - size.z + 3, room.Shape.zMax - 2);
                }
                else if (num == 2)
                {
                    num2 = Rand.RangeInclusive(room.Shape.x - size.x + 3, room.Shape.xMax - 2);
                    num3 = room.Shape.z - size.z + 1;
                }
                else
                {
                    num2 = room.Shape.x - size.x + 1;
                    num3 = Rand.RangeInclusive(room.Shape.z - size.z + 3, room.Shape.zMax - 2);
                }
                CellCuboid cellCuboid = new CellCuboid(num2, room.Shape.y, num3, size.x, size.y, size.z);
                bool flag = false;
                foreach (Vector2Int vector2Int2 in cellCuboid.SurfaceXZ)
                {
                    if (this.tmpAnyRoomSlotAt.Contains(vector2Int2))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    IEnumerable<CellRect> roomNeighbors = this.GetRoomNeighbors(cellCuboid.CellRectXZ, list);
                    if (roomNeighbors.Count<CellRect>() == 1 && !(roomNeighbors.First<CellRect>() != room.Surface))
                    {
                        return new Room(storey, storey.RoomSlots.Count, cellCuboid, layoutRole);
                    }
                }
            }
            return null;
        }

        private void DoErrorChecks(WorldGenMemory memory)
        {
            for (int i = 0; i < memory.storeys.Count - 1; i++)
            {
                foreach (Room room in memory.storeys[i].Rooms)
                {
                    Room roomAbove = room.RoomAbove;
                    if (roomAbove != null && roomAbove.StartY < room.StartY + room.Height - 1)
                    {
                        Log.Error("Overlapping rooms y-wise. First: " + room.Shape.ToString() + " Second: " + roomAbove.Shape.ToString(), false);
                    }
                }
            }
            for (int j = 0; j < memory.storeys.Count; j++)
            {
                foreach (Room room2 in memory.storeys[j].Rooms)
                {
                    if (room2.Shape.x < 0 || room2.Shape.y < 0 || room2.Shape.z < 0)
                    {
                        Log.Error("Room starts at negative position.", false);
                    }
                    if (room2.Height <= 2)
                    {
                        Log.Error("Room has height <= 2. This means it's pointless.", false);
                    }
                    if (room2.Surface.width <= 2 || room2.Surface.height <= 2)
                    {
                        Log.Error("Room has width or depth <= 2. This means it's pointless.", false);
                    }
                }
            }
            int count = memory.storeys[0].RoomSlots.Count;
            for (int k = 0; k < count; k++)
            {
                for (int l = 0; l < memory.storeys.Count; l++)
                {
                    Room room3 = memory.storeys[l].RoomSlots[k];
                    if (room3 != null)
                    {
                        for (int m = l + 2; m < memory.storeys.Count; m++)
                        {
                            Room room4 = memory.storeys[m].RoomSlots[k];
                            if (room4 != null && room3.Shape.Overlaps(room4.Shape))
                            {
                                Log.Error("Rooms with at least 1 storey gap between them have overlapping areas (i.e. room above starts when room below ends). This is not allowed, otherwise RoomAbove and RoomBelow would be misleading and break logic in some room filler code.", false);
                            }
                        }
                    }
                }
            }
        }

        private Storey CreateMainStorey(List<CellRect> roomShapes, WorldGenMemory memory, int startRoomIndex, int endRoomIndex)
        {
            Storey storey = new Storey(Storey.Type.Main, memory);
            for (int i = 0; i < roomShapes.Count; i++)
            {
                bool flag = i == startRoomIndex && GenPass_RoomLayout.UseSpecialEntranceRoom(memory);
                CellCuboid cellCuboid = new CellCuboid(roomShapes[i].x, 1, roomShapes[i].y, roomShapes[i].width, flag ? 5 : 4, roomShapes[i].height);
                Room.LayoutRole layoutRole;
                if (i == startRoomIndex)
                {
                    layoutRole = Room.LayoutRole.Start;
                }
                else if (i == endRoomIndex)
                {
                    layoutRole = Room.LayoutRole.End;
                }
                else
                {
                    layoutRole = Room.LayoutRole.None;
                }
                Room room = new Room(storey, i, cellCuboid, layoutRole);
                storey.RoomSlots.Add(room);
            }
            for (int j = 0; j < 5; j++)
            {
                Room room3;
                if (j == 0)
                {
                    Room room2;
                    if (storey.Rooms.TryGetRandomElement<Room>(out room2))
                    {
                        room2.Shape = room2.Shape.WithAddedY(1);
                    }
                }
                else if (storey.Rooms.Where<Room>((Room x) => x.StartY == 1 && this.AnyAdjacentRoomWithElevation(x, x.StartY + 1)).TryGetRandomElement<Room>(out room3))
                {
                    room3.Shape = room3.Shape.WithAddedY(1);
                }
            }
            return storey;
        }

        private bool AnyAdjacentRoomWithElevation(Room room, int y)
        {
            using (List<Room>.Enumerator enumerator = room.AdjacentRooms.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.StartY == y)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool AllAdjacentRoomsWithElevation(Room room, int y)
        {
            using (List<Room>.Enumerator enumerator = room.AdjacentRooms.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.StartY != y)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private Storey CreateUpperStorey(Storey mainStorey, WorldGenMemory memory)
        {
            Storey storey = new Storey(Storey.Type.Upper, memory);
            for (int i = 0; i < mainStorey.RoomSlots.Count; i++)
            {
                if (mainStorey.RoomSlots[i] == null)
                {
                    Log.Error("Main storey has a null room. Null rooms only make sense for non-main storeys.", false);
                    storey.RoomSlots.Add(null);
                }
                else
                {
                    CellCuboid cellCuboid = new CellCuboid(mainStorey.RoomSlots[i].Shape.x, mainStorey.RoomSlots[i].StartY + mainStorey.RoomSlots[i].Height - 1, mainStorey.RoomSlots[i].Shape.z, mainStorey.RoomSlots[i].Shape.width, 4, mainStorey.RoomSlots[i].Shape.depth);
                    Room room = new Room(storey, i, cellCuboid, Room.LayoutRole.None);
                    storey.RoomSlots.Add(room);
                }
            }
            for (int j = 0; j < 4; j++)
            {
                Room room2;
                if (!storey.Rooms.Where<Room>((Room x) => x.Role == Room.LayoutRole.None && x.IsLeaf).TryGetRandomElement<Room>(out room2))
                {
                    break;
                }
                storey.RoomSlots[storey.RoomSlots.IndexOf(room2)] = null;
            }
            int num = 0;
            Room room3;
            while (num < 1 && storey.Rooms.Where<Room>((Room x) => x.Height == 4 && this.AllAdjacentRoomsWithElevation(x, x.StartY)).TryGetRandomElement<Room>(out room3))
            {
                Room room4 = room3;
                int height = room4.Height;
                room4.Height = height - 1;
                num++;
            }
            return storey;
        }

        private Storey CreateExtrasAboveStorey(WorldGenMemory memory)
        {
            Storey storey = new Storey(Storey.Type.ExtrasAbove, memory);
            int count = memory.storeys[0].RoomSlots.Count;
            for (int i = 0; i < count; i++)
            {
                storey.RoomSlots.Add(null);
            }
            return storey;
        }

        private Storey CreateUndergroundStorey(WorldGenMemory memory)
        {
            Storey storey = new Storey(Storey.Type.Underground, memory);
            int count = memory.storeys[0].RoomSlots.Count;
            for (int i = 0; i < count; i++)
            {
                storey.RoomSlots.Add(null);
            }
            return storey;
        }

        private void GenerateWithSmallestUnusedSpace(List<CellRect> roomShapes, out int startRoomIndex, WorldGenMemory memory)
        {
            roomShapes.Clear();
            List<CellRect> list = new List<CellRect>();
            int num = 0;
            startRoomIndex = -1;
            for (int i = 0; i < 10; i++)
            {
                int num2;
                this.Generate(list, out num2, memory);
                if (num2 != -1)
                {
                    int num3;
                    int num4;
                    int num5;
                    int num6;
                    GenPass_RoomLayout.GetBoundingBox(list, out num3, out num4, out num5, out num6);
                    int num7 = (num5 - num3 + 1) * (num6 - num4 + 1);
                    for (int j = 0; j < list.Count; j++)
                    {
                        num7 -= list[j].Area;
                    }
                    if (i == 0 || num7 < num)
                    {
                        roomShapes.Clear();
                        roomShapes.AddRange(list);
                        startRoomIndex = num2;
                        num = num7;
                    }
                }
            }
        }

        private void Generate(List<CellRect> roomShapes, out int startRoomIndex, WorldGenMemory memory)
        {
            roomShapes.Clear();
            startRoomIndex = -1;
            if (!GenPass_RoomLayout.UseSpecialEntranceRoom(memory))
            {
                roomShapes.Add(new CellRect(0, 0, Rand.RangeInclusive(5, 7), Rand.RangeInclusive(5, 7)));
                startRoomIndex = 0;
            }
            int num = (GenPass_RoomLayout.HasUpperStorey(memory) ? this.RoomCount_MultipleStoreys : this.RoomCount_OneStorey);
            for (int i = 0; i < num; i++)
            {
                CellRect cellRect = this.CreateNewRoom(roomShapes);
                roomShapes.Add(cellRect);
            }
            if (GenPass_RoomLayout.UseSpecialEntranceRoom(memory))
            {
                CellRect? cellRect2 = this.TryCreateSpecialEntranceRoom(roomShapes);
                if (cellRect2 != null)
                {
                    roomShapes.Add(cellRect2.Value);
                    startRoomIndex = roomShapes.Count - 1;
                    return;
                }
                startRoomIndex = -1;
            }
        }

        private CellRect? TryCreateSpecialEntranceRoom(List<CellRect> roomShapes)
        {
            int num;
            int num2;
            int num3;
            int num4;
            GenPass_RoomLayout.GetBoundingBox(roomShapes, out num, out num2, out num3, out num4);
            List<int> list = new List<int>();
            for (int i = num - 3 + 1; i <= num3 - 3 - 1; i++)
            {
                list.Add(i);
            }
            list.Shuffle<int>();
            for (int j = 0; j < list.Count; j++)
            {
                int num5 = list[j];
                int num6 = num2 - 6 + 1;
                CellRect cellRect = new CellRect(num5, num6, 7, 6);
                Vector2Int vector2Int = new Vector2Int(num5 + 3, num6 + 6 - 1);
                int num7 = 0;
                bool flag = false;
                foreach (CellRect cellRect2 in roomShapes)
                {
                    if (cellRect2.Overlaps(cellRect))
                    {
                        CellRect intersectionNoCorners = cellRect2.GetIntersectionNoCorners(cellRect);
                        if (!intersectionNoCorners.Empty)
                        {
                            num7++;
                        }
                        if (!intersectionNoCorners.Contains(vector2Int))
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                if (num7 == 1 && !flag)
                {
                    return new CellRect?(cellRect);
                }
            }
            return null;
        }

        private bool IsGoodLayout(List<CellRect> rooms, int startRoomIndex, out int bestEndRoomIndex, int tries)
        {
            bestEndRoomIndex = startRoomIndex;
            if (rooms.Count == 0 || startRoomIndex == -1)
            {
                return false;
            }
            float num = Calc.Pow(6f, 2f) + (float)(rooms.Count - 1) * Calc.Pow(7f, 2f);
            int num2 = rooms.Sum<CellRect>((CellRect x) => x.Area);
            if (Math.Max(num, (float)num2) / Math.Min(num, (float)num2) > 1.2f)
            {
                return false;
            }
            if (rooms.Count > 6)
            {
                int num3 = BiggestCycleFinderHeur<CellRect>.FindBiggestCycle(rooms[startRoomIndex], (CellRect x) => this.GetRoomNeighbors(x, rooms));
                if (tries < 150)
                {
                    if (num3 < 5)
                    {
                        return false;
                    }
                }
                else if (tries < 200 && num3 < 4)
                {
                    return false;
                }
            }
            BFS<CellRect>.TraverseAll(rooms[startRoomIndex], (CellRect x) => this.GetRoomNeighbors(x, rooms), this.tmpRoomDistances, null);
            int maxRoomDist = 0;
            for (int i = 0; i < rooms.Count; i++)
            {
                int num4;
                if (!this.tmpRoomDistances.TryGetValue(rooms[i], out num4))
                {
                    Log.Error("One of the rooms generated is unreachable.", false);
                    return false;
                }
                maxRoomDist = Math.Max(maxRoomDist, num4);
            }
            BFS<Vector2Int>.TraverseAll(rooms[startRoomIndex].Center, (Vector2Int x) => this.GetCellNeighbors(x, rooms), this.tmpDistances, null);
            CellRect cellRect;
            if (!rooms.Where<CellRect>((CellRect x) => this.tmpRoomDistances[x] == maxRoomDist).TryGetRandomElement<CellRect>(out cellRect))
            {
                return false;
            }
            bestEndRoomIndex = rooms.IndexOf(cellRect);
            return true;
        }

        private IEnumerable<CellRect> GetRoomNeighbors(CellRect room, List<CellRect> rooms)
        {
            int num;
            for (int i = 0; i < rooms.Count; i = num + 1)
            {
                if (!(rooms[i] == room) && !room.GetIntersectionNoCorners(rooms[i]).Empty)
                {
                    yield return rooms[i];
                }
                num = i;
            }
            yield break;
        }

        private IEnumerable<Vector2Int> GetCellNeighbors(Vector2Int cell, List<CellRect> rooms)
        {
            int num;
            for (int i = 0; i < Vector2IntUtility.DirectionsCardinal.Length; i = num + 1)
            {
                Vector2Int neigh = cell + Vector2IntUtility.DirectionsCardinal[i];
                for (int j = 0; j < rooms.Count; j = num + 1)
                {
                    if (rooms[j].Contains(neigh))
                    {
                        yield return neigh;
                    }
                    num = j;
                }
                neigh = default(Vector2Int);
                num = i;
            }
            yield break;
        }

        private CellRect CreateNewRoom(List<CellRect> rooms)
        {
            if (rooms.Count == 0)
            {
                int num = Rand.RangeInclusive(4, 10);
                int num2 = Rand.RangeInclusive(4, 10);
                return new CellRect(0, 0, num, num2);
            }
            int num3;
            int num4;
            int num5;
            int num6;
            GenPass_RoomLayout.GetBoundingBox(rooms, out num3, out num4, out num5, out num6);
            for (int i = 0; i < 100; i++)
            {
                if (this.LogDebugInfo && i == 50)
                {
                    Log.Warning("Tried to generate a new room " + i.ToString() + " times.", false);
                }
                int num7 = Rand.RangeInclusive(4, 10);
                int num8 = Rand.RangeInclusive(4, 10);
                int num9;
                if (i > 25)
                {
                    num9 = Rand.RangeInclusive(0, 3);
                }
                else if (num5 - num3 < num6 - num4)
                {
                    num9 = Rand.RangeInclusive(0, 1);
                }
                else
                {
                    num9 = Rand.RangeInclusive(2, 3);
                }
                int num10;
                int num11;
                int num12;
                int num13;
                if (num9 == 0)
                {
                    num10 = num3 - num7 + 1;
                    num11 = Rand.RangeInclusive(num4 - num8 + 1, num6);
                    num12 = 1;
                    num13 = 0;
                }
                else if (num9 == 1)
                {
                    num10 = num5;
                    num11 = Rand.RangeInclusive(num4 - num8 + 1, num6);
                    num12 = -1;
                    num13 = 0;
                }
                else if (num9 == 2)
                {
                    num10 = Rand.RangeInclusive(num3 - num7 + 1, num5);
                    num11 = num4 - num8 + 1;
                    num12 = 0;
                    num13 = 1;
                }
                else
                {
                    num10 = Rand.RangeInclusive(num3 - num7 + 1, num5);
                    num11 = num6;
                    num12 = 0;
                    num13 = -1;
                }
                do
                {
                    CellRect cellRect = new CellRect(num10, num11, num7, num8);
                    bool flag = false;
                    for (int j = 0; j < rooms.Count; j++)
                    {
                        if (rooms[j].Overlaps(cellRect))
                        {
                            flag = true;
                            if (!cellRect.GetIntersectionNoCorners(rooms[j]).Empty)
                            {
                                return cellRect;
                            }
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                    num10 += num12;
                    num11 += num13;
                }
                while (num10 <= num5 && num11 <= num6 && num10 > num3 - num7 && num11 > num4 - num8);
            }
            Log.Error("Could not generate any new room.", false);
            return default(CellRect);
        }

        private static void GetBoundingBox(List<CellRect> rooms, out int minX, out int minZ, out int maxX, out int maxZ)
        {
            minX = rooms[0].x;
            minZ = rooms[0].y;
            maxX = rooms[0].xMax;
            maxZ = rooms[0].yMax;
            for (int i = 1; i < rooms.Count; i++)
            {
                minX = Math.Min(minX, rooms[i].x);
                minZ = Math.Min(minZ, rooms[i].y);
                maxX = Math.Max(maxX, rooms[i].xMax);
                maxZ = Math.Max(maxZ, rooms[i].yMax);
            }
        }

        private void MoveToPositivePositions(List<CellRect> rooms)
        {
            int num = rooms[0].x;
            int num2 = rooms[0].y;
            for (int i = 1; i < rooms.Count; i++)
            {
                num = Math.Min(num, rooms[i].x);
                num2 = Math.Min(num2, rooms[i].y);
            }
            for (int j = 0; j < rooms.Count; j++)
            {
                CellRect cellRect = rooms[j];
                cellRect.x -= num;
                cellRect.y -= num2;
                cellRect.x++;
                cellRect.y++;
                rooms[j] = cellRect;
            }
        }

        private void MoveRoomsToPositivePositions(WorldGenMemory memory)
        {
            List<Room> list = memory.AllRooms.ToList<Room>();
            int num = list[0].Shape.x;
            int num2 = list[0].Shape.z;
            for (int i = 1; i < list.Count; i++)
            {
                num = Math.Min(num, list[i].Shape.x);
                num2 = Math.Min(num2, list[i].Shape.z);
            }
            for (int j = 0; j < list.Count; j++)
            {
                CellCuboid shape = list[j].Shape;
                shape.x -= num;
                shape.z -= num2;
                shape.x++;
                shape.z++;
                list[j].Shape = shape;
            }
        }

        private bool AssignStoreysTransitions(WorldGenMemory memory)
        {
            if (!GenPass_RoomLayout.HasUpperStorey(memory))
            {
                return true;
            }
            Storey storey = memory.storeys.First<Storey>((Storey x) => x.StoreyType == Storey.Type.Main);
            for (int i = 0; i < 2; i++)
            {
                Room room;
                if (!storey.Rooms.Where<Room>((Room x) => x.Role == Room.LayoutRole.None && x.RoomAbove != null && x.RoomAbove.Role == Room.LayoutRole.None).TryGetRandomElement<Room>(out room))
                {
                    return i > 0;
                }
                room.Role = Room.LayoutRole.StoreysTransition;
                room.RoomAbove.Role = Room.LayoutRole.StoreysTransition;
            }
            return true;
        }

        private bool AssignLeverRoom(WorldGenMemory memory)
        {
            Room room;
            if (memory.storeys.Last<Storey>().Rooms.Where<Room>((Room x) => x.Role == Room.LayoutRole.None).TryGetRandomElement<Room>(out room))
            {
                room.Role = Room.LayoutRole.LeverRoom;
                return true;
            }
            return false;
        }

        private bool AssignLockedRooms(WorldGenMemory memory)
        {
            IEnumerable<Room> enumerable = memory.AllRooms.Where<Room>((Room x) => x.Role == Room.LayoutRole.None && x.IsLeaf);
            bool flag = memory.config.Floor != 1 && memory.config.Floor % 2 == 1;
            if (!flag && memory.AllRooms.Count<Room>() <= 6)
            {
                return true;
            }
            if (flag)
            {
                enumerable = enumerable.Where<Room>((Room x) => x.StartY == x.AdjacentRooms.First<Room>().StartY);
            }
            Room room;
            if (enumerable.TryGetRandomElement<Room>(out room))
            {
                room.Role = (flag ? Room.LayoutRole.LockedBehindSilverDoor : Room.LayoutRole.LockedBehindGate);
                return true;
            }
            return false;
        }

        private void AssignBossRoom(WorldGenMemory memory)
        {
            if (!Get.RunSpec.HasMiniBosses)
            {
                return;
            }
            if (Get.DungeonModifier_NoMiniBosses.IsActiveAndAppliesToCurrentRun() && !Get.DungeonModifier_ExtraMiniBoss.IsActiveAndAppliesToCurrentRun())
            {
                return;
            }
            if (memory.config.Floor == 1 && (!Get.Quest_KillNightmareLord.IsActive() || memory.config.Floor != 1) && !Get.DungeonModifier_ExtraMiniBoss.IsActiveAndAppliesToCurrentRun())
            {
                return;
            }
            Room startRoom = memory.AllRooms.First<Room>((Room x) => x.Role == Room.LayoutRole.Start);
            Room room;
            if (memory.storeys.First<Storey>((Storey x) => x.StoreyType == Storey.Type.Main).Rooms.Where<Room>((Room x) => x.Role != Room.LayoutRole.Start && x.Role != Room.LayoutRole.End && x.Role != Room.LayoutRole.LockedBehindSilverDoor && x.Role != Room.LayoutRole.LockedBehindGate && x.Role != Room.LayoutRole.OptionalChallenge && x.Role != Room.LayoutRole.Secret).TryGetMaxBy<Room, int>((Room x) => x.Shape.InnerCuboid(1).BottomSurfaceCuboid.Center.GetGridDistance(startRoom.Shape.InnerCuboid(1).BottomSurfaceCuboid.Center), out room))
            {
                room.IsBossRoom = true;
            }
        }

        private void TryMakeHighlyElevatedRoom(WorldGenMemory memory)
        {
            Room room;
            if (memory.AllRooms.Where<Room>(delegate (Room x)
            {
                if (x.Role != Room.LayoutRole.LockedBehindSilverDoor && x.Role != Room.LayoutRole.OptionalChallenge && x.Role != Room.LayoutRole.Secret && x.Role != Room.LayoutRole.End && (x.Role != Room.LayoutRole.Start || !GenPass_RoomLayout.UseSpecialEntranceRoom(memory)) && x.RoomBelow == null)
                {
                    if (!x.AdjacentRooms.Any<Room>((Room y) => y.Role == Room.LayoutRole.LockedBehindSilverDoor || y.Role == Room.LayoutRole.OptionalChallenge || y.Role == Room.LayoutRole.End || y.Role == Room.LayoutRole.Secret))
                    {
                        if (x.AdjacentRooms.All<Room>((Room y) => y.RoomAbove == null))
                        {
                            return this.AllAdjacentRoomsWithElevation(x, x.StartY);
                        }
                    }
                }
                return false;
            }).TryGetRandomElement<Room>(out room))
            {
                foreach (Storey storey in memory.storeys)
                {
                    Room room2 = storey.RoomSlots[room.Index];
                    if (room2 != null)
                    {
                        room2.StartY += 2;
                    }
                }
            }
        }

        private void AssignMissingWallsBetweenRooms(WorldGenMemory memory)
        {
            IEnumerable<ValueTuple<Room, Room>> enumerable = from x in memory.AllRooms.SelectMany<Room, ValueTuple<Room, Room>>((Room @from) => @from.AdjacentRooms.Select<Room, ValueTuple<Room, Room>>((Room to) => new ValueTuple<Room, Room>(@from, to)))
                                                             where !x.Item1.MissingWallBetween.Contains(x.Item2) && x.Item1.Role != Room.LayoutRole.Start && x.Item1.Role != Room.LayoutRole.LockedBehindGate && x.Item1.Role != Room.LayoutRole.LockedBehindSilverDoor && x.Item1.Role != Room.LayoutRole.OptionalChallenge && x.Item1.Role != Room.LayoutRole.End && x.Item1.Role != Room.LayoutRole.Secret && x.Item2.Role != Room.LayoutRole.Start && x.Item2.Role != Room.LayoutRole.LockedBehindGate && x.Item2.Role != Room.LayoutRole.LockedBehindSilverDoor && x.Item2.Role != Room.LayoutRole.OptionalChallenge && x.Item2.Role != Room.LayoutRole.End && x.Item2.Role != Room.LayoutRole.Secret && !x.Item1.GetWallBetween(x.Item2).Empty
                                                             select x;
            int num = 1;
            if (Rand.Chance(0.1f))
            {
                num = 2;
            }
            for (int i = 0; i < num; i++)
            {
                ValueTuple<Room, Room> valueTuple;
                if (enumerable.TryGetRandomElement<ValueTuple<Room, Room>>(out valueTuple))
                {
                    valueTuple.Item1.MissingWallBetween.Add(valueTuple.Item2);
                    valueTuple.Item2.MissingWallBetween.Add(valueTuple.Item1);
                }
            }
        }

        private void TryMakeRoomWithIncreasedHeight(WorldGenMemory memory)
        {
            Room room;
            if (memory.AllRooms.Where<Room>((Room x) => x.RoomAbove == null && x.Role != Room.LayoutRole.LockedBehindSilverDoor && x.Role != Room.LayoutRole.OptionalChallenge && x.Role != Room.LayoutRole.Secret && (x.Role != Room.LayoutRole.Start || !GenPass_RoomLayout.UseSpecialEntranceRoom(memory)) && ((x.Storey.IsMainStorey && x.Height == 4) || (x.Storey.IsUpperStorey && x.Height == 4))).TryGetRandomElement<Room>(out room))
            {
                Room room2 = room;
                int num = room2.Height;
                room2.Height = num + 1;
                if (room.RoomAbove != null)
                {
                    Room roomAbove = room.RoomAbove;
                    num = roomAbove.StartY;
                    roomAbove.StartY = num + 1;
                }
            }
        }

        private const int StartingRoomSizeMin = 5;

        private const int StartingRoomSizeMax = 7;

        private const int MinRoomSize = 4;

        private const int MaxRoomSize = 10;

        private const int MainStoreyElevatedRooms = 5;

        private const int UpperStoreyLeavesToRemove = 4;

        private const int UpperStoreyRoomCountDecreasedHeight = 1;

        private const int MainStoreyBaseY = 1;

        private const int MainStoreyBaseHeight = 4;

        private const int UpperStoreyBaseHeight = 4;

        private const int MinRequiredCycle = 5;

        private const int StoreysTransitionsCount = 2;

        private const int AvgRoomSize = 7;

        private const float HighlyElevatedRoomChance = 0.35f;

        private const float MissingWallChance = 0.7f;

        private const float RoomWithIncreasedHeightChance = 0.3f;

        private const int SpecialEntranceWidth = 7;

        private const int SpecialEntranceHeight = 5;

        private const int SpecialEntranceDepth = 6;

        private const int GenerateLayoutWithGoodDistToEndTries = 300;

        private const int GenerateRoomTries = 100;

        private const int GenerateLayoutWithGoodDistToEndTries_GoodCycle = 150;

        private const int GenerateLayoutWithGoodDistToEndTries_GoodEnoughCycle = 200;

        private HashSet<Vector2Int> tmpAnyRoomSlotAt = new HashSet<Vector2Int>();

        private Dictionary<CellRect, int> tmpRoomDistances = new Dictionary<CellRect, int>();

        private Dictionary<Vector2Int, int> tmpDistances = new Dictionary<Vector2Int, int>();
    }
}