using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_SingleEntity : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            Vector3Int vector3Int;
            if (!room.TryFindRandomNonBlockingPosForEntity(this.entitySpec, out vector3Int, true, this.requirePermanentSupport))
            {
                return false;
            }
            Maker.Make(this.entitySpec, null, false, false, true).Spawn(vector3Int);
            return true;
        }

        [Saved]
        private EntitySpec entitySpec;

        [Saved]
        private bool requirePermanentSupport;
    }
}