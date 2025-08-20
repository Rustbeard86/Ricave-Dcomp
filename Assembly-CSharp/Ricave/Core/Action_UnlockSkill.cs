using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Action_UnlockSkill : Action
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
                return Calc.CombineHashes<int, int>(this.skill.MyStableHash, 1612197854);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(Get.MainActor, this.skill);
            }
        }

        protected Action_UnlockSkill()
        {
        }

        public Action_UnlockSkill(ActionSpec spec, SkillSpec skill)
            : base(spec)
        {
            this.skill = skill;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            if (!ignoreActorState)
            {
                if (!Get.MainActor.Spawned)
                {
                    return false;
                }
                if (Get.Player.SkillPoints <= 0)
                {
                    return false;
                }
            }
            return !Get.SkillManager.IsUnlocked(this.skill) && this.skill.PrerequisiteUnlocked;
        }

        protected override bool CalculateConcernsPlayer()
        {
            return true;
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            foreach (Instruction instruction in InstructionSets_Actor.UnlockSkill(this.skill))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield return new Instruction_ChangePlayerSkillPoints(-1);
            yield return new Instruction_Sound(Get.Sound_UnlockSkill, null, 1f, 1f);
            yield break;
            yield break;
        }

        [Saved]
        private SkillSpec skill;
    }
}