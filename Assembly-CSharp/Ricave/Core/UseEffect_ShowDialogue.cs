using System;
using System.Collections.Generic;
using Ricave.UI;

namespace Ricave.Core
{
    public class UseEffect_ShowDialogue : UseEffect
    {
        public DialogueSpec DialogueSpec
        {
            get
            {
                return this.dialogueSpec;
            }
        }

        protected UseEffect_ShowDialogue()
        {
        }

        public UseEffect_ShowDialogue(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            ((UseEffect_ShowDialogue)clone).dialogueSpec = this.dialogueSpec;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (this.dialogueSpec.Ended)
            {
                yield break;
            }
            yield return new Instruction_Immediate(delegate
            {
                ((Window_SingleDialogue)Get.WindowManager.Open(Get.Window_SingleDialogue, true)).Init(this.dialogueSpec);
                Get.PlannedPlayerActions.Interrupt();
            });
            yield break;
        }

        [Saved]
        private DialogueSpec dialogueSpec;
    }
}