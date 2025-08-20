using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Action_RefundSkill : Action
    {
        public SkillSpec Skill
        {
            get
            {
                return this.skill;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, int>(this.skill.MyStableHash, 81214574);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(Get.MainActor, this.skill);
            }
        }

        protected Action_RefundSkill()
        {
        }

        public Action_RefundSkill(ActionSpec spec, SkillSpec skill)
            : base(spec)
        {
            this.skill = skill;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            return (ignoreActorState || Get.MainActor.Spawned) && Get.SkillManager.IsUnlocked(this.skill) && !Get.SkillManager.UnlockedSkills.Any<SkillSpec>((SkillSpec x) => x != this.skill && x.Prerequisites.Contains(this.skill) && !x.Prerequisites.Any<SkillSpec>((SkillSpec y) => y != this.skill && y.IsUnlocked()));
        }

        protected override bool CalculateConcernsPlayer()
        {
            return true;
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            yield return new Instruction_ChangePlayerSkillPoints(1);
            yield return new Instruction_Sound(Get.Sound_UnlockSkill, null, 1f, 1f);
            yield break;
        }

        [Saved]
        private SkillSpec skill;
    }
}