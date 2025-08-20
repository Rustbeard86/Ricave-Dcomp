using System;
using System.Collections.Generic;
using System.Linq;
using Ricave.UI;

namespace Ricave.Core
{
    public class UseEffect_EnchantChosenItem : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Good;
            }
        }

        protected UseEffect_EnchantChosenItem()
        {
        }

        public UseEffect_EnchantChosenItem(UseEffectSpec spec)
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
                Item item = choice as Item;
                if (item != null && user.Inventory.Contains(item))
                {
                    if (!item.Identified)
                    {
                        foreach (Instruction instruction in InstructionSets_Entity.Identify(item, item.TurnsLeftToIdentify, false))
                        {
                            yield return instruction;
                        }
                        IEnumerator<Instruction> enumerator = null;
                    }
                    if (item.Cursed)
                    {
                        yield return new Instruction_SetCursed(item, false);
                        yield return new Instruction_PlayLog("CurseRemoved".Translate(RichText.Label(item)));
                    }
                    if (item.Spec.Item.IsEquippableWeapon)
                    {
                        foreach (Instruction instruction2 in UseEffect_EnchantChosenItem.EnchantWeapon(item, user))
                        {
                            yield return instruction2;
                        }
                        IEnumerator<Instruction> enumerator = null;
                    }
                    else if (item.Spec.Item.IsEquippableNonWeapon)
                    {
                        foreach (Instruction instruction3 in UseEffect_EnchantChosenItem.EnchantWearable(item, user))
                        {
                            yield return instruction3;
                        }
                        IEnumerator<Instruction> enumerator = null;
                    }
                    yield break;
                }
            }
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> EnchantWeapon(Item item, Actor user)
        {
            List<Action> list = FrameLocalPool<List<Action>>.Get();
            UseEffect useEffectToAdd = null;
            TitleSpec title = null;
            if (!item.UseEffects.All.Any<UseEffect>((UseEffect x) => x.Spec == Get.UseEffect_AddCondition && ((UseEffect_AddCondition)x).Condition is Condition_Frozen))
            {
                list.Add(delegate
                {
                    useEffectToAdd = new UseEffect_AddCondition(Get.UseEffect_AddCondition, new Condition_Frozen(Get.Condition_Frozen, 2), 0.17f, UseEffect_AddCondition.StackMode.CanStack, 0);
                    title = Get.Title_Freezing;
                });
            }
            if (!item.UseEffects.All.Any<UseEffect>((UseEffect x) => x.Spec == Get.UseEffect_AddCondition && ((UseEffect_AddCondition)x).Condition is Condition_Entangled))
            {
                list.Add(delegate
                {
                    useEffectToAdd = new UseEffect_AddCondition(Get.UseEffect_AddCondition, new Condition_Entangled(Get.Condition_Entangled, 3), 0.25f, UseEffect_AddCondition.StackMode.CanStack, 0);
                    title = Get.Title_Entanglement;
                });
            }
            if (!item.UseEffects.All.Any<UseEffect>((UseEffect x) => x.Spec == Get.UseEffect_AddCondition && ((UseEffect_AddCondition)x).Condition is Condition_Drunk))
            {
                list.Add(delegate
                {
                    useEffectToAdd = new UseEffect_AddCondition(Get.UseEffect_AddCondition, new Condition_Drunk(Get.Condition_Drunk, 4), 0.17f, UseEffect_AddCondition.StackMode.CanStack, 0);
                    title = Get.Title_Drunkenness;
                });
            }
            if (list.Count != 0)
            {
                if (item.UseEffects.All.Any<UseEffect>((UseEffect x) => x.Spec == Get.UseEffect_AddCondition && ((UseEffect_AddCondition)x).Condition is Condition_Poisoned))
                {
                    goto IL_018C;
                }
            }
            list.Add(delegate
            {
                useEffectToAdd = new UseEffect_AddCondition(Get.UseEffect_AddCondition, new Condition_Poisoned(Get.Condition_Poisoned, 1, 10), 0.5f, UseEffect_AddCondition.StackMode.NeverSameSpec, 0);
                title = Get.Title_Poison;
            });
        IL_018C:
            Action action;
            list.TryGetRandomElement<Action>(out action);
            action();
            foreach (Instruction instruction in InstructionSets_Misc.AddUseEffect(useEffectToAdd, item.UseEffects, user != null && user.IsPlayerParty))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            if (item.Title == null)
            {
                yield return new Instruction_SetTitle(item, title);
            }
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> EnchantWearable(Item item, Actor user)
        {
            List<Action> list = FrameLocalPool<List<Action>>.Get();
            Condition conditionToAdd = null;
            Condition conditionToAdd2 = null;
            TitleSpec title = null;
            list.Add(delegate
            {
                conditionToAdd = new Condition_MaxHPOffset(Get.Condition_MaxHPOffset, 5, 0);
                title = Get.Title_Life;
            });
            list.Add(delegate
            {
                conditionToAdd = new Condition_HPRegen(Get.Condition_HPRegen, 20, 1, false, 0);
                title = Get.Title_HPRegen;
            });
            list.Add(delegate
            {
                conditionToAdd = new Condition_StaminaRegen(Get.Condition_StaminaRegen, 5, 1, false);
                title = Get.Title_StaminaRegen;
            });
            list.Add(delegate
            {
                conditionToAdd = new Condition_ManaRegen(Get.Condition_ManaRegen, 30, 1, false);
                title = Get.Title_ManaRegen;
            });
            if (!item.ConditionsEquipped.AnyOfSpec(Get.Condition_ImmuneToPoison))
            {
                list.Add(delegate
                {
                    conditionToAdd = new Condition_ImmuneToPoison(Get.Condition_ImmuneToPoison);
                    title = Get.Title_PoisonImmunity;
                });
            }
            if (!item.ConditionsEquipped.AnyOfSpec(Get.Condition_ImmuneToFire))
            {
                list.Add(delegate
                {
                    conditionToAdd = new Condition_ImmuneToFire(Get.Condition_ImmuneToFire);
                    title = Get.Title_FireImmunity;
                });
            }
            list.Add(delegate
            {
                conditionToAdd = new Condition_MaxManaOffset(Get.Condition_MaxManaOffset, 2, 0);
                title = Get.Title_Might;
            });
            list.Add(delegate
            {
                conditionToAdd = new Condition_MaxStaminaOffset(Get.Condition_MaxStaminaOffset, 4, 0);
                title = Get.Title_Endurance;
            });
            Action action;
            list.TryGetRandomElement<Action>(out action);
            action();
            foreach (Instruction instruction in InstructionSets_Misc.AddCondition(conditionToAdd, item.ConditionsEquipped, user != null && user.IsPlayerParty, true))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            if (conditionToAdd2 != null)
            {
                foreach (Instruction instruction2 in InstructionSets_Misc.AddCondition(conditionToAdd2, item.ConditionsEquipped, user != null && user.IsPlayerParty, true))
                {
                    yield return instruction2;
                }
                enumerator = null;
            }
            if (item.Title == null)
            {
                yield return new Instruction_SetTitle(item, title);
            }
            yield break;
            yield break;
        }
    }
}