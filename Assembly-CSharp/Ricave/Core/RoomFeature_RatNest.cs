using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_RatNest : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            Vector3Int vector3Int;
            if (!room.FreeOnFloorNonBlocking.TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            Structure structure = Maker.Make<Structure>(Get.Entity_RatNest, null, false, false, true);
            Item item = ItemGenerator.SmallReward(true);
            structure.InnerEntities.Add(item);
            Actor actor = Maker.Make<Actor>(Get.Entity_Rat, delegate (Actor x)
            {
                x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
            }, false, false, true);
            DifficultyUtility.AddConditionsForDifficulty(actor);
            actor.CalculateInitialHPManaAndStamina();
            structure.InnerEntities.Add(actor);
            structure.Spawn(vector3Int);
            return true;
        }
    }
}