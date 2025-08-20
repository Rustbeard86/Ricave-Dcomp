using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class RetainedRoomInfo : ISaveableEventsReceiver
    {
        public List<RetainedRoomInfo.RoomInfo> Rooms
        {
            get
            {
                return this.rooms;
            }
        }

        protected RetainedRoomInfo()
        {
        }

        public RetainedRoomInfo(World world)
        {
            this.world = world;
            Vector3Int size = world.Size;
            this.cachedRoomsAt = new List<RetainedRoomInfo.RoomInfo>[size.x, size.y, size.z];
        }

        public RetainedRoomInfo.RoomInfo Add(CellCuboid shape, RoomSpec spec, Room.LayoutRole role, string name)
        {
            for (int i = 0; i < this.rooms.Count; i++)
            {
                if (this.rooms[i].Shape == shape && this.rooms[i].Spec == spec)
                {
                    Log.Error("Tried to add the same room twice.", false);
                    return null;
                }
            }
            RetainedRoomInfo.RoomInfo roomInfo = new RetainedRoomInfo.RoomInfo(shape, spec, role, name);
            this.rooms.Add(roomInfo);
            this.AddToCache(roomInfo);
            return roomInfo;
        }

        public List<RetainedRoomInfo.RoomInfo> GetRoomsAt(Vector3Int pos)
        {
            return this.cachedRoomsAt[pos.x, pos.y, pos.z] ?? EmptyLists<RetainedRoomInfo.RoomInfo>.Get();
        }

        public bool AnyRoomAt(Vector3Int pos)
        {
            return this.GetRoomsAt(pos).Count != 0;
        }

        public RetainedRoomInfo.RoomInfo GetRoomOfSpecAt(Vector3Int pos, RoomSpec spec)
        {
            List<RetainedRoomInfo.RoomInfo> roomsAt = this.GetRoomsAt(pos);
            for (int i = 0; i < roomsAt.Count; i++)
            {
                if (roomsAt[i].Spec == spec)
                {
                    return roomsAt[i];
                }
            }
            return null;
        }

        public RetainedRoomInfo.RoomInfo GetRoomWithRoleAt(Vector3Int pos, Room.LayoutRole role)
        {
            List<RetainedRoomInfo.RoomInfo> roomsAt = this.GetRoomsAt(pos);
            for (int i = 0; i < roomsAt.Count; i++)
            {
                if (roomsAt[i].Role == role)
                {
                    return roomsAt[i];
                }
            }
            return null;
        }

        public bool AnyRoomOfSpecAt(Vector3Int pos, RoomSpec spec)
        {
            return this.GetRoomOfSpecAt(pos, spec) != null;
        }

        public bool AnyRoomWithRoleAt(Vector3Int pos, Room.LayoutRole role)
        {
            return this.GetRoomWithRoleAt(pos, role) != null;
        }

        public RetainedRoomInfo.RoomInfo GetFirstRoomWithRole(Room.LayoutRole role)
        {
            for (int i = 0; i < this.rooms.Count; i++)
            {
                if (this.rooms[i].Role == role)
                {
                    return this.rooms[i];
                }
            }
            return null;
        }

        public void OnSaved()
        {
        }

        public void OnLoaded()
        {
            Vector3Int size = this.world.Size;
            this.cachedRoomsAt = new List<RetainedRoomInfo.RoomInfo>[size.x, size.y, size.z];
            for (int i = 0; i < this.rooms.Count; i++)
            {
                this.AddToCache(this.rooms[i]);
            }
        }

        private void AddToCache(RetainedRoomInfo.RoomInfo room)
        {
            foreach (Vector3Int vector3Int in room.Shape)
            {
                List<RetainedRoomInfo.RoomInfo> list = this.cachedRoomsAt[vector3Int.x, vector3Int.y, vector3Int.z];
                if (list == null)
                {
                    list = new List<RetainedRoomInfo.RoomInfo>();
                    this.cachedRoomsAt[vector3Int.x, vector3Int.y, vector3Int.z] = list;
                }
                list.Add(room);
            }
        }

        public void OnEntityMoved(Entity entity, Vector3Int prev)
        {
            if (entity.IsNowControlledActor)
            {
                this.SetRoomsVisited(entity.Position);
            }
        }

        public void OnEntitySpawned(Entity entity)
        {
            if (entity.IsNowControlledActor)
            {
                this.SetRoomsVisited(entity.Position);
            }
        }

        private void SetRoomsVisited(Vector3Int at)
        {
            List<RetainedRoomInfo.RoomInfo> roomsAt = this.GetRoomsAt(at);
            for (int i = 0; i < roomsAt.Count; i++)
            {
                roomsAt[i].EverVisited = true;
                roomsAt[i].Known = true;
            }
        }

        public void OnFogChanged(List<Vector3Int> unfoggedOrFogged)
        {
            this.roomsToRecalculateUnfogged.Clear();
            for (int i = 0; i < unfoggedOrFogged.Count; i++)
            {
                Vector3Int vector3Int = unfoggedOrFogged[i];
                List<RetainedRoomInfo.RoomInfo> roomsAt = this.GetRoomsAt(vector3Int);
                for (int j = 0; j < roomsAt.Count; j++)
                {
                    if (Get.FogOfWar.IsUnfogged(vector3Int))
                    {
                        if (!Get.CellsInfo.IsFilled(vector3Int))
                        {
                            roomsAt[j].AnyNonFilledCellUnfogged = true;
                        }
                    }
                    else if (roomsAt[j].AnyNonFilledCellUnfogged)
                    {
                        this.roomsToRecalculateUnfogged.Add(roomsAt[j]);
                    }
                }
                List<Entity> entitiesAt = Get.World.GetEntitiesAt(vector3Int);
                for (int k = 0; k < entitiesAt.Count; k++)
                {
                    if (entitiesAt[k].Spec == Get.Entity_Lever)
                    {
                        RetainedRoomInfo.RoomInfo roomWithRoleAt = this.GetRoomWithRoleAt(vector3Int, Room.LayoutRole.LeverRoom);
                        if (roomWithRoleAt != null)
                        {
                            roomWithRoleAt.Known = true;
                        }
                    }
                    else if (entitiesAt[k].Spec == Get.Entity_Staircase || entitiesAt[k].Spec == Get.Entity_RunEndPortal)
                    {
                        RetainedRoomInfo.RoomInfo roomWithRoleAt2 = this.GetRoomWithRoleAt(vector3Int, Room.LayoutRole.End);
                        if (roomWithRoleAt2 != null)
                        {
                            roomWithRoleAt2.Known = true;
                        }
                    }
                    else if (entitiesAt[k].Spec == Get.Entity_StaircaseRoomSign)
                    {
                        RetainedRoomInfo.RoomInfo firstRoomWithRole = this.GetFirstRoomWithRole(Room.LayoutRole.End);
                        if (firstRoomWithRole != null)
                        {
                            firstRoomWithRole.Known = true;
                        }
                    }
                }
            }
            foreach (RetainedRoomInfo.RoomInfo roomInfo in this.roomsToRecalculateUnfogged)
            {
                roomInfo.AnyNonFilledCellUnfogged = false;
                bool flag = false;
                for (int l = roomInfo.Shape.xMin; l <= roomInfo.Shape.xMax; l++)
                {
                    for (int m = roomInfo.Shape.yMin; m <= roomInfo.Shape.yMax; m++)
                    {
                        for (int n = roomInfo.Shape.zMin; n <= roomInfo.Shape.zMax; n++)
                        {
                            Vector3Int vector3Int2 = new Vector3Int(l, m, n);
                            if (!Get.CellsInfo.IsFilled(vector3Int2) && Get.FogOfWar.IsUnfogged(vector3Int2))
                            {
                                roomInfo.AnyNonFilledCellUnfogged = true;
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                        {
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
            }
            this.roomsToRecalculateUnfogged.Clear();
        }

        [Saved]
        private World world;

        [Saved(Default.New, true)]
        private List<RetainedRoomInfo.RoomInfo> rooms = new List<RetainedRoomInfo.RoomInfo>();

        private List<RetainedRoomInfo.RoomInfo>[,,] cachedRoomsAt;

        private HashSet<RetainedRoomInfo.RoomInfo> roomsToRecalculateUnfogged = new HashSet<RetainedRoomInfo.RoomInfo>();

        public class RoomInfo
        {
            public CellCuboid Shape
            {
                get
                {
                    return this.shape;
                }
            }

            public RoomSpec Spec
            {
                get
                {
                    return this.spec;
                }
            }

            public Room.LayoutRole Role
            {
                get
                {
                    return this.role;
                }
            }

            public string Name
            {
                get
                {
                    return this.name;
                }
            }

            public bool AnyNonFilledCellUnfogged
            {
                get
                {
                    return this.anyNonFilledCellUnfogged;
                }
                set
                {
                    this.anyNonFilledCellUnfogged = value;
                }
            }

            public bool EverVisited
            {
                get
                {
                    return this.everVisited;
                }
                set
                {
                    this.everVisited = value;
                }
            }

            public bool Known
            {
                get
                {
                    return this.known;
                }
                set
                {
                    this.known = value;
                }
            }

            public bool EverVisitedOrKnown
            {
                get
                {
                    return this.everVisited || this.known;
                }
            }

            protected RoomInfo()
            {
            }

            public RoomInfo(CellCuboid shape, RoomSpec spec, Room.LayoutRole role, string name)
            {
                this.shape = shape;
                this.spec = spec;
                this.role = role;
                this.name = name;
            }

            public void SetName(string name)
            {
                this.name = name;
            }

            [Saved]
            private CellCuboid shape;

            [Saved]
            private RoomSpec spec;

            [Saved]
            private Room.LayoutRole role;

            [Saved]
            private string name;

            [Saved]
            private bool anyNonFilledCellUnfogged;

            [Saved]
            private bool everVisited;

            [Saved]
            private bool known;
        }
    }
}