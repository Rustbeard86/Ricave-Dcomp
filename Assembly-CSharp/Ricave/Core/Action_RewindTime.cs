using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Action_RewindTime : Action
    {
        public int Turns
        {
            get
            {
                return this.turns;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, int>(this.turns, 386350142);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.turns);
            }
        }

        protected Action_RewindTime()
        {
        }

        public Action_RewindTime(ActionSpec spec, int turns)
            : base(spec)
        {
            this.turns = turns;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            if (!ignoreActorState)
            {
                if (!Get.Player.HasWatch)
                {
                    return false;
                }
                if (this.turns > Get.Player.TurnsCanRewind)
                {
                    return false;
                }
            }
            return true;
        }

        protected override bool CalculateConcernsPlayer()
        {
            return true;
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            yield return new Instruction_Immediate(delegate
            {
                Get.Sound_RewindTime.PlayOneShot(null, 1f, 1f);
            });
            yield return new Instruction_ChangeTurnsCanRewind(-this.turns);
            yield return new Instruction_Immediate(delegate
            {
                Get.RewindUI.StartRewindAnimation();
            });
            yield return new Instruction_RewindTime(this.turns);
            yield return new Instruction_Immediate(delegate
            {
                Get.Player.OnUsedWatch();
            });
            yield break;
        }

        [Saved]
        private int turns;
    }
}