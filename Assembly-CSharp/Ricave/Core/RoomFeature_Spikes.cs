using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_Spikes : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            World world = Get.World;
            Vector3Int center;
            if (Get.DungeonModifier_MoreTraps.IsActiveAndAppliesToCurrentRun())
            {
                if (!room.FreeOnFloorNonBlocking.TryGetRandomElement<Vector3Int>(out center))
                {
                    return false;
                }
            }
            else
            {
                CellCuboid cellCuboid;
                if (!BiggestRectFinder.TryFindRectOfSize(room.Shape.InnerCuboid(1).BottomSurfaceCuboid, (Vector3Int x) => !world.AnyEntityAt(x) && world.CellsInfo.IsFloorUnderNoActors(x) && !room.IsEntranceCellToAvoidBlockingOnlyFromBelow(x), 3, out cellCuboid))
                {
                    return false;
                }
                center = cellCuboid.Center;
            }
            Maker.Make(Rand.Element<EntitySpec>(Get.Entity_Spikes, Get.Entity_PoisonSpikes, Get.Entity_ContaminatedSpikes), null, false, false, true).Spawn(center);
            return true;
        }
    }
}