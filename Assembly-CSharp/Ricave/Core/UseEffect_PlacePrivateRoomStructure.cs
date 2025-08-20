using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_PlacePrivateRoomStructure : UseEffect
    {
        protected UseEffect_PlacePrivateRoomStructure()
        {
        }

        public UseEffect_PlacePrivateRoomStructure(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            EntitySpec structureSpec = Get.Player.PrivateRoomPlaceStructureUsable.PlacingStructure;
            if (structureSpec == null)
            {
                yield break;
            }
            Structure structure = Maker.Make<Structure>(structureSpec, null, false, false, true);
            foreach (Instruction instruction in InstructionSets_Entity.Spawn(structure, target.Position, null))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield return new Instruction_Immediate(delegate
            {
                Get.PrivateRoom.OnPlaced(structureSpec, target.Position, structure);
            });
            yield return new Instruction_Sound(Get.Sound_DroppedItem, new Vector3?(target.Position), 1f, 1f);
            yield break;
            yield break;
        }
    }
}