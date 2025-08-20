using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Burrow : UseEffect
    {
        protected UseEffect_Burrow()
        {
        }

        public UseEffect_Burrow(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (user == null || !user.Spawned)
            {
                yield break;
            }
            Vector3Int vector3Int = user.Position.Below();
            if (!vector3Int.InBounds() || !Get.CellsInfo.IsFilled(vector3Int))
            {
                yield break;
            }
            if (!user.MovingAllowed || user.ConditionsAccumulated.MovingDisallowedIfCantFly)
            {
                yield break;
            }
            if (!user.HasArm)
            {
                yield break;
            }
            foreach (Instruction instruction in InstructionSets_Entity.DeSpawn(user, false))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield return new Instruction_AddToBurrowedActors(user);
            yield return new Instruction_VisualEffect(Get.VisualEffect_Burrow, user.Position);
            yield break;
            yield break;
        }
    }
}