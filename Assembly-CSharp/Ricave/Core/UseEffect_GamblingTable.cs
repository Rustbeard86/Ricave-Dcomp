using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_GamblingTable : UseEffect
    {
        protected UseEffect_GamblingTable()
        {
        }

        public UseEffect_GamblingTable(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (user != null && user.IsNowControlledActor)
            {
                object choice = UsePrompt.Choice;
                if (choice is int)
                {
                    int num = (int)choice;
                    if (Get.Player.Gold >= num)
                    {
                        if (Rand.Bool)
                        {
                            yield return new Instruction_ChangePlayerGold(num);
                            yield return new Instruction_PlayLog("GambleWin".Translate());
                        }
                        else
                        {
                            yield return new Instruction_ChangePlayerGold(-num);
                            yield return new Instruction_PlayLog("GambleLose".Translate());
                        }
                        yield break;
                    }
                }
            }
            yield break;
        }
    }
}