using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_Beggar : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            if (Get.RunInfo.GeneratedBeggar)
            {
                return false;
            }
            Vector3Int vector3Int;
            if (!room.FreeOnFloorNonBlocking.TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            Item item = Maker.Make<Item>(Get.Entity_Bone, null, false, false, true);
            Actor actor = Maker.Make<Actor>(Get.Entity_Beggar, delegate (Actor x)
            {
                x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
            }, false, false, true);
            DifficultyUtility.AddConditionsForDifficulty(actor);
            actor.CalculateInitialHPManaAndStamina();
            actor.Inventory.Add(item, default(ValueTuple<Vector2Int?, int?, int?>));
            actor.Spawn(vector3Int);
            Get.RunInfo.GeneratedBeggar = true;
            return true;
        }
    }
}