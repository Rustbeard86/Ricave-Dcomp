using System;
using System.Collections.Generic;
using System.Linq;
using Ricave.Core;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.UI
{
    public static class DevConsoleCommandsImpl_World
    {
        public static void Spawn(string[] args)
        {
            string text = args[0];
            EntitySpec entitySpec = Get.Specs.Get<EntitySpec>(text);
            if (entitySpec == null)
            {
                ItemLookSpec itemLookSpec = Get.Specs.Get<ItemLookSpec>(text);
                if (itemLookSpec == null)
                {
                    return;
                }
                entitySpec = Get.IdentificationGroups.GetItemSpecUsingItemLook(itemLookSpec);
            }
            new Action_Debug_Spawn(Get.Action_Debug_Spawn, Maker.Make(entitySpec, null, false, false, true), Get.NowControlledActor.Position).Do(false);
        }

        public static void SpawnAt(string[] args)
        {
            Entity selectedOrMouseoverEntity = Get.DevConsole.SelectedOrMouseoverEntity;
            if (selectedOrMouseoverEntity == null)
            {
                Log.Error("No entity selected.", false);
                return;
            }
            string text = args[0];
            EntitySpec entitySpec = Get.Specs.Get<EntitySpec>(text);
            if (entitySpec == null)
            {
                ItemLookSpec itemLookSpec = Get.Specs.Get<ItemLookSpec>(text);
                if (itemLookSpec == null)
                {
                    return;
                }
                entitySpec = Get.IdentificationGroups.GetItemSpecUsingItemLook(itemLookSpec);
            }
            new Action_Debug_Spawn(Get.Action_Debug_Spawn, Maker.Make(entitySpec, null, false, false, true), selectedOrMouseoverEntity.Position - Get.CameraGravity).Do(false);
        }

        public static void SpawnBlueprint(string[] args)
        {
            Entity selectedOrMouseoverEntity = Get.DevConsole.SelectedOrMouseoverEntity;
            if (selectedOrMouseoverEntity == null)
            {
                Log.Error("No entity selected.", false);
                return;
            }
            string text = args[0];
            BlueprintSpec blueprintSpec = Get.Specs.Get<BlueprintSpec>(text);
            if (blueprintSpec == null)
            {
                Log.Error("Blueprint spec '" + text + "' not found.", false);
                return;
            }
            new Action_Debug_SpawnBlueprint(Get.Action_Debug_SpawnBlueprint, blueprintSpec, selectedOrMouseoverEntity.Position - Get.CameraGravity).Do(false);
        }

        public static void SaveBlueprint(string[] args)
        {
            Entity selectedOrMouseoverEntity = Get.DevConsole.SelectedOrMouseoverEntity;
            if (selectedOrMouseoverEntity == null)
            {
                Log.Error("No entity selected.", false);
                return;
            }
            string text = args[0];
            new Action_Debug_SaveBlueprint(Get.Action_Debug_SaveBlueprint, selectedOrMouseoverEntity.Position - Get.CameraGravity, text).Do(false);
        }

        public static void DeSpawn(string[] args)
        {
            Entity selectedOrMouseoverEntity = Get.DevConsole.SelectedOrMouseoverEntity;
            if (selectedOrMouseoverEntity == null)
            {
                Log.Error("No entity selected.", false);
                return;
            }
            new Action_Debug_DeSpawn(Get.Action_Debug_DeSpawn, selectedOrMouseoverEntity).Do(false);
        }

        public static void Destroy(string[] args)
        {
            Entity selectedOrMouseoverEntity = Get.DevConsole.SelectedOrMouseoverEntity;
            if (selectedOrMouseoverEntity == null)
            {
                Log.Error("No entity selected.", false);
                return;
            }
            new Action_Debug_Destroy(Get.Action_Debug_Destroy, selectedOrMouseoverEntity).Do(false);
        }

        public static void SpawnBoss(string[] args)
        {
            Entity selectedOrMouseoverEntity = Get.DevConsole.SelectedOrMouseoverEntity;
            if (selectedOrMouseoverEntity == null)
            {
                Log.Error("No entity selected.", false);
                return;
            }
            new Action_Debug_Spawn(Get.Action_Debug_Spawn, BossGenerator.Boss(true), selectedOrMouseoverEntity.Position - Get.CameraGravity).Do(false);
        }

        public static void SpawnBoss_WithActorSpec(string[] args)
        {
            Entity selectedOrMouseoverEntity = Get.DevConsole.SelectedOrMouseoverEntity;
            if (selectedOrMouseoverEntity == null)
            {
                Log.Error("No entity selected.", false);
                return;
            }
            string text = args[0];
            EntitySpec entitySpec = Get.Specs.Get<EntitySpec>(text);
            if (entitySpec == null)
            {
                return;
            }
            new Action_Debug_Spawn(Get.Action_Debug_Spawn, BossGenerator.Boss(entitySpec, null, null), selectedOrMouseoverEntity.Position - Get.CameraGravity).Do(false);
        }

        public static void SpawnBaby(string[] args)
        {
            Entity selectedOrMouseoverEntity = Get.DevConsole.SelectedOrMouseoverEntity;
            if (selectedOrMouseoverEntity == null)
            {
                Log.Error("No entity selected.", false);
                return;
            }
            new Action_Debug_Spawn(Get.Action_Debug_Spawn, BabyGenerator.Baby(), selectedOrMouseoverEntity.Position - Get.CameraGravity).Do(false);
        }

        public static void Run(string[] args)
        {
            RunSpec runSpec = Get.Specs.Get<RunSpec>(args[0]);
            if (runSpec == null)
            {
                return;
            }
            Root.LoadPlayScene(RootOnSceneChanged.StartNewRun(new RunConfig(runSpec, Rand.Int, Get.Difficulty_Normal, null, "Current", null, false, null, false, false, null, null)));
        }

        public static void Run_WithRunSpecAndSeed(string[] args)
        {
            string text = args[0];
            int num = int.Parse(args[1]);
            RunSpec runSpec = Get.Specs.Get<RunSpec>(text);
            if (runSpec == null)
            {
                return;
            }
            Root.LoadPlayScene(RootOnSceneChanged.StartNewRun(new RunConfig(runSpec, num, Get.Difficulty_Normal, null, "Current", null, false, null, false, false, null, null)));
        }

        public static void Gen(string[] args)
        {
            Place randomInitialPlace = Get.PlaceManager.GetRandomInitialPlace();
            Get.Run.ReloadScene(RunOnSceneChanged.GenerateWorld(new WorldConfig(((randomInitialPlace != null) ? randomInitialPlace.Spec.WorldSpec : null) ?? Get.World_Standard, Rand.Int, randomInitialPlace, null, null, false)), false);
        }

        public static void Gen_WithWorldSpecOrRoomSpecOrFloor(string[] args)
        {
            string text = args[0];
            int num;
            if (int.TryParse(text, out num))
            {
                Place randomPlaceForFloor_Debug = Get.PlaceManager.GetRandomPlaceForFloor_Debug(num);
                Get.Run.ReloadScene(RunOnSceneChanged.GenerateWorld(new WorldConfig(((randomPlaceForFloor_Debug != null) ? randomPlaceForFloor_Debug.Spec.WorldSpec : null) ?? Get.World_Standard, Rand.Int, randomPlaceForFloor_Debug, null, null, false)), false);
                return;
            }
            RoomSpec roomSpec;
            if (Get.Specs.TryGet<RoomSpec>(text, out roomSpec))
            {
                Place randomInitialPlace = Get.PlaceManager.GetRandomInitialPlace();
                Get.Run.ReloadScene(RunOnSceneChanged.GenerateWorld(new WorldConfig(((randomInitialPlace != null) ? randomInitialPlace.Spec.WorldSpec : null) ?? Get.World_Standard, Rand.Int, randomInitialPlace, null, roomSpec, false)), false);
                return;
            }
            WorldSpec worldSpec = Get.Specs.Get<WorldSpec>(text);
            if (worldSpec == null)
            {
                return;
            }
            Place randomInitialPlace2 = Get.PlaceManager.GetRandomInitialPlace();
            Get.Run.ReloadScene(RunOnSceneChanged.GenerateWorld(new WorldConfig(worldSpec, Rand.Int, randomInitialPlace2, null, null, false)), false);
        }

        public static void Gen_WithWorldSpecAndSeed(string[] args)
        {
            string text = args[0];
            int num = int.Parse(args[1]);
            WorldSpec worldSpec = Get.Specs.Get<WorldSpec>(text);
            if (worldSpec == null)
            {
                return;
            }
            Place randomInitialPlace = Get.PlaceManager.GetRandomInitialPlace();
            Get.Run.ReloadScene(RunOnSceneChanged.GenerateWorld(new WorldConfig(worldSpec, num, randomInitialPlace, null, null, false)), false);
        }

        public static void Rewind(string[] args)
        {
            int num = int.Parse(args[0]);
            if (num > 0)
            {
                new Action_Debug_RewindTime(Get.Action_Debug_RewindTime, num).Do(false);
            }
        }

        public static void KillAll(string[] args)
        {
            foreach (Actor actor in Get.World.Actors.ToTemporaryList<Actor>())
            {
                if (!actor.IsPlayerParty)
                {
                    new Action_Debug_DeSpawn(Get.Action_Debug_DeSpawn, actor).Do(false);
                }
            }
        }

        public static void Give(string[] args)
        {
            string text = args[0];
            EntitySpec entitySpec = Get.Specs.Get<EntitySpec>(text);
            if (entitySpec == null)
            {
                ItemLookSpec itemLookSpec = Get.Specs.Get<ItemLookSpec>(text);
                if (itemLookSpec == null)
                {
                    return;
                }
                entitySpec = Get.IdentificationGroups.GetItemSpecUsingItemLook(itemLookSpec);
            }
            Item item = Maker.Make<Item>(entitySpec, delegate (Item x)
            {
                if (x.Spec == Get.Entity_Gold)
                {
                    x.StackCount = 500;
                }
            }, false, false, true);
            new Action_Debug_AddToInventory(Get.Action_Debug_AddToInventory, Get.NowControlledActor, item).Do(false);
        }

        public static void GiveTo(string[] args)
        {
            Entity selectedOrMouseoverEntity = Get.DevConsole.SelectedOrMouseoverEntity;
            if (selectedOrMouseoverEntity == null)
            {
                Log.Error("No entity selected.", false);
                return;
            }
            string text = args[0];
            EntitySpec entitySpec = Get.Specs.Get<EntitySpec>(text);
            if (entitySpec == null)
            {
                ItemLookSpec itemLookSpec = Get.Specs.Get<ItemLookSpec>(text);
                if (itemLookSpec == null)
                {
                    return;
                }
                entitySpec = Get.IdentificationGroups.GetItemSpecUsingItemLook(itemLookSpec);
            }
            new Action_Debug_AddToInventory(Get.Action_Debug_AddToInventory, (Actor)selectedOrMouseoverEntity, Maker.Make<Item>(entitySpec, null, false, false, true)).Do(false);
        }

        public static void AddCondition(string[] args)
        {
            string text = args[0];
            ConditionSpec conditionSpec = Get.Specs.Get<ConditionSpec>(text);
            if (conditionSpec == null)
            {
                return;
            }
            new Action_Debug_AddCondition(Get.Action_Debug_AddCondition, Get.NowControlledActor.Conditions, conditionSpec).Do(false);
        }

        public static void RemoveCondition(string[] args)
        {
            string text = args[0];
            ConditionSpec conditionSpec = Get.Specs.Get<ConditionSpec>(text);
            if (conditionSpec == null)
            {
                return;
            }
            Condition condition = Get.NowControlledActor.Conditions.All.FirstOrDefault<Condition>((Condition x) => x.Spec == conditionSpec);
            if (condition != null)
            {
                new Action_Debug_RemoveCondition(Get.Action_Debug_RemoveCondition, condition).Do(false);
            }
        }

        public static void AddConditionTo(string[] args)
        {
            Entity selectedOrMouseoverEntity = Get.DevConsole.SelectedOrMouseoverEntity;
            if (selectedOrMouseoverEntity == null)
            {
                Log.Error("No entity selected.", false);
                return;
            }
            string text = args[0];
            ConditionSpec conditionSpec = Get.Specs.Get<ConditionSpec>(text);
            if (conditionSpec == null)
            {
                return;
            }
            ActionSpec action_Debug_AddCondition = Get.Action_Debug_AddCondition;
            Actor actor = selectedOrMouseoverEntity as Actor;
            new Action_Debug_AddCondition(action_Debug_AddCondition, (actor != null) ? actor.Conditions : ((Item)selectedOrMouseoverEntity).ConditionsEquipped, conditionSpec).Do(false);
        }

        public static void RemoveConditionFrom(string[] args)
        {
            Entity selectedOrMouseoverEntity = Get.DevConsole.SelectedOrMouseoverEntity;
            if (selectedOrMouseoverEntity == null)
            {
                Log.Error("No entity selected.", false);
                return;
            }
            string text = args[0];
            ConditionSpec conditionSpec = Get.Specs.Get<ConditionSpec>(text);
            if (conditionSpec == null)
            {
                return;
            }
            Actor actor = selectedOrMouseoverEntity as Actor;
            Condition condition = ((actor != null) ? actor.Conditions : ((Item)selectedOrMouseoverEntity).ConditionsEquipped).All.FirstOrDefault<Condition>((Condition x) => x.Spec == conditionSpec);
            if (condition != null)
            {
                new Action_Debug_RemoveCondition(Get.Action_Debug_RemoveCondition, condition).Do(false);
            }
        }

        public static void AddUseEffect(string[] args)
        {
            string text = args[0];
            UseEffectSpec useEffectSpec = Get.Specs.Get<UseEffectSpec>(text);
            if (useEffectSpec == null)
            {
                return;
            }
            new Action_Debug_AddUseEffect(Get.Action_Debug_AddUseEffect, (Get.NowControlledActor.Inventory.EquippedWeapon != null) ? Get.NowControlledActor.Inventory.EquippedWeapon.UseEffects : Get.NowControlledActor.NativeWeapons[0].UseEffects, useEffectSpec).Do(false);
        }

        public static void RemoveUseEffect(string[] args)
        {
            string text = args[0];
            UseEffectSpec useEffectSpec = Get.Specs.Get<UseEffectSpec>(text);
            if (useEffectSpec == null)
            {
                return;
            }
            UseEffect useEffect = ((Get.NowControlledActor.Inventory.EquippedWeapon != null) ? Get.NowControlledActor.Inventory.EquippedWeapon.UseEffects : Get.NowControlledActor.NativeWeapons[0].UseEffects).All.FirstOrDefault<UseEffect>((UseEffect x) => x.Spec == useEffectSpec);
            if (useEffect != null)
            {
                new Action_Debug_RemoveUseEffect(Get.Action_Debug_RemoveUseEffect, useEffect).Do(false);
            }
        }

        public static void AddUseEffectTo(string[] args)
        {
            Entity selectedOrMouseoverEntity = Get.DevConsole.SelectedOrMouseoverEntity;
            if (selectedOrMouseoverEntity == null)
            {
                Log.Error("No entity selected.", false);
                return;
            }
            string text = args[0];
            UseEffectSpec useEffectSpec = Get.Specs.Get<UseEffectSpec>(text);
            if (useEffectSpec == null)
            {
                return;
            }
            ActionSpec action_Debug_AddUseEffect = Get.Action_Debug_AddUseEffect;
            Actor actor = selectedOrMouseoverEntity as Actor;
            new Action_Debug_AddUseEffect(action_Debug_AddUseEffect, (actor != null) ? actor.NativeWeapons[0].UseEffects : ((Item)selectedOrMouseoverEntity).UseEffects, useEffectSpec).Do(false);
        }

        public static void RemoveUseEffectFrom(string[] args)
        {
            Entity selectedOrMouseoverEntity = Get.DevConsole.SelectedOrMouseoverEntity;
            if (selectedOrMouseoverEntity == null)
            {
                Log.Error("No entity selected.", false);
                return;
            }
            string text = args[0];
            UseEffectSpec useEffectSpec = Get.Specs.Get<UseEffectSpec>(text);
            if (useEffectSpec == null)
            {
                return;
            }
            Actor actor = selectedOrMouseoverEntity as Actor;
            UseEffect useEffect = ((actor != null) ? actor.NativeWeapons[0].UseEffects : ((Item)selectedOrMouseoverEntity).UseEffects).All.FirstOrDefault<UseEffect>((UseEffect x) => x.Spec == useEffectSpec);
            if (useEffect != null)
            {
                new Action_Debug_RemoveUseEffect(Get.Action_Debug_RemoveUseEffect, useEffect).Do(false);
            }
        }

        public static void Fly(string[] args)
        {
            new Action_Debug_AddCondition(Get.Action_Debug_AddCondition, Get.NowControlledActor.Conditions, Get.Condition_Levitating).Do(false);
        }

        public static void SetGravity(string[] args)
        {
            int num = int.Parse(args[0]);
            new Action_Debug_SetGravity(Get.Action_Debug_SetGravity, Get.NowControlledActor, Vector3IntUtility.DirectionsCardinal[num]).Do(false);
        }

        public static void SetGravityFor(string[] args)
        {
            Entity selectedOrMouseoverEntity = Get.DevConsole.SelectedOrMouseoverEntity;
            if (selectedOrMouseoverEntity == null)
            {
                Log.Error("No entity selected.", false);
                return;
            }
            int num = int.Parse(args[0]);
            new Action_Debug_SetGravity(Get.Action_Debug_SetGravity, (Actor)selectedOrMouseoverEntity, Vector3IntUtility.DirectionsCardinal[num]).Do(false);
        }

        public static void SetPlayerScale(string[] args)
        {
            float num = float.Parse(args[0]);
            new Action_Debug_SetScale(Get.Action_Debug_SetScale, Get.NowControlledActor, new Vector3(num, num, num)).Do(false);
        }

        public static void SetScale(string[] args)
        {
            Entity selectedOrMouseoverEntity = Get.DevConsole.SelectedOrMouseoverEntity;
            if (selectedOrMouseoverEntity == null)
            {
                Log.Error("No entity selected.", false);
                return;
            }
            float num = float.Parse(args[0]);
            new Action_Debug_SetScale(Get.Action_Debug_SetScale, selectedOrMouseoverEntity, new Vector3(num, num, num)).Do(false);
        }

        public static void Control(string[] args)
        {
            Actor actor = Get.DevConsole.SelectedOrMouseoverEntity as Actor;
            if (actor == null)
            {
                Log.Error("No actor selected.", false);
                return;
            }
            new Action_Debug_Control(Get.Action_Debug_Control, actor).Do(false);
        }

        public static void AddParty(string[] args)
        {
            Actor actor = Get.DevConsole.SelectedOrMouseoverEntity as Actor;
            if (actor == null)
            {
                Log.Error("No actor selected.", false);
                return;
            }
            new Action_Debug_AddParty(Get.Action_Debug_AddParty, actor).Do(false);
        }

        public static void RemoveParty(string[] args)
        {
            Actor actor = Get.DevConsole.SelectedOrMouseoverEntity as Actor;
            if (actor == null)
            {
                Log.Error("No actor selected.", false);
                return;
            }
            new Action_Debug_RemoveParty(Get.Action_Debug_RemoveParty, actor).Do(false);
        }

        public static void Debug(string[] args)
        {
            Entity selectedOrMouseoverEntity = Get.DevConsole.SelectedOrMouseoverEntity;
            if (selectedOrMouseoverEntity == null)
            {
                Log.Error("No entity selected.", false);
                return;
            }
            ActorTurnResolver.ShowAIDebugFor = (Actor)selectedOrMouseoverEntity;
        }

        public static void Unfog(string[] args)
        {
            new Action_Debug_UnfogAll(Get.Action_Debug_UnfogAll).Do(false);
        }

        public static void Level(string[] args)
        {
            new Action_Debug_LevelUp(Get.Action_Debug_LevelUp).Do(false);
        }

        public static void Media(string[] args)
        {
            MediaCreatorHelper.RandomizeEverything();
        }

        public static void TeleportTo(string[] args)
        {
            string text = args[0];
            EntitySpec entitySpec = Get.Specs.Get<EntitySpec>(text);
            if (entitySpec == null)
            {
                return;
            }
            Entity entity;
            if (!Get.World.GetEntitiesOfSpec(entitySpec).TryGetRandomElement<Entity>(out entity))
            {
                return;
            }
            new Action_Debug_Teleport(Get.Action_Debug_Teleport, entity.Position).Do(false);
        }

        public static void RampUp(string[] args)
        {
            Entity selectedOrMouseoverEntity = Get.DevConsole.SelectedOrMouseoverEntity;
            if (selectedOrMouseoverEntity == null)
            {
                Log.Error("No entity selected.", false);
                return;
            }
            int num = int.Parse(args[0]);
            new Action_Debug_SetRampUp(Get.Action_Debug_SetRampUp, selectedOrMouseoverEntity, num).Do(false);
        }

        public static void Replay(string[] args)
        {
            int turns = int.Parse(args[0]);
            int taken = 0;
            List<Instruction> list = Get.TurnManager.RecentActions.AsEnumerable<Action>().Reverse<Action>().TakeWhile<Action>(delegate (Action x)
            {
                if (x.IsRewindPoint)
                {
                    int taken2 = taken;
                    taken = taken2 + 1;
                }
                return taken < turns;
            })
                .SelectMany<Action, Instruction>((Action x) => x.InstructionsDone.AsEnumerable<Instruction>().Reverse<Instruction>())
                .ToList<Instruction>();
            foreach (Instruction instruction in list)
            {
                instruction.Undo();
            }
            Get.UI.DebugUI.Replay(list);
        }

        public static void AddWorldSituation(string[] args)
        {
            string text = args[0];
            WorldSituationSpec worldSituationSpec = Get.Specs.Get<WorldSituationSpec>(text);
            if (worldSituationSpec == null)
            {
                return;
            }
            new Action_Debug_AddWorldSituation(Get.Action_Debug_AddWorldSituation, worldSituationSpec).Do(false);
        }

        public static void RemoveWorldSituation(string[] args)
        {
            string text = args[0];
            WorldSituationSpec worldSituationSpec = Get.Specs.Get<WorldSituationSpec>(text);
            if (worldSituationSpec == null)
            {
                return;
            }
            WorldSituation firstOfSpec = Get.WorldSituationsManager.GetFirstOfSpec(worldSituationSpec);
            if (firstOfSpec != null)
            {
                new Action_Debug_RemoveWorldSituation(Get.Action_Debug_RemoveWorldSituation, firstOfSpec).Do(false);
            }
        }

        public static void DoWorldEvent(string[] args)
        {
            string text = args[0];
            WorldEventSpec worldEventSpec = Get.Specs.Get<WorldEventSpec>(text);
            if (worldEventSpec == null)
            {
                return;
            }
            new Action_Debug_DoWorldEvent(Get.Action_Debug_DoWorldEvent, worldEventSpec).Do(false);
        }

        public static void AttachToChainPost(string[] args)
        {
            Entity selectedOrMouseoverEntity = Get.DevConsole.SelectedOrMouseoverEntity;
            if (selectedOrMouseoverEntity == null)
            {
                Log.Error("No entity selected.", false);
                return;
            }
            Actor actor = null;
            ChainPost chainPost = null;
            Actor actor2 = selectedOrMouseoverEntity as Actor;
            if (actor2 != null)
            {
                actor = actor2;
                Entity entity;
                if (!Get.World.GetEntitiesOfSpec(Get.Entity_ChainPost).TryGetMinBy<Entity, int>((Entity x) => x.Position.GetGridDistance(actor.Position), out entity))
                {
                    return;
                }
                chainPost = (ChainPost)entity;
            }
            else
            {
                ChainPost chainPost2 = selectedOrMouseoverEntity as ChainPost;
                if (chainPost2 == null)
                {
                    return;
                }
                chainPost = chainPost2;
                if (!Get.World.Actors.TryGetMinBy<Actor, int>((Actor x) => x.Position.GetGridDistance(chainPost.Position), out actor))
                {
                    return;
                }
            }
            new Action_Debug_AttachToChainPost(Get.Action_Debug_AttachToChainPost, actor, chainPost).Do(false);
        }

        public static void Test(string[] args)
        {
            IconGenerator.GenerateIcon(Get.Specs.Get<EntitySpec>("Toxofungus").Prefab, false, false).ToFile("temp.png");
            IconGenerator.GenerateIcon(Get.Specs.Get<EntitySpec>("BeerBarrel").Prefab, false, true).ToFile("a.png");
            IconGenerator.GenerateIcon(Get.Specs.Get<EntitySpec>("DevilStatue").Prefab, false, true).ToFile("b.png");
            string text = "";
            DebugUI.LogEvents = true;
            foreach (EntitySpec entitySpec in Get.Specs.GetAll<EntitySpec>())
            {
                if (entitySpec.IsActor)
                {
                    text = text + entitySpec.SpecID + "\n";
                }
            }
            Log.Message(text);
        }
    }
}