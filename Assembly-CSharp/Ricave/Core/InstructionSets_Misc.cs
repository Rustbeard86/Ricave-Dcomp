using System;
using System.Collections.Generic;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public static class InstructionSets_Misc
    {
        public static IEnumerable<Instruction> AddCondition(Condition condition, Conditions conditions, bool forceAddPlayLog = false, bool canAddPlayLog = true)
        {
            yield return new Instruction_AddCondition(condition, conditions);
            if (canAddPlayLog && !condition.Hidden && (forceAddPlayLog || ActionUtility.TargetConcernsPlayer(condition.AffectedActor)))
            {
                yield return new Instruction_PlayLog("EntityGainsCondition".Translate(condition.Parent, condition));
            }
            foreach (Instruction instruction in condition.MakePostAddInstructions())
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            if (condition.AffectedActor != null)
            {
                foreach (Instruction instruction2 in condition.MakeOnNewlyAffectingActorInstructions())
                {
                    yield return instruction2;
                }
                enumerator = null;
            }
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> RemoveCondition(Condition condition, bool forceAddPlayLog = false, bool canAddPlayLog = true)
        {
            Conditions removedFrom = condition.Parent;
            yield return new Instruction_RemoveCondition(condition);
            Actor affectedActor = removedFrom.GetAffectedActor();
            if (canAddPlayLog && !condition.Hidden && (forceAddPlayLog || ActionUtility.TargetConcernsPlayer(affectedActor)))
            {
                string text = "ConditionWearsOff".Translate(condition);
                if (affectedActor != null && !affectedActor.IsNowControlledActor)
                {
                    text = text + " (" + RichText.Label(affectedActor) + ")";
                }
                yield return new Instruction_PlayLog(text);
                yield return new Instruction_Sound(Get.Sound_ConditionWornOff, null, 1f, 1f);
            }
            foreach (Instruction instruction in condition.MakePostRemoveInstructions(removedFrom))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            if (affectedActor != null)
            {
                foreach (Instruction instruction2 in condition.MakeOnNoLongerAffectingActorInstructions(affectedActor))
                {
                    yield return instruction2;
                }
                enumerator = null;
            }
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> TryRenewCondition(Condition existingCondition, Condition newCondition)
        {
            if (existingCondition.Spec != newCondition.Spec)
            {
                yield break;
            }
            if (existingCondition.TurnsLeft > 0 && newCondition.TurnsLeft > 0 && newCondition.TurnsLeft > existingCondition.TurnsLeft)
            {
                int num = newCondition.TurnsLeft - existingCondition.TurnsLeft;
                yield return new Instruction_ChangeConditionTurnsLeft(existingCondition, num);
            }
            yield break;
        }

        public static IEnumerable<Instruction> AddUseEffect(UseEffect useEffect, UseEffects useEffects, bool forceAddPlayLog = false)
        {
            yield return new Instruction_AddUseEffect(useEffect, useEffects);
            if (!useEffect.Hidden && (forceAddPlayLog || ActionUtility.UsableConcernsPlayer(useEffect.Parent.Parent)))
            {
                yield return new Instruction_PlayLog("EntityGainsUseEffect".Translate(useEffect.Parent, useEffect));
            }
            yield break;
        }

        public static IEnumerable<Instruction> RemoveUseEffect(UseEffect useEffect, bool forceAddPlayLog = false, bool canAddPlayLog = true)
        {
            UseEffects removedFrom = useEffect.Parent;
            yield return new Instruction_RemoveUseEffect(useEffect);
            if (canAddPlayLog && !useEffect.Hidden && (forceAddPlayLog || ActionUtility.UsableConcernsPlayer(removedFrom.Parent)))
            {
                string text = "UseEffectWearsOff".Translate(useEffect);
                Actor wieldingActor = removedFrom.GetWieldingActor();
                if (wieldingActor == null || !wieldingActor.IsNowControlledActor)
                {
                    text = text + " (" + RichText.Label(removedFrom.Parent) + ")";
                }
                yield return new Instruction_PlayLog(text);
                yield return new Instruction_Sound(Get.Sound_UseEffectWornOff, null, 1f, 1f);
            }
            yield break;
        }

        public static IEnumerable<Instruction> GiveExperience(int amount, bool fromEnemyKill, Entity floatingTextAbove = null, Vector3? experienceParticlesSource = null)
        {
            if (amount <= 0)
            {
                yield break;
            }
            if (!Get.MainActor.Spawned)
            {
                yield break;
            }
            int levelBefore = Get.Player.Level;
            int spellsBefore = SpellChooserUtility.AvailableSpellCountAtCurrentLevel;
            yield return new Instruction_ChangePlayerExperience(amount);
            if (fromEnemyKill)
            {
                yield return new Instruction_ChangeExpFromKillingEnemies(amount);
            }
            yield return new Instruction_Immediate(delegate
            {
                Get.Player.LastExpGainTime = Clock.UnscaledTime;
            });
            int levelsGained = Get.Player.Level - levelBefore;
            if (levelsGained > 0)
            {
                int num = Math.Min(levelsGained * LevelUtility.MaxHPPerLevelAdjusted, Get.MainActor.MaxHP - Get.MainActor.HP);
                if (num > 0)
                {
                    yield return new Instruction_ChangeHP(Get.MainActor, num);
                }
                foreach (Actor actor in Get.Player.Party.ToTemporaryList<Actor>())
                {
                    if (!actor.IsMainActor && actor.Spawned && actor.HP > 0 && actor.Spec.Actor.PlayerLevelAffectsMaxHP)
                    {
                        int num2 = Math.Min(levelsGained * 3, actor.MaxHP - actor.HP);
                        if (num2 > 0)
                        {
                            yield return new Instruction_ChangeHP(actor, num2);
                        }
                    }
                }
                List<Actor>.Enumerator enumerator = default(List<Actor>.Enumerator);
                yield return new Instruction_Sound(Get.Sound_LevelUp, null, 1f, 1f);
                yield return new Instruction_VisualEffect(Get.VisualEffect_LevelUp, Get.MainActor.Position);
                string text = "LeveledUp".Translate();
                if (SpellChooserUtility.AvailableSpellCountAtCurrentLevel > spellsBefore && Get.MainActor.Spells.Count < 7)
                {
                    text = text.AppendedWithSpace("LeveledUp_CanChooseNewSpell".Translate());
                }
                yield return new Instruction_PlayLog(text);
                yield return new Instruction_Immediate(delegate
                {
                    Get.Player.LastLevelUpTime = Clock.UnscaledTime;
                });
                if (floatingTextAbove != null)
                {
                    yield return new Instruction_AddFloatingText(floatingTextAbove, "LeveledUpShort".Translate(), new Color(0.7f, 0.7f, 0.22f), 0.32f, 0f, -0.27f, null);
                }
            }
            if (experienceParticlesSource != null)
            {
                yield return new Instruction_ExperienceParticles(experienceParticlesSource.Value, amount);
            }
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> ResetTurnsCanRewind()
        {
            if (Get.Player.TurnsCanRewind != 0)
            {
                yield return new Instruction_ChangeTurnsCanRewind(-Get.Player.TurnsCanRewind);
            }
            if (Get.Player.ReplenishTurnsCanRewindTurnsPassed != 0)
            {
                yield return new Instruction_ChangeReplenishTurnsCanRewindTurnsPassed(-Get.Player.ReplenishTurnsCanRewindTurnsPassed);
            }
            yield break;
        }

        public static IEnumerable<Instruction> GainScore(int score, string label, bool applyDifficultyFactor = true, bool applyBoostFactor = true, bool applyDungeonModifiersFactor = true)
        {
            float num = 1f;
            if (applyDifficultyFactor)
            {
                num *= Get.Difficulty.ScoreFactor;
            }
            if (applyBoostFactor && Get.UnlockableManager.IsUnlocked(Get.Unlockable_ScoreBoost, null))
            {
                num *= 2f;
            }
            if (applyDungeonModifiersFactor)
            {
                num *= DungeonModifiersUtility.GetCurrentRunScoreFactor();
            }
            if (num != 1f)
            {
                score = Calc.RoundToIntHalfUp((float)score * num);
            }
            if (score <= 0)
            {
                yield break;
            }
            yield return new Instruction_ChangeScore(score);
            yield return new Instruction_ScoreGainEffect(label, score);
            yield break;
        }

        public static IEnumerable<Instruction> AddWorldSituation(WorldSituation situation)
        {
            yield return new Instruction_AddWorldSituation(situation);
            foreach (Instruction instruction in situation.MakePostAddInstructions())
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> RemoveWorldSituation(WorldSituation situation)
        {
            yield return new Instruction_RemoveWorldSituation(situation);
            foreach (Instruction instruction in situation.MakePostRemoveInstructions())
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> DoWorldEvent(WorldEventSpec worldEventSpec)
        {
            foreach (Instruction instruction in worldEventSpec.Behavior.MakeEventInstructions())
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield return new Instruction_ShowWorldEventNotification(worldEventSpec);
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> AddFaction(Faction faction)
        {
            yield return new Instruction_AddFaction(faction);
            foreach (Faction faction2 in FactionUtility.DefaultHostileFactions(faction))
            {
                yield return new Instruction_AddFactionHostility(faction, faction2);
            }
            List<Faction>.Enumerator enumerator = default(List<Faction>.Enumerator);
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> RemoveFaction(Faction faction)
        {
            yield return new Instruction_RemoveFaction(faction);
            foreach (Faction faction2 in Get.FactionManager.Factions)
            {
                if (faction2 != faction && Get.FactionManager.HostilityExists(faction, faction2))
                {
                    yield return new Instruction_RemoveFactionHostility(faction, faction2);
                }
            }
            List<Faction>.Enumerator enumerator = default(List<Faction>.Enumerator);
            yield break;
            yield break;
        }
    }
}