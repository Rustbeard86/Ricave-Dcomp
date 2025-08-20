using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public static class InstructionSets_Actor
    {
        public static IEnumerable<Instruction> PickUpItem(Actor actor, Item item, bool pickedUpByPet = false)
        {
            foreach (Instruction instruction in InstructionSets_Entity.DeSpawn(item, false))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            foreach (Instruction instruction2 in InstructionSets_Actor.AddToInventory(actor, item))
            {
                yield return instruction2;
            }
            enumerator = null;
            if (item.Spec.IsLobbyItemOrLobbyRelated)
            {
                yield return new Instruction_Sound(Get.Sound_PickUpSpecialItem, new Vector3?(actor.Position), 1f, 1f);
            }
            else
            {
                yield return new Instruction_Sound(Get.Sound_PickUpItem, new Vector3?(actor.Position), 1f, 1f);
            }
            if (actor.IsPlayerParty)
            {
                string text = "EntityPicksUpItem".Translate(pickedUpByPet ? Get.Entity_PetRat.LabelAdjustedCap : actor, item);
                if (item.Spec == Get.Entity_Gold)
                {
                    text = "{0} ({1})".Formatted(text, "TotalItems".Translate(Get.Player.Gold));
                }
                else if (item.Spec == Get.Entity_Stardust)
                {
                    text = "{0} ({1})".Formatted(text, "TotalItems".Translate(Get.TotalLobbyItems.Stardust + Get.ThisRunLobbyItems.Stardust));
                }
                else if (item.Spec == Get.Entity_AncientMechanism)
                {
                    text = "{0} ({1}/{2})".Formatted(text, actor.Inventory.GetCount(Get.Entity_AncientMechanism, true).ToStringCached(), 3.ToStringCached());
                }
                else if (item.Spec == Get.Entity_GreenKeyFragment)
                {
                    text = "{0} ({1}/{2})".Formatted(text, actor.Inventory.GetCount(Get.Entity_GreenKeyFragment, true).ToStringCached(), 3.ToStringCached());
                }
                else if (item.Spec == Get.Entity_MoonlightMedallion)
                {
                    text = "{0} ({1}/{2})".Formatted(text, Get.ThisRunLobbyItems.GetCount(Get.Entity_MoonlightMedallion).ToStringCached(), 4.ToStringCached());
                }
                else if (item.Spec == Get.Entity_Note && !Get.InLobby)
                {
                    text = "{0} ({1}/{2})".Formatted(text, (Get.Progress.CollectedNoteSpecs.Count + Get.Player.CollectedNoteSpecs.Count).ToStringCached(), Get.Specs.GetAll<NoteSpec>().Count.ToStringCached());
                }
                else if (item.Spec == Get.Entity_PuzzlePiece)
                {
                    text = "{0} ({1}/{2})".Formatted(text, (Get.TotalLobbyItems.GetCount(Get.Entity_PuzzlePiece) + Get.ThisRunLobbyItems.GetCount(Get.Entity_PuzzlePiece)).ToStringCached(), 10.ToStringCached());
                }
                yield return new Instruction_PlayLog(text);
                if (Get.LessonManager.CurrentLesson == Get.Lesson_PickingUpItems)
                {
                    yield return new Instruction_Immediate(delegate
                    {
                        Get.LessonManager.FinishIfCurrent(Get.Lesson_PickingUpItems);
                    });
                }
            }
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> AddToInventory(Actor actor, Item item)
        {
            if (item.Spawned)
            {
                Log.Error("Tried to make instructions for adding spawned item to inventory. This item should be despawned first. Item: " + item.ToStringSafe(), false);
                yield break;
            }
            if (item.ForSale && actor.IsPlayerParty)
            {
                yield return new Instruction_RemovePriceTag(item);
            }
            UnlockableAsItem unlockable = item as UnlockableAsItem;
            if (unlockable != null && actor.IsPlayerParty)
            {
                yield return new Instruction_Immediate(delegate
                {
                    if (!Get.UnlockableManager.IsUnlocked(unlockable.UnlockableSpec, null))
                    {
                        Get.UnlockableManager.Unlock(unlockable.UnlockableSpec);
                    }
                });
            }
            else
            {
                PrivateRoomStructureAsItem privateRoomStructure = item as PrivateRoomStructureAsItem;
                if (privateRoomStructure != null && actor.IsPlayerParty)
                {
                    yield return new Instruction_ChangePrivateRoomStructures(privateRoomStructure.PrivateRoomStructure, item.StackCount);
                    if (privateRoomStructure.PrivateRoomStructure == Get.Entity_Relic && Get.Quest_FindRelic.IsActive())
                    {
                        yield return new Instruction_CompleteQuest(Get.Quest_FindRelic);
                    }
                }
                else if (item.Spec == Get.Entity_Gold && actor.IsPlayerParty)
                {
                    yield return new Instruction_ChangePlayerGold(item.StackCount);
                }
                else if (item.Spec.Item.LobbyItem && actor.IsPlayerParty)
                {
                    InstructionSets_Actor.<> c__DisplayClass1_1 CS$<> 8__locals2 = new InstructionSets_Actor.<> c__DisplayClass1_1();
                    yield return new Instruction_ChangeLobbyItems(item.Spec, item.StackCount);
                    if (item.Spec == Get.Entity_MemoryPiece1 && Get.Quest_MemoryPiece1.IsActive())
                    {
                        yield return new Instruction_CompleteQuest(Get.Quest_MemoryPiece1);
                    }
                    else if (item.Spec == Get.Entity_MemoryPiece2 && Get.Quest_MemoryPiece2.IsActive())
                    {
                        yield return new Instruction_CompleteQuest(Get.Quest_MemoryPiece2);
                    }
                    else if (item.Spec == Get.Entity_MemoryPiece3 && Get.Quest_MemoryPiece3.IsActive())
                    {
                        yield return new Instruction_CompleteQuest(Get.Quest_MemoryPiece3);
                    }
                    else if (item.Spec == Get.Entity_MemoryPiece4 && Get.Quest_MemoryPiece4.IsActive())
                    {
                        yield return new Instruction_CompleteQuest(Get.Quest_MemoryPiece4);
                    }
                    if (item.Spec == Get.Entity_MoonlightMedallion && Get.Quest_MoonlightMedallions.IsActive() && Get.ThisRunLobbyItems.GetCount(Get.Entity_MoonlightMedallion) >= 4)
                    {
                        yield return new Instruction_CompleteQuest(Get.Quest_MoonlightMedallions);
                    }
                    if (item.Spec == Get.Entity_MemoryPiece1 || item.Spec == Get.Entity_MemoryPiece2 || item.Spec == Get.Entity_MemoryPiece3 || item.Spec == Get.Entity_MemoryPiece4)
                    {
                        yield return new Instruction_Immediate(delegate
                        {
                            ((Window_MemoryPiece)Get.WindowManager.Open(Get.Window_MemoryPiece, true)).SetMemoryPiece(item.Spec);
                        });
                    }
                    CS$<> 8__locals2.note = item as Note;
                    if (CS$<> 8__locals2.note != null)
					{
                        if (CS$<> 8__locals2.note.NoteSpec != null)
						{
                            yield return new Instruction_AddToCollectedNotes(CS$<> 8__locals2.note.NoteSpec);
                            yield return new Instruction_Immediate(delegate
                            {
                                ((Window_Note)Get.WindowManager.Open(Get.Window_Note, true)).SetNoteSpec(CS$<> 8__locals2.note.NoteSpec);
                            });
                        }

                        else if (Get.InLobby)
                        {
                            yield return new Instruction_Immediate(delegate
                            {
                                ((Window_Note)Get.WindowManager.Open(Get.Window_Note, true)).SetCustomText("DemonNoteText".Translate());
                            });
                            yield return new Instruction_Immediate(delegate
                            {
                                Get.Progress.DemonNotePickedUp = true;
                            });
                        }
                    }
                    if (item.Spec == Get.Entity_Stardust && !Get.Player.ShownStardustInfo && !Get.Progress.EverShownStardustInfo)
                    {
                        yield return new Instruction_Immediate(delegate
                        {
                            Get.Player.OnShownStardustInfo();
                        });
                        yield return new Instruction_Immediate(delegate
                        {
                            ((Window_TutorialPopup)Get.WindowManager.Open(Get.Window_TutorialPopup, true)).SetText(Get.Entity_Stardust.Icon, "StardustInfo".Translate());
                        });
                    }
                    if (item.Spec == Get.Entity_PuzzlePiece && !Get.Player.ShownPuzzlePieceInfo && !Get.Progress.EverShownPuzzlePieceInfo)
                    {
                        yield return new Instruction_Immediate(delegate
                        {
                            Get.Player.OnShownPuzzlePieceInfo();
                        });
                        yield return new Instruction_Immediate(delegate
                        {
                            ((Window_TutorialPopup)Get.WindowManager.Open(Get.Window_TutorialPopup, true)).SetText(Get.Entity_PuzzlePiece.Icon, "PuzzlePieceInfo".Translate(10));
                        });
                    }
                    if (Get.TotalLobbyItems.GetCount(Get.Entity_PuzzlePiece) + Get.ThisRunLobbyItems.GetCount(Get.Entity_PuzzlePiece) >= 10 && !Get.Achievement_CollectAllPuzzlePieces.IsCompleted)
                    {
                        yield return new Instruction_CompleteAchievement(Get.Achievement_CollectAllPuzzlePieces);
                    }
                    CS$<> 8__locals2 = null;
                }
                else
                {
                    Item itemToStackWith = actor.Inventory.GetItemToStackWith(item);
                    if (itemToStackWith != null)
                    {
                        yield return new Instruction_ChangeStackCount(itemToStackWith, item.StackCount);
                        yield return new Instruction_ChangeStackCount(item, -item.StackCount);
                    }
                    else
                    {
                        yield return new Instruction_AddToInventory(actor, item, null, null);
                        if (actor.IsPlayerParty && actor.Inventory.AllItems.Any())
                        {
                            yield return new Instruction_Immediate(delegate
                            {
                                item.IdentifyOrder = actor.Inventory.AllItems.Max<Item>((Item x) => x.IdentifyOrder) + 1;
                            });
                        }
                    }
                }
                privateRoomStructure = null;
            }
            yield break;
        }

        public static IEnumerable<Instruction> AddToInventoryOrSpawnNear(Actor actor, Item item)
        {
            if (item.Spawned)
            {
                Log.Error("Tried to make instructions for spawning or adding spawned item to inventory. This item should be despawned first. Item: " + item.ToStringSafe(), false);
                yield break;
            }
            if (InstructionSets_Actor.HasSpaceToPickUp(actor, item))
            {
                foreach (Instruction instruction in InstructionSets_Actor.AddToInventory(actor, item))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            else
            {
                Vector3Int vector3Int = SpawnPositionFinder.Near(actor.Position, item, false, false, null);
                foreach (Instruction instruction2 in InstructionSets_Entity.Spawn(item, vector3Int, null))
                {
                    yield return instruction2;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> RemoveFromInventory(Item item)
        {
            Inventory inventory = item.ParentInventory;
            if (inventory == null)
            {
                yield break;
            }
            bool wasEquipped = inventory.Equipped.IsEquipped(item);
            yield return new Instruction_RemoveFromInventory(item);
            if (wasEquipped)
            {
                foreach (Instruction instruction in InstructionSets_Actor.PostUnequip(item, inventory.Owner))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> Equip(Actor actor, Item item)
        {
            yield return new Instruction_Equip(actor, item);
            foreach (Instruction instruction in InstructionSets_Actor.PostEquip(item))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        private static IEnumerable<Instruction> PostEquip(Item item)
        {
            foreach (Instruction instruction in InstructionSets_Actor.CheckEquippedCursedItem(item, true))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            foreach (Condition condition in item.ConditionsEquipped.All.ToTemporaryList<Condition>())
            {
                foreach (Instruction instruction2 in condition.MakeOnNewlyAffectingActorInstructions())
                {
                    yield return instruction2;
                }
                enumerator = null;
            }
            List<Condition>.Enumerator enumerator2 = default(List<Condition>.Enumerator);
            if (Get.LessonManager.CurrentLesson == Get.Lesson_Inventory && item.ParentInventory.Owner.IsNowControlledActor)
            {
                yield return new Instruction_Immediate(delegate
                {
                    Get.LessonManager.FinishIfCurrent(Get.Lesson_Inventory);
                });
            }
            yield break;
            yield break;
        }

        private static IEnumerable<Instruction> PostUnequip(Item item, Actor actor)
        {
            foreach (Condition condition in item.ConditionsEquipped.All.ToTemporaryList<Condition>())
            {
                foreach (Instruction instruction in condition.MakeOnNoLongerAffectingActorInstructions(actor))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator2 = null;
            }
            List<Condition>.Enumerator enumerator = default(List<Condition>.Enumerator);
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> CheckEquippedCursedItem(Item item, bool canAddPlayLogEntry = true)
        {
            if (!item.Cursed || item.ParentInventory == null || item.ParentInventory.Owner == null || !item.ParentInventory.Owner.IsPlayerParty)
            {
                yield break;
            }
            IEnumerator<Instruction> enumerator;
            if (!item.Identified)
            {
                foreach (Instruction instruction in InstructionSets_Entity.Identify(item, item.TurnsLeftToIdentify, false))
                {
                    yield return instruction;
                }
                enumerator = null;
                if (canAddPlayLogEntry)
                {
                    yield return new Instruction_PlayLog("CursedItemIdentifiedAfterEquipping".Translate(item, RichText.Cursed("CursedLower".Translate())));
                }
            }
            if (item.ParentInventory.Owner == Get.NowControlledActor)
            {
                yield return new Instruction_Sound(Get.Sound_EquipCursed, null, 1f, 1f);
            }
            foreach (Instruction instruction2 in InstructionSets_Misc.ResetTurnsCanRewind())
            {
                yield return instruction2;
            }
            enumerator = null;
            yield break;
            yield break;
        }

        public static bool HasSpaceToPickUp(Actor actor, Item item)
        {
            if (actor.IsPlayerParty)
            {
                if (item is UnlockableAsItem || item is PrivateRoomStructureAsItem)
                {
                    return true;
                }
                if (item.Spec == Get.Entity_Gold || item.Spec.Item.LobbyItem)
                {
                    return true;
                }
            }
            return actor.Inventory.GetItemToStackWith(item) != null || !actor.Inventory.BackpackAndQuickbarFull;
        }

        public static IEnumerable<Instruction> RemoveOneFromInventory(Item item)
        {
            foreach (Instruction instruction in InstructionSets_Actor.RemoveCountFromInventory(item, 1))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> RemoveCountFromInventory(Item item, int count)
        {
            if (count <= 0)
            {
                yield break;
            }
            if (item.StackCount <= count)
            {
                foreach (Instruction instruction in InstructionSets_Actor.RemoveFromInventory(item))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            else
            {
                yield return new Instruction_ChangeStackCount(item, -count);
            }
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> RemoveCountFromInventoryAndGiveTo(Item item, int count, Actor giveTo)
        {
            if (count <= 0)
            {
                yield break;
            }
            if (item.StackCount <= count)
            {
                foreach (Instruction instruction in InstructionSets_Actor.RemoveFromInventory(item))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
                foreach (Instruction instruction2 in InstructionSets_Actor.AddToInventoryOrSpawnNear(giveTo, item))
                {
                    yield return instruction2;
                }
                enumerator = null;
            }
            else
            {
                Item split = item.SplitOff(count);
                yield return new Instruction_ChangeStackCount(item, -count);
                foreach (Instruction instruction3 in InstructionSets_Actor.AddToInventoryOrSpawnNear(giveTo, split))
                {
                    yield return instruction3;
                }
                IEnumerator<Instruction> enumerator = null;
                split = null;
            }
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> DamageBodyPart(BodyPart bodyPart, int damage, DamageTypeSpec damageType, string extraReason, Vector3Int? impactSource, bool critical, bool forceAddPlayLogEntry = false, Vector3? exactImpactPos = null, bool smallerVisuals = false, bool canConsumeEquippedItemsCharges = true, bool canAngerFaction = false)
        {
            if (bodyPart.IsMissing || !bodyPart.Actor.Spawned)
            {
                Log.Error("Tried to make damage body part instructions for an unspawned actor or missing body part. Only spawned entities can be harmed. Actor: " + bodyPart.Actor.ToStringSafe(), false);
                yield break;
            }
            damage = Calc.Clamp(damage, 0, bodyPart.HP);
            bool destroyed = bodyPart.HP <= damage;
            if (damage > 0)
            {
                yield return new Instruction_ChangeBodyPartHP(bodyPart, -damage);
            }
            if (forceAddPlayLogEntry || ActionUtility.TargetConcernsPlayer(bodyPart.Actor))
            {
                string text = ((!extraReason.NullOrEmpty()) ? (" (" + extraReason + ")") : "");
                string text2;
                if (destroyed)
                {
                    text2 = "BodyPartTakesDamageAndIsDestroyed".Translate(bodyPart, RichText.HP(damage.ToStringCached()), damageType.Adjective);
                }
                else if (damage == 0)
                {
                    text2 = "BodyPartTakesNoDamage".Translate(bodyPart, damageType.Adjective);
                }
                else
                {
                    text2 = "BodyPartTakesDamage".Translate(bodyPart, RichText.HP(damage.ToStringCached()), damageType.Adjective);
                }
                yield return new Instruction_PlayLog(text2 + text);
            }
            foreach (Instruction instruction in InstructionSets_Entity.DamageVisualEffects(bodyPart.Actor, damage, damageType, impactSource, exactImpactPos, new Color(0.8f, 0.6f, 0.2f), destroyed, critical, smallerVisuals, bodyPart))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            if (destroyed)
            {
                foreach (Instruction instruction2 in InstructionSets_Actor.DestroyBodyPart(bodyPart, impactSource, false, false, canAngerFaction))
                {
                    yield return instruction2;
                }
                enumerator = null;
            }
            foreach (Instruction instruction3 in InstructionSets_Actor.RespondToTakenDamage(bodyPart.Actor, impactSource))
            {
                yield return instruction3;
            }
            enumerator = null;
            foreach (Instruction instruction4 in InstructionSets_Actor.CheckAddConditionsFromTakenDamage(bodyPart.Actor, bodyPart, damage, damageType, impactSource, forceAddPlayLogEntry))
            {
                yield return instruction4;
            }
            enumerator = null;
            foreach (Instruction instruction5 in InstructionSets_Actor.PostTakenDamage(bodyPart.Actor, damage, damageType, true, canConsumeEquippedItemsCharges))
            {
                yield return instruction5;
            }
            enumerator = null;
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> HealBodyPart(BodyPart bodyPart, int amount, bool forceAddPlayLogEntry = false, bool canAddPlayLog = true)
        {
            if (bodyPart.IsMissing || !bodyPart.Actor.Spawned)
            {
                Log.Error("Tried to make heal body part instructions for an unspawned actor or missing body part. Only spawned entities can be healed. Actor: " + bodyPart.Actor.ToStringSafe(), false);
                yield break;
            }
            amount = Calc.Clamp(amount, 0, bodyPart.MaxHP - bodyPart.HP);
            if (amount <= 0)
            {
                yield break;
            }
            yield return new Instruction_ChangeBodyPartHP(bodyPart, amount);
            if (canAddPlayLog && (forceAddPlayLogEntry || ActionUtility.TargetConcernsPlayer(bodyPart.Actor)))
            {
                yield return new Instruction_PlayLog("BodyPartGainsHP".Translate(bodyPart, RichText.HP(amount)));
            }
            foreach (Instruction instruction in InstructionSets_Entity.HealVisualEffects(bodyPart.Actor, amount, bodyPart))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> RestoreStamina(Actor actor, int amount, bool forceAddPlayLogEntry = false, bool canAddPlayLog = true, bool canAddFloatingText = true)
        {
            amount = Calc.Clamp(amount, 0, actor.MaxStamina - actor.Stamina);
            if (amount <= 0)
            {
                yield break;
            }
            yield return new Instruction_ChangeStamina(actor, amount);
            if (canAddPlayLog && (forceAddPlayLogEntry || ActionUtility.TargetConcernsPlayer(actor)))
            {
                yield return new Instruction_PlayLog("ActorGainsStamina".Translate(actor, RichText.Stamina(amount)));
            }
            if (canAddFloatingText && !actor.IsNowControlledActor)
            {
                yield return new Instruction_AddFloatingText(actor, amount.ToStringOffset(true), new Color(0.8f, 0.7f, 0.2f), 1f, 0f, 0f, null);
            }
            yield break;
        }

        public static IEnumerable<Instruction> RestoreMana(Actor actor, int amount, bool forceAddPlayLogEntry = false, bool canAddPlayLog = true, bool canAddFloatingText = true)
        {
            amount = Calc.Clamp(amount, 0, actor.MaxMana - actor.Mana);
            if (amount <= 0)
            {
                yield break;
            }
            yield return new Instruction_ChangeMana(actor, amount);
            if (canAddPlayLog && (forceAddPlayLogEntry || ActionUtility.TargetConcernsPlayer(actor)))
            {
                yield return new Instruction_PlayLog("ActorGainsMana".Translate(actor, RichText.Mana(amount)));
            }
            if (canAddFloatingText && !actor.IsNowControlledActor)
            {
                yield return new Instruction_AddFloatingText(actor, amount.ToStringOffset(true), new Color(0.3f, 0.4f, 0.8f), 1f, 0f, 0f, null);
            }
            yield break;
        }

        public static IEnumerable<Instruction> DestroyBodyPart(BodyPart bodyPart, Vector3Int? impactSource = null, bool forceAddPlayLogEntry = false, bool canAddPlayLog = true, bool canAngerFaction = false)
        {
            if (bodyPart.HP != 0)
            {
                yield return new Instruction_ChangeBodyPartHP(bodyPart, -bodyPart.HP);
            }
            if (bodyPart.Actor.Spawned)
            {
                if (!bodyPart.Actor.IsNowControlledActor)
                {
                    yield return new Instruction_BodyPartShatterEffect(bodyPart, impactSource);
                }
                if (impactSource != null && !bodyPart.Actor.IsNowControlledActor && bodyPart.Actor.Position.IsAdjacentOrInside(Get.NowControlledActor.Position))
                {
                    yield return new Instruction_VisualEffect(Get.VisualEffect_ActorDestroyedNearby, bodyPart.Actor.Position);
                }
                if (!bodyPart.Actor.IsNowControlledActor)
                {
                    yield return new Instruction_Sound(EntitySoundUtility.GetDestroySound(bodyPart.Actor), new Vector3?(bodyPart.Actor.Position), EntitySoundUtility.GetPitchFromScale(bodyPart.Actor), 1f);
                }
            }
            if (canAddPlayLog && (forceAddPlayLogEntry || ActionUtility.TargetConcernsPlayer(bodyPart.Actor)))
            {
                yield return new Instruction_PlayLog("BodyPartDestroyedDirectly".Translate(bodyPart.Actor, bodyPart));
            }
            IEnumerator<Instruction> enumerator;
            if (canAngerFaction)
            {
                foreach (Instruction instruction in InstructionSets_Actor.CheckAngerFaction(bodyPart.Actor))
                {
                    yield return instruction;
                }
                enumerator = null;
            }
            foreach (Instruction instruction2 in InstructionSets_Actor.CheckAddOrRemoveBodyPartConditions(bodyPart))
            {
                yield return instruction2;
            }
            enumerator = null;
            if (!bodyPart.Actor.IsPlayerParty)
            {
                foreach (Actor actor in Get.PlayerParty.ToTemporaryList<Actor>())
                {
                    foreach (Condition condition in actor.ConditionsAccumulated.AllConditions.ToTemporaryList<Condition>())
                    {
                        foreach (Instruction instruction3 in condition.MakeOtherActorBodyPartDestroyedInstructions(bodyPart))
                        {
                            yield return instruction3;
                        }
                        enumerator = null;
                    }
                    List<Condition>.Enumerator enumerator3 = default(List<Condition>.Enumerator);
                }
                List<Actor>.Enumerator enumerator2 = default(List<Actor>.Enumerator);
            }
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> RestoreMissingBodyPart(BodyPart bodyPart, bool forceAddPlayLog = false)
        {
            if (!bodyPart.IsMissing)
            {
                yield break;
            }
            if (forceAddPlayLog || ActionUtility.TargetConcernsPlayer(bodyPart.Actor))
            {
                yield return new Instruction_PlayLog("BodyPartIsRestored".Translate(bodyPart.Actor, bodyPart));
            }
            yield return new Instruction_ChangeBodyPartHP(bodyPart, bodyPart.MaxHP - bodyPart.HP);
            foreach (Instruction instruction in InstructionSets_Actor.CheckAddOrRemoveBodyPartConditions(bodyPart))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        private static IEnumerable<Instruction> CheckAddOrRemoveBodyPartConditions(BodyPart bodyPart)
        {
            InstructionSets_Actor.<> c__DisplayClass18_0 CS$<> 8__locals1;
            CS$<> 8__locals1.bodyPart = bodyPart;
            bool allDestroyed = true;
            bool anyDestroyed = false;
            foreach (BodyPart bodyPart2 in CS$<> 8__locals1.bodyPart.Actor.BodyParts)
			{
                if (bodyPart2.Spec == CS$<> 8__locals1.bodyPart.Spec)
				{
                    if (bodyPart2.IsMissing)
                    {
                        anyDestroyed = true;
                    }
                    else
                    {
                        allDestroyed = false;
                    }
                }
            }
            if (CS$<> 8__locals1.bodyPart.Spec.ConditionOnAllDestroyed != null)
			{
                if (allDestroyed)
                {
                    if (InstructionSets_Actor.< CheckAddOrRemoveBodyPartConditions > g__ExistingConditionOnDestroyed | 18_0(ref CS$<> 8__locals1) != null)
                    {
                        foreach (Instruction instruction in InstructionSets_Misc.RemoveCondition(InstructionSets_Actor.< CheckAddOrRemoveBodyPartConditions > g__ExistingConditionOnDestroyed | 18_0(ref CS$<> 8__locals1), false, false))
                        {
                            yield return instruction;
                        }
                        IEnumerator<Instruction> enumerator2 = null;
                    }
                    if (InstructionSets_Actor.< CheckAddOrRemoveBodyPartConditions > g__ExistingConditionOnAllDestroyed | 18_1(ref CS$<> 8__locals1) == null)
                    {
                        Condition conditionCopy = CS$<> 8__locals1.bodyPart.Spec.ConditionOnAllDestroyed.Clone();
                        conditionCopy.OriginBodyPart = CS$<> 8__locals1.bodyPart.Spec;
                        foreach (Instruction instruction2 in InstructionSets_Misc.AddCondition(conditionCopy, CS$<> 8__locals1.bodyPart.Actor.Conditions, false, false))
                        {
                            yield return instruction2;
                        }
                        IEnumerator<Instruction> enumerator2 = null;
                        if (CS$<> 8__locals1.bodyPart.Actor.IsNowControlledActor)
						{
                            yield return new Instruction_ShowNewImportantCondition(conditionCopy);
                        }
                        conditionCopy = null;
                    }
                }
                else if (InstructionSets_Actor.< CheckAddOrRemoveBodyPartConditions > g__ExistingConditionOnAllDestroyed | 18_1(ref CS$<> 8__locals1) != null)
                {
                    foreach (Instruction instruction3 in InstructionSets_Misc.RemoveCondition(InstructionSets_Actor.< CheckAddOrRemoveBodyPartConditions > g__ExistingConditionOnAllDestroyed | 18_1(ref CS$<> 8__locals1), false, false))
                    {
                        yield return instruction3;
                    }
                    IEnumerator<Instruction> enumerator2 = null;
                }
            }
            if (CS$<> 8__locals1.bodyPart.Spec.ConditionOnDestroyed != null)
			{
                if (anyDestroyed && (!allDestroyed || CS$<> 8__locals1.bodyPart.Spec.ConditionOnAllDestroyed == null))
				{
                    if (InstructionSets_Actor.< CheckAddOrRemoveBodyPartConditions > g__ExistingConditionOnAllDestroyed | 18_1(ref CS$<> 8__locals1) != null)
                    {
                        foreach (Instruction instruction4 in InstructionSets_Misc.RemoveCondition(InstructionSets_Actor.< CheckAddOrRemoveBodyPartConditions > g__ExistingConditionOnAllDestroyed | 18_1(ref CS$<> 8__locals1), false, false))
                        {
                            yield return instruction4;
                        }
                        IEnumerator<Instruction> enumerator2 = null;
                    }
                    if (InstructionSets_Actor.< CheckAddOrRemoveBodyPartConditions > g__ExistingConditionOnDestroyed | 18_0(ref CS$<> 8__locals1) == null)
                    {
                        Condition conditionCopy = CS$<> 8__locals1.bodyPart.Spec.ConditionOnDestroyed.Clone();
                        conditionCopy.OriginBodyPart = CS$<> 8__locals1.bodyPart.Spec;
                        foreach (Instruction instruction5 in InstructionSets_Misc.AddCondition(conditionCopy, CS$<> 8__locals1.bodyPart.Actor.Conditions, false, false))
                        {
                            yield return instruction5;
                        }
                        IEnumerator<Instruction> enumerator2 = null;
                        if (CS$<> 8__locals1.bodyPart.Actor.IsNowControlledActor)
						{
                            yield return new Instruction_ShowNewImportantCondition(conditionCopy);
                        }
                        conditionCopy = null;
                    }
                }

                else if (InstructionSets_Actor.< CheckAddOrRemoveBodyPartConditions > g__ExistingConditionOnDestroyed | 18_0(ref CS$<> 8__locals1) != null)
                {
                    foreach (Instruction instruction6 in InstructionSets_Misc.RemoveCondition(InstructionSets_Actor.< CheckAddOrRemoveBodyPartConditions > g__ExistingConditionOnDestroyed | 18_0(ref CS$<> 8__locals1), false, false))
                    {
                        yield return instruction6;
                    }
                    IEnumerator<Instruction> enumerator2 = null;
                }
            }
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> RespondToTakenDamage(Actor takenDamage, Vector3Int? impactSource)
        {
            if (takenDamage.Conditions.AnyOfSpec(Get.Condition_Sleeping))
            {
                foreach (Instruction instruction in InstructionSets_Actor.WakeUp(takenDamage, "WakeUpReason_Harmed".Translate()))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            if (!takenDamage.IsPlayerParty && impactSource != null && takenDamage.AIMemory.AttackTarget == null && !AIUtility.SeesOrJustSeen(takenDamage, Get.NowControlledActor))
            {
                foreach (Instruction instruction2 in InstructionSets_Actor.GoCheckPossibleEnemyLocation(takenDamage, impactSource.Value))
                {
                    yield return instruction2;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> PostTakenDamage(Actor actor, int damage, DamageTypeSpec damageType, bool onBodyPart, bool canConsumeEquippedItemsCharges = true)
        {
            if (damage > 0)
            {
                foreach (Condition condition in actor.ConditionsAccumulated.AllConditions.ToTemporaryList<Condition>())
                {
                    foreach (Instruction instruction in condition.MakeAffectedActorLostHPInstructions(damage, onBodyPart))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator2 = null;
                }
                List<Condition>.Enumerator enumerator = default(List<Condition>.Enumerator);
            }
            if (damageType != Get.DamageType_Bleeding && damageType != Get.DamageType_Starvation)
            {
                yield return new Instruction_SetLastHarmedSequence(actor, Get.TurnManager.CurrentSequence);
            }
            if (actor.IsNowControlledActor)
            {
                yield return new Instruction_Immediate(delegate
                {
                    Get.PlannedPlayerActions.Interrupt();
                });
            }
            if (canConsumeEquippedItemsCharges)
            {
                Func<Condition, bool> <> 9__1;
                foreach (Item item in actor.Inventory.EquippedItems.ToTemporaryList<Item>())
                {
                    if (item.ChargesLeft > 0)
                    {
                        IEnumerable<Condition> all = item.ConditionsEquipped.All;
                        Func<Condition, bool> func;
                        if ((func = <> 9__1) == null)
                        {
                            func = (<> 9__1 = delegate (Condition x)
                            {
                                Condition_IncomingDamageFactor condition_IncomingDamageFactor = x as Condition_IncomingDamageFactor;
                                if (condition_IncomingDamageFactor == null || condition_IncomingDamageFactor.DamageType != damageType)
                                {
                                    Condition_IncomingDamageOffset condition_IncomingDamageOffset = x as Condition_IncomingDamageOffset;
                                    return condition_IncomingDamageOffset != null && condition_IncomingDamageOffset.DamageType == damageType;
                                }
                                return true;
                            });
                        }
                        if (all.Any<Condition>(func))
                        {
                            foreach (Instruction instruction2 in InstructionSets_Usable.TryDecrementChargesLeft(item))
                            {
                                yield return instruction2;
                            }
                            IEnumerator<Instruction> enumerator2 = null;
                        }
                    }
                }
                List<Item>.Enumerator enumerator3 = default(List<Item>.Enumerator);
            }
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> GoCheckPossibleEnemyLocation(Actor actor, Vector3Int pos)
        {
            if (actor.Position == pos || actor.IsMainActor || actor.IsPlayerParty || !actor.IsHostile(Get.NowControlledActor) || actor.AIMemory.AttackTarget != null || AIUtility.SeesOrJustSeen(actor, Get.NowControlledActor) || Get.World.AnyEntityOfSpecAt(pos, Get.Entity_Fire))
            {
                yield break;
            }
            List<Actor> actors = Get.World.Actors;
            for (int i = 0; i < actors.Count; i++)
            {
                if (actors[i] != actor && actor.IsHostile(actors[i]) && actor.Sees(actors[i], null))
                {
                    yield break;
                }
            }
            yield return new Instruction_SetAttackTarget(actor, Get.NowControlledActor, pos);
            yield return new Instruction_AddFloatingText(actor, "GoesToCheckPossibleEnemyLocation".Translate(), new Color(1f, 0.5f, 0.5f), 0.5f, 0f, 0.15f, null);
            yield break;
        }

        public static IEnumerable<Instruction> CheckAddConditionsFromTakenDamage(Actor actor, BodyPart bodyPart, int damage, DamageTypeSpec damageType, Vector3Int? impactSource, bool forceAddPlayLogEntry = false)
        {
            float pctOfMax = (float)damage / (float)actor.MaxHP;
            int i = -1;
            foreach (DamageTypeSpec.ConditionFromDamage conditionFromDamage in damageType.ConditionsFromDamage)
            {
                int num = i;
                i = num + 1;
                if (actor.IsMainActor)
                {
                    bool flag = false;
                    using (HashSet<TraitSpec>.Enumerator enumerator2 = Get.TraitManager.Chosen.GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            if (enumerator2.Current.ImmuneToConditionsFromDamage.Contains(conditionFromDamage.Condition.Spec))
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (flag)
                    {
                        continue;
                    }
                }
                if (!(conditionFromDamage.Condition is Condition_Bleeding) || actor.Spec.Actor.CanBleed)
                {
                    int num2 = Calc.CombineHashes<int, int, int, int, int, int>(actor.MyStableHash, damageType.MyStableHash, i, actor.Sequence, actor.HP, actor.Conditions.All.Count);
                    if (Rand.EventHappensSeeded(pctOfMax, 1f / conditionFromDamage.Chance, num2, 0.95f))
                    {
                        Condition condition = conditionFromDamage.Condition.Clone();
                        foreach (Instruction instruction in InstructionSets_Misc.AddCondition(condition, actor.Conditions, forceAddPlayLogEntry, true))
                        {
                            yield return instruction;
                        }
                        IEnumerator<Instruction> enumerator3 = null;
                        if (actor.IsNowControlledActor)
                        {
                            yield return new Instruction_ShowNewImportantCondition(condition);
                        }
                        if (!actor.Spawned)
                        {
                            break;
                        }
                        condition = null;
                    }
                }
            }
            List<DamageTypeSpec.ConditionFromDamage>.Enumerator enumerator = default(List<DamageTypeSpec.ConditionFromDamage>.Enumerator);
            BodyPart bodyPart2;
            if (damageType.DestroyNPCRandomBodyPart && bodyPart == null && !actor.IsPlayerParty && actor.Spawned && actor.BodyParts.Where<BodyPart>((BodyPart x) => !x.IsMissing && x.HP <= damage).TryGetRandomElement<BodyPart>(out bodyPart2))
            {
                foreach (Instruction instruction2 in InstructionSets_Actor.DestroyBodyPart(bodyPart2, impactSource, false, true, false))
                {
                    yield return instruction2;
                }
                IEnumerator<Instruction> enumerator3 = null;
            }
            if (damageType.CanDestroyPlayerBodyPart && Get.RunSpec.PlayerCanLoseLimbs && bodyPart == null && actor.IsPlayerParty && actor.Spawned)
            {
                int totalBodyPartsMaxHPLost = 0;
                foreach (BodyPart bodyPart3 in actor.BodyParts)
                {
                    if (bodyPart3.IsMissing)
                    {
                        totalBodyPartsMaxHPLost += bodyPart3.MaxHP;
                    }
                }
                int num3 = Calc.CombineHashes<int, int, int>(actor.MyStableHash, actor.Sequence, totalBodyPartsMaxHPLost);
                BodyPart bodyPart4;
                if (Rand.EventHappensSeeded(pctOfMax, 10f, num3, 0.95f) && actor.BodyParts.Where<BodyPart>((BodyPart x) => !x.IsMissing && actor.HP <= actor.MaxHP - x.MaxHP - totalBodyPartsMaxHPLost).TryGetRandomElement<BodyPart>(out bodyPart4))
                {
                    foreach (Instruction instruction3 in InstructionSets_Actor.DestroyBodyPart(bodyPart4, impactSource, false, true, false))
                    {
                        yield return instruction3;
                    }
                    IEnumerator<Instruction> enumerator3 = null;
                }
            }
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> WakeUp(Actor actor, string reason = null)
        {
            if (!actor.Spawned)
            {
                yield break;
            }
            Condition firstOfSpec = actor.Conditions.GetFirstOfSpec(Get.Condition_Sleeping);
            if (firstOfSpec == null)
            {
                yield break;
            }
            foreach (Instruction instruction in InstructionSets_Misc.RemoveCondition(firstOfSpec, false, false))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield return new Instruction_AddSequence(actor, actor.SequencePerTurn);
            yield return new Instruction_Awareness_PreAction(actor);
            yield return new Instruction_Awareness_PostAction(actor);
            if (actor.Spec.Actor.CallSound != null)
            {
                yield return new Instruction_Sound(actor.Spec.Actor.CallSound, new Vector3?(actor.Position), EntitySoundUtility.GetPitchFromScale(actor), 1f);
            }
            string text = "ActorWakesUp".Translate(actor);
            if (!reason.NullOrEmpty())
            {
                text = "{0} ({1})".Formatted(text, reason);
            }
            yield return new Instruction_PlayLog(text);
            if (ActionUtility.TargetConcernsPlayer(actor) && !actor.IsNowControlledActor)
            {
                yield return new Instruction_AddFloatingText(actor, "WakesUp".Translate(), new Color(0.8f, 0.8f, 0.8f), 0.4f, 0f, 0f, null);
            }
            foreach (Instruction instruction2 in InstructionSets_Noise.MakeNoise(actor.Position, 4, NoiseType.ActorWokeUp, actor, false, true))
            {
                yield return instruction2;
            }
            enumerator = null;
            if (!actor.IsPlayerParty && Get.NowControlledActor.Spawned && actor.Sees(Get.NowControlledActor, null) && actor.IsHostile(Get.NowControlledActor))
            {
                yield return new Instruction_Immediate(delegate
                {
                    Get.PlannedPlayerActions.Interrupt();
                });
            }
            if (Get.Trait_Scary.IsChosen() && !actor.IsPlayerParty && Get.MainActor.Spawned && actor.IsHostile(Get.MainActor) && actor.Sees(Get.MainActor, null))
            {
                Condition_Panic condition_Panic = new Condition_Panic(Get.Condition_Panic, 3);
                foreach (Instruction instruction3 in InstructionSets_Misc.AddCondition(condition_Panic, actor.Conditions, false, false))
                {
                    yield return instruction3;
                }
                enumerator = null;
            }
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> UnlockSkill(SkillSpec skill)
        {
            yield return new Instruction_UnlockSkill(skill);
            if (skill == Get.Skill_IncreasedMaxHP && Get.MainActor.HP > 0)
            {
                yield return new Instruction_ChangeHP(Get.MainActor, 1);
            }
            if (skill == Get.Skill_IncreasedMaxStamina)
            {
                yield return new Instruction_ChangeStamina(Get.MainActor, 2);
            }
            if (skill == Get.Skill_IncreasedMaxMana)
            {
                yield return new Instruction_ChangeMana(Get.MainActor, 1);
            }
            yield break;
        }

        public static IEnumerable<Instruction> CheckAngerFaction(Actor victim)
        {
            if (!victim.IsMainActor && victim.Faction != null && Get.Player.Faction != null && victim.Faction != Get.Player.Faction && (victim.AIMemory.LastViolentlyAttackedBy == null || victim.AIMemory.LastViolentlyAttackedBy.Faction == Get.Player.Faction) && !Get.FactionManager.HostilityExists(Get.Player.Faction, victim.Faction))
            {
                yield return new Instruction_AddFactionHostility(Get.Player.Faction, victim.Faction);
                yield return new Instruction_PlayLog("FactionBecameHostile".Translate(victim.Faction));
            }
            yield break;
        }

        public static IEnumerable<Instruction> SwitchNowControlledActor(Actor switchTo)
        {
            yield return new Instruction_SetNowControlledActor(switchTo);
            List<Vector3Int> cellsToNewlyUnfogAt = Get.FogOfWar.GetCellsToNewlyUnfogAt(switchTo.Position);
            if (cellsToNewlyUnfogAt.Count != 0)
            {
                yield return new Instruction_Unfog(cellsToNewlyUnfogAt);
            }
            yield break;
        }

        [CompilerGenerated]
        internal static Condition<CheckAddOrRemoveBodyPartConditions> g__ExistingConditionOnDestroyed|18_0(ref InstructionSets_Actor.<>c__DisplayClass18_0 A_0)
		{
			foreach (Condition condition in A_0.bodyPart.Actor.Conditions.All)
			{
				if (condition.OriginBodyPart == A_0.bodyPart.Spec)
				{
					ConditionSpec spec = condition.Spec;
        Condition conditionOnDestroyed = A_0.bodyPart.Spec.ConditionOnDestroyed;
					if (spec == ((conditionOnDestroyed != null) ? conditionOnDestroyed.Spec : null))
					{
						return condition;
					}
}
			}
			return null;
		}

		[CompilerGenerated]
internal static Condition<CheckAddOrRemoveBodyPartConditions> g__ExistingConditionOnAllDestroyed|18_1(ref InstructionSets_Actor.<>c__DisplayClass18_0 A_0)
		{
			foreach (Condition condition in A_0.bodyPart.Actor.Conditions.All)
			{
				if (condition.OriginBodyPart == A_0.bodyPart.Spec)
				{
					ConditionSpec spec = condition.Spec;
Condition conditionOnAllDestroyed = A_0.bodyPart.Spec.ConditionOnAllDestroyed;
if (spec == ((conditionOnAllDestroyed != null) ? conditionOnAllDestroyed.Spec : null))
{
    return condition;
}
				}
			}
			return null;
		}
	}
}