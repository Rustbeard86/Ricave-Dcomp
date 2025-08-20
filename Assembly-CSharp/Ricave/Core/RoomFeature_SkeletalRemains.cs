using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_SkeletalRemains : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            Vector3Int vector3Int;
            if (!room.FreeOnFloorNoEntranceBlocking.TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            Item item = ItemGenerator.SmallReward(true);
            Structure structure = Maker.Make<Structure>(Get.Entity_SkeletalRemains, null, false, false, true);
            if (Rand.Bool || item == null)
            {
                Actor actor = Maker.Make<Actor>(Get.Entity_Skeleton, delegate (Actor x)
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