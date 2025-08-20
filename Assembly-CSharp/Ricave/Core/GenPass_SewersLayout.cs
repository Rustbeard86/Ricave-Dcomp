using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class GenPass_SewersLayout : GenPass
    {
        public override int SeedPart
        {
            get
            {
                return 572318572;
            }
        }

        public override void DoPass(WorldGenMemory memory)
        {
            for (int i = 0; i < memory.storeys.Count; i++)
            {
                this.CalculateSewersLayout(memory.storeys[i]);
            }
        }

        private void CalculateSewersLayout(Storey storey)
        {
            this.roomCells.Clear();
            this.nonSharedEdgeCells.Clear();
            foreach (Room room in storey.Rooms)
            {
                foreach (Vector3Int vector3Int in room.Shape.EdgeCells)
                {
                    if (this.nonSharedEdgeCells.Contains(vector3Int))
                    {
                        this.nonSharedEdgeCells.Remove(vector3Int);
                    }
                    else if (!this.roomCells.Contains(vector3Int))
                    {
                        this.nonSharedEdgeCells.Add(vector3Int);
                    }
                }
                foreach (Vector3Int vector3Int2 in room.Shape)
                {
                    this.roomCells.Add(vector3Int2);
                }
            }
            for (int i = 0; i < 1; i++)
            {
                this.GenerateOne(storey);
            }
        }

        private void GenerateOne(Storey storey)
        {
            int num = 0;
            while (num < 200 && !this.TryGenerateOne(storey))
            {
                num++;
            }
        }

        private bool TryGenerateOne(Storey storey)
        {
            Vector3Int vector3Int;
            if (!this.nonSharedEdgeCells.TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            Vector3Int vector3Int2 = Vector3Int.zero;
            foreach (Vector3Int vector3Int3 in vector3Int.AdjacentCardinalCellsXZ())
            {
                if (!this.roomCells.Contains(vector3Int3))
                {
                    vector3Int2 = vector3Int3 - vector3Int;
                    break;
                }
            }
            if (vector3Int2 == Vector3Int.zero)
            {
                return false;
            }
            Vector3Int vector3Int4 = new Vector3Int(vector3Int2.z, 0, vector3Int2.x);
            int num = Rand.RangeInclusive(2, 7);
            HashSet<Vector3Int> hashSet = new HashSet<Vector3Int>();
            if (Rand.Bool)
            {
                Vector3Int vector3Int5 = vector3Int + vector3Int2;
                for (int i = 0; i < num; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        hashSet.Add(vector3Int5 + vector3Int2 * i + vector3Int4 * j);
                    }
                }
            }
            else
            {
                Vector3Int vector3Int6 = vector3Int + vector3Int2;
                for (int k = 0; k < num; k++)
                {
                    for (int l = 0; l < 2; l++)
                    {
                        hashSet.Add(vector3Int6 + vector3Int4 * k + vector3Int2 * l);
                    }
                }
            }
            if (this.IsGood(hashSet, storey))
            {
                this.Apply(hashSet, storey);
                return true;
            }
            return false;
        }

        private bool IsGood(HashSet<Vector3Int> candidate, Storey storey)
        {
            foreach (Vector3Int vector3Int in candidate)
            {
                if (this.roomCells.Contains(vector3Int))
                {
                    return false;
                }
                if (storey.Sewers.Contains(vector3Int))
                {
                    return false;
                }
            }
            return true;
        }

        private void Apply(HashSet<Vector3Int> candidate, Storey storey)
        {
        }

        private HashSet<Vector3Int> roomCells = new HashSet<Vector3Int>();

        private HashSet<Vector3Int> nonSharedEdgeCells = new HashSet<Vector3Int>();

        private const int SewersPerStorey = 1;

        private const int Tries = 200;

        private const int MinSewersLength = 2;

        private const int MaxSewersLength = 7;

        private const int SewersWidth = 2;
    }
}