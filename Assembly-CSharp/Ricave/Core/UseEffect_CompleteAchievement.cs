using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_CompleteAchievement : UseEffect
    {
        public AchievementSpec Achievement
        {
            get
            {
                return this.achievement;
            }
        }

        protected UseEffect_CompleteAchievement()
        {
        }

        public UseEffect_CompleteAchievement(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            ((UseEffect_CompleteAchievement)clone).achievement = this.achievement;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (this.achievement != null && !this.achievement.IsCompleted)
            {
                yield return new Instruction_CompleteAchievement(this.achievement);
            }
            yield break;
        }

        [Saved]
        private AchievementSpec achievement;
    }
}