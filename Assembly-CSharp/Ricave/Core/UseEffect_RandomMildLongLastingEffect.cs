using System;
using System.Collections.Generic;
using Ricave.UI;

namespace Ricave.Core
{
    public class UseEffect_RandomMildLongLastingEffect : UseEffect
    {
        protected UseEffect_RandomMildLongLastingEffect()
        {
        }

        public UseEffect_RandomMildLongLastingEffect(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (user == null)
            {
                yield break;
            }
            int num;
            if (user.IsNowControlledPlayerParty)
            {
                object choice = UsePrompt.Choice;
                if (choice is bool)
                {
                    bool flag = (bool)choice;
                    if (flag && Get.Player.Gold >= 50)
                    {
                        yield return new Instruction_ChangePlayerGold(-50);
                        num = Rand.RangeInclusive(0, 3);
                        goto IL_00E9;
                    }
                }
            }
            if (user.Inventory.EquippedWeapon != null && !user.Inventory.EquippedWeapon.Cursed)
            {
                num = Rand.RangeInclusive(0, 7);
            }
            else
            {
                num = Rand.RangeInclusive(0, 6);
            }
        IL_00E9:
            Condition conditionToAdd = null;
            Condition conditionToAdd2 = null;
            Item toCurse;
            switch (num)
            {
                case 0:
                    conditionToAdd = new Condition_MaxHPOffset(Get.Condition_MaxHPOffset, 2, 350);
                    break;
                case 1:
                    conditionToAdd = new Condition_SeeRangeOffset(Get.Condition_SeeRangeOffset, 2, 350);
                    break;
                case 2:
                    conditionToAdd = new Condition_DealtDamageOffset(Get.Condition_DealtDamageOffset, 1, 1, Get.DamageType_Physical, 350);
                    break;
                case 3:
                    conditionToAdd = new Condition_MaxStaminaOffset(Get.Condition_MaxStaminaOffset, 2, 350);
                    break;
                case 4:
                    conditionToAdd = new Condition_MaxHPOffset(Get.Condition_MaxHPOffset, -2, 350);
                    break;
                case 5:
                    conditionToAdd = new Condition_MaxStaminaOffset(Get.Condition_MaxStaminaOffset, -2, 350);
                    break;
                case 6:
                    conditionToAdd = new Condition_SeeRangeOffset(Get.Condition_SeeRangeOffset, -1, 350);
                    break;
                case 7:
                    {
                        toCurse = user.Inventory.EquippedWeapon;
                        yield return new Instruction_SetCursed(toCurse, true);
                        foreach (Instruction instruction in InstructionSets_Actor.CheckEquippedCursedItem(toCurse, false))
                        {
                            yield return instruction;
                        }
                        IEnumerator<Instruction> enumerator = null;
                        if (user.IsNowControlledActor)
                        {
                            yield return new Instruction_PlayLog("ItemHasBeenCursed".Translate(RichText.Label(toCurse), RichText.Cursed("CursedLower".Translate())));
                        }
                        break;
                    }
            }
            toCurse = null;
            if (conditionToAdd != null)
            {
                foreach (Instruction instruction2 in InstructionSets_Misc.AddCondition(conditionToAdd, user.Conditions, user != null && user.IsPlayerParty, true))
                {
                    yield return instruction2;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            if (conditionToAdd2 != null)
            {
                foreach (Instruction instruction3 in InstructionSets_Misc.AddCondition(conditionToAdd2, user.Conditions, user != null && user.IsPlayerParty, true))
                {
                    yield return instruction3;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            yield break;
            yield break;
        }

        public const int GoodEffectGoldPrice = 50;
    }
}