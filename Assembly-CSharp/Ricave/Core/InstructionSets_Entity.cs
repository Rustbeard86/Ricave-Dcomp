using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public static class InstructionSets_Entity
    {
        public static IEnumerable<Instruction> Spawn(Entity entity, Vector3Int pos, Quaternion? rot = null)
        {
            if (entity.Spawned)
            {
                Log.Error("Tried to make spawn instructions for an already spawned entity: " + entity.ToStringSafe(), false);
                yield break;
            }
            yield return new Instruction_Spawn(entity, pos, rot);
            Actor actor = entity as Actor;
            IEnumerator<Instruction> enumerator3;
            if (actor != null)
            {
                yield return new Instruction_AddSequenceable(actor, actor.SequencePerTurn, Get.TurnManager.OrderAfterCurrent);
                foreach (Condition condition in actor.ConditionsAccumulated.AllConditions)
                {
                    yield return new Instruction_AddSequenceable(condition, 12, Get.TurnManager.OrderAfterCurrent);
                }
                List<Condition>.Enumerator enumerator = default(List<Condition>.Enumerator);
                if (actor.Spec.Actor.CallSound != null)
                {
                    yield return new Instruction_Sound(actor.Spec.Actor.CallSound, new Vector3?(actor.Position), EntitySoundUtility.GetPitchFromScale(actor), 1f);
                }
                foreach (WorldSituation worldSituation in Get.WorldSituationsManager.Situations)
                {
                    if (worldSituation.Spec.EveryoneConditions != null && worldSituation.Spec.EveryoneConditions.Any)
                    {
                        foreach (Condition condition2 in worldSituation.Spec.EveryoneConditions.All)
                        {
                            Condition condition3 = condition2.Clone();
                            condition3.OriginWorldSituation = worldSituation;
                            foreach (Instruction instruction in InstructionSets_Misc.AddCondition(condition3, actor.Conditions, false, false))
                            {
                                yield return instruction;
                            }
                            enumerator3 = null;
                        }
                        enumerator = default(List<Condition>.Enumerator);
                    }
                    worldSituation = null;
                }
                List<WorldSituation>.Enumerator enumerator2 = default(List<WorldSituation>.Enumerator);
                if (actor.Spawned)
                {
                    foreach (Instruction instruction2 in InstructionSets_Usable.CheckAutoUseStructuresOnPassingActor(actor))
                    {
                        yield return instruction2;
                    }
                    enumerator3 = null;
                }
            }
            else
            {
                ISequenceable sequenceable = entity as ISequenceable;
                if (sequenceable != null)
                {
                    yield return new Instruction_AddSequenceable(sequenceable, 12, Get.TurnManager.OrderAfterCurrent);
                }
            }
            if (entity.Spec == Get.Entity_Fire)
            {
                foreach (Entity entity2 in Get.World.GetEntitiesAt(entity.Position).ToTemporaryList<Entity>())
                {
                    if (entity2.Spec.IsItem && entity2.Spec.Item.CookedItemSpec != null && entity2.Spawned)
                    {
                        foreach (Instruction instruction3 in UseEffect_SetOnFire.CookSpawnedItem((Item)entity2))
                        {
                            yield return instruction3;
                        }
                        enumerator3 = null;
                    }
                }
                List<Entity>.Enumerator enumerator4 = default(List<Entity>.Enumerator);
            }
            foreach (Instruction instruction4 in Get.ModsEventsManager.CallEvent(ModEventWithInstructionsType.EntitySpawned, entity))
            {
                yield return instruction4;
            }
            enumerator3 = null;
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> DeSpawn(Entity entity, bool fadeOut = false)
        {
            if (!entity.Spawned)
            {
                yield break;
            }
            if (entity.Spec.IsPermanent)
            {
                Log.Warning("Despawning permanent entity " + ((entity != null) ? entity.ToString() : null) + ".", false);
            }
            yield return new Instruction_DeSpawn(entity, fadeOut);
            ISequenceable sequenceable = entity as ISequenceable;
            if (sequenceable != null)
            {
                yield return new Instruction_RemoveSequenceable(sequenceable);
            }
            Actor actor = entity as Actor;
            IEnumerator<Instruction> enumerator2;
            if (actor != null)
            {
                foreach (Condition condition in actor.ConditionsAccumulated.AllConditions)
                {
                    yield return new Instruction_RemoveSequenceable(condition);
                }
                List<Condition>.Enumerator enumerator = default(List<Condition>.Enumerator);
                foreach (Condition condition2 in actor.Conditions.All.ToTemporaryList<Condition>())
                {
                    if (condition2.OriginWorldSituation != null)
                    {
                        foreach (Instruction instruction in InstructionSets_Misc.RemoveCondition(condition2, false, false))
                        {
                            yield return instruction;
                        }
                        enumerator2 = null;
                    }
                }
                enumerator = default(List<Condition>.Enumerator);
            }
            else if (entity is Structure && Get.NowControlledActor != null && Get.NowControlledActor.Spawned)
            {
                List<Vector3Int> cellsToNewlyUnfogAt = Get.FogOfWar.GetCellsToNewlyUnfogAt(Get.NowControlledActor.Position);
                if (cellsToNewlyUnfogAt.Count != 0)
                {
                    yield return new Instruction_Unfog(cellsToNewlyUnfogAt);
                }
            }
            Actor actor2 = entity as Actor;
            if (actor2 != null && actor2.AttachedToChainPostDirect != null)
            {
                yield return new Instruction_DetachFromChainPost(actor2);
            }
            else
            {
                ChainPost chainPost = entity as ChainPost;
                if (chainPost != null && chainPost.AttachedActorDirect != null)
                {
                    yield return new Instruction_DetachFromChainPost(chainPost.AttachedActorDirect);
                }
            }
            if (ItemOrStructureFallUtility.CanAnythingEverBeAttachedToOrAffectsGravity(entity.Spec))
            {
                int num;
                for (int i = 0; i < ItemOrStructureFallUtility.DirectionsCardinalInFallCheckOrder.Length; i = num + 1)
                {
                    Vector3Int vector3Int = entity.Position + ItemOrStructureFallUtility.DirectionsCardinalInFallCheckOrder[i];
                    if (vector3Int.InBounds() && Get.World.AnyEntityAt(vector3Int))
                    {
                        foreach (Instruction instruction2 in InstructionSets_Entity.CheckItemsAndStructuresFallAt(vector3Int))
                        {
                            yield return instruction2;
                        }
                        enumerator2 = null;
                    }
                    num = i;
                }
            }
            if (Get.World.AnyEntityAt(entity.Position))
            {
                foreach (Entity entity2 in Get.World.GetEntitiesAt(entity.Position).ToTemporaryList<Entity>())
                {
                    if ((entity2.Spec.DestroyOnFilledEntityDespawned && entity.Spec.IsStructure && entity.Spec.Structure.IsFilled) || (entity2.Spec.DestroyOnEntityOfSpecDespawned != null && entity2.Spec.DestroyOnEntityOfSpecDespawned.Contains(entity.Spec)))
                    {
                        foreach (Instruction instruction3 in InstructionSets_Entity.Destroy(entity2, null, null))
                        {
                            yield return instruction3;
                        }
                        enumerator2 = null;
                    }
                }
                List<Entity>.Enumerator enumerator3 = default(List<Entity>.Enumerator);
            }
            Actor actor3 = entity as Actor;
            if (actor3 != null && actor3.IsNowControlledActor && !actor3.IsMainActor)
            {
                foreach (Instruction instruction4 in InstructionSets_Actor.SwitchNowControlledActor(Get.MainActor))
                {
                    yield return instruction4;
                }
                enumerator2 = null;
            }
            foreach (Instruction instruction5 in Get.ModsEventsManager.CallEvent(ModEventWithInstructionsType.EntityDeSpawned, entity))
            {
                yield return instruction5;
            }
            enumerator2 = null;
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> Move(Entity entity, Vector3Int newPos, bool checkFall = true, bool playSound = true)
        {
            if (!entity.Spawned)
            {
                Log.Error("Tried to make move instructions for an unspawned entity: " + entity.ToStringSafe(), false);
                yield break;
            }
            if (entity.Position == newPos)
            {
                Log.Error("Tried to make move instructions to the same position.", false);
                yield break;
            }
            if (entity.IsNowControlledActor)
            {
                List<Vector3Int> cellsToNewlyUnfogAt = Get.FogOfWar.GetCellsToNewlyUnfogAt(newPos);
                if (cellsToNewlyUnfogAt.Count != 0)
                {
                    yield return new Instruction_Unfog(cellsToNewlyUnfogAt);
                }
            }
            Vector3Int prevPos = entity.Position;
            yield return new Instruction_ChangePosition(entity, newPos);
            List<Entity> entities = Get.World.GetEntitiesAt(entity.Position);
            Actor actor = entity as Actor;
            if (entity is Structure && Get.NowControlledActor != null && Get.NowControlledActor.Spawned)
            {
                List<Vector3Int> cellsToNewlyUnfogAt2 = Get.FogOfWar.GetCellsToNewlyUnfogAt(Get.NowControlledActor.Position);
                if (cellsToNewlyUnfogAt2.Count != 0)
                {
                    yield return new Instruction_Unfog(cellsToNewlyUnfogAt2);
                }
            }
            IEnumerator<Instruction> enumerator;
            if (ItemOrStructureFallUtility.CanAnythingEverBeAttachedToOrAffectsGravity(entity.Spec))
            {
                int num;
                for (int i = 0; i < ItemOrStructureFallUtility.DirectionsCardinalInFallCheckOrder.Length; i = num + 1)
                {
                    Vector3Int vector3Int = prevPos + ItemOrStructureFallUtility.DirectionsCardinalInFallCheckOrder[i];
                    if (vector3Int.InBounds() && !(vector3Int == entity.Position))
                    {
                        foreach (Instruction instruction in InstructionSets_Entity.CheckItemsAndStructuresFallAt(vector3Int))
                        {
                            yield return instruction;
                        }
                        enumerator = null;
                    }
                    num = i;
                }
            }
            if (checkFall)
            {
                foreach (Instruction instruction2 in InstructionSets_Entity.CheckItemsAndStructuresFallAt(entity.Position))
                {
                    yield return instruction2;
                }
                enumerator = null;
            }
            if (actor != null)
            {
                if (playSound)
                {
                    SoundSpec moveSound = EntitySoundUtility.GetMoveSound(actor, prevPos);
                    if (moveSound != null)
                    {
                        yield return new Instruction_Sound(moveSound, new Vector3?(entity.Position), 1f, 1f);
                    }
                    if (actor.ActorGOC != null && actor.ActorGOC.DesiredGameObjectScaleWithoutCapAndAnimation.y >= 1.35f && !actor.CanFly && !Get.CellsInfo.IsFallingAt(actor.Position, actor, false) && Get.NowControlledActor.Spawned && actor.Position.GetGridDistance(Get.NowControlledActor.Position) <= 4)
                    {
                        yield return new Instruction_VisualEffect(Get.VisualEffect_BigActorMovedNearby, actor.Position);
                    }
                    if (actor.Spec.Actor.CallSound != null && actor != Get.NowControlledActor && Get.NowControlledActor != null && Get.NowControlledActor.Spawned && actor.Position.IsAdjacent(Get.NowControlledActor.Position) && actor.Sees(Get.NowControlledActor, null))
                    {
                        yield return new Instruction_Sound(actor.Spec.Actor.CallSound, new Vector3?(actor.Position), EntitySoundUtility.GetPitchFromScale(actor), 0.3f);
                    }
                }
                if (!actor.IsNowControlledActor && actor.Position.GetGridDistance(Get.NowControlledActor.Position) <= 4 && !Get.NowControlledActor.Sees(actor, null) && !Get.NowControlledActor.Sees(prevPos, null))
                {
                    yield return new Instruction_AddFloatingText(actor.Position, "?", new Color(0.8f, 0.8f, 0.8f), 1f, 0f, 0f, null);
                }
            }
            if (actor != null && actor.IsNowControlledPlayerParty)
            {
                for (int j = entities.Count - 1; j >= 0; j--)
                {
                    Entity entity2 = entities[j];
                    Item item = entity2 as Item;
                    if (item != null && item.ForSale)
                    {
                        Func<Action> <> 9__4;
                        Action<>9__2;
                        yield return new Instruction_Immediate(delegate
                        {
                            List<WheelSelector.Option> list = new List<WheelSelector.Option>();
                            if (item.PriceTag.CanAfford(actor) && InstructionSets_Actor.HasSpaceToPickUp(actor, item))
                            {
                                List<WheelSelector.Option> list2 = list;
                                string text = "Buy".Translate();
                                Action action;
                                if ((action = <> 9__2) == null)
                                {
                                    action = (<> 9__2 = delegate
                                    {
                                        Func<Action> func;
                                        if ((func = <> 9__4) == null)
                                        {
                                            func = (<> 9__4 = () => new Action_BuyItem(Get.Action_BuyItem, actor, item));
                                        }
                                        ActionViaInterfaceHelper.TryDo(func);
                                    });
                                }
                                list2.Add(new WheelSelector.Option(text, action, null, null));
                            }
                            list.Add(new WheelSelector.Option("Leave".Translate(), delegate
                            {
                                Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                            }, null, null));
                            string text2 = "BuyConfirmation".Translate(item, item.PriceTag.PriceRichText);
                            UnlockableAsItem unlockableAsItem = item as UnlockableAsItem;
                            if (unlockableAsItem != null && unlockableAsItem.UnlockableSpec.ResetAfterRun)
                            {
                                text2 = text2.AppendedInDoubleNewLine("BuyConfirmation_SingleUseUnlockable".Translate());
                            }
                            Get.UI.OpenWheelSelector(list, item, text2, false, false, false);
                        });
                        break;
                    }
                }
            }
            if (actor != null && actor.IsNowControlledActor)
            {
                int num;
                for (int i = entities.Count - 1; i >= 0; i = num - 1)
                {
                    Item item3 = entities[i] as Item;
                    if (item3 != null && !item3.ForSale && InstructionSets_Actor.HasSpaceToPickUp(actor, item3))
                    {
                        foreach (Instruction instruction3 in InstructionSets_Actor.PickUpItem(actor, item3, false))
                        {
                            yield return instruction3;
                        }
                        enumerator = null;
                    }
                    num = i;
                }
            }
            if (actor != null && !actor.IsPlayerParty && !Get.CellsInfo.AnyActorAt(prevPos) && !Get.CellsInfo.AnyItemAt(prevPos) && Get.World.AnyEntityOfSpecAt(prevPos, Get.Entity_TemporarilyOpenedDoor))
            {
                foreach (Entity door in Get.World.GetEntitiesAt(prevPos).ToTemporaryList<Entity>())
                {
                    if (door.Spec == Get.Entity_TemporarilyOpenedDoor && ((TemporarilyOpenedDoor)door).OpenedBy == actor)
                    {
                        foreach (Instruction instruction4 in InstructionSets_Entity.DeSpawn(door, false))
                        {
                            yield return instruction4;
                        }
                        enumerator = null;
                        foreach (Instruction instruction5 in InstructionSets_Entity.Spawn(Maker.Make(Get.Entity_Door, null, false, false, true), door.Position, new Quaternion?(door.Rotation)))
                        {
                            yield return instruction5;
                        }
                        enumerator = null;
                        yield return new Instruction_Sound(Get.Sound_CloseDoor, new Vector3?(door.Position), 1f, 1f);
                        door = null;
                    }
                }
                List<Entity>.Enumerator enumerator2 = default(List<Entity>.Enumerator);
            }
            if (actor != null)
            {
                if (Get.RunConfig.HasPetRat && actor.IsNowControlledPlayerParty)
                {
                    foreach (Vector3Int vector3Int2 in Vector3IntUtility.Directions)
                    {
                        Vector3Int vector3Int3 = actor.Position + vector3Int2;
                        if (vector3Int3.InBounds() && !Get.CellsInfo.IsFilled(vector3Int3) && !Get.World.AnyEntityOfSpecAt(vector3Int3, Get.Entity_Cage) && !Get.World.AnyEntityOfSpecAt(vector3Int3, Get.Entity_HangingCage) && LineOfSight.IsLineOfSight(actor.Position, vector3Int3) && LineOfSight.IsLineOfFire(actor.Position, vector3Int3))
                        {
                            Item item2 = (Item)Get.World.GetFirstEntityOfSpecAt(vector3Int3, Get.Entity_Gold);
                            if (item2 != null && !item2.ForSale && InstructionSets_Actor.HasSpaceToPickUp(actor, item2))
                            {
                                foreach (Instruction instruction6 in InstructionSets_Actor.PickUpItem(actor, item2, true))
                                {
                                    yield return instruction6;
                                }
                                enumerator = null;
                            }
                        }
                    }
                    Vector3Int[] array = null;
                }
                if (actor.AttachedToChainPostDirect != null && actor.Position.GetGridDistance(actor.AttachedToChainPostDirect.Position) >= 4)
                {
                    yield return new Instruction_DetachFromChainPost(actor);
                }
                foreach (Instruction instruction7 in InstructionSets_Usable.CheckAutoUseStructuresOnPassingActor(actor))
                {
                    yield return instruction7;
                }
                enumerator = null;
                if (actor.IsPlayerParty)
                {
                    foreach (Instruction instruction8 in InstructionSets_Usable.CheckAutoUseStructuresOnPlayerMovingOutOfTop(prevPos, actor))
                    {
                        yield return instruction8;
                    }
                    enumerator = null;
                }
                foreach (Instruction instruction9 in InstructionSets_Noise.MakeNoise(actor.Position, actor.MoveNoiseRadius, NoiseType.ActorMoved, actor, false, true))
                {
                    yield return instruction9;
                }
                enumerator = null;
                foreach (Condition condition in actor.ConditionsAccumulated.AllConditions.ToTemporaryList<Condition>())
                {
                    foreach (Instruction instruction10 in condition.MakeAffectedActorMovedInstructions(prevPos))
                    {
                        yield return instruction10;
                    }
                    enumerator = null;
                }
                List<Condition>.Enumerator enumerator3 = default(List<Condition>.Enumerator);
                if (actor.IsPlayerParty)
                {
                    List<Entity> entitiesWithComp = Get.World.GetEntitiesWithComp<ProximityTriggerComp>();
                    foreach (Entity entity3 in entitiesWithComp.ToTemporaryList<Entity>())
                    {
                        if (entity3.Spawned && entity3.Position.IsAdjacentOrInside(actor.Position))
                        {
                            foreach (Instruction instruction11 in entity3.GetComp<ProximityTriggerComp>().MakePlayerActorAdjacentInstructions())
                            {
                                yield return instruction11;
                            }
                            enumerator = null;
                        }
                    }
                    List<Entity>.Enumerator enumerator2 = default(List<Entity>.Enumerator);
                }
                if (Get.LessonManager.CurrentLesson == Get.Lesson_ClimbingLadders && actor.IsNowControlledActor && actor.Position.y == 2)
                {
                    yield return new Instruction_Immediate(delegate
                    {
                        Get.LessonManager.FinishIfCurrent(Get.Lesson_ClimbingLadders);
                    });
                }
            }
            foreach (Instruction instruction12 in Get.ModsEventsManager.CallEvent(ModEventWithInstructionsType.EntityMoved, entity))
            {
                yield return instruction12;
            }
            enumerator = null;
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> Damage(Entity target, int damage, DamageTypeSpec damageType, string extraReason, Vector3Int? impactSource, bool critical, bool forceAddPlayLogEntry = false, Vector3? exactImpactPos = null, Actor responsible = null, bool smallerVisuals = false, bool canConsumeEquippedItemsCharges = true)
        {
            if (!target.Spawned)
            {
                Log.Error("Tried to make damage instructions for an unspawned entity. Only spawned entities can be harmed. Entity: " + target.ToStringSafe(), false);
                yield break;
            }
            damage = Calc.Clamp(damage, 0, target.HP);
            bool dies = target.HP <= damage;
            IEnumerator<Instruction> enumerator;
            if (dies)
            {
                Actor targetActor = target as Actor;
                if (targetActor != null && targetActor.Inventory.AnyEquippedItemHealsWearerOnKilled)
                {
                    Item item = targetActor.Inventory.EquippedItems.FirstOrDefault<Item>((Item x) => x.Spec.Item.HealWearerOnKilled && x.Spec != Get.Entity_AmuletOfYerdon);
                    if (item == null)
                    {
                        item = targetActor.Inventory.EquippedItems.First<Item>((Item x) => x.Spec.Item.HealWearerOnKilled);
                    }
                    foreach (Instruction instruction in InstructionSets_Actor.RemoveFromInventory(item))
                    {
                        yield return instruction;
                    }
                    enumerator = null;
                    yield return new Instruction_ChangeHP(target, target.MaxHP - target.HP);
                    if (forceAddPlayLogEntry || ActionUtility.TargetConcernsPlayer(target))
                    {
                        if (target.IsNowControlledActor)
                        {
                            yield return new Instruction_PlayLog("PlayerHealedOnKilledByWearable".Translate(item));
                        }
                        else
                        {
                            yield return new Instruction_PlayLog("HealedOnKilledByWearable".Translate(target, item));
                        }
                    }
                    yield return new Instruction_VisualEffect(Get.VisualEffect_HealedOnKilledByWearable, target.Position);
                    if (item.Spec == Get.Entity_AmuletOfYerdon)
                    {
                        IEnumerable<Actor> enumerable = Get.World.Actors.Where<Actor>((Actor x) => x.Position.GetGridDistance(targetActor.Position) <= 3 && LineOfSight.IsLineOfFire(targetActor.Position, x.Position));
                        foreach (Actor actor2 in enumerable.ToTemporaryList<Actor>())
                        {
                            if (actor2 != targetActor && actor2.Spawned && targetActor.IsHostile(actor2))
                            {
                                foreach (Instruction instruction2 in InstructionSets_Entity.Destroy(actor2, new Vector3Int?(targetActor.Position), targetActor))
                                {
                                    yield return instruction2;
                                }
                                enumerator = null;
                            }
                        }
                        List<Actor>.Enumerator enumerator2 = default(List<Actor>.Enumerator);
                    }
                    dies = false;
                    item = null;
                    goto IL_05F6;
                }
            }
            if (damage > 0)
            {
                yield return new Instruction_ChangeHP(target, -damage);
            }
            Actor actor3 = target as Actor;
            if (actor3 != null && (forceAddPlayLogEntry || ActionUtility.TargetConcernsPlayer(target)))
            {
                string text = ((!extraReason.NullOrEmpty()) ? (" (" + extraReason + ")") : "");
                string text2;
                if (dies)
                {
                    text2 = "EntityTakesDamageAndDies".Translate(target, RichText.HP(damage.ToStringCached()), damageType.Adjective);
                    text2 = text2.Concatenated(text);
                    if (actor3.KilledExperience != 0)
                    {
                        text2 = text2.AppendedWithSpace("({0})".Formatted("EntityTakesDamageAndDies_Exp".Translate(actor3.KilledExperience)));
                    }
                }
                else if (damage == 0)
                {
                    text2 = "EntityTakesNoDamage".Translate(target, damageType.Adjective);
                    text2 = text2.Concatenated(text);
                }
                else
                {
                    text2 = "EntityTakesDamage".Translate(target, RichText.HP(damage.ToStringCached()), damageType.Adjective);
                    text2 = text2.Concatenated(text);
                }
                yield return new Instruction_PlayLog(text2);
            }
        IL_05F6:
            foreach (Instruction instruction3 in InstructionSets_Entity.DamageVisualEffects(target, damage, damageType, impactSource, exactImpactPos, new Color(0.8f, 0.2f, 0.2f), dies, critical, smallerVisuals, null))
            {
                yield return instruction3;
            }
            enumerator = null;
            if (dies)
            {
                foreach (Instruction instruction4 in InstructionSets_Entity.Destroy(target, impactSource, responsible))
                {
                    yield return instruction4;
                }
                enumerator = null;
            }
            else
            {
                Actor actor = target as Actor;
                if (actor != null)
                {
                    foreach (Instruction instruction5 in InstructionSets_Actor.RespondToTakenDamage(actor, impactSource))
                    {
                        yield return instruction5;
                    }
                    enumerator = null;
                    foreach (Instruction instruction6 in InstructionSets_Actor.CheckAddConditionsFromTakenDamage(actor, null, damage, damageType, impactSource, forceAddPlayLogEntry))
                    {
                        yield return instruction6;
                    }
                    enumerator = null;
                }
                actor = null;
            }
            Actor actor4 = target as Actor;
            if (actor4 != null)
            {
                foreach (Instruction instruction7 in InstructionSets_Actor.PostTakenDamage(actor4, damage, damageType, false, canConsumeEquippedItemsCharges))
                {
                    yield return instruction7;
                }
                enumerator = null;
            }
            if (damage > 0 && target.IsPlayerParty && responsible != null && responsible != target)
            {
                yield return new Instruction_ChangeHPLostFromEnemies(damage);
            }
            if (damage > 0 && target.IsPlayerParty)
            {
                yield return new Instruction_ChangeHPLost(damage);
            }
            if (damage > 0 && target.IsNowControlledActor && responsible != null && responsible != target)
            {
                yield return new Instruction_Immediate(delegate
                {
                    responsible.OnCausedPlayerToLoseHP();
                });
            }
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> Heal(Entity target, int amount, bool forceAddPlayLogEntry = false, bool canAddPlayLog = true)
        {
            if (!target.Spawned)
            {
                Log.Error("Tried to make heal instructions for an unspawned entity. Only spawned entities can be healed. Entity: " + target.ToStringSafe(), false);
                yield break;
            }
            amount = Calc.Clamp(amount, 0, target.MaxHP - target.HP);
            if (amount <= 0)
            {
                yield break;
            }
            yield return new Instruction_ChangeHP(target, amount);
            if (canAddPlayLog && target is Actor && (forceAddPlayLogEntry || ActionUtility.TargetConcernsPlayer(target)))
            {
                yield return new Instruction_PlayLog("EntityGainsHP".Translate(target, RichText.HP(amount)));
            }
            foreach (Instruction instruction in InstructionSets_Entity.HealVisualEffects(target, amount, null))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> DamageVisualEffects(Entity target, int damage, DamageTypeSpec damageType, Vector3Int? impactSource, Vector3? exactImpactPos, Color floatingTextColor, bool dies, bool critical, bool smallerVisuals = false, BodyPart forBodyPart = null)
        {
            if (target.IsNowControlledActor)
            {
                if (damage > 0)
                {
                    if (impactSource != null)
                    {
                        yield return new Instruction_VisualEffect(Get.VisualEffect_PlayerHit_Impact, target.Position);
                    }
                    else
                    {
                        yield return new Instruction_VisualEffect(Get.VisualEffect_PlayerHit_NoImpact, target.Position);
                    }
                }
                if (impactSource != null)
                {
                    yield return new Instruction_AnimateActorOffsetFromImpact((Actor)target, impactSource.Value, critical);
                    if (damageType != Get.DamageType_Fall && (!impactSource.Value.IsAdjacent(target.Position) || Vector3.Dot(Get.CameraTransform.forward, (impactSource.Value - Get.CameraPosition).normalized) <= 0.8f))
                    {
                        yield return new Instruction_PlayerHitMarker(impactSource.Value, (damage > 0) ? new Color(1f, 0f, 0f, 0.55f) : new Color(1f, 1f, 1f, 0.55f));
                    }
                }
            }
            else
            {
                yield return new Instruction_AddFloatingText(target, damage.ToStringCached(), floatingTextColor, smallerVisuals ? 0.5f : 1f, 0f, smallerVisuals ? (-0.05f) : 0f, forBodyPart);
                Actor actor = target as Actor;
                if (actor != null)
                {
                    if (impactSource != null)
                    {
                        yield return new Instruction_AnimateActorOffsetFromImpact(actor, impactSource.Value, critical);
                    }
                    if (damage > 0 && !dies)
                    {
                        yield return new Instruction_SetLastTimeLostHPAnimation(actor);
                    }
                    if (damage > 0)
                    {
                        Vector3 vector;
                        Quaternion quaternion;
                        if (exactImpactPos != null)
                        {
                            vector = exactImpactPos.Value;
                            quaternion = Get.CameraTransform.rotation;
                        }
                        else
                        {
                            vector = actor.RenderPosition;
                            if (impactSource != null && impactSource.Value != actor.Position && impactSource.Value != vector)
                            {
                                quaternion = Quaternion.LookRotation((vector - impactSource.Value).normalized);
                            }
                            else
                            {
                                quaternion = Quaternion.LookRotation(Vector3.up);
                            }
                        }
                        yield return new Instruction_VisualEffect(Get.VisualEffect_ActorHit, vector, quaternion);
                    }
                }
                actor = null;
            }
            yield break;
        }

        public static IEnumerable<Instruction> HealVisualEffects(Entity target, int amount, BodyPart forBodyPart = null)
        {
            if (target != Get.NowControlledActor)
            {
                yield return new Instruction_AddFloatingText(target, amount.ToStringOffset(true), new Color(0.2f, 0.8f, 0.2f), 1f, 0f, 0f, forBodyPart);
            }
            yield break;
        }

        public static IEnumerable<Instruction> Destroy(Entity entity, Vector3Int? impactSource = null, Actor responsible = null)
        {
            if (entity.HP != 0)
            {
                yield return new Instruction_ChangeHP(entity, -entity.HP);
            }
            bool wasSpawned = entity.Spawned;
            Vector3Int posBeforeDestroy = entity.Position;
            if (wasSpawned)
            {
                Actor actor = entity as Actor;
                if (actor != null && actor.IfDestroyedMoveDrawPosToDestInstantly)
                {
                    yield return new Instruction_Immediate(delegate
                    {
                        actor.ActorGOC.TeleportToDestOnDestroyed();
                    });
                }
                Vector3 renderPositionComputedCenter = entity.RenderPositionComputedCenter;
                if (!(entity is Ethereal) && !entity.IsNowControlledActor && (!(entity is Structure) || !entity.Spec.Structure.DisableShatterAnimation))
                {
                    yield return new Instruction_ShatterEffect(entity, impactSource);
                }
                Actor actor5 = entity as Actor;
                if (actor5 != null && impactSource != null && !actor5.IsNowControlledActor && actor5.Position.IsAdjacent(Get.NowControlledActor.Position))
                {
                    yield return new Instruction_VisualEffect(actor5.IsBoss ? Get.VisualEffect_BossDestroyedNearby : Get.VisualEffect_ActorDestroyedNearby, actor5.Position);
                }
                Actor actor6 = entity as Actor;
                if (actor6 != null && actor6.IsNowControlledActor)
                {
                    yield return new Instruction_VisualEffect(Get.VisualEffect_PlayerDestroyed, actor6.Position);
                }
                if ((entity.Spec.DestroyEffect != null || entity is Structure) && !entity.Spec.DisableDestroyEffect)
                {
                    VisualEffectSpec visualEffectSpec;
                    if (entity.Spec.DestroyEffect != null)
                    {
                        visualEffectSpec = entity.Spec.DestroyEffect;
                    }
                    else if (entity.Position.Below().InBounds() && Get.CellsInfo.IsFloorUnder(entity.Position) && Get.CellsInfo.IsFilled(entity.Position.Below()))
                    {
                        visualEffectSpec = Get.VisualEffect_Destroyed;
                    }
                    else
                    {
                        visualEffectSpec = Get.VisualEffect_DestroyedNoFloor;
                    }
                    yield return new Instruction_VisualEffect(visualEffectSpec, entity.Position, entity.Rotation);
                }
                SoundSpec destroySound = EntitySoundUtility.GetDestroySound(entity);
                Vector3? vector = new Vector3?(entity.Position);
                Actor actor7 = entity as Actor;
                yield return new Instruction_Sound(destroySound, vector, (actor7 != null) ? EntitySoundUtility.GetPitchFromScale(actor7) : 1f, 1f);
                foreach (Instruction instruction in InstructionSets_Entity.DeSpawn(entity, false))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
                foreach (Instruction instruction2 in InstructionSets_Entity.DropLoot(entity))
                {
                    yield return instruction2;
                }
                enumerator = null;
                Structure structure = entity as Structure;
                if (structure != null && structure.Spec.Structure.DestroyConnectedOnDestroyed)
                {
                    foreach (Instruction instruction3 in UseEffect_DestroyConnected.DestroyConnectedInstructions(structure.Position, structure.Spec))
                    {
                        yield return instruction3;
                    }
                    enumerator = null;
                }
                Structure structure2 = entity as Structure;
                if (structure2 != null && structure2.Spec.Structure.AutoUseOnDestroyed && structure2.UseEffects.Any)
                {
                    foreach (Instruction instruction4 in InstructionSets_Usable.Use(null, structure2, structure2, null))
                    {
                        yield return instruction4;
                    }
                    enumerator = null;
                }
                Actor actor8 = entity as Actor;
                if (actor8 != null && actor8.UsableOnDeath != null && actor8.UsableOnDeath.UseEffects.Any)
                {
                    foreach (Instruction instruction5 in InstructionSets_Usable.Use(actor8, actor8.UsableOnDeath, actor8, null))
                    {
                        yield return instruction5;
                    }
                    enumerator = null;
                }
            }
            else
            {
                Item item = entity as Item;
                if (item != null && item.ParentInventory != null)
                {
                    foreach (Instruction instruction6 in InstructionSets_Actor.RemoveFromInventory(item))
                    {
                        yield return instruction6;
                    }
                    IEnumerator<Instruction> enumerator = null;
                }
            }
            Actor actor2 = entity as Actor;
            if (actor2 != null && !actor2.IsPlayerParty)
            {
                IEnumerator<Instruction> enumerator;
                if (actor2.KilledExperience != 0)
                {
                    bool flag;
                    if (Get.KillCounter.LastWorthyKillSequence != null)
                    {
                        int currentSequence = Get.TurnManager.CurrentSequence;
                        int? num = Get.KillCounter.LastWorthyKillSequence + 36;
                        flag = (currentSequence <= num.GetValueOrDefault()) & (num != null);
                    }
                    else
                    {
                        flag = false;
                    }
                    if (!flag && Get.Player.CurrentKillCombo != 0)
                    {
                        yield return new Instruction_ChangeCurrentKillCombo(-Get.Player.CurrentKillCombo);
                    }
                    foreach (Instruction instruction7 in InstructionSets_Misc.GiveExperience(actor2.KilledExperience, true, actor2, new Vector3?(posBeforeDestroy + new Vector3(0f, -0.27f, 0f))))
                    {
                        yield return instruction7;
                    }
                    enumerator = null;
                    string text;
                    if (Get.Player.CurrentKillCombo <= 0)
                    {
                        text = "ScoreGain_Kill".Translate();
                    }
                    else if (Get.Player.CurrentKillCombo == 1)
                    {
                        text = "ScoreGain_DoubleKill".Translate();
                    }
                    else if (Get.Player.CurrentKillCombo == 2)
                    {
                        text = "ScoreGain_TripleKill".Translate();
                    }
                    else
                    {
                        text = "ScoreGain_KillCombo".Translate(Get.Player.CurrentKillCombo + 1);
                    }
                    foreach (Instruction instruction8 in InstructionSets_Misc.GainScore(actor2.KilledExperience * 10 + Get.Player.CurrentKillCombo * 10, text, true, true, true))
                    {
                        yield return instruction8;
                    }
                    enumerator = null;
                    yield return new Instruction_ChangeCurrentKillCombo(1);
                    yield return new Instruction_SetLastWorthyKillSequence(Get.TurnManager.CurrentSequence);
                    yield return new Instruction_ChangeExpectedHPLostFromEnemies(2.8f * (float)actor2.KilledExperience * RampUpUtility.ApplyRampUpToFloat(1f, actor2.RampUp, 1.2500001f));
                    if (Get.MainActor.Spawned)
                    {
                        int hpOffset = Math.Min(Get.TraitManager.HPPerWorthyKill, Get.MainActor.MaxHP - Get.MainActor.HP);
                        if (hpOffset > 0)
                        {
                            foreach (Instruction instruction9 in InstructionSets_Entity.Heal(Get.MainActor, hpOffset, false, false))
                            {
                                yield return instruction9;
                            }
                            enumerator = null;
                        }
                        int num2 = Math.Min(Get.TraitManager.ManaPerWorthyKill, Get.MainActor.MaxMana - Get.MainActor.Mana);
                        if (num2 > 0)
                        {
                            foreach (Instruction instruction10 in InstructionSets_Actor.RestoreMana(Get.MainActor, num2, false, false, true))
                            {
                                yield return instruction10;
                            }
                            enumerator = null;
                        }
                        if (Get.Trait_Berserker.IsChosen())
                        {
                            Condition_Berserker condition_Berserker = new Condition_Berserker(Get.Condition_Berserker, 6);
                            Condition firstOfSpec = Get.MainActor.Conditions.GetFirstOfSpec(Get.Condition_Berserker);
                            if (firstOfSpec != null)
                            {
                                foreach (Instruction instruction11 in InstructionSets_Misc.TryRenewCondition(firstOfSpec, condition_Berserker))
                                {
                                    yield return instruction11;
                                }
                                enumerator = null;
                            }
                            else
                            {
                                foreach (Instruction instruction12 in InstructionSets_Misc.AddCondition(condition_Berserker, Get.MainActor.Conditions, false, false))
                                {
                                    yield return instruction12;
                                }
                                enumerator = null;
                            }
                        }
                        if (hpOffset < 0)
                        {
                            int num3 = DamageUtility.ApplyDamageProtectionAndClamp(Get.MainActor, -hpOffset, Get.DamageType_Pacifist);
                            foreach (Instruction instruction13 in InstructionSets_Entity.Damage(Get.MainActor, num3, Get.DamageType_Pacifist, null, null, false, false, null, null, false, true))
                            {
                                yield return instruction13;
                            }
                            enumerator = null;
                        }
                    }
                }
                if (actor2.Spec.Actor.ExtraKillScore != 0)
                {
                    foreach (Instruction instruction14 in InstructionSets_Misc.GainScore(actor2.Spec.Actor.ExtraKillScore, "ScoreGain_SpecificKill".Translate(actor2.Spec.LabelCap), true, true, true))
                    {
                        yield return instruction14;
                    }
                    enumerator = null;
                }
                yield return new Instruction_IncrementKillCount(actor2.Spec);
                yield return new Instruction_SetLastKillSequence(Get.TurnManager.CurrentSequence);
                if (actor2.IsBoss)
                {
                    yield return new Instruction_RegisterKilledBoss(actor2, Get.Floor);
                    yield return new Instruction_ShowBossSlainText(actor2);
                }
                yield return new Instruction_Immediate(delegate
                {
                    Get.CameraEffects.OnActorKilled(actor2);
                });
                if (Get.Quest_KillNightmareLord.IsActive() && actor2.IsBoss && Get.Floor == 1 && actor2.Spec == Get.Entity_Skeleton)
                {
                    yield return new Instruction_CompleteQuest(Get.Quest_KillNightmareLord);
                }
                if (Get.Quest_KillHypnorak.IsActive() && actor2.IsBoss && Get.Floor == 3 && actor2.Spec == Get.Entity_Dragon)
                {
                    yield return new Instruction_CompleteQuest(Get.Quest_KillHypnorak);
                }
                if (Get.Quest_KillPhantasmos.IsActive() && actor2.IsBoss && Get.Floor == 4 && actor2.Spec == Get.Entity_Mummy)
                {
                    yield return new Instruction_CompleteQuest(Get.Quest_KillPhantasmos);
                }
                if (actor2.Spec == Get.Entity_Skeleton && Get.Quest_Introduction.IsActive())
                {
                    yield return new Instruction_ChangeQuestState("Introduction_KillCount", 1);
                    if (Get.TotalQuestsState.Get("Introduction_KillCount") + Get.ThisRunQuestsState.Get("Introduction_KillCount") >= 3)
                    {
                        yield return new Instruction_CompleteQuest(Get.Quest_Introduction);
                    }
                    else
                    {
                        yield return new Instruction_Immediate(delegate
                        {
                            Get.ActiveQuestsReadout.Show();
                        });
                    }
                }
                if (actor2.Spec == Get.Entity_Skeleton && Get.Quest_KillSkeletons.IsActive())
                {
                    yield return new Instruction_ChangeQuestState("KillSkeletons_KillCount", 1);
                    if (Get.TotalQuestsState.Get("KillSkeletons_KillCount") + Get.ThisRunQuestsState.Get("KillSkeletons_KillCount") >= 10)
                    {
                        yield return new Instruction_CompleteQuest(Get.Quest_KillSkeletons);
                    }
                    else
                    {
                        yield return new Instruction_Immediate(delegate
                        {
                            Get.ActiveQuestsReadout.Show();
                        });
                    }
                }
                if (actor2.Spec == Get.Entity_Slime && Get.Quest_KillSlimes.IsActive())
                {
                    yield return new Instruction_ChangeQuestState("KillSlimes_KillCount", 1);
                    if (Get.TotalQuestsState.Get("KillSlimes_KillCount") + Get.ThisRunQuestsState.Get("KillSlimes_KillCount") >= 10)
                    {
                        yield return new Instruction_CompleteQuest(Get.Quest_KillSlimes);
                    }
                    else
                    {
                        yield return new Instruction_Immediate(delegate
                        {
                            Get.ActiveQuestsReadout.Show();
                        });
                    }
                }
                if (actor2.Spec == Get.Entity_Mimic && Get.Quest_KillMimics.IsActive())
                {
                    yield return new Instruction_ChangeQuestState("KillMimics_KillCount", 1);
                    if (Get.TotalQuestsState.Get("KillMimics_KillCount") + Get.ThisRunQuestsState.Get("KillMimics_KillCount") >= 10)
                    {
                        yield return new Instruction_CompleteQuest(Get.Quest_KillMimics);
                    }
                    else
                    {
                        yield return new Instruction_Immediate(delegate
                        {
                            Get.ActiveQuestsReadout.Show();
                        });
                    }
                }
                if (actor2.Spec == Get.Entity_Rat && Get.Quest_RatInfestation.IsActive() && Get.Floor == 2)
                {
                    yield return new Instruction_ChangeQuestState("RatInfestation_KillCount", 1);
                    if (Get.ThisRunQuestsState.Get("RatInfestation_KillCount") >= 5)
                    {
                        yield return new Instruction_CompleteQuest(Get.Quest_RatInfestation);
                    }
                }
                if (actor2.Spec == Get.Entity_Phantom && Get.Quest_PhantomSwarm.IsActive() && Get.Floor == 3)
                {
                    yield return new Instruction_ChangeQuestState("PhantomSwarm_KillCount", 1);
                    if (Get.ThisRunQuestsState.Get("PhantomSwarm_KillCount") >= 3)
                    {
                        yield return new Instruction_CompleteQuest(Get.Quest_PhantomSwarm);
                    }
                }
                if (Get.LessonManager.CurrentLesson == Get.Lesson_Fighting)
                {
                    yield return new Instruction_Immediate(delegate
                    {
                        Get.LessonManager.FinishIfCurrent(Get.Lesson_Fighting);
                    });
                }
                else if (Get.LessonManager.CurrentLesson == Get.Lesson_TargetingBodyParts)
                {
                    yield return new Instruction_Immediate(delegate
                    {
                        Get.LessonManager.FinishIfCurrent(Get.Lesson_TargetingBodyParts);
                    });
                }
                if (actor2.Spec == Get.Entity_Googon && !Get.Achievement_KillGoogon.IsCompleted)
                {
                    yield return new Instruction_CompleteAchievement(Get.Achievement_KillGoogon);
                }
                if (actor2.Spec == Get.Entity_Demon && !Get.Achievement_KillDemon.IsCompleted)
                {
                    yield return new Instruction_CompleteAchievement(Get.Achievement_KillDemon);
                }
                if (actor2.Spec == Get.Entity_Shopkeeper && !Get.Achievement_KillShopkeeper.IsCompleted)
                {
                    yield return new Instruction_CompleteAchievement(Get.Achievement_KillShopkeeper);
                }
                if (actor2.Spec == Get.Entity_Ghost && !Get.Achievement_KillGhost.IsCompleted)
                {
                    yield return new Instruction_CompleteAchievement(Get.Achievement_KillGhost);
                }
                foreach (Instruction instruction15 in InstructionSets_Actor.CheckAngerFaction(actor2))
                {
                    yield return instruction15;
                }
                enumerator = null;
                foreach (Actor actor9 in Get.PlayerParty.ToTemporaryList<Actor>())
                {
                    if (actor2 != actor9)
                    {
                        foreach (Condition condition in actor9.ConditionsAccumulated.AllConditions.ToTemporaryList<Condition>())
                        {
                            foreach (Instruction instruction16 in condition.MakeOtherActorDestroyedInstructions(actor2))
                            {
                                yield return instruction16;
                            }
                            enumerator = null;
                        }
                        List<Condition>.Enumerator enumerator3 = default(List<Condition>.Enumerator);
                    }
                }
                List<Actor>.Enumerator enumerator2 = default(List<Actor>.Enumerator);
                if (responsible != null && responsible.IsPlayerParty && !Get.InLobby)
                {
                    if (responsible.ChargedAttack == 0)
                    {
                        yield return new Instruction_SetChargedAttack(responsible, 1);
                    }
                    Action_Use action_Use = Action.ExecutingAction as Action_Use;
                    if (action_Use != null && action_Use.User == responsible)
                    {
                        action_Use.ResetChargedAttackToOne = true;
                    }
                }
            }
            if (Get.LessonManager.CurrentLesson == Get.Lesson_QuickAttack && entity.Spec == Get.Entity_Crates && Get.World.GetEntitiesOfSpec(Get.Entity_Crates).Count <= 1)
            {
                yield return new Instruction_Immediate(delegate
                {
                    Get.LessonManager.FinishIfCurrent(Get.Lesson_QuickAttack);
                });
            }
            List<EntityComp>.Enumerator enumerator5;
            if (wasSpawned)
            {
                Actor actor4 = entity as Actor;
                if (actor4 != null)
                {
                    foreach (Entity entity2 in Get.World.GetEntitiesAt(posBeforeDestroy).ToTemporaryList<Entity>())
                    {
                        foreach (EntityComp entityComp in entity2.AllComps)
                        {
                            foreach (Instruction instruction17 in entityComp.MakeActorDestroyedHereInstructions(actor4))
                            {
                                yield return instruction17;
                            }
                            IEnumerator<Instruction> enumerator = null;
                        }
                        enumerator5 = default(List<EntityComp>.Enumerator);
                    }
                    List<Entity>.Enumerator enumerator4 = default(List<Entity>.Enumerator);
                }
            }
            foreach (EntityComp entityComp2 in entity.AllComps)
            {
                foreach (Instruction instruction18 in entityComp2.MakeParentDestroyedInstructions(wasSpawned ? new Vector3Int?(posBeforeDestroy) : null))
                {
                    yield return instruction18;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            enumerator5 = default(List<EntityComp>.Enumerator);
            Actor actor10 = entity as Actor;
            if (actor10 != null && actor10.IsPlayerParty && !actor10.IsMainActor)
            {
                yield return new Instruction_RemovePartyMember(actor10);
            }
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> DropLoot(Entity entity)
        {
            Actor actor = entity as Actor;
            if (actor != null && !actor.IsMainActor)
            {
                foreach (Item loot in actor.Inventory.AllItems.ToTemporaryList<Item>())
                {
                    foreach (Instruction instruction in InstructionSets_Actor.RemoveFromInventory(loot))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator2 = null;
                    foreach (Instruction instruction2 in InstructionSets_Entity.Spawn(loot, SpawnPositionFinder.Near(actor.Position, loot, false, false, null), null))
                    {
                        yield return instruction2;
                    }
                    enumerator2 = null;
                    yield return new Instruction_StartItemJumpAnimation(loot);
                    loot = null;
                }
                List<Item>.Enumerator enumerator = default(List<Item>.Enumerator);
            }
            else
            {
                Structure structure = entity as Structure;
                if (structure != null)
                {
                    foreach (Entity loot2 in structure.InnerEntities.ToTemporaryList<Entity>())
                    {
                        yield return new Instruction_RemoveFromInnerEntities(loot2, structure);
                        foreach (Instruction instruction3 in InstructionSets_Entity.Spawn(loot2, SpawnPositionFinder.Near(structure.Position, loot2, false, true, null), null))
                        {
                            yield return instruction3;
                        }
                        IEnumerator<Instruction> enumerator2 = null;
                        Item loot = loot2 as Item;
                        if (loot != null)
                        {
                            yield return new Instruction_StartItemJumpAnimation(loot);
                            if (Get.NowControlledActor.Spawned && Get.NowControlledActor.Position == loot.Position && !loot.ForSale && InstructionSets_Actor.HasSpaceToPickUp(Get.NowControlledActor, loot))
                            {
                                foreach (Instruction instruction4 in InstructionSets_Actor.PickUpItem(Get.NowControlledActor, loot, false))
                                {
                                    yield return instruction4;
                                }
                                enumerator2 = null;
                            }
                        }
                        loot = null;
                        loot2 = null;
                    }
                    List<Entity>.Enumerator enumerator3 = default(List<Entity>.Enumerator);
                }
                structure = null;
            }
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> Push(Entity entity, Vector3Int directionNonNormalized, int distance, bool pulling = false, bool canAddPlayLog = true, bool playSound = true)
        {
            if (!entity.Spawned)
            {
                Log.Error("Tried to push an unspawned entity: " + entity.ToStringSafe(), false);
                yield break;
            }
            if (canAddPlayLog && ActionUtility.TargetConcernsPlayer(entity))
            {
                yield return new Instruction_PlayLog((pulling ? "EntityIsPulled" : "EntityIsPushed").Translate(entity));
            }
            bool playedSound = false;
            bool flag = false;
            Func<Vector3Int> <> 9__0;
            Func<Vector3Int> func;
            if ((func = <> 9__0) == null)
            {
                func = (<> 9__0 = () => entity.Position);
            }
            foreach (Vector3Int cell in PushUtility.Push(func, directionNonNormalized, distance).Skip<Vector3Int>(1))
            {
                if (!playedSound && playSound)
                {
                    playedSound = true;
                    yield return new Instruction_Sound(Get.Sound_EntityPushed, new Vector3?(entity.Position), 1f, 1f);
                }
                foreach (Instruction instruction in InstructionSets_Entity.Move(entity, cell, false, true))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator2 = null;
                flag = true;
                if (!entity.Spawned)
                {
                    break;
                }
                cell = default(Vector3Int);
            }
            IEnumerator<Vector3Int> enumerator = null;
            if (flag && entity.Spawned)
            {
                foreach (Instruction instruction2 in InstructionSets_Entity.CheckItemsAndStructuresFallAt(entity.Position))
                {
                    yield return instruction2;
                }
                IEnumerator<Instruction> enumerator2 = null;
            }
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> Identify(Item item, int identifyTurns, bool canAddPlayLog = true)
        {
            if (item.Identified)
            {
                yield break;
            }
            bool identified = item.TurnsLeftToIdentify <= identifyTurns;
            yield return new Instruction_ChangeTurnsLeftToIdentify(item, -Math.Min(identifyTurns, item.TurnsLeftToIdentify));
            if (identified)
            {
                if (Get.IdentificationGroups.ShouldAddToIdentifiedListOnIdentified(item.Spec))
                {
                    yield return new Instruction_AddToIdentifiedList(item.Spec);
                }
                if (canAddPlayLog)
                {
                    yield return new Instruction_PlayLog("ItemIdentified".Translate(item));
                    yield return new Instruction_Sound(Get.Sound_ItemIdentified, null, 1f, 1f);
                }
            }
            yield break;
        }

        private static IEnumerable<Instruction> CheckItemsAndStructuresFallAt(Vector3Int pos)
        {
            Vector3Int vector3Int = pos.Below();
            if (vector3Int.InBounds() && Get.CellsInfo.CanPassThroughNoActors(vector3Int) && Get.World.AnyEntityAt(pos))
            {
                foreach (Entity entity in Get.World.GetEntitiesAt(pos).ToTemporaryList<Entity>())
                {
                    InstructionSets_Entity.<> c__DisplayClass11_0 CS$<> 8__locals1;
                    CS$<> 8__locals1.entity = entity;
                    if (CS$<> 8__locals1.entity.Spawned && !(CS$<> 8__locals1.entity.Position != pos))
					{
                        if (!(CS$<> 8__locals1.entity is Item))
						{
                            Structure structure = CS$<> 8__locals1.entity as Structure;
                            if (structure == null || structure.Spec.Structure.FallBehavior != StructureFallBehavior.Fall)
                            {
                                continue;
                            }
                        }
                        if (!ItemOrStructureFallUtility.HasSupport(CS$<> 8__locals1.entity))
                        {
                            Vector3Int destination = pos;
                            while (!Get.CellsInfo.AnyGivesGravitySupportInside(destination) && destination.Below().InBounds() && InstructionSets_Entity.< CheckItemsAndStructuresFallAt > g__CanPassThroughCheck | 11_0(destination.Below(), ref CS$<> 8__locals1))
                            {
                                destination = destination.Below();
                            }
                            if (destination != pos)
                            {
                                foreach (Instruction instruction in InstructionSets_Entity.Move(CS$<> 8__locals1.entity, destination, true, true))
                                {
                                    yield return instruction;
                                }
                                IEnumerator<Instruction> enumerator2 = null;
                                yield return new Instruction_VisualEffect(Get.VisualEffect_ItemOrStructureFall, destination);
                                Item item = CS$<> 8__locals1.entity as Item;
                                if (item != null && Get.NowControlledActor.Spawned && Get.NowControlledActor.Position == item.Position && !item.ForSale && InstructionSets_Actor.HasSpaceToPickUp(Get.NowControlledActor, item))
                                {
                                    foreach (Instruction instruction2 in InstructionSets_Actor.PickUpItem(Get.NowControlledActor, item, false))
                                    {
                                        yield return instruction2;
                                    }
                                    enumerator2 = null;
                                }
                            }
                            destination = default(Vector3Int);
                            CS$<> 8__locals1 = default(InstructionSets_Entity.<> c__DisplayClass11_0);
                        }
                    }
                }
                List<Entity>.Enumerator enumerator = default(List<Entity>.Enumerator);
            }
            if (Get.World.AnyEntityAt(pos))
            {
                foreach (Entity entity2 in Get.World.GetEntitiesAt(pos).ToTemporaryList<Entity>())
                {
                    if (entity2.Spawned && !(entity2.Position != pos))
                    {
                        Structure structure2 = entity2 as Structure;
                        if (structure2 != null && structure2.Spec.Structure.FallBehavior == StructureFallBehavior.Destroy && !ItemOrStructureFallUtility.HasSupport(entity2))
                        {
                            foreach (Instruction instruction3 in InstructionSets_Entity.Destroy(entity2, null, null))
                            {
                                yield return instruction3;
                            }
                            IEnumerator<Instruction> enumerator2 = null;
                        }
                    }
                }
                List<Entity>.Enumerator enumerator = default(List<Entity>.Enumerator);
            }
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> PreViolentlyAttackedBy(Entity victim, Actor attacker)
        {
            Actor actor = victim as Actor;
            if (actor != null)
            {
                yield return new Instruction_SetLastViolentlyAttackedBy(actor, attacker);
            }
            yield break;
        }

        public static IEnumerable<Instruction> PostViolentlyAttackedBy(Entity victim, Actor attacker)
        {
            Actor victimActor = victim as Actor;
            IEnumerator<Instruction> enumerator;
            if (victimActor != null && attacker.IsPlayerParty)
            {
                Condition firstOfSpec = victimActor.Conditions.GetFirstOfSpec(Get.Condition_Shield);
                if (firstOfSpec != null)
                {
                    foreach (Instruction instruction in InstructionSets_Misc.RemoveCondition(firstOfSpec, false, false))
                    {
                        yield return instruction;
                    }
                    enumerator = null;
                    yield return new Instruction_PlayLog("ShieldRemovedBecauseHarmed".Translate(victimActor));
                }
            }
            foreach (Instruction instruction2 in InstructionSets_Noise.MakeNoise(victim.Position, 1, NoiseType.GotAttacked, attacker, false, true))
            {
                yield return instruction2;
            }
            enumerator = null;
            yield break;
            yield break;
        }

        [CompilerGenerated]
        internal static bool <CheckItemsAndStructuresFallAt>g__CanPassThroughCheck|11_0(Vector3Int at, ref InstructionSets_Entity.<>c__DisplayClass11_0 A_1)
		{
			if (A_1.entity is Item)
			{
				return Get.CellsInfo.CanPassThroughNoActors(at);
			}
			return Get.CellsInfo.CanPassThrough(at);
		}
	}
}