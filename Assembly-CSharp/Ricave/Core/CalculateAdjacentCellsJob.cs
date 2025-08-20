using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Ricave.Core
{
    [BurstCompile]
    public struct CalculateAdjacentCellsJob : IJobParallelFor
    {
        public void Execute(int index)
        {
            int3 @int = this.cells[index];
            this.result.Add(@int);
            if (@int.x + 1 < this.mapSizeX)
            {
                this.result.Add(new int3(@int.x + 1, @int.y, @int.z));
            }
            if (@int.x - 1 >= 0)
            {
                this.result.Add(new int3(@int.x - 1, @int.y, @int.z));
            }
            if (@int.y + 1 < this.mapSizeY)
            {
                this.result.Add(new int3(@int.x, @int.y + 1, @int.z));
            }
            if (@int.y - 1 >= 0)
            {
                this.result.Add(new int3(@int.x, @int.y - 1, @int.z));
            }
            if (@int.z + 1 < this.mapSizeZ)
            {
                this.result.Add(new int3(@int.x, @int.y, @int.z + 1));
            }
            if (@int.z - 1 >= 0)
            {
                this.result.Add(new int3(@int.x, @int.y, @int.z - 1));
            }
            if (@int.x + 1 < this.mapSizeX && @int.y + 1 < this.mapSizeY)
            {
                this.result.Add(new int3(@int.x + 1, @int.y + 1, @int.z));
            }
            if (@int.x + 1 < this.mapSizeX && @int.y - 1 >= 0)
            {
                this.result.Add(new int3(@int.x + 1, @int.y - 1, @int.z));
            }
            if (@int.x - 1 >= 0 && @int.y + 1 < this.mapSizeY)
            {
                this.result.Add(new int3(@int.x - 1, @int.y + 1, @int.z));
            }
            if (@int.x - 1 >= 0 && @int.y - 1 >= 0)
            {
                this.result.Add(new int3(@int.x - 1, @int.y - 1, @int.z));
            }
            if (@int.x + 1 < this.mapSizeX && @int.z + 1 < this.mapSizeZ)
            {
                this.result.Add(new int3(@int.x + 1, @int.y, @int.z + 1));
            }
            if (@int.x + 1 < this.mapSizeX && @int.z - 1 >= 0)
            {
                this.result.Add(new int3(@int.x + 1, @int.y, @int.z - 1));
            }
            if (@int.x - 1 >= 0 && @int.z + 1 < this.mapSizeZ)
            {
                this.result.Add(new int3(@int.x - 1, @int.y, @int.z + 1));
            }
            if (@int.x - 1 >= 0 && @int.z - 1 >= 0)
            {
                this.result.Add(new int3(@int.x - 1, @int.y, @int.z - 1));
            }
            if (@int.y + 1 < this.mapSizeY && @int.z + 1 < this.mapSizeZ)
            {
                this.result.Add(new int3(@int.x, @int.y + 1, @int.z + 1));
            }
            if (@int.y + 1 < this.mapSizeY && @int.z - 1 >= 0)
            {
                this.result.Add(new int3(@int.x, @int.y + 1, @int.z - 1));
            }
            if (@int.y - 1 >= 0 && @int.z + 1 < this.mapSizeZ)
            {
                this.result.Add(new int3(@int.x, @int.y - 1, @int.z + 1));
            }
            if (@int.y - 1 >= 0 && @int.z - 1 >= 0)
            {
                this.result.Add(new int3(@int.x, @int.y - 1, @int.z - 1));
            }
            if (@int.x + 1 < this.mapSizeX && @int.y + 1 < this.mapSizeY && @int.z + 1 < this.mapSizeZ)
            {
                this.result.Add(new int3(@int.x + 1, @int.y + 1, @int.z + 1));
            }
            if (@int.x - 1 >= 0 && @int.y - 1 >= 0 && @int.z - 1 >= 0)
            {
                this.result.Add(new int3(@int.x - 1, @int.y - 1, @int.z - 1));
            }
            if (@int.x + 1 < this.mapSizeX && @int.y + 1 < this.mapSizeY && @int.z - 1 >= 0)
            {
                this.result.Add(new int3(@int.x + 1, @int.y + 1, @int.z - 1));
            }
            if (@int.x - 1 >= 0 && @int.y + 1 < this.mapSizeY && @int.z - 1 >= 0)
            {
                this.result.Add(new int3(@int.x - 1, @int.y + 1, @int.z - 1));
            }
            if (@int.x - 1 >= 0 && @int.y + 1 < this.mapSizeY && @int.z + 1 < this.mapSizeZ)
            {
                this.result.Add(new int3(@int.x - 1, @int.y + 1, @int.z + 1));
            }
            if (@int.x + 1 < this.mapSizeX && @int.y - 1 >= 0 && @int.z + 1 < this.mapSizeZ)
            {
                this.result.Add(new int3(@int.x + 1, @int.y - 1, @int.z + 1));
            }
            if (@int.x + 1 < this.mapSizeX && @int.y - 1 >= 0 && @int.z - 1 >= 0)
            {
                this.result.Add(new int3(@int.x + 1, @int.y - 1, @int.z - 1));
            }
            if (@int.x - 1 >= 0 && @int.y - 1 >= 0 && @int.z + 1 < this.mapSizeZ)
            {
                this.result.Add(new int3(@int.x - 1, @int.y - 1, @int.z + 1));
            }
        }

        [ReadOnly]
        public NativeArray<int3> cells;

        [WriteOnly]
        public NativeHashSet<int3>.ParallelWriter result;

        [ReadOnly]
        public int mapSizeX;

        [ReadOnly]
        public int mapSizeY;

        [ReadOnly]
        public int mapSizeZ;
    }
}