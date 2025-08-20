using System;
using UnityEngine;

namespace Ricave.Core
{
    public static class DamageUtility
    {
        public static int ApplyDamageProtectionAndClamp(Entity target, int damage, DamageTypeSpec damageType)
        {
            int num = damage;
            int num2 = damage;
            DamageUtility.ApplyDamageProtectionAndClamp(target, null, damageType, ref damage, ref num, ref num2);
            return damage;
        }

        public static void ApplyDamageProtectionAndClamp(Entity target, BodyPart targetBodyPart, DamageTypeSpec damageType, ref int damage, ref int minPossible, ref int maxPossible)
        {
            Actor actor = target as Actor;
            if (actor != null)
            {
                foreach (Condition condition in actor.ConditionsAccumulated.AllConditions)
                {
                    ValueTuple<IntRange, DamageTypeSpec> incomingDamageOffset = condition.IncomingDamageOffset;
                    IntRange item = incomingDamageOffset.Item1;
                    if (incomingDamageOffset.Item2 == damageType)
                    {
                        damage += item.RandomInRange;
                        minPossible += item.from;
                        maxPossible += item.to;
                    }
                }
            }
            float num = 1f;
            if (actor != null)
            {
                if ((damageType == Get.DamageType_Fire && actor.ImmuneToFire) || (damageType == Get.DamageType_Poison && actor.ImmuneToPoison))
                {
                    num = 0f;
                }
                foreach (Condition condition2 in actor.ConditionsAccumulated.AllConditions)
                {
                    ValueTuple<float, DamageTypeSpec> incomingDamageFactor = condition2.IncomingDamageFactor;
                    float item2 = incomingDamageFactor.Item1;
                    if (incomingDamageFactor.Item2 == damageType)
                    {
                        num *= item2;
                    }
                }
                if (actor.IsMainActor && damageType == Get.DamageType_Explosion && Get.Skill_ExplosionProtection.IsUnlocked())
                {
                    num *= 0.2f;
                }
                if (actor.IsMainActor && damageType == Get.DamageType_Fall && Get.Skill_NoFallDamage.IsUnlocked())
                {
                    num = 0f;
                }
                if (actor.Spec.Actor.DisableFallDamage && damageType == Get.DamageType_Fall)
                {
                    num = 0f;
                }
                if (actor.IsMainActor && damageType == Get.DamageType_Magic && Get.Skill_IncreasedMaxMana.IsUnlocked())
                {
                    num *= 0.5f;
                }
                if (actor.IsMainActor)
                {
                    if (damageType == Get.DamageType_Poison)
                    {
                        num *= Get.TraitManager.PoisonIncomingDamageFactor;
                    }
                    else if (damageType == Get.DamageType_Fall)
                    {
                        num *= Get.TraitManager.FallIncomingDamageFactor;
                    }
                    else if (damageType == Get.DamageType_Fire)
                    {
                        num *= Get.TraitManager.FireIncomingDamageFactor;
                    }
                    else if (damageType == Get.DamageType_Magic)
                    {
                        num *= Get.TraitManager.MagicIncomingDamageFactor;
                    }
                }
                if (actor.IsMainActor)
                {
                    if (damageType == Get.DamageType_Poison)
                    {
                        ClassSpec @class = Get.Player.Class;
                        if (@class != null && @class.ImmuneToPoison)
                        {
                            num = 0f;
                            goto IL_0237;
                        }
                    }
                    if (damageType == Get.DamageType_Fire)
                    {
                        float num2 = num;
                        ClassSpec class2 = Get.Player.Class;
                        num = num2 * ((class2 != null) ? class2.FireIncomingDamageFactor : 1f);
                    }
                }
            }
        IL_0237:
            damage = Rand.ProbabilisticRound((float)damage * num);
            minPossible = Calc.FloorToInt((float)minPossible * num);
            maxPossible = Calc.CeilToInt((float)maxPossible * num);
            damage = Calc.Clamp(damage, 0, (targetBodyPart != null) ? targetBodyPart.HP : target.HP);
            minPossible = Math.Max(minPossible, 0);
            maxPossible = Math.Max(maxPossible, 0);
        }

        public static Vector3Int? DeduceImpactSource(Actor user, Target target, Target originalTarget)
        {
            if (target != originalTarget)
            {
                return new Vector3Int?(originalTarget.Position);
            }
            if (user != null)
            {
                return new Vector3Int?(user.Position);
            }
            return null;
        }
    }
}