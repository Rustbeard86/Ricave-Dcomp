using System;
using System.Collections.Generic;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Visual_OpenChest : UseEffect
    {
        protected UseEffect_Visual_OpenChest()
        {
        }

        public UseEffect_Visual_OpenChest(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            ((UseEffect_Visual_OpenChest)clone).playSound = this.playSound;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Structure chest = usable as Structure;
            if (chest == null)
            {
                yield break;
            }
            if (this.playSound)
            {
                yield return new Instruction_Sound(Get.Sound_OpenChest, new Vector3?(chest.Position), 1f, 1f);
            }
            yield return UseEffect_Visual_OpenChest.OpenChestAnimationInstruction(chest);
            yield break;
        }

        public static Instruction_Immediate OpenChestAnimationInstruction(Structure chest)
        {
            return new Instruction_Immediate(delegate
            {
                GameObject gameObject = chest.GameObject;
                if (gameObject == null || !gameObject.activeInHierarchy)
                {
                    return;
                }
                VisualEffectsManager visualEffectsManager = Get.VisualEffectsManager;
                VisualEffectSpec visualEffect_OpenChestAnimation = Get.VisualEffect_OpenChestAnimation;
                Vector3 position = gameObject.transform.position;
                Quaternion rotation = gameObject.transform.rotation;
                Vector3 localScale = gameObject.transform.localScale;
                EntitySpec spec = chest.Spec;
                visualEffectsManager.DoOneShot(visualEffect_OpenChestAnimation, position, rotation, localScale, null, spec, null);
            });
        }

        [Saved(true, false)]
        private bool playSound = true;
    }
}