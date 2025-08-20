using System;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_Pit : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            World world = Get.World;
            Vector3Int vector3Int;
            int num;
            int num2;
            Func<Vector3Int, Vector3Int> func;
            RoomPart_TrapsPit.GetDirectionalDimensions(room, out vector3Int, out num, out num2, out func);
            CellCuboid cellCuboid = new CellCuboid(1, 0, 1, num - 2, 1, num2 - 4);
            CellCuboid cellCuboid2 = new CellCuboid(1, 0, num2 - 3, num - 2, 1, 2);
            Vector3Int vector3Int2 = new Vector3Int(num / 2, 1, num2 - 2);
            foreach (Vector3Int vector3Int3 in cellCuboid.Select<Vector3Int, Vector3Int>(func))
            {
                Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int3);
            }
            foreach (Vector3Int vector3Int4 in cellCuboid2.Select<Vector3Int, Vector3Int>(func))
            {
                Maker.Make(Get.Entity_DescendTrigger, null, false, false, true).Spawn(vector3Int4);
                Maker.Make(Get.Entity_VoidSoundSource, null, false, false, true).Spawn(vector3Int4);
            }
            Vector3Int vector3Int5 = func(vector3Int2);
            Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int5.Below());
            Structure structure = Maker.Make<Structure>(Get.Entity_Chest, null, false, false, true);
            structure.InnerEntities.Add(ItemGenerator.Reward(false));
            structure.Spawn(vector3Int5, -vector3Int);
        }
    }
}