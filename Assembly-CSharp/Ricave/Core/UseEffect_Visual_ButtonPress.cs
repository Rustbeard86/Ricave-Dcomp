using System;
using System.Collections.Generic;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Visual_ButtonPress : UseEffect
    {
        protected UseEffect_Visual_ButtonPress()
        {
        }

        public UseEffect_Visual_ButtonPress(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Structure button = usable as Structure;
            if (button == null)
            {
                yield break;
            }
            yield return new Instruction_Sound(Get.Sound_ButtonPress, new Vector3?(button.Position), 1f, 1f);
            yield return new Instruction_Immediate(delegate
            {
                GameObject gameObject = button.GameObject;
                if (gameObject == null || !gameObject.activeInHierarchy)
                {
                    return;
                }
                VisualEffectsManager visualEffectsManager = Get.VisualEffectsManager;
                VisualEffectSpec visualEffect_ButtonPressAnimation = Get.VisualEffect_ButtonPressAnimation;
                Vector3 position = gameObject.transform.position;
                Quaternion rotation = gameObject.transform.rotation;
                Vector3 localScale = gameObject.transform.localScale;
                EntitySpec spec = button.Spec;
                visualEffectsManager.DoOneShot(visualEffect_ButtonPressAnimation, position, rotation, localScale, null, null, spec);
            });
            yield break;
        }
    }
}