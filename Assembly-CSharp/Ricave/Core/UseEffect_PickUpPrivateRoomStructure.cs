using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_PickUpPrivateRoomStructure : UseEffect
    {
        protected UseEffect_PickUpPrivateRoomStructure()
        {
        }

        public UseEffect_PickUpPrivateRoomStructure(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Entity entity = target.Entity;
            Structure structure = entity as Structure;
            if (structure == null)
            {
                yield break;
            }
            yield return new Instruction_Immediate(delegate
            {
                Get.PrivateRoom.OnMovedToInventory(structure);
            });
            foreach (Instruction instruction in InstructionSets_Entity.DeSpawn(structure, false))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield return new Instruction_Sound(Get.Sound_PickUpItem, null, 1f, 1f);
            foreach (Instruction instruction2 in Get.PrivateRoom.RevalidateAllPlacedStructuresAndSyncWithWorld())
            {
                yield return instruction2;
            }
            enumerator = null;
            yield break;
            yield break;
        }
    }
}