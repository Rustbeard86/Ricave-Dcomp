using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public static class TutorialUtility
    {
        public static IEnumerable<Instruction> DeSpawnClosestGate()
        {
            Entity gate;
            if (!Get.World.GetEntitiesOfSpec(Get.Entity_Gate).TryGetMinBy<Entity, int>((Entity x) => x.Position.GetGridDistance(Get.NowControlledActor.Position), out gate))
            {
                yield break;
            }
            yield return new Instruction_Sound(Get.Sound_OpenGate, new Vector3?(gate.Position), 1f, 1f);
            foreach (Instruction instruction in InstructionSets_Entity.DeSpawn(gate, true))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield return new Instruction_Immediate(delegate
            {
                Get.LessonManager.OnTutorialGateDespawned();
            });
            yield break;
            yield break;
        }
    }
}