using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_DragonRemains : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            if (memory.config.Floor < 3)
            {
                return false;
            }
            Vector3Int vector3Int;
            if (!room.FreeOnFloorNoEntranceBlocking.TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            Structure structure = Maker.Make<Structure>(Get.Entity_DragonRemains, null, false, false, true);
            Item item = ItemGenerator.SmallReward(true);
            if (Rand.Bool || item == null)
            {
                Actor actor = Maker.Make<Actor>(Get.Entity_Dragon, delegate (Actor x)
                {
                    x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                }, false, false, true);
                DifficultyUtility.AddConditionsForDifficulty(actor);
                actor.CalculateInitialHPManaAndStamina();
                if (item != null)
                {
                    actor.Inventory.Add(item, default(ValueTuple<Vector2Int?, int?, int?>));
                }
                structure.InnerEntities.Add(actor);
            }
            else if (item != null)
            {
                structure.InnerEntities.Add(item);
            }
            structure.Spawn(vector3Int);
            return true;
        }
    }
}