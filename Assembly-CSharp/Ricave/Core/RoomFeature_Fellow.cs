using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_Fellow : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            if (!Rand.ChanceSeeded(0.3f, Calc.CombineHashes<int, int>(Get.RunConfig.RunSeed, 982347561)))
            {
                return false;
            }
            int num = Calc.HashToRangeInclusive(Calc.CombineHashes<int, int>(Get.RunConfig.RunSeed, 127987456), 1, Get.RunSpec.FloorCount ?? 10);
            if (num == 5 || num == 9)
            {
                num--;
            }
            if (Get.Floor != num)
            {
                return false;
            }
            Vector3Int vector3Int;
            if (!room.FreeOnFloorNonBlocking.TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            Actor actor = Maker.Make<Actor>(Get.Entity_Fellow, delegate (Actor x)
            {
                x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
            }, false, false, true);
            DifficultyUtility.AddConditionsForDifficulty(actor);
            actor.CalculateInitialHPManaAndStamina();
            actor.Spawn(vector3Int);
            return true;
        }
    }
}