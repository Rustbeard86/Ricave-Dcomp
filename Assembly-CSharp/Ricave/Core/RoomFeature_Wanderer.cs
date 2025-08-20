using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_Wanderer : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            Vector3Int vector3Int;
            if (!room.FreeOnFloorNonBlocking.TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            Actor actor = Maker.Make<Actor>(Get.Entity_Wanderer, delegate (Actor x)
            {
                x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
            }, false, false, true);
            DifficultyUtility.AddConditionsForDifficulty(actor);
            actor.CalculateInitialHPManaAndStamina();
            actor.Inventory.Add(Maker.Make<Item>(Get.Entity_Bandage, null, false, false, true), default(ValueTuple<Vector2Int?, int?, int?>));
            actor.Spawn(vector3Int);
            return true;
        }
    }
}