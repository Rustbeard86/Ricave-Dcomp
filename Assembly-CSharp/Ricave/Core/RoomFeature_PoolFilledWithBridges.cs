using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_PoolFilledWithBridges : RoomFeature_Pool
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            CellCuboid? cellCuboid = base.TryGeneratePool(room, memory);
            if (cellCuboid == null)
            {
                return false;
            }
            Vector3Int vector3Int;
            if (Rand.Chance(0.8f) && cellCuboid.Value.TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                ItemGenerator.Reward(true).Spawn(vector3Int.Below());
            }
            return true;
        }
    }
}