using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_MimicChests : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            Vector3Int vector3Int;
            if (memory.generatedMimicChest || !Rand.ChanceSeeded(0.7f, Calc.CombineHashes<int, int>(memory.config.WorldSeed, 913254774)) || !Rand.Chance(0.3f) || !room.FreeOnFloorNonBlocking.TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return;
            }
            Actor actor = Maker.Make<Actor>(Get.Entity_Mimic, delegate (Actor x)
            {
                x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
            }, false, false, true);
            DifficultyUtility.AddConditionsForDifficulty(actor);
            actor.CalculateInitialHPManaAndStamina();
            Structure structure = Maker.Make<Structure>(Get.Entity_Chest, null, false, false, true);
            structure.InnerEntities.Add(actor);
            structure.Spawn(vector3Int);
            memory.generatedMimicChest = true;
        }
    }
}