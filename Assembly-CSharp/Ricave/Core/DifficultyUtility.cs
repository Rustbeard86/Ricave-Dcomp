using System;

namespace Ricave.Core
{
    public static class DifficultyUtility
    {
        public static void AddConditionsForDifficulty(Actor actor)
        {
            if (actor.IsPlayerParty || (actor.Faction != null && actor.Faction == Get.Player.Faction) || (actor.Faction == null && actor.Spec.Actor.DefaultFaction == null))
            {
                return;
            }
            int num = Calc.RoundToIntHalfUp((float)actor.MaxHP * (Get.Difficulty.EnemyHPFactor - 1f));
            if (num != 0)
            {
                Condition_MaxHPOffset condition_MaxHPOffset = new Condition_MaxHPOffset(Get.Condition_MaxHPOffset, num, 0);
                condition_MaxHPOffset.OriginDifficulty = Get.Difficulty;
                actor.Conditions.AddCondition(condition_MaxHPOffset, -1);
            }
        }

        public static void AddPlayerConditionsForDifficulty(Actor actor)
        {
            if (Get.Difficulty.PlayerHPOffset != 0)
            {
                Condition_MaxHPOffset condition_MaxHPOffset = new Condition_MaxHPOffset(Get.Condition_MaxHPOffset, Get.Difficulty.PlayerHPOffset, 0);
                condition_MaxHPOffset.OriginDifficulty = Get.Difficulty;
                actor.Conditions.AddCondition(condition_MaxHPOffset, -1);
            }
            if (Get.Difficulty.PlayerConditions != null)
            {
                foreach (Condition condition in Get.Difficulty.PlayerConditions.All)
                {
                    Condition condition2 = condition.Clone();
                    condition2.OriginDifficulty = Get.Difficulty;
                    actor.Conditions.AddCondition(condition2, -1);
                }
            }
        }
    }
}