using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_OpenDoor : UseEffect
    {
        public bool OpenPermanently
        {
            get
            {
                return this.openPermanently;
            }
        }

        protected UseEffect_OpenDoor()
        {
        }

        public UseEffect_OpenDoor(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            ((UseEffect_OpenDoor)clone).openPermanently = this.openPermanently;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Entity usableEntity = usable as Entity;
            if (usableEntity == null || !usableEntity.Spawned)
            {
                yield break;
            }
            Actor passingActor = user ?? (target.Entity as Actor);
            if (target.IsPlayerParty && Get.LessonManager.CurrentLesson == Get.Lesson_OpeningDoors)
            {
                yield return new Instruction_Immediate(delegate
                {
                    Get.LessonManager.FinishIfCurrent(Get.Lesson_OpeningDoors);
                });
            }
            if (target.IsPlayerParty && this.openPermanently)
            {
                foreach (Instruction instruction in InstructionSets_Entity.DeSpawn(usableEntity, false))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            else if (usableEntity.Spec != Get.Entity_TemporarilyOpenedDoor)
            {
                foreach (Instruction instruction2 in InstructionSets_Entity.DeSpawn(usableEntity, false))
                {
                    yield return instruction2;
                }
                IEnumerator<Instruction> enumerator = null;
                Entity entity = Maker.Make(Get.Entity_TemporarilyOpenedDoor, delegate (Entity x)
                {
                    ((TemporarilyOpenedDoor)x).OpenedBy = passingActor;
                }, false, false, true);
                foreach (Instruction instruction3 in InstructionSets_Entity.Spawn(entity, usableEntity.Position, new Quaternion?(usableEntity.Rotation)))
                {
                    yield return instruction3;
                }
                enumerator = null;
            }
            yield break;
            yield break;
        }

        [Saved]
        private bool openPermanently;
    }
}