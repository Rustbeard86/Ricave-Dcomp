using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class CellsInfo
    {
        public CellsInfo.CellInfo[,,] Cells
        {
            get
            {
                return this.cellsInfo;
            }
        }

        public CellsInfo(World world)
        {
            this.world = world;
            this.worldSize = world.Size;
            this.worldSizeXTimesY = this.worldSize.x * this.worldSize.y;
            this.cellsInfo = new CellsInfo.CellInfo[this.worldSize.x, this.worldSize.y, this.worldSize.z];
            this.adjacentCanPassThrough = new ValueTuple<Vector3Int[], int>[this.worldSize.x * this.worldSize.y * this.worldSize.z];
            this.adjacentCanPassThroughDirty = new bool[this.worldSize.x * this.worldSize.y * this.worldSize.z];
            this.RecalculateAll();
        }

        private void RecalculateAll()
        {
            Vector3Int size = this.world.Size;
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    for (int k = 0; k < size.z; k++)
                    {
                        Vector3Int vector3Int = new Vector3Int(i, j, k);
                        this.cellsInfo[i, j, k] = this.CalculateCellInfo(vector3Int);
                    }
                }
            }
            for (int l = 0; l < this.adjacentCanPassThroughDirty.Length; l++)
            {
                this.adjacentCanPassThroughDirty[l] = true;
            }
        }

        public bool IsFloorUnder(Vector3Int pos)
        {
            return this.IsFloorUnder(pos, Vector3Int.down);
        }

        public bool IsFloorUnder(Vector3Int pos, Vector3Int gravity)
        {
            Vector3Int vector3Int = pos + gravity;
            return this.world.InBounds(vector3Int) && !this.cellsInfo[vector3Int.x, vector3Int.y, vector3Int.z].canPassThrough;
        }

        public bool IsFloorUnderNoActors(Vector3Int pos)
        {
            return this.IsFloorUnderNoActors(pos, Vector3Int.down);
        }

        public bool IsFloorUnderNoActors(Vector3Int pos, Vector3Int gravity)
        {
            Vector3Int vector3Int = pos + gravity;
            return this.world.InBounds(vector3Int) && !this.cellsInfo[vector3Int.x, vector3Int.y, vector3Int.z].canPassThroughNoActors;
        }

        public bool IsLadderUnder(Vector3Int pos)
        {
            return this.IsLadderUnder(pos, Vector3Int.down);
        }

        public bool IsLadderUnder(Vector3Int pos, Vector3Int gravity)
        {
            Vector3Int vector3Int = pos + gravity;
            return this.world.InBounds(vector3Int) && this.cellsInfo[vector3Int.x, vector3Int.y, vector3Int.z].anyLadder;
        }

        public bool CanPassThrough(Vector3Int pos)
        {
            return this.cellsInfo[pos.x, pos.y, pos.z].canPassThrough;
        }

        public bool CanPassThroughNoActors(Vector3Int pos)
        {
            return this.cellsInfo[pos.x, pos.y, pos.z].canPassThroughNoActors;
        }

        public bool CanPassThroughOrAnyDoorAt(Vector3Int pos)
        {
            return this.CanPassThrough(pos) || this.AnyDoorAt(pos);
        }

        public bool CanProjectilesPassThrough(Vector3Int pos)
        {
            return this.cellsInfo[pos.x, pos.y, pos.z].canProjectilesPassThrough;
        }

        public bool CanSeeThrough(Vector3Int pos)
        {
            return this.cellsInfo[pos.x, pos.y, pos.z].canSeeThrough;
        }

        public bool CanSeeThroughToImpassable(Vector3Int pos)
        {
            return this.cellsInfo[pos.x, pos.y, pos.z].canSeeThroughToImpassable;
        }

        public bool BlocksDiagonalMovement(Vector3Int pos)
        {
            return this.cellsInfo[pos.x, pos.y, pos.z].blocksDiagonalMovement;
        }

        public bool AnyActorAt(Vector3Int pos)
        {
            return this.cellsInfo[pos.x, pos.y, pos.z].anyActor;
        }

        public bool AnyStairsAt(Vector3Int pos)
        {
            return this.cellsInfo[pos.x, pos.y, pos.z].anyStairs;
        }

        public bool AnyLadderAt(Vector3Int pos)
        {
            return this.cellsInfo[pos.x, pos.y, pos.z].anyLadder;
        }

        public bool AnyDoorAt(Vector3Int pos)
        {
            return this.cellsInfo[pos.x, pos.y, pos.z].anyDoor;
        }

        public bool AnyItemAt(Vector3Int pos)
        {
            return this.cellsInfo[pos.x, pos.y, pos.z].anyItem;
        }

        public bool AnyStructureAt(Vector3Int pos)
        {
            return this.cellsInfo[pos.x, pos.y, pos.z].anyStructure;
        }

        public bool AnyWaterAt(Vector3Int pos)
        {
            return this.cellsInfo[pos.x, pos.y, pos.z].anyWater;
        }

        public bool AnyFilledImpassableAt(Vector3Int pos)
        {
            return this.cellsInfo[pos.x, pos.y, pos.z].anyFilledImpassable;
        }

        public bool AnyFilledCantSeeThroughAt(Vector3Int pos)
        {
            return this.cellsInfo[pos.x, pos.y, pos.z].anyFilledCantSeeThrough;
        }

        public bool AnyPermanentFilledImpassableAt(Vector3Int pos)
        {
            return this.cellsInfo[pos.x, pos.y, pos.z].anyPermanentFilledImpassable;
        }

        public bool AnyPermanentImpassableAt(Vector3Int pos)
        {
            return this.cellsInfo[pos.x, pos.y, pos.z].anyPermanentImpassable;
        }

        public bool AnySemiPermanentFilledImpassableAt(Vector3Int pos)
        {
            return this.cellsInfo[pos.x, pos.y, pos.z].anySemiPermanentFilledImpassable;
        }

        public bool AnySemiPermanentImpassableAt(Vector3Int pos)
        {
            return this.cellsInfo[pos.x, pos.y, pos.z].anySemiPermanentImpassable;
        }

        public bool AnyAIAvoidsAt(Vector3Int pos)
        {
            return this.cellsInfo[pos.x, pos.y, pos.z].anyAIAvoids;
        }

        public bool AnyAutoUseOnPassingActors(Vector3Int pos)
        {
            return this.cellsInfo[pos.x, pos.y, pos.z].anyAutoUseOnPassingActors;
        }

        public bool AnySupportsTargetingLocations(Vector3Int pos)
        {
            return this.cellsInfo[pos.x, pos.y, pos.z].anySupportsTargetingLocation;
        }

        public bool AnyBlocksBevels(Vector3Int pos)
        {
            return this.cellsInfo[pos.x, pos.y, pos.z].anyBlocksBevels;
        }

        public bool AnyGivesGravitySupportInside(Vector3Int pos)
        {
            return this.cellsInfo[pos.x, pos.y, pos.z].anyGivesGravitySupportInside;
        }

        public bool IsFilled(Vector3Int pos)
        {
            return this.cellsInfo[pos.x, pos.y, pos.z].isFilled;
        }

        public bool IsFallingAt(Vector3Int pos, Actor actor, bool noActors = false)
        {
            return this.IsFallingAt(pos, actor.Gravity, actor.CanFly, actor.CanUseLadders, noActors);
        }

        public bool IsFallingAt(Vector3Int pos, Vector3Int gravity, bool canFly, bool canUseLadders, bool noActors = false)
        {
            if (canFly)
            {
                return false;
            }
            if (noActors)
            {
                if (this.IsFloorUnderNoActors(pos, gravity))
                {
                    return false;
                }
            }
            else if (this.IsFloorUnder(pos, gravity))
            {
                return false;
            }
            return (!canUseLadders || (!this.AnyLadderAt(pos) && !this.IsLadderUnder(pos, gravity))) && !this.AnyGivesGravitySupportInside(pos) && (pos + gravity).InBounds();
        }

        public float OffsetFromStairsAt(Vector3Int pos)
        {
            return this.cellsInfo[pos.x, pos.y, pos.z].offsetFromStairs;
        }

        public Vector3Int[] GetAdjacentCanPassThrough(int pos, out int length)
        {
            if (this.adjacentCanPassThroughDirty[pos])
            {
                this.adjacentCanPassThroughDirty[pos] = false;
                Vector3Int[] array = this.adjacentCanPassThrough[pos].Item1;
                if (array == null)
                {
                    array = new Vector3Int[26];
                }
                Vector3Int vector3Int = this.IndexToPos(pos);
                int num = 0;
                for (int i = 0; i < Vector3IntUtility.Directions.Length; i++)
                {
                    Vector3Int vector3Int2 = Vector3IntUtility.Directions[i];
                    int num2 = vector3Int.x + vector3Int2.x;
                    int num3 = vector3Int.y + vector3Int2.y;
                    int num4 = vector3Int.z + vector3Int2.z;
                    if (this.world.InBounds(num2, num3, num4) && this.cellsInfo[num2, num3, num4].canPassThrough)
                    {
                        array[num++] = new Vector3Int(num2, num3, num4);
                    }
                }
                this.adjacentCanPassThrough[pos] = new ValueTuple<Vector3Int[], int>(array, num);
            }
            ValueTuple<Vector3Int[], int> valueTuple = this.adjacentCanPassThrough[pos];
            length = valueTuple.Item2;
            return valueTuple.Item1;
        }

        public void OnEntityMoved(Entity entity, Vector3Int prev)
        {
            this.ProcessEntityChangedAt(prev);
            this.ProcessEntityChangedAt(entity.Position);
        }

        public void OnEntitySpawned(Entity entity)
        {
            this.ProcessEntityChangedAt(entity.Position);
        }

        public void OnEntityDeSpawned(Entity entity)
        {
            this.ProcessEntityChangedAt(entity.Position);
        }

        private void ProcessEntityChangedAt(Vector3Int pos)
        {
            this.cellsInfo[pos.x, pos.y, pos.z] = this.CalculateCellInfo(pos);
            for (int i = 0; i < Vector3IntUtility.Directions.Length; i++)
            {
                Vector3Int vector3Int = Vector3IntUtility.Directions[i];
                Vector3Int vector3Int2 = new Vector3Int(pos.x + vector3Int.x, pos.y + vector3Int.y, pos.z + vector3Int.z);
                if (vector3Int2.InBounds())
                {
                    this.adjacentCanPassThroughDirty[this.PosToIndex(vector3Int2.x, vector3Int2.y, vector3Int2.z)] = true;
                }
            }
        }

        private CellsInfo.CellInfo CalculateCellInfo(Vector3Int pos)
        {
            CellsInfo.CellInfo cellInfo = default(CellsInfo.CellInfo);
            List<Entity> entitiesAt = this.world.GetEntitiesAt(pos);
            cellInfo.canPassThrough = true;
            cellInfo.canPassThroughNoActors = true;
            cellInfo.canProjectilesPassThrough = true;
            cellInfo.canSeeThrough = true;
            cellInfo.canSeeThroughToImpassable = true;
            int i = 0;
            int count = entitiesAt.Count;
            while (i < count)
            {
                Entity entity = entitiesAt[i];
                EntitySpec spec = entity.Spec;
                if (!spec.CanPassThrough)
                {
                    cellInfo.canPassThrough = false;
                    if (!(entity is Actor))
                    {
                        cellInfo.canPassThroughNoActors = false;
                    }
                }
                if (!spec.CanProjectilesPassThrough)
                {
                    cellInfo.canProjectilesPassThrough = false;
                }
                if (!spec.CanSeeThrough)
                {
                    cellInfo.canSeeThrough = false;
                }
                if (!spec.CanSeeThroughToImpassable)
                {
                    cellInfo.canSeeThroughToImpassable = false;
                }
                if (spec.BlocksDiagonalMovement)
                {
                    cellInfo.blocksDiagonalMovement = true;
                }
                if (spec.IsPermanentFilledImpassable)
                {
                    cellInfo.anyPermanentFilledImpassable = true;
                }
                if (!spec.CanPassThrough && spec.IsPermanent)
                {
                    cellInfo.anyPermanentImpassable = true;
                }
                if (spec.IsSemiPermanentFilledImpassable)
                {
                    cellInfo.anySemiPermanentFilledImpassable = true;
                }
                if (!spec.CanPassThrough && spec.IsSemiPermanent)
                {
                    cellInfo.anySemiPermanentImpassable = true;
                }
                if (entity is Actor)
                {
                    cellInfo.anyActor = true;
                }
                else if (entity is Structure)
                {
                    cellInfo.anyStructure = true;
                    StructureProps structure = spec.Structure;
                    if (structure.IsFilled)
                    {
                        cellInfo.isFilled = true;
                        if (!spec.CanPassThrough)
                        {
                            cellInfo.anyFilledImpassable = true;
                        }
                        if (!spec.CanSeeThrough)
                        {
                            cellInfo.anyFilledCantSeeThrough = true;
                        }
                    }
                    if (structure.IsStairs)
                    {
                        cellInfo.anyStairs = true;
                        cellInfo.offsetFromStairs = Math.Max(cellInfo.offsetFromStairs, structure.OffsetFromStairs);
                    }
                    if (structure.IsLadder)
                    {
                        cellInfo.anyLadder = true;
                    }
                    if (structure.IsDoor)
                    {
                        cellInfo.anyDoor = true;
                    }
                    if (structure.IsWater)
                    {
                        cellInfo.anyWater = true;
                    }
                    if (structure.AIAvoids)
                    {
                        cellInfo.anyAIAvoids = true;
                    }
                    if (structure.AutoUseOnActorsPassing)
                    {
                        cellInfo.anyAutoUseOnPassingActors = true;
                    }
                    if (structure.SupportsTargetingLocation)
                    {
                        cellInfo.anySupportsTargetingLocation = true;
                    }
                    if (structure.BlocksBevels)
                    {
                        cellInfo.anyBlocksBevels = true;
                    }
                    if (structure.GivesGravitySupportInside)
                    {
                        cellInfo.anyGivesGravitySupportInside = true;
                    }
                }
                else if (entity is Item)
                {
                    cellInfo.anyItem = true;
                }
                i++;
            }
            return cellInfo;
        }

        private int PosToIndex(int x, int y, int z)
        {
            return z * this.worldSizeXTimesY + y * this.worldSize.x + x;
        }

        private Vector3Int IndexToPos(int index)
        {
            int num = index / this.worldSizeXTimesY;
            index -= num * this.worldSizeXTimesY;
            int num2 = index / this.worldSize.x;
            return new Vector3Int(index - num2 * this.worldSize.x, num2, num);
        }

        private World world;

        private CellsInfo.CellInfo[,,] cellsInfo;

        private ValueTuple<Vector3Int[], int>[] adjacentCanPassThrough;

        private bool[] adjacentCanPassThroughDirty;

        private Vector3Int worldSize;

        private int worldSizeXTimesY;

        public struct CellInfo
        {
            public bool canPassThrough;

            public bool canPassThroughNoActors;

            public bool canProjectilesPassThrough;

            public bool canSeeThrough;

            public bool canSeeThroughToImpassable;

            public bool blocksDiagonalMovement;

            public bool anyActor;

            public bool anyStairs;

            public bool anyLadder;

            public bool anyDoor;

            public bool anyItem;

            public bool anyStructure;

            public bool anyWater;

            public bool anyFilledImpassable;

            public bool anyFilledCantSeeThrough;

            public bool anyPermanentFilledImpassable;

            public bool anyPermanentImpassable;

            public bool anySemiPermanentFilledImpassable;

            public bool anySemiPermanentImpassable;

            public bool anyAIAvoids;

            public bool anyAutoUseOnPassingActors;

            public bool anySupportsTargetingLocation;

            public bool anyBlocksBevels;

            public bool anyGivesGravitySupportInside;

            public bool isFilled;

            public float offsetFromStairs;
        }
    }
}