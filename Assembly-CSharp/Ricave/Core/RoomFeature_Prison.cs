using System;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_Prison : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            if (room.Height != 4)
            {
                return false;
            }
            CellCuboid ceiling = room.Shape.InnerCuboid(1).TopSurfaceCuboid;
            Vector3Int vector3Int;
            if (ceiling.Where<Vector3Int>((Vector3Int x) => ceiling.Contains(x + Vector3IntUtility.Right * 2) && ceiling.Contains(x + Vector3IntUtility.Forward * 2) && ceiling.Contains(x + Vector3IntUtility.Right * 2 + Vector3IntUtility.Forward * 2)).TryGetRandomElementWhere<Vector3Int>((Vector3Int x) => base.< TryGenerate > g__IsValidSpotForHangingCage | 0(x) && base.< TryGenerate > g__IsValidSpotForHangingCage | 0(x + Vector3IntUtility.Right * 2) && base.< TryGenerate > g__IsValidSpotForHangingCage | 0(x + Vector3IntUtility.Forward * 2) && base.< TryGenerate > g__IsValidSpotForHangingCage | 0(x + Vector3IntUtility.Right * 2 + Vector3IntUtility.Forward * 2), out vector3Int))
            {
                Structure structure = Maker.Make<Structure>(Get.Entity_HangingCage, null, false, false, true);
                Actor actor = Maker.Make<Actor>(Get.Entity_Dog, delegate (Actor x)
                {
                    x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                }, false, false, true);
                DifficultyUtility.AddConditionsForDifficulty(actor);
                actor.CalculateInitialHPManaAndStamina();
                structure.InnerEntities.Add(actor);
                structure.Spawn(vector3Int);
                Maker.Make(Get.Entity_Placeholder, null, false, false, true).Spawn(vector3Int.WithY(room.Shape.yMin + 1));
                Structure structure2 = Maker.Make<Structure>(Get.Entity_HangingCage, null, false, false, true);
                structure2.Spawn(vector3Int + Vector3IntUtility.Forward * 2);
                ItemGenerator.SmallReward(true).Spawn(structure2.Position);
                Maker.Make(Get.Entity_Placeholder, null, false, false, true).Spawn((vector3Int + Vector3IntUtility.Forward * 2).WithY(room.Shape.yMin + 1));
                Structure structure3 = Maker.Make<Structure>(Get.Entity_HangingCage, null, false, false, true);
                Actor actor2 = Maker.Make<Actor>(Get.Entity_Spider, delegate (Actor x)
                {
                    x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                }, false, false, true);
                DifficultyUtility.AddConditionsForDifficulty(actor2);
                actor2.CalculateInitialHPManaAndStamina();
                actor2.Inventory.Add(ItemGenerator.SmallReward(true), default(ValueTuple<Vector2Int?, int?, int?>));
                structure3.InnerEntities.Add(actor2);
                structure3.Spawn(vector3Int + Vector3IntUtility.Right * 2 + Vector3IntUtility.Forward * 2);
                Maker.Make(Get.Entity_Placeholder, null, false, false, true).Spawn((vector3Int + Vector3IntUtility.Right * 2 + Vector3IntUtility.Forward * 2).WithY(room.Shape.yMin + 1));
                return true;
            }
            return false;
        }
    }
}