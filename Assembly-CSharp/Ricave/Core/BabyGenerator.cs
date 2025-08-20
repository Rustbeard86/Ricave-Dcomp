using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public static class BabyGenerator
    {
        public static Actor Baby()
        {
            Place place = Get.Place;
            IEnumerable<EntitySpec> enumerable;
            if (!((place != null) ? place.Enemies : null).NullOrEmpty<EntitySpec>())
            {
                enumerable = Get.Place.Enemies.Where<EntitySpec>((EntitySpec x) => x.Actor.CanBeBaby);
            }
            else
            {
                enumerable = from x in Get.Specs.GetAll<EntitySpec>()
                             where x.IsActor && x.Actor.KilledExperience > 0 && x.Actor.CanBeBaby && Get.Floor >= x.Actor.GenerateMinFloor && x.Actor.GenerateSelectionWeight > 0f && !x.Actor.AlwaysBoss
                             select x;
            }
            EntitySpec entitySpec;
            if (enumerable.TryGetRandomElement<EntitySpec>(out entitySpec, (EntitySpec x) => x.Actor.GenerateSelectionWeight))
            {
                return Maker.Make<Actor>(entitySpec, new Action<Actor>(BabyGenerator.Configure), false, false, true);
            }
            return null;
        }

        private static void Configure(Actor baby)
        {
            baby.RampUp = RampUpUtility.GenerateRandomRampUpFor(baby, true);
            baby.IsBaby = true;
            DifficultyUtility.AddConditionsForDifficulty(baby);
            int num = Calc.FloorToInt((float)baby.MaxHP * 0.4f);
            if (num > 0)
            {
                baby.Conditions.AddCondition(new Condition_MaxHPOffset(Get.Condition_MaxHPOffset, -num, 0), -1);
            }
            foreach (NativeWeapon nativeWeapon in baby.NativeWeapons)
            {
                foreach (UseEffect useEffect in nativeWeapon.UseEffects.All)
                {
                    UseEffect_Damage useEffect_Damage = useEffect as UseEffect_Damage;
                    if (useEffect_Damage != null)
                    {
                        if (useEffect_Damage.BaseFrom > 0)
                        {
                            UseEffect_Damage useEffect_Damage2 = useEffect_Damage;
                            int num2 = useEffect_Damage2.BaseFrom;
                            useEffect_Damage2.BaseFrom = num2 - 1;
                        }
                        else if (useEffect_Damage.BaseTo > 1)
                        {
                            UseEffect_Damage useEffect_Damage3 = useEffect_Damage;
                            int num2 = useEffect_Damage3.BaseTo;
                            useEffect_Damage3.BaseTo = num2 - 1;
                        }
                    }
                }
            }
            baby.CalculateInitialHPManaAndStamina();
        }
    }
}