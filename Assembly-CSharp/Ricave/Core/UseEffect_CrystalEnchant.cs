using System;
using System.Collections.Generic;
using System.Linq;
using Ricave.UI;

namespace Ricave.Core
{
    public class UseEffect_CrystalEnchant : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Good;
            }
        }

        protected UseEffect_CrystalEnchant()
        {
        }

        public UseEffect_CrystalEnchant(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override bool PreventEntireUse(Actor user, IUsable usable, Target target, StringSlot outReason = null)
        {
            Item item;
            Item item2;
            if (!this.TryFind2SameCrystals(user, usable, out item, out item2))
            {
                if (outReason != null)
                {
                    outReason.Set("Need2SameCrystals".Translate());
                }
                return true;
            }
            return false;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (user != null && user.IsNowControlledActor)
            {
                object choice = UsePrompt.Choice;
                Item item = choice as Item;
                Item crystal;
                Item crystal2;
                if (item != null && user.Inventory.Contains(item) && this.TryFind2SameCrystals(user, usable, out crystal, out crystal2))
                {
                    foreach (Instruction instruction in InstructionSets_Actor.RemoveOneFromInventory(crystal))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator = null;
                    foreach (Instruction instruction2 in InstructionSets_Actor.RemoveOneFromInventory(crystal2))
                    {
                        yield return instruction2;
                    }
                    enumerator = null;
                    if (!item.Identified)
                    {
                        foreach (Instruction instruction3 in InstructionSets_Entity.Identify(item, item.TurnsLeftToIdentify, false))
                        {
                            yield return instruction3;
                        }
                        enumerator = null;
                    }
                    if (item.Cursed)
                    {
                        yield return new Instruction_SetCursed(item, false);
                        yield return new Instruction_PlayLog("CurseRemoved".Translate(RichText.Label(item)));
                    }
                    if (crystal.Spec == Get.Entity_MinedRedCrystal)
                    {
                        foreach (Instruction instruction4 in UseEffect_CrystalEnchant.UpgradeWeapon(item, user))
                        {
                            yield return instruction4;
                        }
                        enumerator = null;
                    }
                    else if (crystal.Spec == Get.Entity_MinedGreenCrystal)
                    {
                        foreach (Instruction instruction5 in UseEffect_CrystalEnchant.UpgradeWearable(item, user))
                        {
                            yield return instruction5;
                        }
                        enumerator = null;
                    }
                    else if (crystal.Spec == Get.Entity_MinedBlueCrystal)
                    {
                        if (item.Spec.Item.IsEquippableWeapon)
                        {
                            foreach (Instruction instruction6 in UseEffect_CrystalEnchant.EnchantWeapon(item, user))
                            {
                                yield return instruction6;
                            }
                            enumerator = null;
                        }
                        else if (item.Spec.Item.IsEquippableNonWeapon)
                        {
                            foreach (Instruction instruction7 in UseEffect_CrystalEnchant.EnchantWearable(item, user))
                            {
                                yield return instruction7;
                            }
                            enumerator = null;
                        }
                    }
                    yield break;
                }
            }
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> UpgradeWeapon(Item item, Actor user)
        {
            UseEffect_Damage useEffect_Damage = (UseEffect_Damage)item.UseEffects.GetFirstOfSpec(Get.UseEffect_Damage);
            if (useEffect_Damage == null)
            {
                yield break;
            }
            Condition_DealtDamageFactor condition_DealtDamageFactor = new Condition_DealtDamageFactor(Get.Condition_DealtDamageFactor, 1.1f, useEffect_Damage.DamageType, 0);
            foreach (Instruction instruction in InstructionSets_Misc.AddCondition(condition_DealtDamageFactor, item.ConditionsEquipped, user != null && user.IsPlayerParty, true))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> UpgradeWearable(Item item, Actor user)
        {
            Condition_IncomingDamageOffset condition_IncomingDamageOffset = (Condition_IncomingDamageOffset)item.ConditionsEquipped.GetFirstOfSpec(Get.Condition_IncomingDamageOffset);
            if (condition_IncomingDamageOffset == null)
            {
                yield break;
            }
            Condition_IncomingDamageFactor condition_IncomingDamageFactor = new Condition_IncomingDamageFactor(Get.Condition_IncomingDamageFactor, 0.9f, condition_IncomingDamageOffset.DamageType, 0);
            foreach (Instruction instruction in InstructionSets_Misc.AddCondition(condition_IncomingDamageFactor, item.ConditionsEquipped, user != null && user.IsPlayerParty, true))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
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
                    useEffectToAdd = new UseEffect_AddCondition(Get.UseEffect_AddCondition, new Condition_Frozen(Get.Condition_Frozen, 2), 0.25f, UseEffect_AddCondition.StackMode.CanStack, 3);
                    title = Get.Title_Freezing;
                });
            }
            if (!item.UseEffects.All.Any<UseEffect>((UseEffect x) => x.Spec == Get.UseEffect_AddCondition && ((UseEffect_AddCondition)x).Condition is Condition_Entangled))
            {
                list.Add(delegate
                {
                    useEffectToAdd = new UseEffect_AddCondition(Get.UseEffect_AddCondition, new Condition_Entangled(Get.Condition_Entangled, 3), 0.25f, UseEffect_AddCondition.StackMode.CanStack, 3);
                    title = Get.Title_Entanglement;
                });
            }
            if (!item.UseEffects.All.Any<UseEffect>((UseEffect x) => x.Spec == Get.UseEffect_AddCondition && ((UseEffect_AddCondition)x).Condition is Condition_Drunk))
            {
                list.Add(delegate
                {
                    useEffectToAdd = new UseEffect_AddCondition(Get.UseEffect_AddCondition, new Condition_Drunk(Get.Condition_Drunk, 4), 0.25f, UseEffect_AddCondition.StackMode.CanStack, 3);
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
                useEffectToAdd = new UseEffect_AddCondition(Get.UseEffect_AddCondition, new Condition_Poisoned(Get.Condition_Poisoned, 1, 10), 0.5f, UseEffect_AddCondition.StackMode.NeverSameSpec, 3);
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
                conditionToAdd = new Condition_MaxHPOffset(Get.Condition_MaxHPOffset, 3, 0);
                title = Get.Title_Life;
            });
            list.Add(delegate
            {
                conditionToAdd = new Condition_StaminaRegen(Get.Condition_StaminaRegen, 5, 1, false);
                title = Get.Title_StaminaRegen;
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
            if (!item.ConditionsEquipped.AnyOfSpec(Get.Condition_Levitating))
            {
                list.Add(delegate
                {
                    conditionToAdd = new Condition_Levitating(Get.Condition_Levitating);
                    title = Get.Title_Levitation;
                });
            }
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

        private bool TryFind2SameCrystals(Actor user, IUsable usable, out Item crystal1, out Item crystal2)
        {
            if (user == null)
            {
                crystal1 = null;
                crystal2 = null;
                return false;
            }
            Item item = usable as Item;
            if (item != null)
            {
                crystal1 = item;
                foreach (Item item2 in user.Inventory.AllItems)
                {
                    if (item2 != crystal1 && item2.Spec == crystal1.Spec)
                    {
                        crystal2 = item2;
                        return true;
                    }
                }
                crystal2 = null;
                return false;
            }
            crystal1 = null;
            crystal2 = null;
            return false;
        }
    }
}