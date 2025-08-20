using System;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_Grave : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            World world = Get.World;
            Vector3Int vector3Int;
            if (!room.FreeOnFloorNonBlocking.Where<Vector3Int>((Vector3Int x) => world.CellsInfo.AnyFilledImpassableAt(x.Below())).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            Structure structure = Maker.Make<Structure>(Get.Entity_Grave, null, false, false, true);
            RoomFeature_Grave.PopulateGrave(structure);
            structure.Spawn(vector3Int);
            return true;
        }

        public static void PopulateGrave(Structure grave)
        {
            Item item = ItemGenerator.SmallReward(true);
            if (Rand.Chance(0.8f))
            {
                Actor actor = Maker.Make<Actor>(Get.Entity_Mummy, delegate (Actor x)
                {
                    x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                }, false, false, true);
                DifficultyUtility.AddConditionsForDifficulty(actor);
                actor.CalculateInitialHPManaAndStamina();
                actor.Inventory.Add(item, default(ValueTuple<Vector2Int?, int?, int?>));
                grave.InnerEntities.Add(actor);
                return;
            }
            grave.InnerEntities.Add(item);
        }
    }
}