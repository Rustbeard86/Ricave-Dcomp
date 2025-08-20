using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class GenPass_FillRooms : GenPass
    {
        public override int SeedPart
        {
            get
            {
                return 378512090;
            }
        }

        public override void DoPass(WorldGenMemory memory)
        {
            Get.TiledDecals.TemporarilyIgnoreEntityChanges = true;
            try
            {
                this.assignedRoomSpecs.Clear();
                int num = 0;
                while (num < memory.storeys.Count && !memory.debugStopFillingRooms)
                {
                    Profiler.Begin("Storey " + num.ToString());
                    try
                    {
                        this.FillStorey(num, memory);
                    }
                    finally
                    {
                        Profiler.End();
                    }
                    num++;
                }
                this.AutoRotateStructures();
                this.DistributeGold(memory);
                this.DoSanityChecks(memory);
            }
            finally
            {
                Get.TiledDecals.TemporarilyIgnoreEntityChanges = false;
            }
        }

        private void FillStorey(int storeyIndex, WorldGenMemory memory)
        {
            List<Room> list = memory.storeys[storeyIndex].Rooms.InRandomOrder<Room>().ToList<Room>();
            for (int i = 0; i < list.Count; i++)
            {
                if (memory.debugStopFillingRooms)
                {
                    break;
                }
                Room room = list[i];
                Rand.PushState(Calc.CombineHashes<int, int, int>(memory.config.WorldSeed, this.SeedPart, i));
                try
                {
                    RoomSpec roomSpec;
                    if (memory.config.OnlyAllowedRoomSpec != null && room.Role != Room.LayoutRole.Start)
                    {
                        roomSpec = memory.config.OnlyAllowedRoomSpec;
                    }
                    else
                    {
                        roomSpec = (from x in (from x in Get.Specs.GetAll<RoomSpec>()
                                               where this.CanConsiderForRoom(x, room)
                                               select x).InRandomOrder<RoomSpec>((RoomSpec x) => x.Filler.GetSelectionWeight(room, memory))
                                    orderby x.Priority descending
                                    select x).FirstOrDefault<RoomSpec>();
                    }
                    if (roomSpec != null)
                    {
                        room.AssignedRoomSpec = roomSpec;
                        this.assignedRoomSpecs.Add(roomSpec);
                        RetainedRoomInfo.RoomInfo roomInfo = Get.World.RetainedRoomInfo.Add(room.Shape, roomSpec, room.Role, "");
                        Profiler.Begin(roomSpec.SpecID);
                        try
                        {
                            roomSpec.Filler.FillRoom(room, memory);
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Error in RoomFiller.FillRoom().", ex);
                        }
                        finally
                        {
                            Profiler.End();
                        }
                        room.Generated = true;
                        roomInfo.SetName(this.ResolveRoomName(room));
                    }
                    else
                    {
                        Log.Error("Could not find any RoomSpec for room.", false);
                    }
                }
                finally
                {
                    Rand.PopState();
                }
            }
        }

        private bool CanConsiderForRoom(RoomSpec spec, Room forRoom)
        {
            if (!spec.AllowedLayoutRoles.Contains(forRoom.Role))
            {
                return false;
            }
            if (spec.MaxPerMap != null)
            {
                int num = this.assignedRoomSpecs.Count(spec);
                int? maxPerMap = spec.MaxPerMap;
                if ((num >= maxPerMap.GetValueOrDefault()) & (maxPerMap != null))
                {
                    return false;
                }
            }
            if (spec.MaxOnePerMapTag != null)
            {
                for (int i = 0; i < this.assignedRoomSpecs.Count; i++)
                {
                    RoomSpec roomSpec = this.assignedRoomSpecs[i];
                    if (roomSpec == spec || roomSpec.MaxOnePerMapTag == spec.MaxOnePerMapTag)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private string ResolveRoomName(Room room)
        {
            string text;
            try
            {
                text = room.AssignedRoomSpec.Filler.GetRoomName(room);
            }
            catch (Exception ex)
            {
                Log.Error("Error in RoomFiller.GetRoomName().", ex);
                text = null;
            }
            return text;
        }

        private void AutoRotateStructures()
        {
            foreach (Structure structure in Get.World.Structures)
            {
                if (!structure.SpawnedWithSpecificRotationUnsaved)
                {
                    structure.AutoRotate();
                }
            }
        }

        private void DistributeGold(WorldGenMemory memory)
        {
            float num = (float)Get.RunSpec.GoldPerFloor * Get.Difficulty.GoldPerFloorMultiplier;
            Place place = memory.config.Place;
            int num2 = Calc.RoundToIntHalfUp(num * ((place != null) ? place.GoldFactor : 1f) * Get.TraitManager.GoldPerFloorFactor);
            if (Get.UnlockableManager.IsUnlocked(Get.Unlockable_GoldBoost, null))
            {
                num2 = Calc.RoundToIntHalfUp((float)num2 * 1.25f);
            }
            int num3 = Math.Max(num2 - memory.generatedGold, 0);
            List<Entity> list = memory.goldWanters.InRandomOrder<Entity>().ToList<Entity>();
            int num4 = (int)((float)list.Count * 0.3f);
            int num5 = list.Count - num4;
            if (num5 == 0)
            {
                return;
            }
            int num6 = Math.Max(num3 / num5, 1);
            int num7 = 0;
            while (num7 < num5 && num3 > 0)
            {
                int num8;
                if (num7 == num5 - 1)
                {
                    num8 = num3;
                }
                else
                {
                    num8 = Calc.RoundToIntHalfUp((float)num6 * Rand.Range(0.8f, 1.2f));
                    num8 = Calc.Clamp(num8, 1, num3);
                }
                num3 -= num8;
                Entity entity = list[num7];
                Actor actor = entity as Actor;
                if (actor != null)
                {
                    actor.Inventory.Add(ItemGenerator.Gold(num8), default(ValueTuple<Vector2Int?, int?, int?>));
                }
                else
                {
                    ((Structure)entity).InnerEntities.Add(ItemGenerator.Gold(num8));
                }
                num7++;
            }
        }

        private void DoSanityChecks(WorldGenMemory memory)
        {
            World world = Get.World;
            foreach (Vector3Int vector3Int in world.Bounds)
            {
                List<Entity> entitiesAt = world.GetEntitiesAt(vector3Int);
                bool flag = false;
                bool flag2 = false;
                for (int i = 0; i < entitiesAt.Count; i++)
                {
                    if (entitiesAt[i] is Structure && entitiesAt[i].Spec.Structure.IsFilled && !flag2)
                    {
                        if (flag)
                        {
                            Log.Error("World generator generated 2 filled entities in the same place. This is most likely wrong. Entity: " + entitiesAt[i].ToStringSafe() + " at " + entitiesAt[i].Position.ToString(), false);
                            flag2 = true;
                        }
                        else
                        {
                            flag = true;
                        }
                    }
                    if (!ItemOrStructureFallUtility.HasSupport(entitiesAt[i]))
                    {
                        Log.Error("World generator spawned an item or structure such that it doesn't have any support and will immediately fall. Entity: " + entitiesAt[i].ToStringSafe() + " at " + entitiesAt[i].Position.ToString(), false);
                    }
                    Actor actor = entitiesAt[i] as Actor;
                    if (actor != null && world.CellsInfo.IsFallingAt(actor.Position, actor, false))
                    {
                        Log.Error("World generator spawned an actor who is currently falling. Actor: " + actor.ToStringSafe(), false);
                    }
                    for (int j = i + 1; j < entitiesAt.Count; j++)
                    {
                        if (entitiesAt[i].Spec == entitiesAt[j].Spec && entitiesAt[i].Rotation == entitiesAt[j].Rotation)
                        {
                            Log.Error("World generator generated 2 entities of the same spec in the same place. This is most likely wrong. Entity: " + entitiesAt[i].ToStringSafe() + " at " + entitiesAt[i].Position.ToString(), false);
                            break;
                        }
                    }
                }
                if (world.CellsInfo.AnyActorAt(vector3Int) && !world.CellsInfo.CanPassThroughNoActors(vector3Int) && world.CellsInfo.IsFilled(vector3Int))
                {
                    Log.Error("World generator spawned an actor inside a wall.", false);
                }
                if (world.CellsInfo.AnyItemAt(vector3Int) && !world.CellsInfo.CanPassThroughNoActors(vector3Int) && world.CellsInfo.IsFilled(vector3Int))
                {
                    Log.Error("World generator spawned an item inside a wall.", false);
                }
            }
            if (!world.GetEntitiesOfSpec(Get.Entity_Lever).Any())
            {
                Log.Error("World generator didn't spawn lever anywhere.", false);
            }
            if (!world.GetEntitiesOfSpec(Get.Entity_Staircase).Any() && !world.GetEntitiesOfSpec(Get.Entity_RunEndPortal).Any())
            {
                Log.Error("World generator didn't spawn staircase anywhere.", false);
            }
            using (List<Actor>.Enumerator enumerator2 = memory.unusedBaseActors.GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    if (enumerator2.Current.Spawned)
                    {
                        Log.Error("World generator spawned an actor from unusedBaseActors but didn't remove it from the list.", false);
                    }
                }
            }
            memory.unusedBaseActors.RemoveAll((Actor x) => x.Spawned);
            foreach (Item item in memory.unusedBaseItems)
            {
                if (item.Spawned)
                {
                    Log.Error("World generator spawned an item from unusedBaseItems but didn't remove it from the list.", false);
                }
                else if (item.ParentInventory != null)
                {
                    Log.Error("World generator added an item from unusedBaseItems to someone's inventory but didn't remove it from the list.", false);
                }
            }
            memory.unusedBaseItems.RemoveAll((Item x) => x.Spawned || x.ParentInventory != null);
            this.tmpUnusedEntities.Clear();
            this.tmpUnusedEntities.AddRange<Entity>(memory.unusedBaseActors);
            this.tmpUnusedEntities.AddRange<Entity>(memory.unusedBaseItems);
            foreach (Structure structure in world.Structures)
            {
                foreach (Entity entity in structure.InnerEntities)
                {
                    if (this.tmpUnusedEntities.Contains(entity))
                    {
                        Log.Error(string.Concat(new string[]
                        {
                            "World generator added ",
                            entity.ToStringSafe(),
                            " from unusedBaseItems or unusedBaseActors to ",
                            structure.ToStringSafe(),
                            "'s InnerEntities but didn't remove it from the list."
                        }), false);
                    }
                    if (!this.tmpInnerEntities.Add(entity))
                    {
                        Log.Error("World generator added the same Entity to 2 InnerEntities.", false);
                    }
                    if (entity.Spawned)
                    {
                        Log.Error("World generator added a spawned Entity to InnerEntities.", false);
                    }
                    Item item2 = entity as Item;
                    if (item2 != null && item2.ParentInventory != null)
                    {
                        Log.Error("World generator added an item to someone's inventory and to some structure's InnerEntities at the same time.", false);
                    }
                }
            }
            this.tmpUnusedEntities.Clear();
        }

        private List<RoomSpec> assignedRoomSpecs = new List<RoomSpec>();

        private HashSet<Entity> tmpUnusedEntities = new HashSet<Entity>();

        private HashSet<Entity> tmpInnerEntities = new HashSet<Entity>();
    }
}