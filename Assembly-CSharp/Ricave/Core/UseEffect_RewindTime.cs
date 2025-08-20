using System;
using System.Collections.Generic;
using Ricave.UI;

namespace Ricave.Core
{
    public class UseEffect_RewindTime : UseEffect
    {
        public int RewindTurns
        {
            get
            {
                return this.rewindTurns;
            }
        }

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(RichText.Turns(StringUtility.TurnsString(this.rewindTurns)));
            }
        }

        protected UseEffect_RewindTime()
        {
        }

        public UseEffect_RewindTime(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            ((UseEffect_RewindTime)clone).rewindTurns = this.rewindTurns;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (!LastUsedToRewindTimeUtility.CanRewindTime(usable))
            {
                yield return new Instruction_PlayLog("EntityHasAlreadyBeenUsed".Translate(usable));
                yield break;
            }
            if (this.rewindTurns >= 2)
            {
                yield return new Instruction_Sound(Get.Sound_RewindTimeLong, null, 1f, 1f);
            }
            yield return new Instruction_SetLastUsedToRewindTimeSequence(usable, Get.TurnManager.CurrentSequence);
            yield return new Instruction_Immediate(delegate
            {
                Get.Sound_RewindTime.PlayOneShot(null, 1f, 1f);
            });
            yield return new Instruction_RewindTime(this.rewindTurns);
            yield break;
        }

        [Saved]
        private int rewindTurns;
    }
}