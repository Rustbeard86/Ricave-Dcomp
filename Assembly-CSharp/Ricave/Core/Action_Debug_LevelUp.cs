using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Action_Debug_LevelUp : Action
    {
        protected override int RandSeedPart
        {
            get
            {
                return 481730971;
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose;
            }
        }

        protected Action_Debug_LevelUp()
        {
        }

        public Action_Debug_LevelUp(ActionSpec spec)
            : base(spec)
        {
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            return true;
        }

        protected override bool CalculateConcernsPlayer()
        {
            return true;
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            int experienceToNextLevel = Get.Player.ExperienceToNextLevel;
            foreach (Instruction instruction in InstructionSets_Misc.GiveExperience(experienceToNextLevel, false, null, null))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }
    }
}