using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Visual_CannonTriggered : UseEffect
    {
        public override bool AoEHandledManually
        {
            get
            {
                return true;
            }
        }

        protected UseEffect_Visual_CannonTriggered()
        {
        }

        public UseEffect_Visual_CannonTriggered(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Entity usableEntity = usable as Entity;
            if (usableEntity != null)
            {
                UseEffect_Visual_CannonTriggered.tmpAoEArea.Clear();
                AoEUtility.GetAoEArea(target.Position, this, usable, UseEffect_Visual_CannonTriggered.tmpAoEArea, null);
                foreach (Vector3Int vector3Int in UseEffect_Visual_CannonTriggered.tmpAoEArea)
                {
                    if (!Get.CellsInfo.AnyFilledCantSeeThroughAt(vector3Int))
                    {
                        yield return new Instruction_VisualEffect(Get.VisualEffect_CellAffectedByCannon, vector3Int, usableEntity.Rotation);
                    }
                }
                List<Vector3Int>.Enumerator enumerator = default(List<Vector3Int>.Enumerator);
            }
            yield break;
            yield break;
        }

        private static List<Vector3Int> tmpAoEArea = new List<Vector3Int>();
    }
}