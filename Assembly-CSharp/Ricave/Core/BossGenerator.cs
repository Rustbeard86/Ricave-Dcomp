using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public static class BossGenerator
    {
        public static Actor Boss(bool allowQuestBoss = true)
        {
            if (allowQuestBoss)
            {
                if (Get.Quest_KillNightmareLord.IsActive() && Get.Floor == 1)
                {
                    return BossGenerator.Boss(Get.Entity_Skeleton, "NightmareLord".Translate(), null);
                }
                if (Get.Quest_KillHypnorak.IsActive() && Get.Floor == 3)
                {
                    return BossGenerator.Boss(Get.Entity_Dragon, "Hypnorak".Translate(), null);
                }
                if (Get.Quest_KillPhantasmos.IsActive() && Get.Floor == 4)
                {
                    return BossGenerator.Boss(Get.Entity_Mummy, "Phantasmos".Translate(), null);
                }
            }
            Place place = Get.Place;
            IEnumerable<EntitySpec> enumerable;
            if (!((place != null) ? place.Enemies : null).NullOrEmpty<EntitySpec>())
            {
                enumerable = Get.Place.Enemies.Where<EntitySpec>((EntitySpec x) => x.Actor.CanBeBoss);
            }
            else
            {
                enumerable = from x in Get.Specs.GetAll<EntitySpec>()
                             where x.IsActor && x.Actor.CanBeBoss && Get.Floor >= x.Actor.GenerateMinFloor && x.Actor.GenerateSelectionWeight > 0f
                             select x;
            }
            EntitySpec entitySpec;
            if (enumerable.TryGetRandomElement<EntitySpec>(out entitySpec, (EntitySpec x) => x.Actor.GenerateSelectionWeight))
            {
                return BossGenerator.Boss(entitySpec, null, null);
            }
            return null;
        }

        public static Actor Boss(EntitySpec actorSpec, string specificName = null, int? specificRampUp = null)
        {
            return Maker.Make<Actor>(actorSpec, delegate (Actor x)
            {
                BossGenerator.Configure(x, specificName, specificRampUp);
            }, false, false, true);
        }

        private static void Configure(Actor boss, string specificName = null, int? specificRampUp = null)
        {
            boss.RampUp = specificRampUp ?? RampUpUtility.GenerateRandomRampUpFor(boss, true);
            boss.IsBoss = true;
            if (!specificName.NullOrEmpty())
            {
                boss.Name = specificName;
            }
            else if (!boss.Spec.Actor.AlwaysBoss)
            {
                boss.Name = NameGenerator.Name(false);
            }
            DifficultyUtility.AddConditionsForDifficulty(boss);
            if (!boss.Spec.Actor.AlwaysBoss)
            {
                boss.Conditions.AddCondition(new Condition_MaxHPOffset(Get.Condition_MaxHPOffset, boss.MaxHP, 0), -1);
                if (Rand.Chance(0.5f))
                {
                    boss.Conditions.AddCondition(new Condition_HPRegen(Get.Condition_HPRegen, 5, 1, true, 0), -1);
                }
                if (Rand.Chance(0.4f))
                {
                    if ((Rand.Bool || boss.ImmuneToFire) && !boss.ImmuneToPoison)
                    {
                        boss.Conditions.AddCondition(new Condition_ImmuneToPoison(Get.Condition_ImmuneToPoison), -1);
                    }
                    else if (!boss.ImmuneToFire)
                    {
                        boss.Conditions.AddCondition(new Condition_ImmuneToFire(Get.Condition_ImmuneToFire), -1);
                    }
                }
                foreach (NativeWeapon nativeWeapon in boss.NativeWeapons)
                {
                    foreach (UseEffect useEffect in nativeWeapon.UseEffects.All)
                    {
                        UseEffect_Damage useEffect_Damage = useEffect as UseEffect_Damage;
                        if (useEffect_Damage != null && useEffect_Damage.BaseTo > 0)
                        {
                            UseEffect_Damage useEffect_Damage2 = useEffect_Damage;
                            int baseTo = useEffect_Damage2.BaseTo;
                            useEffect_Damage2.BaseTo = baseTo + 1;
                        }
                    }
                }
            }
            boss.CalculateInitialHPManaAndStamina();
            if (Get.Floor >= 2 && !boss.Spec.Actor.AlwaysBoss)
            {
                BossGenerator.AddSpecialUseEffectToWeapon(boss);
            }
        }

        private static void AddSpecialUseEffectToWeapon(Actor boss)
        {
            if (boss.NativeWeapons.Count == 0)
            {
                return;
            }
            int num = Rand.RangeInclusive(0, 5);
            NativeWeapon nativeWeapon = boss.NativeWeapons[0];
            switch (num)
            {
                case 0:
                    nativeWeapon.UseEffects.AddUseEffect(new UseEffect_TeleportRandomly(Get.UseEffect_TeleportRandomly, 1f, 1), -1);
                    return;
                case 1:
                    nativeWeapon.UseEffects.AddUseEffect(new UseEffect_AddCondition(Get.UseEffect_AddCondition, new Condition_Poisoned(Get.Condition_Poisoned, 1, 10), 0.3f, UseEffect_AddCondition.StackMode.NeverSameSpec, 1), -1);
                    return;
                case 2:
                    nativeWeapon.UseEffects.AddUseEffect(new UseEffect_AddCondition(Get.UseEffect_AddCondition, new Condition_SeeRangeOffset(Get.Condition_SeeRangeOffset, -4, 10), 1f, UseEffect_AddCondition.StackMode.CanStack, 1), -1);
                    return;
                case 3:
                    nativeWeapon.UseEffects.AddUseEffect(new UseEffect_Push(Get.UseEffect_Push, 4, 1f, 2), -1);
                    return;
                case 4:
                    nativeWeapon.UseEffects.AddUseEffect(new UseEffect_Spawn(Get.UseEffect_Spawn, Get.Entity_MagicWall, false, 1f, 5), -1);
                    return;
                case 5:
                    nativeWeapon.UseEffects.AddUseEffect(new UseEffect_AddCondition(Get.UseEffect_AddCondition, new Condition_Entangled(Get.Condition_Entangled, 2), 1f, UseEffect_AddCondition.StackMode.CanStack, 1), -1);
                    return;
                default:
                    return;
            }
        }
    }
}