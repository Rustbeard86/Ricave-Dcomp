using System;
using System.Collections.Generic;
using System.Linq;
using Ricave.Rendering;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class Player
    {
        public Actor MainActor
        {
            get
            {
                return this.mainActor;
            }
        }

        public Actor NowControlledActor
        {
            get
            {
                return this.nowControlledActor;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.nowControlledActor = value;
            }
        }

        public List<Actor> OtherPartyMembers
        {
            get
            {
                return this.otherPartyMembers;
            }
        }

        public int MaxTurnsCanRewind
        {
            get
            {
                if (!this.mainActor.Inventory.HasSuperwatch)
                {
                    return 8;
                }
                return 15;
            }
        }

        public int Gold
        {
            get
            {
                return this.gold;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.gold = value;
            }
        }

        public int Experience
        {
            get
            {
                return this.experience;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.experience = value;
            }
        }

        public int TurnsCanRewind
        {
            get
            {
                return this.turnsCanRewind;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.turnsCanRewind = value;
            }
        }

        public int ReplenishTurnsCanRewindTurnsPassed
        {
            get
            {
                return this.replenishTurnsCanRewindTurnsPassed;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.replenishTurnsCanRewindTurnsPassed = value;
            }
        }

        public int Level
        {
            get
            {
                return LevelUtility.GetLevel(this.experience);
            }
        }

        public int ExperienceBetweenLevels
        {
            get
            {
                return LevelUtility.GetTotalExperienceRequired(this.Level + 1) - LevelUtility.GetTotalExperienceRequired(this.Level);
            }
        }

        public int ExperienceSinceLeveling
        {
            get
            {
                return LevelUtility.GetExperienceSinceLeveling(this.experience);
            }
        }

        public int ExperienceToNextLevel
        {
            get
            {
                return LevelUtility.GetTotalExperienceRequired(this.Level + 1) - this.experience;
            }
        }

        public int AncientDevicesResearched
        {
            get
            {
                return this.ancientDevicesResearched;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.ancientDevicesResearched = value;
            }
        }

        public int CurrentKillCombo
        {
            get
            {
                return this.currentKillCombo;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.currentKillCombo = value;
            }
        }

        public int Satiation
        {
            get
            {
                return this.satiation;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.satiation = value;
            }
        }

        public float LastLevelUpTime
        {
            get
            {
                return this.lastLevelUpTime;
            }
            set
            {
                this.lastLevelUpTime = value;
            }
        }

        public float LastExpGainTime
        {
            get
            {
                return this.lastExpGainTime;
            }
            set
            {
                this.lastExpGainTime = value;
            }
        }

        public float LastChargedAttackChangeTime
        {
            get
            {
                return this.lastChargedAttackChangeTime;
            }
            set
            {
                this.lastChargedAttackChangeTime = value;
            }
        }

        public bool HadManaBar
        {
            get
            {
                return this.hadManaBar;
            }
            set
            {
                this.hadManaBar = value;
            }
        }

        public bool HadStaminaBar
        {
            get
            {
                return this.hadStaminaBar;
            }
            set
            {
                this.hadStaminaBar = value;
            }
        }

        public double Playtime
        {
            get
            {
                return this.playtime;
            }
        }

        public bool UsedWatch
        {
            get
            {
                return this.usedWatch;
            }
        }

        public bool InspectedAnything
        {
            get
            {
                return this.inspectedAnything;
            }
        }

        public bool ShownStardustInfo
        {
            get
            {
                return this.shownStardustInfo;
            }
        }

        public bool ShownPuzzlePieceInfo
        {
            get
            {
                return this.shownPuzzlePieceInfo;
            }
        }

        public int Score
        {
            get
            {
                return this.score;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.score = value;
            }
        }

        public int ChestsOpened
        {
            get
            {
                return this.chestsOpened;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.chestsOpened = value;
            }
        }

        public int RecipesUsed
        {
            get
            {
                return this.recipesUsed;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.recipesUsed = value;
            }
        }

        public int ExpFromKillingEnemies
        {
            get
            {
                return this.expFromKillingEnemies;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.expFromKillingEnemies = value;
            }
        }

        public float ExpectedHPLostFromEnemies
        {
            get
            {
                return this.expectedHPLostFromEnemies;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.expectedHPLostFromEnemies = value;
            }
        }

        public int HPLostFromEnemies
        {
            get
            {
                return this.hpLostFromEnemies;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.hpLostFromEnemies = value;
            }
        }

        public int HPLost
        {
            get
            {
                return this.hpLost;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.hpLost = value;
            }
        }

        public int SkillPoints
        {
            get
            {
                return this.skillPoints;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.skillPoints = value;
            }
        }

        public int SpiritsSetFree
        {
            get
            {
                return this.spiritsSetFree;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.spiritsSetFree = value;
            }
        }

        public HashSet<EntitySpec> SeenActorSpecs
        {
            get
            {
                return this.seenActorSpecs;
            }
        }

        public HashSet<EntitySpec> SeenItemSpecs
        {
            get
            {
                return this.seenItemSpecs;
            }
        }

        public HashSet<NoteSpec> CollectedNoteSpecs
        {
            get
            {
                return this.collectedNoteSpecs;
            }
        }

        public ClassSpec Class
        {
            get
            {
                return Get.RunConfig.PlayerClass;
            }
        }

        public List<Actor> Party
        {
            get
            {
                this.tmpParty.Clear();
                if (this.mainActor != null)
                {
                    this.tmpParty.Add(this.mainActor);
                }
                this.tmpParty.AddRange(this.otherPartyMembers);
                return this.tmpParty;
            }
        }

        public List<Actor> Followers
        {
            get
            {
                this.tmpFollowers.Clear();
                foreach (Actor actor in Get.World.Actors)
                {
                    if (this.IsPlayerFollower(actor))
                    {
                        this.tmpFollowers.Add(actor);
                    }
                }
                return this.tmpFollowers;
            }
        }

        public Faction Faction
        {
            get
            {
                Actor actor = this.mainActor;
                if (actor == null)
                {
                    return null;
                }
                return actor.Faction;
            }
        }

        public bool HasWatch
        {
            get
            {
                foreach (Actor actor in this.Party)
                {
                    if (actor.Spawned && actor.Inventory.HasWatch)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool? SurroundedBonusJustBeforeUsing
        {
            get
            {
                return this.surroundedBonusJustBeforeUsing;
            }
            set
            {
                this.surroundedBonusJustBeforeUsing = value;
            }
        }

        public PrivateRoomPlaceStructureUsable PrivateRoomPlaceStructureUsable
        {
            get
            {
                return this.privateRoomPlaceStructureUsable;
            }
        }

        public PrivateRoomPickUpStructureUsable PrivateRoomPickUpStructureUsable
        {
            get
            {
                return this.privateRoomPickUpStructureUsable;
            }
        }

        public PlayerMovementManager PlayerMovementManager
        {
            get
            {
                return this.playerMovementManager;
            }
        }

        public InteractionManager InteractionManager
        {
            get
            {
                return this.interactionManager;
            }
        }

        public SavedCameraPosition SavedCameraPosition
        {
            get
            {
                return this.savedCameraPosition;
            }
        }

        public ThisRunLobbyItems CollectedLobbyItems
        {
            get
            {
                return this.collectedLobbyItems;
            }
        }

        public ThisRunPrivateRoomStructures CollectedPrivateRoomStructures
        {
            get
            {
                return this.collectedPrivateRoomStructures;
            }
        }

        public ThisRunQuestsState QuestsState
        {
            get
            {
                return this.questsState;
            }
        }

        public KillCounter KillCounter
        {
            get
            {
                return this.killCounter;
            }
        }

        public ThisRunCompletedQuests CompletedQuests
        {
            get
            {
                return this.completedQuests;
            }
        }

        public SkillManager SkillManager
        {
            get
            {
                return this.skillManager;
            }
        }

        public PlayerModel PlayerModel
        {
            get
            {
                return this.playerModel;
            }
        }

        public void MakeActor()
        {
            if (this.mainActor != null)
            {
                Log.Error("Tried to create player actor but he's already created.", false);
                return;
            }
            if (Get.Trait_AllIn.IsChosen())
            {
                this.skillPoints = 3;
            }
            if (!Get.InLobby && !Get.RunConfig.IsDailyChallenge)
            {
                this.experience = Get.Progress.NextRunRetainedExp;
                this.gold = Get.Progress.NextRunRetainedGold;
            }
            try
            {
                this.mainActor = Maker.Make<Actor>(Get.RunSpec.PlayerActorSpec, delegate (Actor x)
                {
                    this.mainActor = x;
                    this.nowControlledActor = x;
                    if (!Get.InLobby)
                    {
                        this.AddUnlockableInventory(x);
                        this.AddInventoryFromTraits(x);
                        for (int i = 0; i < Get.Difficulty.StartingHealthPotions; i++)
                        {
                            new Instruction_AddToInventory(x, Maker.Make<Item>(Get.Entity_Potion_Health, null, false, false, true), null, null).Do();
                        }
                    }
                    DifficultyUtility.AddPlayerConditionsForDifficulty(x);
                    x.CalculateInitialHPManaAndStamina();
                    foreach (TraitSpec traitSpec in Get.TraitManager.Chosen)
                    {
                        foreach (GenericUsableSpec genericUsableSpec in traitSpec.Abilities)
                        {
                            GenericUsable genericUsable = new GenericUsable(genericUsableSpec, x);
                            x.Abilities.Add(genericUsable);
                        }
                    }
                    if (this.Class != null)
                    {
                        foreach (SpellSpec spellSpec in this.Class.Spells)
                        {
                            Spell spell = Maker.Make(spellSpec);
                            x.Spells.AddSpell(spell, -1);
                        }
                    }
                }, false, false, true);
                this.privateRoomPlaceStructureUsable.PostMakePlayerActor();
                this.privateRoomPickUpStructureUsable.PostMakePlayerActor();
            }
            catch (Exception ex)
            {
                throw new Exception("Could not create player actor.", ex);
            }
        }

        public void MakeChooseablePartyMemberActor()
        {
            if (Get.RunConfig.PartyMember == null)
            {
                return;
            }
            try
            {
                Maker.Make<Actor>(Get.RunConfig.PartyMember.ActorSpec, delegate (Actor x)
                {
                    this.otherPartyMembers.Add(x);
                    x.CalculateInitialHPManaAndStamina();
                }, false, false, true);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not create party member actor.", ex);
            }
        }

        public void SpawnMainActor(Vector3Int spawnPos, Vector3Int spawnDir)
        {
            if (this.mainActor == null)
            {
                Log.Error("Tried to spawn player actor but he doesn't exist.", false);
                return;
            }
            if (this.mainActor.Spawned)
            {
                Log.Error("Tried to spawn player actor but he's already spawned.", false);
                return;
            }
            try
            {
                foreach (Instruction instruction in InstructionSets_Entity.Spawn(this.mainActor, spawnPos, new Quaternion?(spawnDir.CardinalDirToQuaternion())))
                {
                    instruction.Do();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not spawn player actor.", ex);
            }
        }

        public void DeSpawnMainActor()
        {
            if (this.mainActor == null)
            {
                Log.Error("Tried to despawn player actor but he doesn't exist.", false);
                return;
            }
            if (!this.mainActor.Spawned)
            {
                Log.Error("Tried to despawn player actor but he's already despawned.", false);
                return;
            }
            try
            {
                foreach (Instruction instruction in InstructionSets_Entity.DeSpawn(this.mainActor, false))
                {
                    instruction.Do();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not despawn player actor.", ex);
            }
        }

        public void SpawnPlayerPartyAndFollowers(Vector3Int spawnPos, Vector3Int spawnDir)
        {
            this.SpawnMainActor(spawnPos, spawnDir);
            foreach (Actor actor in this.Party.ToTemporaryList<Actor>())
            {
                if (actor != this.mainActor && !actor.Spawned && actor.HP > 0)
                {
                    foreach (Instruction instruction in InstructionSets_Entity.Spawn(actor, SpawnPositionFinder.Near(spawnPos, actor, false, false, null), new Quaternion?(spawnDir.CardinalDirToQuaternion())))
                    {
                        instruction.Do();
                    }
                }
            }
            foreach (Actor actor2 in this.followersFromPreviousFloor)
            {
                if (!actor2.Spawned && actor2.HP > 0)
                {
                    foreach (Instruction instruction2 in InstructionSets_Entity.Spawn(actor2, SpawnPositionFinder.Near(spawnPos, actor2, false, false, null), new Quaternion?(spawnDir.CardinalDirToQuaternion())))
                    {
                        instruction2.Do();
                    }
                }
            }
            this.followersFromPreviousFloor.Clear();
        }

        public void DeSpawnPlayerPartyAndFollowers(bool followersCanFollow)
        {
            List<Actor> list = (from x in this.Followers.ToList<Actor>()
                                where x.Spawned && this.FollowerCanFollowPlayerIntoNextFloor(x, true)
                                select x).ToList<Actor>();
            this.DeSpawnMainActor();
            foreach (Actor actor in this.Party.ToTemporaryList<Actor>())
            {
                if (actor != this.mainActor && actor.Spawned)
                {
                    foreach (Instruction instruction in InstructionSets_Entity.DeSpawn(actor, false))
                    {
                        instruction.Do();
                    }
                }
            }
            this.followersFromPreviousFloor.Clear();
            if (followersCanFollow)
            {
                foreach (Actor actor2 in list)
                {
                    foreach (Instruction instruction2 in InstructionSets_Entity.DeSpawn(actor2, false))
                    {
                        instruction2.Do();
                    }
                    this.followersFromPreviousFloor.Add(actor2);
                }
            }
        }

        public void DoInitialInstructions()
        {
            try
            {
                if (this.nowControlledActor == null || (!this.nowControlledActor.Spawned && this.nowControlledActor != this.mainActor))
                {
                    new Instruction_SetNowControlledActor(this.mainActor).Do();
                }
                List<Vector3Int> cellsToNewlyUnfogAt = Get.FogOfWar.GetCellsToNewlyUnfogAt(this.nowControlledActor.Position);
                if (cellsToNewlyUnfogAt.Count != 0)
                {
                    new Instruction_Unfog(cellsToNewlyUnfogAt).Do();
                }
                foreach (Actor actor in this.Party)
                {
                    foreach (Item item in actor.Inventory.AllItems.ToTemporaryList<Item>())
                    {
                        if (item.Spec.Item.DestroyOnDescend)
                        {
                            foreach (Instruction instruction in InstructionSets_Actor.RemoveFromInventory(item))
                            {
                                instruction.Do();
                            }
                        }
                    }
                }
                if (Get.Floor >= 2 && Get.PlaceSpec != null && Get.PlaceSpec.GiveNewFloorReachedScore)
                {
                    foreach (Instruction instruction2 in InstructionSets_Misc.GainScore(100 + (Get.Floor - 2) * 20, "ScoreGain_NewFloor".Translate(), true, true, true))
                    {
                        instruction2.Do();
                    }
                }
                if (this.ancientDevicesResearched != 0)
                {
                    new Instruction_ChangeAncientDevicesResearched(-this.ancientDevicesResearched).Do();
                }
                if (Get.WorldConfig.UsedLink != null)
                {
                    PlaceLink link = Get.WorldConfig.UsedLink;
                    if (link.ItemReward != null)
                    {
                        Item item2 = Maker.Make<Item>(link.ItemReward.Value.EntitySpec, delegate (Item x)
                        {
                            x.StackCount = link.ItemReward.Value.Count;
                            if (link.ItemRampUp != 0)
                            {
                                x.RampUp = link.ItemRampUp;
                            }
                            x.TurnsLeftToIdentify = 0;
                        }, false, false, true);
                        foreach (Instruction instruction3 in InstructionSets_Actor.AddToInventoryOrSpawnNear(this.mainActor, item2))
                        {
                            instruction3.Do();
                        }
                        new Instruction_PlayLog("ReceivedItem".Translate(item2)).Do();
                    }
                    if (link.HealHP != 0)
                    {
                        foreach (Instruction instruction4 in InstructionSets_Entity.Heal(this.mainActor, link.HealHP, true, true))
                        {
                            instruction4.Do();
                        }
                    }
                }
                if (Get.Floor == 3 && this.HPLost <= 0 && !Get.Achievement_ReachThirdFloorWithoutLosingHP.IsCompleted)
                {
                    new Instruction_CompleteAchievement(Get.Achievement_ReachThirdFloorWithoutLosingHP).Do();
                }
                if (Get.Floor == 5 && this.HPLost <= 0 && !Get.Achievement_ReachFifthFloorWithoutLosingHP.IsCompleted)
                {
                    new Instruction_CompleteAchievement(Get.Achievement_ReachFifthFloorWithoutLosingHP).Do();
                }
                if (Get.InLobby && !Get.Achievement_FinishTutorial.IsCompleted)
                {
                    new Instruction_CompleteAchievement(Get.Achievement_FinishTutorial).Do();
                }
                if (Get.InLobby && Get.Progress.FinishedGame && !Get.Achievement_FinishGame.IsCompleted)
                {
                    new Instruction_CompleteAchievement(Get.Achievement_FinishGame).Do();
                }
                if (Get.InLobby && Get.TotalLobbyItems.GetCount(Get.Entity_PuzzlePiece) >= 10 && !Get.Achievement_CollectAllPuzzlePieces.IsCompleted)
                {
                    new Instruction_CompleteAchievement(Get.Achievement_CollectAllPuzzlePieces).Do();
                }
                if (Get.InLobby && Get.Unlockable_PrivateRoom.IsUnlocked() && !Get.Achievement_BuyPrivateRoom.IsCompleted)
                {
                    new Instruction_CompleteAchievement(Get.Achievement_BuyPrivateRoom).Do();
                }
                if (Get.InLobby && Get.TraitManager.Unlocked.Count >= Get.Specs.GetAll<TraitSpec>().Count && !Get.Achievement_UnlockAllTraits.IsCompleted)
                {
                    new Instruction_CompleteAchievement(Get.Achievement_UnlockAllTraits).Do();
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error while doing initial move instructions.", ex);
            }
        }

        public void Update()
        {
            if (!Get.UI.IsEscapeMenuOpen && this.mainActor.Spawned)
            {
                this.playtime += (double)Math.Min(Clock.UnscaledDeltaTime, 1f);
            }
            try
            {
                Profiler.Begin("interactionManager.Update()");
                this.interactionManager.Update();
            }
            catch (Exception ex)
            {
                Log.Error("Error in interactionManager.Update().", ex);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("playerMovementManager.Update()");
                this.playerMovementManager.Update();
            }
            catch (Exception ex2)
            {
                Log.Error("Error in playerMovementManager.Update().", ex2);
            }
            finally
            {
                Profiler.End();
            }
        }

        public void OnGUI()
        {
            try
            {
                Profiler.Begin("playerMovementManager.OnGUI()");
                this.playerMovementManager.OnGUI();
            }
            catch (Exception ex)
            {
                Log.Error("Error in playerMovementManager.OnGUI().", ex);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("interactionManager.OnGUI()");
                this.interactionManager.OnGUI();
            }
            catch (Exception ex2)
            {
                Log.Error("Error in interactionManager.OnGUI().", ex2);
            }
            finally
            {
                Profiler.End();
            }
        }

        public void OnGUIAfterUI()
        {
            try
            {
                Profiler.Begin("interactionManager.OnGUIAfterUI()");
                this.interactionManager.OnGUIAfterUI();
            }
            catch (Exception ex)
            {
                Log.Error("Error in interactionManager.OnGUIAfterUI().", ex);
            }
            finally
            {
                Profiler.End();
            }
        }

        public void OnPlayerMoved(Vector3Int prev)
        {
            try
            {
                Profiler.Begin("privateRoomPlaceStructureUsable.OnPlayerMoved()");
                this.privateRoomPlaceStructureUsable.OnPlayerMoved(prev);
            }
            catch (Exception ex)
            {
                Log.Error("Error in privateRoomPlaceStructureUsable.OnPlayerMoved().", ex);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("privateRoomPickUpStructureUsable.OnPlayerMoved()");
                this.privateRoomPickUpStructureUsable.OnPlayerMoved(prev);
            }
            catch (Exception ex2)
            {
                Log.Error("Error in privateRoomPickUpStructureUsable.OnPlayerMoved().", ex2);
            }
            finally
            {
                Profiler.End();
            }
        }

        public void OnUsedWatch()
        {
            this.usedWatch = true;
        }

        public void OnInspectedSomething()
        {
            this.inspectedAnything = true;
        }

        public void OnShownStardustInfo()
        {
            this.shownStardustInfo = true;
        }

        public void OnShownPuzzlePieceInfo()
        {
            this.shownPuzzlePieceInfo = true;
        }

        private void AddUnlockableInventory(Actor actor)
        {
            if (Get.Unlockable_ExtraWatch.IsUnlocked())
            {
                new Instruction_AddToInventory(actor, Maker.Make<Item>(Get.Entity_Watch, null, false, false, true), null, null).Do();
            }
            if (Get.Unlockable_ExtraShield.IsUnlocked())
            {
                new Instruction_AddToInventory(actor, Maker.Make<Item>(Get.Entity_Shield, null, false, false, true), null, null).Do();
            }
            if (Get.Unlockable_ExtraSpear.IsUnlocked())
            {
                new Instruction_AddToInventory(actor, Maker.Make<Item>(Get.Entity_Spear, null, false, false, true), null, null).Do();
            }
            if (Get.Unlockable_ExtraHealthPotion.IsUnlocked())
            {
                Item item = Maker.Make<Item>(Get.Entity_Potion_Health, null, false, false, true);
                new Instruction_AddToInventory(actor, item, null, null).Do();
                foreach (Instruction instruction in InstructionSets_Entity.Identify(item, item.TurnsLeftToIdentify, false))
                {
                    instruction.Do();
                }
            }
            if (Get.Unlockable_ExtraScroll.IsUnlocked())
            {
                Item item2 = ItemGenerator.Scroll(false);
                new Instruction_AddToInventory(actor, item2, null, null).Do();
                foreach (Instruction instruction2 in InstructionSets_Entity.Identify(item2, item2.TurnsLeftToIdentify, false))
                {
                    instruction2.Do();
                }
            }
            if (Get.Unlockable_ExtraSword.IsUnlocked())
            {
                new Instruction_AddToInventory(actor, Maker.Make<Item>(Get.Entity_Sword, delegate (Item x)
                {
                    x.TurnsLeftToIdentify = 0;
                    x.RampUp = 1;
                }, false, false, true), null, null).Do();
            }
            if (Get.Unlockable_ExtraAmuletOfYerdon.IsUnlocked())
            {
                Item item3 = Maker.Make<Item>(Get.Entity_AmuletOfYerdon, delegate (Item x)
                {
                    x.TurnsLeftToIdentify = 0;
                }, false, false, true);
                if (actor.Inventory.Equipped.GetEquippedItemCollidingWith(item3) == null)
                {
                    using (IEnumerator<Instruction> enumerator = InstructionSets_Actor.Equip(actor, item3).GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            Instruction instruction3 = enumerator.Current;
                            instruction3.Do();
                        }
                        return;
                    }
                }
                new Instruction_AddToInventory(actor, item3, null, null).Do();
            }
        }

        private void AddInventoryFromTraits(Actor actor)
        {
            foreach (TraitSpec traitSpec in Get.TraitManager.Chosen)
            {
                foreach (EntitySpec entitySpec in traitSpec.ExtraStartingItems)
                {
                    new Instruction_AddToInventory(actor, Maker.Make<Item>(entitySpec, null, false, false, true), null, null).Do();
                }
            }
        }

        public void OnEntitiesSeen(List<Entity> entities)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].Spec.IsActor)
                {
                    this.seenActorSpecs.Add(entities[i].Spec);
                }
                else
                {
                    Item item = entities[i] as Item;
                    if (item != null && (item.Identified || Get.IdentificationGroups.GetIdentificationGroup(item.Spec) == null))
                    {
                        this.seenItemSpecs.Add(entities[i].Spec);
                    }
                }
            }
        }

        public int GetPartyInventoryCount(EntitySpec itemSpec)
        {
            int num = 0;
            foreach (Actor actor in this.Party)
            {
                num += actor.Inventory.GetCount(itemSpec, true);
            }
            return num;
        }

        public bool PartyHasAnyItemOfSpec(EntitySpec itemSpec)
        {
            using (List<Actor>.Enumerator enumerator = this.Party.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Inventory.HasAnyItemOfSpec(itemSpec))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool PartyHasAnyConditionOfSpec(ConditionSpec conditionSpec)
        {
            using (List<Actor>.Enumerator enumerator = this.Party.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Conditions.AnyOfSpec(conditionSpec))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsPlayerFollower(Actor actor)
        {
            return actor.AI == Get.AI_PlayerFollower && !actor.IsMainActor && !actor.IsPlayerParty && actor.Spec != Get.Entity_Ghost && actor.AttachedToChainPost == null && !actor.IsHostile(Get.MainActor);
        }

        public bool FollowerCanFollowPlayerIntoNextFloor(Actor follower, bool allowSpecialRecursiveCheck = true)
        {
            if (!this.IsPlayerFollower(follower))
            {
                return false;
            }
            if (!follower.Spawned)
            {
                return false;
            }
            foreach (Actor actor in this.Party)
            {
                if (actor.Spawned)
                {
                    if (follower.Position.IsAdjacentOrInside(actor.Position) && LineOfSight.IsLineOfFire(follower.Position, actor.Position))
                    {
                        return true;
                    }
                    if (follower.Position.GetGridDistance(actor.Position) <= 3)
                    {
                        List<Vector3Int> list = Get.PathFinder.FindPath(follower.Position, actor.Position, new PathFinder.Request(PathFinder.Request.Mode.Touch, follower), 5, this.tmpListForPathFinder);
                        if (list != null && list.Count <= 4)
                        {
                            return true;
                        }
                    }
                }
            }
            if (allowSpecialRecursiveCheck)
            {
                List<Actor> followers = this.Followers;
                for (int i = 0; i < followers.Count; i++)
                {
                    Actor actor2 = followers[i];
                    if (actor2 != follower && actor2.Spawned && follower.Position.IsAdjacentOrInside(actor2.Position) && LineOfSight.IsLineOfFire(follower.Position, actor2.Position) && this.FollowerCanFollowPlayerIntoNextFloor(actor2, false))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void OnSwitchedNowControlledActor(Actor prevActor)
        {
            Get.VisibilityCache.OnSwitchedNowControlledActor();
            Get.FPPControllerGOC.OnSwitchedNowControlledActor();
            Get.SectionsManager.OnSwitchedNowControlledActor();
            if (prevActor != null && prevActor.ActorGOC != null)
            {
                prevActor.ActorGOC.OnNoLongerNowControlledActor();
            }
            if (Get.NowControlledActor.ActorGOC != null)
            {
                Get.NowControlledActor.ActorGOC.OnBecameNowControlledActor();
            }
            Get.SeenEntitiesDrawer.OnSwitchedNowControlledActor(prevActor);
            Get.IconOverlayDrawer.OnSwitchedNowControlledActor(prevActor);
            Get.StrikeOverlays.OnSwitchedNowControlledActor();
            Get.PlayerHitMarkers.OnSwitchedNowControlledActor();
            Get.UseOnTargetUI.StopTargeting();
            Get.UI.CloseWheelSelector(true);
            Get.Minimap.Regenerate();
        }

        [Saved]
        private Actor mainActor;

        [Saved]
        private Actor nowControlledActor;

        [Saved(Default.New, true)]
        private List<Actor> otherPartyMembers = new List<Actor>();

        [Saved]
        private int gold;

        [Saved]
        private int experience;

        [Saved(8, false)]
        private int turnsCanRewind = 8;

        [Saved]
        private int replenishTurnsCanRewindTurnsPassed;

        [Saved]
        private int ancientDevicesResearched;

        [Saved]
        private int currentKillCombo;

        [Saved(600, false)]
        private int satiation = 600;

        [Saved]
        private double playtime;

        [Saved]
        private bool usedWatch;

        [Saved]
        private bool inspectedAnything;

        [Saved]
        private bool shownStardustInfo;

        [Saved]
        private bool shownPuzzlePieceInfo;

        [Saved]
        private int score;

        [Saved]
        private int chestsOpened;

        [Saved]
        private int recipesUsed;

        [Saved]
        private int expFromKillingEnemies;

        [Saved]
        private float expectedHPLostFromEnemies;

        [Saved]
        private int hpLostFromEnemies;

        [Saved]
        private int hpLost;

        [Saved]
        private int skillPoints;

        [Saved]
        private int spiritsSetFree;

        [Saved(Default.New, true)]
        private HashSet<EntitySpec> seenActorSpecs = new HashSet<EntitySpec>();

        [Saved(Default.New, true)]
        private HashSet<EntitySpec> seenItemSpecs = new HashSet<EntitySpec>();

        [Saved(Default.New, true)]
        private HashSet<NoteSpec> collectedNoteSpecs = new HashSet<NoteSpec>();

        private bool? surroundedBonusJustBeforeUsing;

        private List<Actor> followersFromPreviousFloor = new List<Actor>();

        [Saved(Default.New, false)]
        private PrivateRoomPlaceStructureUsable privateRoomPlaceStructureUsable = new PrivateRoomPlaceStructureUsable();

        [Saved(Default.New, false)]
        private PrivateRoomPickUpStructureUsable privateRoomPickUpStructureUsable = new PrivateRoomPickUpStructureUsable();

        private PlayerMovementManager playerMovementManager = new PlayerMovementManager();

        [Saved(Default.New, false)]
        private InteractionManager interactionManager = new InteractionManager();

        [Saved(Default.New, false)]
        private SavedCameraPosition savedCameraPosition = new SavedCameraPosition();

        [Saved(Default.New, false)]
        private ThisRunLobbyItems collectedLobbyItems = new ThisRunLobbyItems();

        [Saved(Default.New, false)]
        private ThisRunPrivateRoomStructures collectedPrivateRoomStructures = new ThisRunPrivateRoomStructures();

        [Saved(Default.New, false)]
        private KillCounter killCounter = new KillCounter();

        [Saved(Default.New, false)]
        private ThisRunQuestsState questsState = new ThisRunQuestsState();

        [Saved(Default.New, false)]
        private ThisRunCompletedQuests completedQuests = new ThisRunCompletedQuests();

        [Saved(Default.New, false)]
        private SkillManager skillManager = new SkillManager();

        private PlayerModel playerModel = new PlayerModel();

        private float lastLevelUpTime = -99999f;

        private float lastExpGainTime = -99999f;

        private float lastChargedAttackChangeTime = -99999f;

        private bool hadManaBar;

        private bool hadStaminaBar;

        public const int MaxTurnsCanRewind_Watch = 8;

        public const int MaxTurnsCanRewind_Superwatch = 15;

        public const int ReplenishTurnsCanRewindEveryTurns = 3;

        public const int HearingDistance = 4;

        private List<Actor> tmpParty = new List<Actor>();

        private List<Actor> tmpFollowers = new List<Actor>();

        private List<Vector3Int> tmpListForPathFinder = new List<Vector3Int>();
    }
}