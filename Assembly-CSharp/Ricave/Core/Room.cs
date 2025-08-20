using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class Room
    {
        public Storey Storey
        {
            get
            {
                return this.storey;
            }
        }

        public int Index
        {
            get
            {
                return this.index;
            }
        }

        public Room.LayoutRole Role
        {
            get
            {
                return this.layoutRole;
            }
            set
            {
                this.layoutRole = value;
            }
        }

        public RoomSpec AssignedRoomSpec
        {
            get
            {
                return this.assignedRoomSpec;
            }
            set
            {
                this.assignedRoomSpec = value;
            }
        }

        public bool Generated
        {
            get
            {
                return this.generated;
            }
            set
            {
                this.generated = value;
            }
        }

        public CellCuboid Shape
        {
            get
            {
                return this.shape;
            }
            set
            {
                this.shape = value;
            }
        }

        public CellRect Surface
        {
            get
            {
                return this.Shape.CellRectXZ;
            }
        }

        public int StartY
        {
            get
            {
                return this.shape.y;
            }
            set
            {
                this.shape.y = value;
            }
        }

        public int Height
        {
            get
            {
                return this.shape.height;
            }
            set
            {
                this.shape.height = value;
            }
        }

        public bool IsLeaf
        {
            get
            {
                return this.AdjacentRooms.Take<Room>(2).Count<Room>() == 1;
            }
        }

        public List<RoomFeatureSpec> RoomFeaturesGenerated
        {
            get
            {
                return this.roomFeaturesGenerated;
            }
        }

        public List<Room> MissingWallBetween
        {
            get
            {
                return this.missingWallBetween;
            }
        }

        public bool IsBossRoom
        {
            get
            {
                return this.isBossRoom;
            }
            set
            {
                this.isBossRoom = value;
            }
        }

        public Room RoomAbove
        {
            get
            {
                List<Storey> storeys = this.storey.Memory.storeys;
                if (this.storey.Index + 1 >= storeys.Count)
                {
                    return null;
                }
                return storeys[this.storey.Index + 1].RoomSlots[this.index];
            }
        }

        public Room RoomAboveAbove
        {
            get
            {
                List<Storey> storeys = this.storey.Memory.storeys;
                if (this.storey.Index + 2 >= storeys.Count)
                {
                    return null;
                }
                return storeys[this.storey.Index + 2].RoomSlots[this.index];
            }
        }

        public Room RoomBelow
        {
            get
            {
                List<Storey> storeys = this.storey.Memory.storeys;
                if (this.storey.Index - 1 < 0)
                {
                    return null;
                }
                return storeys[this.storey.Index - 1].RoomSlots[this.index];
            }
        }

        public Room RoomBelowBelow
        {
            get
            {
                List<Storey> storeys = this.storey.Memory.storeys;
                if (this.storey.Index - 2 < 0)
                {
                    return null;
                }
                return storeys[this.storey.Index - 2].RoomSlots[this.index];
            }
        }

        public List<Room> AdjacentRooms
        {
            get
            {
                if (this.adjacentRoomsCached != null && this.canKeepAdjacentRoomsCache)
                {
                    return this.adjacentRoomsCached;
                }
                if (this.adjacentRoomsCached == null)
                {
                    this.adjacentRoomsCached = new List<Room>();
                }
                else
                {
                    this.adjacentRoomsCached.Clear();
                }
                foreach (Room room in this.storey.RoomSlots)
                {
                    if (room != null && room != this && !this.GetIntersectionNoCorners(room).Empty)
                    {
                        this.adjacentRoomsCached.Add(room);
                    }
                }
                return this.adjacentRoomsCached;
            }
        }

        public IEnumerable<Vector3Int> FreeOnFloor
        {
            get
            {
                return this.Shape.InnerCuboid(1).BottomSurfaceCuboid.Where<Vector3Int>((Vector3Int x) => !Get.World.AnyEntityAt(x) && Get.CellsInfo.IsFloorUnderNoActors(x));
            }
        }

        public IEnumerable<Vector3Int> FreeOnFloorNonBlocking
        {
            get
            {
                return this.FreeOnFloor.Where<Vector3Int>((Vector3Int x) => !this.WouldPotentiallyBlockPath(x, true));
            }
        }

        public IEnumerable<Vector3Int> FreeOnFloorNoEntranceBlocking
        {
            get
            {
                return this.FreeOnFloor.Where<Vector3Int>((Vector3Int x) => !this.IsEntranceCellToAvoidBlocking(x, true));
            }
        }

        public IEnumerable<Vector3Int> FreeBelowCeiling
        {
            get
            {
                return this.Shape.InnerCuboid(1).TopSurfaceCuboid.Where<Vector3Int>((Vector3Int x) => !Get.World.AnyEntityAt(x));
            }
        }

        public IEnumerable<Vector3Int> FreeBelowCeilingNonBlocking
        {
            get
            {
                IEnumerable<Vector3Int> enumerable = this.FreeBelowCeilingNoEntranceBlocking;
                if (this.Height == 3)
                {
                    enumerable = enumerable.Where<Vector3Int>((Vector3Int x) => !this.WouldPotentiallyBlockPath(x, true));
                }
                return enumerable;
            }
        }

        public IEnumerable<Vector3Int> FreeBelowCeilingNoEntranceBlocking
        {
            get
            {
                return this.FreeBelowCeiling.Where<Vector3Int>((Vector3Int x) => !this.IsEntranceCellToAvoidBlocking(x, true));
            }
        }

        public IEnumerable<Vector3Int> OnFloorLevelEntrances
        {
            get
            {
                return this.Shape.InnerCuboid(1).BottomSurfaceCuboid.OuterCuboidXZ(1).EdgeCellsXZ.Where<Vector3Int>((Vector3Int x) => this.IsEntrance(x, true, true));
            }
        }

        public IEnumerable<Vector3Int> OnFloorLevelEntrancesNonSecret
        {
            get
            {
                return this.Shape.InnerCuboid(1).BottomSurfaceCuboid.OuterCuboidXZ(1).EdgeCellsXZ.Where<Vector3Int>((Vector3Int x) => this.IsEntrance(x, false, true));
            }
        }

        public IEnumerable<Vector3Int> OnFloorLevelEntrancesNonSecretOrOptionalChallengeRoom
        {
            get
            {
                return this.Shape.InnerCuboid(1).BottomSurfaceCuboid.OuterCuboidXZ(1).EdgeCellsXZ.Where<Vector3Int>((Vector3Int x) => this.IsEntrance(x, false, false));
            }
        }

        public Room(Storey storey, int index, CellCuboid shape, Room.LayoutRole layoutRole)
        {
            this.storey = storey;
            this.index = index;
            this.Shape = shape;
            this.layoutRole = layoutRole;
        }

        public CellRect GetIntersectionNoCorners(Room otherRoom)
        {
            return this.Surface.GetIntersectionNoCorners(otherRoom.Surface);
        }

        public CellCuboid GetWallBetween(Room otherRoom)
        {
            if (otherRoom.Storey != this.storey)
            {
                return new CellCuboid(this.shape.x, this.shape.y, this.shape.z, 0, 0, 0);
            }
            int num = Math.Max(this.StartY + 1, otherRoom.StartY + 1);
            int num2 = Math.Min(this.Shape.yMax - 1, otherRoom.Shape.yMax - 1);
            if (num2 < num)
            {
                return new CellCuboid(this.shape.x, this.shape.y, this.shape.z, 0, 0, 0);
            }
            CellRect intersectionNoCorners = this.GetIntersectionNoCorners(otherRoom);
            return new CellCuboid(intersectionNoCorners.x, num, intersectionNoCorners.y, intersectionNoCorners.width, num2 - num + 1, intersectionNoCorners.height);
        }

        public bool IsWallBetween(Room otherRoom, Vector3Int pos)
        {
            return this.GetWallBetween(otherRoom).Contains(pos);
        }

        public bool WouldPotentiallyBlockPath(Vector3Int pos, bool ensureAccessToInterestingEntities = true)
        {
            if (this.IsEntranceCellToAvoidBlocking(pos, true))
            {
                return true;
            }
            foreach (Vector3Int vector3Int in Vector3IntUtility.DirectionsXZCardinal)
            {
                Vector3Int vector3Int2 = pos + vector3Int;
                if (vector3Int2.InBounds())
                {
                    Entity firstEntityOfSpecAt = Get.World.GetFirstEntityOfSpecAt(vector3Int2, Get.Entity_Staircase);
                    if (firstEntityOfSpecAt != null && firstEntityOfSpecAt.DirectionCardinal == -vector3Int)
                    {
                        return true;
                    }
                }
            }
            int num = -1;
            int num2 = -1;
            int num3 = -1;
            int num4 = -1;
            int num5 = 0;
            int num6 = 0;
            World world = Get.World;
            CellsInfo cellsInfo = world.CellsInfo;
            Vector3Int[] directionsXZAround = Vector3IntUtility.DirectionsXZAround;
            for (int j = 0; j < directionsXZAround.Length; j++)
            {
                Vector3Int vector3Int3 = pos + directionsXZAround[j];
                if (world.InBounds(vector3Int3) && cellsInfo.CanPassThroughNoActors(vector3Int3) && !cellsInfo.AnyAIAvoidsAt(vector3Int3) && cellsInfo.IsFloorUnderNoActors(vector3Int3))
                {
                    num5++;
                    if (num == -1)
                    {
                        num = j;
                    }
                    num2 = j;
                }
                else
                {
                    num6++;
                    if (num3 == -1)
                    {
                        num3 = j;
                    }
                    num4 = j;
                }
            }
            if (num5 == directionsXZAround.Length)
            {
                return false;
            }
            bool flag = num2 - num + 1 == num5;
            bool flag2 = num4 - num3 + 1 == num6;
            if (!flag && !flag2)
            {
                return true;
            }
            if (ensureAccessToInterestingEntities)
            {
                for (int k = 0; k < directionsXZAround.Length; k++)
                {
                    Vector3Int vector3Int4 = pos + directionsXZAround[k];
                    if (world.InBounds(vector3Int4) && (!cellsInfo.CanPassThroughNoActors(vector3Int4) || cellsInfo.AnyAIAvoidsAt(vector3Int4)))
                    {
                        bool flag3 = false;
                        List<Entity> entitiesAt = world.GetEntitiesAt(vector3Int4);
                        for (int l = 0; l < entitiesAt.Count; l++)
                        {
                            if (this.ShouldProvideAccessTo(entitiesAt[l]))
                            {
                                flag3 = true;
                                break;
                            }
                        }
                        if (flag3)
                        {
                            bool flag4 = false;
                            Vector3Int[] directionsXZCardinal2 = Vector3IntUtility.DirectionsXZCardinal;
                            for (int m = 0; m < directionsXZCardinal2.Length; m++)
                            {
                                Vector3Int vector3Int5 = vector3Int4 + directionsXZCardinal2[m];
                                if (world.InBounds(vector3Int5) && cellsInfo.CanPassThroughNoActors(vector3Int5) && !cellsInfo.AnyAIAvoidsAt(vector3Int5) && cellsInfo.IsFloorUnderNoActors(vector3Int5) && vector3Int5.IsAdjacentXZ(pos))
                                {
                                    flag4 = true;
                                    break;
                                }
                            }
                            if (!flag4)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public bool IsEntranceCellToAvoidBlocking(Vector3Int pos, bool allowSecret = true)
        {
            Vector3Int[] directionsCardinalAndInside = Vector3IntUtility.DirectionsCardinalAndInside;
            for (int i = 0; i < directionsCardinalAndInside.Length; i++)
            {
                if (directionsCardinalAndInside[i].y != 1)
                {
                    Vector3Int vector3Int = pos + directionsCardinalAndInside[i];
                    if (this.IsEntrance(vector3Int, allowSecret, true))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsEntranceCellToAvoidBlockingOnlyFromBelow(Vector3Int pos)
        {
            if (pos.y != this.shape.yMin + 1)
            {
                return false;
            }
            Vector3Int vector3Int = pos + Vector3IntUtility.Down;
            return this.IsEntrance(vector3Int, true, true);
        }

        public bool TryGetOnFloorLevelEntrancesToAvoidBlockingMinMax(out int minX, out int maxX, out int minZ, out int maxZ, bool allowSecret = true)
        {
            List<Vector3Int> list = this.Shape.InnerCuboid(1).BottomSurfaceCuboid.Where<Vector3Int>((Vector3Int x) => this.IsEntranceCellToAvoidBlocking(x, allowSecret)).ToList<Vector3Int>();
            if (!list.Any())
            {
                minX = (maxX = (minZ = (maxZ = 0)));
                return false;
            }
            minX = list.Min<Vector3Int>((Vector3Int x) => x.x);
            maxX = list.Max<Vector3Int>((Vector3Int x) => x.x);
            minZ = list.Min<Vector3Int>((Vector3Int x) => x.z);
            maxZ = list.Max<Vector3Int>((Vector3Int x) => x.z);
            return true;
        }

        public bool IsEntrance(Vector3Int pos, bool allowSecret = true, bool allowOptionalChallengeRoom = true)
        {
            if (!this.Shape.IsOnEdgeXZ(pos) && pos.y != this.Shape.yMin)
            {
                return false;
            }
            World world = Get.World;
            return (allowSecret || !world.AnyEntityOfSpecAt(pos, Get.Entity_SecretPassage)) && (allowOptionalChallengeRoom || !world.AnyEntityOfSpecAt(pos, Get.Entity_DangerDoor)) && (world.CellsInfo.CanPassThroughNoActors(pos) || world.CellsInfo.AnyDoorAt(pos) || world.CellsInfo.AnyStairsAt(pos) || world.CellsInfo.AnyLadderAt(pos));
        }

        public bool IsExclusive(Vector3Int pos)
        {
            if (!this.Shape.Contains(pos))
            {
                return false;
            }
            List<Storey> storeys = this.storey.Memory.storeys;
            for (int i = 0; i < storeys.Count; i++)
            {
                List<Room> roomSlots = storeys[i].RoomSlots;
                for (int j = 0; j < roomSlots.Count; j++)
                {
                    if (roomSlots[j] != null && roomSlots[j] != this && roomSlots[j].Shape.Contains(pos))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool AllCellsToOneBelowCeilingFree(Vector3Int pos)
        {
            World world = Get.World;
            for (int i = pos.y; i <= this.Shape.yMax - 1; i++)
            {
                if (world.AnyEntityAt(new Vector3Int(pos.x, i, pos.z)))
                {
                    return false;
                }
            }
            return true;
        }

        public bool AllCellsToCeilingFree(Vector3Int pos)
        {
            World world = Get.World;
            for (int i = pos.y; i <= this.Shape.yMax; i++)
            {
                if (world.AnyEntityAt(new Vector3Int(pos.x, i, pos.z)))
                {
                    return false;
                }
            }
            return true;
        }

        public bool AllCellsToOneBelowCeilingPredicate(Vector3Int pos, Predicate<Vector3Int> predicate)
        {
            for (int i = pos.y; i <= this.Shape.yMax - 1; i++)
            {
                if (!predicate(new Vector3Int(pos.x, i, pos.z)))
                {
                    return false;
                }
            }
            return true;
        }

        public bool AllCellsToCeilingPredicate(Vector3Int pos, Predicate<Vector3Int> predicate)
        {
            for (int i = pos.y; i <= this.Shape.yMax; i++)
            {
                if (!predicate(new Vector3Int(pos.x, i, pos.z)))
                {
                    return false;
                }
            }
            return true;
        }

        public bool AllCellsToOneAboveFloorFree(Vector3Int pos)
        {
            World world = Get.World;
            for (int i = pos.y; i >= this.Shape.yMin + 1; i--)
            {
                if (world.AnyEntityAt(new Vector3Int(pos.x, i, pos.z)))
                {
                    return false;
                }
            }
            return true;
        }

        public bool AllCellsToFloorFree(Vector3Int pos)
        {
            World world = Get.World;
            for (int i = pos.y; i >= this.Shape.yMin; i--)
            {
                if (world.AnyEntityAt(new Vector3Int(pos.x, i, pos.z)))
                {
                    return false;
                }
            }
            return true;
        }

        public bool AllCellsToOneAboveFloorPredicate(Vector3Int pos, Predicate<Vector3Int> predicate)
        {
            for (int i = pos.y; i >= this.Shape.yMin + 1; i--)
            {
                if (!predicate(new Vector3Int(pos.x, i, pos.z)))
                {
                    return false;
                }
            }
            return true;
        }

        public bool AllCellsToFloorPredicate(Vector3Int pos, Predicate<Vector3Int> predicate)
        {
            for (int i = pos.y; i >= this.Shape.yMax; i--)
            {
                if (!predicate(new Vector3Int(pos.x, i, pos.z)))
                {
                    return false;
                }
            }
            return true;
        }

        public IEnumerable<Vector3Int> GetNonBlockingPositionsForEntity(EntitySpec entitySpec, bool onFloorOrIfAttachesToCeilingBelowCeiling = true, bool requirePermanentSupport = false)
        {
            bool flag = !entitySpec.CanPassThrough || (entitySpec.IsStructure && (entitySpec.Structure.AIAvoids || entitySpec.Structure.AutoUseOnActorsPassing));
            IEnumerable<Vector3Int> enumerable;
            if ((entitySpec.IsStructure && entitySpec.Structure.AttachesToAnything) || !onFloorOrIfAttachesToCeilingBelowCeiling)
            {
                enumerable = from x in this.shape.InnerCuboid(1)
                             where !Get.World.AnyEntityAt(x)
                             select x;
                if (entitySpec.IsStructure || entitySpec.IsItem)
                {
                    enumerable = enumerable.Where<Vector3Int>((Vector3Int x) => ItemOrStructureFallUtility.WouldHaveSupport(entitySpec, x, null) || (entitySpec.IsStructure && entitySpec.Structure.AttachesToCeiling && x.y == this.shape.yMax - 1 && this.RoomAbove != null));
                }
                else if (entitySpec.IsActor)
                {
                    enumerable = enumerable.Where<Vector3Int>((Vector3Int x) => Get.CellsInfo.IsFloorUnderNoActors(x));
                }
                if (onFloorOrIfAttachesToCeilingBelowCeiling)
                {
                    if (entitySpec.IsStructure && entitySpec.Structure.AttachesToCeiling)
                    {
                        enumerable = enumerable.Where<Vector3Int>((Vector3Int x) => x.y == this.shape.yMax - 1 || (entitySpec.Structure.AttachesToSameSpecAbove && Get.World.AnyEntityOfSpecAt(x.Above(), entitySpec)));
                    }
                    else
                    {
                        enumerable = enumerable.Where<Vector3Int>((Vector3Int x) => x.y == this.shape.y + 1 && Get.CellsInfo.IsFloorUnderNoActors(x));
                    }
                }
                if (requirePermanentSupport)
                {
                    if (entitySpec.IsItem || entitySpec.IsActor)
                    {
                        enumerable = enumerable.Where<Vector3Int>((Vector3Int x) => Get.CellsInfo.AnyPermanentImpassableAt(x.Below()));
                    }
                    else if (entitySpec.IsStructure && entitySpec.Structure.FallBehavior != StructureFallBehavior.None)
                    {
                        if (entitySpec.Structure.AttachesToCeiling)
                        {
                            enumerable = enumerable.Where<Vector3Int>((Vector3Int x) => Get.CellsInfo.AnyPermanentFilledImpassableAt(x.Above()) || (entitySpec.Structure.AttachesToSameSpecAbove && Get.World.AnyEntityOfSpecAt(x.Above(), entitySpec)));
                        }
                        else if (entitySpec.Structure.AttachesToBack)
                        {
                            enumerable = enumerable.Where<Vector3Int>((Vector3Int x) => x.AdjacentCardinalCellsXZ().Any<Vector3Int>((Vector3Int y) => Get.CellsInfo.AnyPermanentFilledImpassableAt(y)));
                        }
                        else
                        {
                            enumerable = enumerable.Where<Vector3Int>((Vector3Int x) => Get.CellsInfo.AnyPermanentImpassableAt(x.Below()));
                        }
                    }
                }
                if (flag)
                {
                    enumerable = enumerable.Where<Vector3Int>((Vector3Int x) => (x.y != this.shape.y + 1 || !this.WouldPotentiallyBlockPath(x, true)) && !this.IsEntranceCellToAvoidBlocking(x, true));
                }
                else
                {
                    enumerable = enumerable.Where<Vector3Int>((Vector3Int x) => !this.IsEntranceCellToAvoidBlocking(x, true));
                }
            }
            else
            {
                if (flag)
                {
                    enumerable = this.FreeOnFloorNonBlocking;
                }
                else
                {
                    enumerable = this.FreeOnFloorNoEntranceBlocking;
                }
                if (requirePermanentSupport && (!entitySpec.IsStructure || entitySpec.Structure.FallBehavior != StructureFallBehavior.None))
                {
                    enumerable = enumerable.Where<Vector3Int>((Vector3Int x) => Get.CellsInfo.AnyPermanentImpassableAt(x.Below()));
                }
            }
            return enumerable;
        }

        public bool TryFindRandomNonBlockingPosForEntity(EntitySpec entitySpec, out Vector3Int pos, bool onFloorOrIfAttachesToCeilingBelowCeiling = true, bool requirePermanentSupport = false)
        {
            return this.GetNonBlockingPositionsForEntity(entitySpec, onFloorOrIfAttachesToCeilingBelowCeiling, requirePermanentSupport).TryGetRandomElement<Vector3Int>(out pos);
        }

        public bool ContainsAnyEntityOfSpec(EntitySpec spec)
        {
            World world = Get.World;
            if (!world.AnyEntityOfSpec(spec))
            {
                return false;
            }
            foreach (Vector3Int vector3Int in this.Shape)
            {
                if (world.AnyEntityOfSpecAt(vector3Int, spec))
                {
                    return true;
                }
            }
            return false;
        }

        private bool ShouldProvideAccessTo(Entity entity)
        {
            return entity.Spec.IsItem || entity.Spec.IsActor || (entity.Spec.IsStructure && (entity.Spec.Structure.IsDoor || entity.Spec.Structure.IsStairs || entity.Spec.Structure.IsSpecial || ((Structure)entity).InnerEntities.Count != 0));
        }

        public void OnRoomFeatureGenerated(RoomFeatureSpec roomFeature)
        {
            this.roomFeaturesGenerated.Add(roomFeature);
        }

        public Room.TransitionType GetTransitionType(Room other)
        {
            if (this.storey != other.storey || this.GetIntersectionNoCorners(other).Empty)
            {
                return Room.TransitionType.None;
            }
            if (this.StartY == other.StartY)
            {
                return Room.TransitionType.SameLevel;
            }
            if (Math.Abs(this.StartY - other.StartY) != 1)
            {
                return Room.TransitionType.Shaft;
            }
            if (((this.StartY < other.StartY) ? this : other).Height <= 3)
            {
                return Room.TransitionType.Shaft;
            }
            return Room.TransitionType.Stairs;
        }

        public Room GetAdjacentRoomAtNoCorners(Vector2Int pos)
        {
            if (!this.Surface.Contains(pos))
            {
                Log.Error("Called GetAdjacentRoomAtNoCorners() but the provided cell is not even in this room.", false);
                return null;
            }
            foreach (Room room in this.AdjacentRooms)
            {
                if (this.GetIntersectionNoCorners(room).Contains(pos))
                {
                    return room;
                }
            }
            return null;
        }

        public bool ShouldHaveMissingWallBetween(Room other)
        {
            return this.missingWallBetween.Contains(other);
        }

        public Vector3Int GetAdjacentRoomDir(Room other)
        {
            CellRect intersectionNoCorners = this.GetIntersectionNoCorners(other);
            if (intersectionNoCorners.Empty)
            {
                Log.Error("Called GetAdjacentRoomDir() but the passed room is not adjacent to this one.", false);
                return default(Vector3Int);
            }
            if (intersectionNoCorners.y == this.shape.zMax)
            {
                return Vector3IntUtility.North;
            }
            if (intersectionNoCorners.x == this.shape.xMax)
            {
                return Vector3IntUtility.East;
            }
            if (intersectionNoCorners.y == this.shape.z)
            {
                return Vector3IntUtility.South;
            }
            return Vector3IntUtility.West;
        }

        public void CacheAdjacentRooms()
        {
            this.adjacentRoomsCached = null;
            this.canKeepAdjacentRoomsCache = true;
        }

        private Storey storey;

        private int index;

        private Room.LayoutRole layoutRole;

        private CellCuboid shape;

        private List<Room> missingWallBetween = new List<Room>();

        private bool isBossRoom;

        private RoomSpec assignedRoomSpec;

        private bool generated;

        private List<RoomFeatureSpec> roomFeaturesGenerated = new List<RoomFeatureSpec>();

        private List<Room> adjacentRoomsCached;

        private bool canKeepAdjacentRoomsCache;

        public Dictionary<object, object> custom = new Dictionary<object, object>();

        public enum LayoutRole
        {
            None,

            Start,

            End,

            LockedBehindGate,

            LockedBehindSilverDoor,

            LeverRoom,

            StoreysTransition,

            Secret,

            OptionalChallenge
        }

        public enum TransitionType
        {
            None,

            SameLevel,

            Stairs,

            Shaft
        }
    }
}