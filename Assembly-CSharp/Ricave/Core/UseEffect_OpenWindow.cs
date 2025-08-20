using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_OpenWindow : UseEffect
    {
        public WindowSpec WindowSpec
        {
            get
            {
                return this.windowSpec;
            }
        }

        protected UseEffect_OpenWindow()
        {
        }

        public UseEffect_OpenWindow(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            ((UseEffect_OpenWindow)clone).windowSpec = this.windowSpec;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            yield return new Instruction_Immediate(delegate
            {
                Get.WindowManager.Open(this.windowSpec, true);
                Get.PlannedPlayerActions.Interrupt();
            });
            yield break;
        }

        [Saved]
        private WindowSpec windowSpec;
    }
}