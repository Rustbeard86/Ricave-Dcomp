using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_PlayLog : UseEffect
    {
        public string Text
        {
            get
            {
                return this.text;
            }
        }

        protected UseEffect_PlayLog()
        {
        }

        public UseEffect_PlayLog(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            ((UseEffect_PlayLog)clone).text = this.text;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (ActionUtility.TargetConcernsPlayer(user) || ActionUtility.UsableConcernsPlayer(usable) || ActionUtility.TargetConcernsPlayer(target))
            {
                yield return new Instruction_PlayLog(this.text);
            }
            yield break;
        }

        [Saved]
        [Translatable]
        private string text;
    }
}