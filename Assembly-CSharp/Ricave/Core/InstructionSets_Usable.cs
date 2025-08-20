using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public static class InstructionSets_Usable
    {
        public static IEnumerable<Instruction> Use(Actor user, IUsable usable, Target target, BodyPart targetBodyPart = null)
        {
            bool miss;
            if (target.Entity is Actor)
            {
                float num = usable.MissChance;
                if (user != null)
                {
                    num *= user.ConditionsAccumulated.MissChanceFactor;
                }
                if (user != null && num > 0f)
                {
                    num = Math.Max(num, user.ConditionsAccumulated.MinMissChanceOverride);
                }
                miss = Rand.Chance(num);
            }
            else
            {
                miss = false;
            }
            UseEffects useEffectsSource = (miss ? usable.MissUseEffects : usable.UseEffects);
            UseEffects useEffects2 = useEffectsSource;
            List<UseEffect> useEffects = ((useEffects2 != null) ? useEffects2.All.ToTemporaryList<UseEffect>() : null);
            List<List<Target>> targets = FrameLocalPool<List<List<Target>>>.Get();
            if (useEffects != null)
            {
                for (int j = 0; j < useEffects.Count; j++)
                {
                    targets.Add(FrameLocalPool<List<Target>>.Get());
                    if ((useEffects[j].AoERadius == null && !useEffects[j].HasCustomAoEArea) || useEffects[j].AoEHandledManually)
                    {
                        targets[j].Add(target);
                    }
                    else
                    {
                        AoEUtility.GetAoETargets(target.Position, useEffects[j], usable, user, targets[j]);
                    }
                }
            }
            if (user != null)
            {
                if (usable.ManaCost != 0)
                {
                    int num2 = -Math.Min(usable.ManaCost, user.Mana);
                    if (num2 != 0)
                    {
                        yield return new Instruction_ChangeMana(user, num2);
                    }
                }
                if (usable.StaminaCost != 0)
                {
                    int num3 = -Math.Min(usable.StaminaCost, user.Stamina);
                    if (num3 != 0)
                    {
                        yield return new Instruction_ChangeStamina(user, num3);
                    }
                }
            }
            if (miss)
            {
                foreach (Instruction instruction in InstructionSets_Usable.ExtraMissInstructions(user, usable, target))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            if (useEffects != null)
            {
                bool? prevSurroundedBonusJustBeforeUsing = Get.Player.SurroundedBonusJustBeforeUsing;
                Get.Player.SurroundedBonusJustBeforeUsing = new bool?(user != null && user.IsMainActor && Get.Skill_SurroundedBonus.IsUnlocked() && SkillUtility.TwoAdjacentEnemiesToPlayer);
                try
                {
                    int num4;
                    for (int i = 0; i < useEffects.Count; i = num4 + 1)
                    {
                        if (useEffectsSource.Contains_ProbablyAt(useEffects[i], i))
                        {
                            bool usedOnAnything = false;
                            foreach (Target target2 in targets[i])
                            {
                                if (Rand.Chance(useEffects[i].Chance) && (!(useEffects[i] is UseEffect_AddCondition) || target2.Spawned))
                                {
                                    usedOnAnything = true;
                                    foreach (Instruction instruction2 in useEffects[i].MakeUseInstructions(user, usable, target2, target, targetBodyPart))
                                    {
                                        yield return instruction2;
                                    }
                                    IEnumerator<Instruction> enumerator = null;
                                }
                            }
                            List<Target>.Enumerator enumerator2 = default(List<Target>.Enumerator);
                            if (usedOnAnything)
                            {
                                foreach (Instruction instruction3 in InstructionSets_Usable.PostUsedUseEffect(useEffects[i]))
                                {
                                    yield return instruction3;
                                }
                                IEnumerator<Instruction> enumerator = null;
                            }
                        }
                        num4 = i;
                    }
                }
                finally
                {
                    Get.Player.SurroundedBonusJustBeforeUsing = prevSurroundedBonusJustBeforeUsing;
                }
                prevSurroundedBonusJustBeforeUsing = null;
            }
            yield return new Instruction_SetLastUseSequence(usable, Get.TurnManager.CurrentSequence);
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> CheckAutoUseStructuresOnPassingActor(Actor actor)
        {
            if (!Get.CellsInfo.AnyAutoUseOnPassingActors(actor.Position))
            {
                yield break;
            }
            foreach (Entity entity in Get.World.GetEntitiesAt(actor.Position).ToTemporaryList<Entity>())
            {
                Structure structure = entity as Structure;
                if (structure != null && structure.Spec.Structure.AutoUseOnActorsPassing)
                {
                    RetractableComp comp = structure.GetComp<RetractableComp>();
                    if ((comp == null || comp.Active) && actor.CanUseOn(structure, actor, null, false, null))
                    {
                        if (actor.IsNowControlledActor)
                        {
                            UsePrompt usePrompt = structure.UsePrompt;
                            if (usePrompt != null)
                            {
                                if (!Get.UI.IsWheelSelectorOpen)
                                {
                                    usePrompt.ShowUsePrompt(new Action_Use(Get.Action_Use, actor, structure, actor, null));
                                    continue;
                                }
                                continue;
                            }
                        }
                        bool prevMoveDrawPos = actor.IfDestroyedMoveDrawPosToDestInstantly;
                        actor.IfDestroyedMoveDrawPosToDestInstantly = true;
                        try
                        {
                            foreach (Instruction instruction in InstructionSets_Usable.Use(null, structure, actor, null))
                            {
                                yield return instruction;
                            }
                            IEnumerator<Instruction> enumerator2 = null;
                        }
                        finally
                        {
                            actor.IfDestroyedMoveDrawPosToDestInstantly = prevMoveDrawPos;
                        }
                        if (!actor.Spawned)
                        {
                            break;
                        }
                    }
                }
            }
            List<Entity>.Enumerator enumerator = default(List<Entity>.Enumerator);
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> CheckAutoUseStructureOnActors(Structure structure)
        {
            if (structure.Spec.Structure.AutoUseOnActorsPassing)
            {
                RetractableComp comp = structure.GetComp<RetractableComp>();
                if ((comp == null || comp.Active) && Get.CellsInfo.AnyActorAt(structure.Position))
                {
                    foreach (Entity entity in Get.World.GetEntitiesAt(structure.Position).ToTemporaryList<Entity>())
                    {
                        Actor actor = entity as Actor;
                        if (actor != null && actor.CanUseOn(structure, actor, null, false, null))
                        {
                            bool prevMoveDrawPos = actor.IfDestroyedMoveDrawPosToDestInstantly;
                            actor.IfDestroyedMoveDrawPosToDestInstantly = true;
                            try
                            {
                                foreach (Instruction instruction in InstructionSets_Usable.Use(null, structure, actor, null))
                                {
                                    yield return instruction;
                                }
                                IEnumerator<Instruction> enumerator2 = null;
                            }
                            finally
                            {
                                actor.IfDestroyedMoveDrawPosToDestInstantly = prevMoveDrawPos;
                            }
                            actor = null;
                        }
                    }
                    List<Entity>.Enumerator enumerator = default(List<Entity>.Enumerator);
                    yield break;
                }
            }
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> CheckAutoUseStructuresOnPlayerMovingOutOfTop(Vector3Int prevPos, Actor actor)
        {
            Vector3Int vector3Int = prevPos.Below();
            if (!vector3Int.InBounds())
            {
                yield break;
            }
            if (!Get.CellsInfo.AnyStructureAt(vector3Int))
            {
                yield break;
            }
            foreach (Entity entity in Get.World.GetEntitiesAt(vector3Int).ToTemporaryList<Entity>())
            {
                Structure structure = entity as Structure;
                if (structure != null && structure.Spec.Structure.AutoUseIfPlayerMovedOutOfTop)
                {
                    bool prevMoveDrawPos = actor.IfDestroyedMoveDrawPosToDestInstantly;
                    actor.IfDestroyedMoveDrawPosToDestInstantly = true;
                    try
                    {
                        foreach (Instruction instruction in InstructionSets_Usable.Use(null, structure, structure, null))
                        {
                            yield return instruction;
                        }
                        IEnumerator<Instruction> enumerator2 = null;
                    }
                    finally
                    {
                        actor.IfDestroyedMoveDrawPosToDestInstantly = prevMoveDrawPos;
                    }
                }
            }
            List<Entity>.Enumerator enumerator = default(List<Entity>.Enumerator);
            yield break;
            yield break;
        }

        private static IEnumerable<Instruction> PostUsedUseEffect(UseEffect useEffect)
        {
            if (useEffect.Parent == null)
            {
                yield break;
            }
            if (useEffect.UsesLeft > 0)
            {
                bool useEffectDisappears = useEffect.UsesLeft == 1;
                yield return new Instruction_ChangeUseEffectUsesLeft(useEffect, -1);
                if (useEffectDisappears)
                {
                    foreach (Instruction instruction in InstructionSets_Misc.RemoveUseEffect(useEffect, false, true))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator = null;
                }
            }
            yield break;
            yield break;
        }

        private static IEnumerable<Instruction> ExtraMissInstructions(Actor user, IUsable usable, Target target)
        {
            if (!target.IsNowControlledActor)
            {
                yield return new Instruction_AddFloatingText(target, "Miss".Translate(), new Color(0.8f, 0.8f, 0.8f), 0.4f, 0f, 0f, null);
            }
            if (ActionUtility.TargetConcernsPlayer(user) || ActionUtility.TargetConcernsPlayer(target))
            {
                yield return new Instruction_PlayLog("ActorMisses".Translate(user ?? usable));
            }
            yield break;
        }

        public static IEnumerable<Instruction> TryDecrementChargesLeft(Item item)
        {
            if (item.ChargesLeft <= 0)
            {
                yield break;
            }
            bool itemDisappears = item.ChargesLeft == 1;
            yield return new Instruction_ChangeItemChargesLeft(item, -1);
            if (itemDisappears)
            {
                foreach (Instruction instruction in InstructionSets_Entity.Destroy(item, null, null))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
                yield return new Instruction_Sound(Get.Sound_DestroyItemInInventory, null, 1f, 1f);
            }
            yield break;
            yield break;
        }
    }
}