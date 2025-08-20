using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_Crates : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            this.DoCrates(room, memory);
            this.DoShelves(room, memory);
        }

        private void DoCrates(Room room, WorldGenMemory memory)
        {
            if (!memory.UnusedBaseMiscItems.Any<Item>())
            {
                return;
            }
            int num = Rand.RangeInclusive((room.Shape.BottomSurfaceCuboid.InnerCuboidXZ(1).Volume >= 25) ? 2 : 1, 4);
            List<Entity> list = RoomFillerUtility.DoPatchOf(room, memory, Get.Entity_Crates, num, false, false, false, null);
            if (Rand.Chance(0.3f) && list.Count != 0)
            {
                list.RemoveAt(Rand.RangeInclusive(0, list.Count - 1));
            }
            foreach (Entity entity in list)
            {
                Structure structure = (Structure)entity;
                Item item;
                if (!memory.generatedCrateWithGold && Rand.Chance(0.2f))
                {
                    memory.generatedCrateWithGold = true;
                    memory.goldWanters.Add(structure);
                }
                else if (memory.UnusedBaseMiscItems.TryGetRandomElement<Item>(out item))
                {
                    structure.InnerEntities.Add(item);
                    memory.unusedBaseItems.Remove(item);
                }
            }
        }

        private void DoShelves(Room room, WorldGenMemory memory)
        {
            if (Get.World.GetEntitiesOfSpec(Get.Entity_Shelf).Count >= 2)
            {
                return;
            }
            if (!Rand.Chance(0.6f))
            {
                return;
            }
            Item item;
            if (!memory.UnusedBaseMiscItems.TryGetRandomElement<Item>(out item))
            {
                return;
            }
            Vector3Int vector3Int;
            if (room.FreeOnFloor.Where<Vector3Int>(delegate (Vector3Int x)
            {
                if (ItemOrStructureFallUtility.WouldHaveSupport(Get.Entity_Shelf, x, null))
                {
                    return x.AdjacentCardinalCellsXZ().Any<Vector3Int>((Vector3Int y) => Get.CellsInfo.AnyFilledImpassableAt(y));
                }
                return false;
            }).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                Structure structure = Maker.Make<Structure>(Get.Entity_Shelf, null, false, false, true);
                structure.InnerEntities.Add(item);
                structure.Spawn(vector3Int);
                memory.unusedBaseItems.Remove(item);
            }
        }
    }
}