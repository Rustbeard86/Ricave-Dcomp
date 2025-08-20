using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_Reward : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            int num = Rand.RangeInclusive(0, 2);
            if (num == 0)
            {
                Vector3Int vector3Int;
                if (room.FreeOnFloorNonBlocking.TryGetRandomElement<Vector3Int>(out vector3Int))
                {
                    Maker.Make(Get.Entity_Shrine, null, false, false, true).Spawn(vector3Int);
                }
            }
            else if (num == 1)
            {
                Vector3Int vector3Int2;
                if (room.FreeOnFloorNonBlocking.TryGetRandomElement<Vector3Int>(out vector3Int2))
                {
                    Maker.Make(Get.Entity_FountainOfLife, null, false, false, true).Spawn(vector3Int2);
                }
                Vector3Int vector3Int3;
                if (room.FreeOnFloorNoEntranceBlocking.TryGetRandomElement<Vector3Int>(out vector3Int3) || room.FreeOnFloor.TryGetRandomElement<Vector3Int>(out vector3Int3))
                {
                    ItemGenerator.Potion(false).Spawn(vector3Int3);
                }
            }
            else
            {
                int num2 = 0;
                Vector3Int vector3Int4;
                while (num2 < 2 && (room.FreeOnFloorNoEntranceBlocking.TryGetRandomElement<Vector3Int>(out vector3Int4) || room.FreeOnFloor.TryGetRandomElement<Vector3Int>(out vector3Int4)))
                {
                    ItemGenerator.Scroll(false).Spawn(vector3Int4);
                    num2++;
                }
                Vector3Int vector3Int5;
                if (room.FreeOnFloorNoEntranceBlocking.TryGetRandomElement<Vector3Int>(out vector3Int5) || room.FreeOnFloor.TryGetRandomElement<Vector3Int>(out vector3Int5))
                {
                    ItemGenerator.Gold(50).Spawn(vector3Int5);
                }
            }
            Vector3Int vector3Int6;
            if (Rand.Chance(0.5f) && room.FreeOnFloor.TryGetRandomElement<Vector3Int>(out vector3Int6))
            {
                Actor actor = Maker.Make<Actor>(Get.Entity_Gorhorn, delegate (Actor x)
                {
                    x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                }, false, false, true);
                DifficultyUtility.AddConditionsForDifficulty(actor);
                actor.CalculateInitialHPManaAndStamina();
                actor.Conditions.AddCondition(new Condition_Sleeping(Get.Condition_Sleeping), -1);
                actor.Spawn(vector3Int6);
            }
        }
    }
}