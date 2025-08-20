using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Visual_CeilingSpikesTriggered : UseEffect
    {
        public override bool AoEHandledManually
        {
            get
            {
                return true;
            }
        }

        protected UseEffect_Visual_CeilingSpikesTriggered()
        {
        }

        public UseEffect_Visual_CeilingSpikesTriggered(UseEffectSpec spec)
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
                UseEffect_Visual_CeilingSpikesTriggered.tmpAoEArea.Clear();
                AoEUtility.GetAoEArea(target.Position, this, usable, UseEffect_Visual_CeilingSpikesTriggered.tmpAoEArea, null);
                Vector3Int animationDestination = UseEffect_Visual_CeilingSpikesTriggered.tmpAoEArea.OrderByDescending<Vector3Int, int>((Vector3Int x) => x.GetGridDistance(usableEntity.Position)).First<Vector3Int>();
                Vector3Int dir = (animationDestination - usableEntity.Position).NormalizedToDir();
                animationDestination -= dir;
                yield return new Instruction_Immediate(delegate
                {
                    Get.VisualEffectsManager.DoOneShot(Get.VisualEffect_CeilingSpikesAnimation, target.Position, Quaternion.identity, Vector3.one, new Vector3?(animationDestination + dir * 0.6f), null, null);
                });
            }
            yield break;
        }

        private static List<Vector3Int> tmpAoEArea = new List<Vector3Int>();
    }
}