using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class SkillManager
    {
        public List<SkillSpec> UnlockedSkills
        {
            get
            {
                return this.unlockedSkills;
            }
        }

        public bool IsUnlocked(SkillSpec skillSpec)
        {
            return skillSpec != null && this.unlockedSkills.Contains(skillSpec);
        }

        public void Unlock(SkillSpec skillSpec)
        {
            Instruction.ThrowIfNotExecuting();
            if (skillSpec == null)
            {
                Log.Error("Tried to unlock null SkillSpec.", false);
                return;
            }
            if (this.IsUnlocked(skillSpec))
            {
                Log.Error("Tried to unlock the same SkillSpec twice.", false);
                return;
            }
            this.unlockedSkills.Add(skillSpec);
            this.timeUnlocked[skillSpec] = Clock.UnscaledTime;
            if ((skillSpec == Get.Skill_FasterHealing || skillSpec == Get.Skill_ShieldStamina || skillSpec == Get.Skill_FasterStaminaRegen) && !Get.Achievement_ReachSkillTreeTop.IsCompleted)
            {
                Get.Achievement_ReachSkillTreeTop.TryComplete();
            }
        }

        public void UndoUnlock(SkillSpec skillSpec)
        {
            Instruction.ThrowIfNotExecuting();
            if (skillSpec == null)
            {
                Log.Error("Tried to undo unlock null SkillSpec.", false);
                return;
            }
            if (!this.IsUnlocked(skillSpec))
            {
                Log.Error("Tried to undo unlock non-unlocked SkillSpec.", false);
                return;
            }
            this.unlockedSkills.Remove(skillSpec);
            this.timeUnlocked.Remove(skillSpec);
        }

        public float GetTimeUnlocked(SkillSpec skillSpec)
        {
            if (skillSpec == null)
            {
                return -99999f;
            }
            return this.timeUnlocked.GetOrDefault(skillSpec, -99999f);
        }

        [Saved(Default.New, true)]
        private List<SkillSpec> unlockedSkills = new List<SkillSpec>();

        private Dictionary<SkillSpec, float> timeUnlocked = new Dictionary<SkillSpec, float>();
    }
}