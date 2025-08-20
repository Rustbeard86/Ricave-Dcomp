using System;
using System.Collections.Generic;
using System.Linq;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_OpenElectronicSafe : UseEffect
    {
        protected UseEffect_OpenElectronicSafe()
        {
        }

        public UseEffect_OpenElectronicSafe(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Entity safe = (from x in Get.World.GetEntitiesOfSpec(Get.Entity_ElectronicSafe)
                           orderby x.MyStableHash
                           select x).FirstOrDefault<Entity>();
            if (safe != null)
            {
                yield return new Instruction_Sound(Get.Sound_OpenSafe, new Vector3?(safe.Position), 1f, 1f);
                foreach (Instruction instruction in InstructionSets_Entity.DeSpawn(safe, true))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
                foreach (Instruction instruction2 in InstructionSets_Entity.DropLoot(safe))
                {
                    yield return instruction2;
                }
                enumerator = null;
                yield return new Instruction_PlayLog("SafeOpens".Translate(RichText.Label(Get.Entity_ElectronicSafe, false)));
            }
            yield break;
            yield break;
        }
    }
}