using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public static class RampUpUtility
    {
        public static float GetDesiredRampUpOnFloorFloat(int floor)
        {
            int num = floor - 1;
            if (num < 0)
            {
                return 0f;
            }
            if (num >= RampUpUtility.RampUpOnFloor.Length)
            {
                num = RampUpUtility.RampUpOnFloor.Length - 1;
            }
            return RampUpUtility.RampUpOnFloor[num] * Get.Difficulty.RampUpMultiplier * Get.RunSpec.RampUpMultiplier;
        }

        public static int GetDesiredRampUpOnFloor(int floor)
        {
            return Calc.RoundToIntHalfUp(RampUpUtility.GetDesiredRampUpOnFloorFloat(floor));
        }

        public static int GetDesiredRampUpOnFloorWithRandomVariation(int floor)
        {
            return Rand.ProbabilisticRound(RampUpUtility.GetDesiredRampUpOnFloorFloat(floor));
        }

        public static int GenerateRandomRampUpFor(Actor actor, bool allowRandomVariation = true)
        {
            if (!RampUpUtility.AffectedByRampUp(actor))
            {
                return 0;
            }
            return RampUpUtility.ResolveNonRepeatedRampUpFor(actor, allowRandomVariation ? RampUpUtility.GetDesiredRampUpOnFloorWithRandomVariation(Get.Floor) : RampUpUtility.GetDesiredRampUpOnFloor(Get.Floor));
        }

        public static int GenerateRandomRampUpFor(Item item, bool allowRandomVariation = true)
        {
            if (!RampUpUtility.AffectedByRampUp(item))
            {
                return 0;
            }
            return RampUpUtility.ResolveNonRepeatedRampUpFor(item, allowRandomVariation ? RampUpUtility.GetDesiredRampUpOnFloorWithRandomVariation(Get.Floor) : RampUpUtility.GetDesiredRampUpOnFloor(Get.Floor));
        }

        public static bool AffectedByRampUp(Actor actor)
        {
            if (actor.Spec.Actor.MaxHPRampUpFactor != 1f)
            {
                return true;
            }
            using (List<NativeWeapon>.Enumerator enumerator = actor.NativeWeapons.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (RampUpUtility.AffectedByRampUp(enumerator.Current.UseEffects))
                    {
                        return true;
                    }
                }
            }
            using (List<Spell>.Enumerator enumerator2 = actor.Spells.All.GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    if (RampUpUtility.AffectedByRampUp(enumerator2.Current.UseEffects))
                    {
                        return true;
                    }
                }
            }
            return RampUpUtility.AffectedByRampUp(actor.Conditions);
        }

        public static bool AffectedByRampUp(Item item)
        {
            return RampUpUtility.AffectedByRampUp(item.UseEffects) || RampUpUtility.AffectedByRampUp(item.ConditionsEquipped);
        }

        private static bool AffectedByRampUp(UseEffects useEffects)
        {
            using (List<UseEffect>.Enumerator enumerator = useEffects.All.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (RampUpUtility.AffectedByRampUp(enumerator.Current))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool AffectedByRampUp(UseEffect useEffect)
        {
            UseEffect_Damage useEffect_Damage = useEffect as UseEffect_Damage;
            return useEffect_Damage != null && useEffect_Damage.DamageRampUpFactor != 1f;
        }

        private static bool AffectedByRampUp(Conditions conditions)
        {
            using (List<Condition>.Enumerator enumerator = conditions.All.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (RampUpUtility.AffectedByRampUp(enumerator.Current))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool AffectedByRampUp(Condition condition)
        {
            Condition_IncomingDamageOffset condition_IncomingDamageOffset = condition as Condition_IncomingDamageOffset;
            if (condition_IncomingDamageOffset != null && condition_IncomingDamageOffset.OffsetRampUpFactor != 1f)
            {
                return true;
            }
            Condition_HPRegen condition_HPRegen = condition as Condition_HPRegen;
            if (condition_HPRegen != null && condition_HPRegen.RampUpFactor != 1f)
            {
                return true;
            }
            Condition_ManaRegen condition_ManaRegen = condition as Condition_ManaRegen;
            if (condition_ManaRegen != null && condition_ManaRegen.RampUpFactor != 1f)
            {
                return true;
            }
            Condition_StaminaRegen condition_StaminaRegen = condition as Condition_StaminaRegen;
            if (condition_StaminaRegen != null && condition_StaminaRegen.RampUpFactor != 1f)
            {
                return true;
            }
            Condition_DealtDamageOffset condition_DealtDamageOffset = condition as Condition_DealtDamageOffset;
            if (condition_DealtDamageOffset != null && condition_DealtDamageOffset.OffsetRampUpFactor != 1f)
            {
                return true;
            }
            Condition_MaxHPOffset condition_MaxHPOffset = condition as Condition_MaxHPOffset;
            return condition_MaxHPOffset != null && condition_MaxHPOffset.OffsetRampUpFactor != 1f;
        }

        public static int GetOffsetFromRampUp(int baseValue, int rampUp, float rampUpFactor)
        {
            return RampUpUtility.ApplyRampUpToInt(baseValue, rampUp, rampUpFactor) - baseValue;
        }

        public static int ResolveNonRepeatedRampUpFor(Actor actor, int rampUp)
        {
            if (rampUp == 0)
            {
                return 0;
            }
            if (!RampUpUtility.AffectedByRampUp(actor))
            {
                return 0;
            }
            int num = rampUp;
            while (num != 0)
            {
                if (RampUpUtility.IntRampUpDifferentFromPrevious(actor.Spec.MaxHP, num, actor.Spec.Actor.MaxHPRampUpFactor))
                {
                    return num;
                }
                using (List<NativeWeapon>.Enumerator enumerator = actor.NativeWeapons.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (RampUpUtility.RampUpDifferentFromPrevious(enumerator.Current.UseEffects, num))
                        {
                            return num;
                        }
                    }
                }
                using (List<Spell>.Enumerator enumerator2 = actor.Spells.All.GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        if (RampUpUtility.RampUpDifferentFromPrevious(enumerator2.Current.UseEffects, num))
                        {
                            return num;
                        }
                    }
                }
                if (RampUpUtility.RampUpDifferentFromPrevious(actor.Conditions, num))
                {
                    return num;
                }
                num += ((num > 0) ? (-1) : 1);
                continue;
            }
            return 0;
        }

        public static int ResolveNonRepeatedRampUpFor(Item item, int rampUp)
        {
            if (rampUp == 0)
            {
                return 0;
            }
            if (!RampUpUtility.AffectedByRampUp(item))
            {
                return 0;
            }
            for (int num = rampUp; num != 0; num += ((num > 0) ? (-1) : 1))
            {
                if (RampUpUtility.RampUpDifferentFromPrevious(item.UseEffects, num))
                {
                    return num;
                }
                if (RampUpUtility.RampUpDifferentFromPrevious(item.ConditionsEquipped, num))
                {
                    return num;
                }
            }
            return 0;
        }

        private static bool RampUpDifferentFromPrevious(UseEffects useEffects, int rampUp)
        {
            using (List<UseEffect>.Enumerator enumerator = useEffects.All.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (RampUpUtility.RampUpDifferentFromPrevious(enumerator.Current, rampUp))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool RampUpDifferentFromPrevious(UseEffect useEffect, int rampUp)
        {
            UseEffect_Damage useEffect_Damage = useEffect as UseEffect_Damage;
            return useEffect_Damage != null && (RampUpUtility.IntRampUpDifferentFromPrevious(useEffect_Damage.BaseFrom, rampUp, useEffect_Damage.DamageRampUpFactor) || RampUpUtility.IntRampUpDifferentFromPrevious(useEffect_Damage.BaseTo, rampUp, useEffect_Damage.DamageRampUpFactor));
        }

        private static bool RampUpDifferentFromPrevious(Conditions conditions, int rampUp)
        {
            using (List<Condition>.Enumerator enumerator = conditions.All.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (RampUpUtility.RampUpDifferentFromPrevious(enumerator.Current, rampUp))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool RampUpDifferentFromPrevious(Condition condition, int rampUp)
        {
            Condition_IncomingDamageOffset condition_IncomingDamageOffset = condition as Condition_IncomingDamageOffset;
            if (condition_IncomingDamageOffset != null && (RampUpUtility.IntRampUpDifferentFromPrevious(condition_IncomingDamageOffset.BaseOffsetFrom, rampUp, condition_IncomingDamageOffset.OffsetRampUpFactor) || RampUpUtility.IntRampUpDifferentFromPrevious(condition_IncomingDamageOffset.BaseOffsetTo, rampUp, condition_IncomingDamageOffset.OffsetRampUpFactor)))
            {
                return true;
            }
            Condition_HPRegen condition_HPRegen = condition as Condition_HPRegen;
            if (condition_HPRegen != null && RampUpUtility.TimesPerIntervalRampUpDifferentFromPrevious(condition_HPRegen.BaseAmount, condition_HPRegen.BaseIntervalTurns, rampUp, condition_HPRegen.RampUpFactor))
            {
                return true;
            }
            Condition_ManaRegen condition_ManaRegen = condition as Condition_ManaRegen;
            if (condition_ManaRegen != null && RampUpUtility.TimesPerIntervalRampUpDifferentFromPrevious(condition_ManaRegen.BaseAmount, condition_ManaRegen.BaseIntervalTurns, rampUp, condition_ManaRegen.RampUpFactor))
            {
                return true;
            }
            Condition_StaminaRegen condition_StaminaRegen = condition as Condition_StaminaRegen;
            if (condition_StaminaRegen != null && RampUpUtility.TimesPerIntervalRampUpDifferentFromPrevious(condition_StaminaRegen.BaseAmount, condition_StaminaRegen.BaseIntervalTurns, rampUp, condition_StaminaRegen.RampUpFactor))
            {
                return true;
            }
            Condition_DealtDamageOffset condition_DealtDamageOffset = condition as Condition_DealtDamageOffset;
            if (condition_DealtDamageOffset != null && (RampUpUtility.IntRampUpDifferentFromPrevious(condition_DealtDamageOffset.BaseOffsetFrom, rampUp, condition_DealtDamageOffset.OffsetRampUpFactor) || RampUpUtility.IntRampUpDifferentFromPrevious(condition_DealtDamageOffset.BaseOffsetTo, rampUp, condition_DealtDamageOffset.OffsetRampUpFactor)))
            {
                return true;
            }
            Condition_MaxHPOffset condition_MaxHPOffset = condition as Condition_MaxHPOffset;
            return condition_MaxHPOffset != null && RampUpUtility.IntRampUpDifferentFromPrevious(condition_MaxHPOffset.BaseOffset, rampUp, condition_MaxHPOffset.OffsetRampUpFactor);
        }

        private static bool IntRampUpDifferentFromPrevious(int value, int rampUp, float rampUpFactor)
        {
            return RampUpUtility.ApplyRampUpToInt(value, rampUp, rampUpFactor) != RampUpUtility.ApplyRampUpToInt(value, RampUpUtility.GetPreviousRampUp(rampUp), rampUpFactor);
        }

        public static int ApplyRampUpToInt(int value, int rampUp, float rampUpFactor)
        {
            if (rampUp == 0)
            {
                return value;
            }
            if (value == 0)
            {
                return 0;
            }
            return Calc.RoundToIntHalfUp((float)value * Calc.Pow(rampUpFactor, (float)rampUp));
        }

        public static float ApplyRampUpToFloat(float value, int rampUp, float rampUpFactor)
        {
            if (rampUp == 0)
            {
                return value;
            }
            if (value == 0f)
            {
                return 0f;
            }
            return value * Calc.Pow(rampUpFactor, (float)rampUp);
        }

        private static bool TimesPerIntervalRampUpDifferentFromPrevious(int timesPerInterval, int interval, int rampUp, float rampUpFactor)
        {
            int num;
            int num2;
            RampUpUtility.ApplyRampUpToTimesPerInterval(timesPerInterval, interval, rampUp, rampUpFactor, out num, out num2);
            int num3;
            int num4;
            RampUpUtility.ApplyRampUpToTimesPerInterval(timesPerInterval, interval, RampUpUtility.GetPreviousRampUp(rampUp), rampUpFactor, out num3, out num4);
            return num != num3 || num2 != num4;
        }

        public static void ApplyRampUpToTimesPerInterval(int timesPerInterval, int interval, int rampUp, float rampUpFactor, out int newTimesPerInterval, out int newInterval)
        {
            if (rampUp == 0 || interval <= 0 || rampUpFactor == 1f)
            {
                newTimesPerInterval = timesPerInterval;
                newInterval = interval;
                return;
            }
            Calc.FrequencyToTimesPerInterval(RampUpUtility.ApplyRampUpToFloat((float)timesPerInterval / (float)interval, rampUp, rampUpFactor), out newTimesPerInterval, out newInterval);
        }

        private static int GetPreviousRampUp(int rampUp)
        {
            if (rampUp == 0)
            {
                return 0;
            }
            if (rampUp > 0)
            {
                return rampUp - 1;
            }
            return rampUp + 1;
        }

        public const float DefaultRampUpFactor = 1.2500001f;

        private static readonly float[] RampUpOnFloor = new float[]
        {
            0f, 1f, 2f, 3f, 4f, 4f, 5f, 6f, 7f, 7f,
            8f, 9f, 10f, 11f, 11f
        };
    }
}