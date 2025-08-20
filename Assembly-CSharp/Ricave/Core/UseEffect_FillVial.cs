using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_FillVial : UseEffect
    {
        protected UseEffect_FillVial()
        {
        }

        public UseEffect_FillVial(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Item item = usable as Item;
            if (item == null)
            {
                yield break;
            }
            Structure fluidSource = target.Entity as Structure;
            if (fluidSource == null)
            {
                yield break;
            }
            foreach (Instruction instruction in InstructionSets_Actor.RemoveOneFromInventory(item))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            Item item2 = Maker.Make<Item>(this.GetSpecFromFluidSource(fluidSource.Spec), null, false, false, true);
            foreach (Instruction instruction2 in InstructionSets_Actor.AddToInventoryOrSpawnNear(user, item2))
            {
                yield return instruction2;
            }
            enumerator = null;
            yield return new Instruction_Sound(Get.Sound_FillVial, new Vector3?(user.Position), 1f, 1f);
            yield break;
            yield break;
        }

        private EntitySpec GetSpecFromFluidSource(EntitySpec fluidSource)
        {
            if (fluidSource == Get.Entity_ToxicWater || fluidSource == Get.Entity_PipeSmoke)
            {
                return Get.Entity_Potion_Poison;
            }
            if (fluidSource == Get.Entity_FountainOfLife)
            {
                return Get.Entity_Potion_Health;
            }
            if (fluidSource == Get.Entity_AntidoteBasin)
            {
                return Get.Entity_Potion_Antidote;
            }
            if (fluidSource == Get.Entity_BeerBarrel)
            {
                return Get.Entity_Potion_Beer;
            }
            return Get.Entity_VialWithWater;
        }
    }
}