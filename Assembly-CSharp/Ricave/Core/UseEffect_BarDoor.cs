using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_BarDoor : UseEffect
    {
        protected UseEffect_BarDoor()
        {
        }

        public UseEffect_BarDoor(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Entity entity = target.Entity;
            Structure door = entity as Structure;
            if (door == null || door.Spec != Get.Entity_Door || !door.Spawned)
            {
                yield break;
            }
            Entity entity2 = Maker.Make(Get.Entity_BarredDoor, null, false, false, true);
            foreach (Instruction instruction in InstructionSets_Entity.Spawn(entity2, door.Position, new Quaternion?(door.Rotation)))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            foreach (Instruction instruction2 in InstructionSets_Entity.DeSpawn(door, false))
            {
                yield return instruction2;
            }
            enumerator = null;
            yield break;
            yield break;
        }
    }
}