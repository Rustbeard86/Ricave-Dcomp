using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class GenPass_CrossRoomDependencies : GenPass
    {
        public override int SeedPart
        {
            get
            {
                return 490513262;
            }
        }

        public override void DoPass(WorldGenMemory memory)
        {
            this.DoElectronicSafeButton(memory);
            this.DoIronOreForAnvil(memory);
            if (WorldGenUtility.MiniChallengeForCurrentWorld == MiniChallenge.AncientDevices)
            {
                this.DoAncientDevices(memory);
            }
            if (Rand.Chance(0.2f) && Get.RunSpec != Get.Run_Main1 && Get.RunSpec != Get.Run_Main2 && Get.RunSpec != Get.Run_Main3)
            {
                this.DoGlyphs(memory);
            }
            if (Rand.Chance(0.3f))
            {
                this.DoSecretCompartment(memory);
            }
            if (Rand.Chance(0.8f))
            {
                this.DoWallWithHole(memory);
            }
            if (Rand.Chance(0.35f) && Get.RunSpec != Get.Run_Main1 && Get.RunSpec != Get.Run_Main2 && Get.RunSpec != Get.Run_Main3 && Get.RunSpec != Get.Run_Main4)
            {
                this.ChainRandomEnemy(memory);
            }
        }

        private void DoElectronicSafeButton(WorldGenMemory memory)
        {
            Entity electronicSafe = Get.World.GetEntitiesOfSpec(Get.Entity_ElectronicSafe).FirstOrDefault<Entity>();
            if (electronicSafe == null)
            {
                return;
            }
            bool flag = false;
            int playerDistToElectronicSafe = electronicSafe.Position.GetGridDistance(memory.playerStartPos);
            Func<Vector3Int, bool> <> 9__1;
            for (int i = 0; i < 3; i++)
            {
                bool flag2 = i >= 1;
                bool veryDesperate = i == 2;
                Func<Vector3Int, bool> <> 9__0;
                foreach (Room room in memory.AllRooms.InRandomOrder<Room>())
                {
                    if ((room.Role != Room.LayoutRole.Start || veryDesperate) && room.Role != Room.LayoutRole.LockedBehindSilverDoor && room.Role != Room.LayoutRole.OptionalChallenge && room.Role != Room.LayoutRole.Secret && (flag2 || room.Shape.Center.GetGridDistance(memory.playerStartPos) > playerDistToElectronicSafe))
                    {
                        IEnumerable<Vector3Int> nonBlockingPositionsForEntity = room.GetNonBlockingPositionsForEntity(Get.Entity_ElectronicSafeButton, true, !veryDesperate);
                        Func<Vector3Int, bool> func;
                        if ((func = <> 9__0) == null)
                        {
                            func = (<> 9__0 = (Vector3Int x) => x.GetGridDistance(electronicSafe.Position) >= (veryDesperate ? 1 : 6));
                        }
                        IEnumerable<Vector3Int> enumerable = nonBlockingPositionsForEntity.Where<Vector3Int>(func);
                        if (!flag2)
                        {
                            IEnumerable<Vector3Int> enumerable2 = enumerable;
                            Func<Vector3Int, bool> func2;
                            if ((func2 = <> 9__1) == null)
                            {
                                func2 = (<> 9__1 = (Vector3Int x) => x.GetGridDistance(memory.playerStartPos) > playerDistToElectronicSafe);
                            }
                            enumerable = enumerable2.Where<Vector3Int>(func2);
                        }
                        Vector3Int vector3Int;
                        if (enumerable.TryGetRandomElement<Vector3Int>(out vector3Int))
                        {
                            Maker.Make(Get.Entity_ElectronicSafeButton, null, false, false, true).Spawn(vector3Int);
                            flag = true;
                            break;
                        }
                    }
                }
                if (flag)
                {
                    break;
                }
            }
            if (!flag)
            {
                Log.Warning("Could not spawn electronic safe button anywhere.", false);
            }
        }

        private void DoIronOreForAnvil(WorldGenMemory memory)
        {
            Entity entity = Get.World.GetEntitiesOfSpec(Get.Entity_Anvil).FirstOrDefault<Entity>();
            if (entity == null)
            {
                return;
            }
            bool flag = false;
            for (int i = 0; i < 2; i++)
            {
                foreach (Room room in memory.AllRooms.InRandomOrder<Room>())
                {
                    Vector3Int vector3Int;
                    if ((room.Role != Room.LayoutRole.Start || i != 0) && room.Role != Room.LayoutRole.LockedBehindSilverDoor && room.Role != Room.LayoutRole.OptionalChallenge && room.Role != Room.LayoutRole.Secret && (!room.Shape.Contains(entity.Position) || i != 0) && room.GetNonBlockingPositionsForEntity(Get.Entity_IronOre, true, true).TryGetRandomElement<Vector3Int>(out vector3Int))
                    {
                        Maker.Make(Get.Entity_IronOre, null, false, false, true).Spawn(vector3Int);
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    break;
                }
            }
            if (!flag)
            {
                Log.Warning("Could not spawn iron ore for anvil anywhere.", false);
            }
        }

        private void DoAncientDevices(WorldGenMemory memory)
        {
            int num = 0;
            for (int i = 0; i < 2; i++)
            {
                bool flag = i == 1;
                foreach (Room room in memory.AllRooms.InRandomOrder<Room>())
                {
                    Vector3Int vector3Int;
                    if ((room.Role != Room.LayoutRole.Start || flag) && room.Role != Room.LayoutRole.LockedBehindSilverDoor && room.Role != Room.LayoutRole.OptionalChallenge && room.Role != Room.LayoutRole.Secret && room.FreeOnFloorNonBlocking.TryGetRandomElement<Vector3Int>(out vector3Int))
                    {
                        Maker.Make(Get.Entity_AncientDevice, null, false, false, true).Spawn(vector3Int);
                        num++;
                        if (num >= 3)
                        {
                            break;
                        }
                    }
                }
                if (num >= 3)
                {
                    break;
                }
            }
            if (num < 3)
            {
                Log.Warning("Could not spawn all ancient devices.", false);
                foreach (Entity entity in Get.World.GetEntitiesOfSpec(Get.Entity_AncientDevice).ToTemporaryList<Entity>())
                {
                    entity.DeSpawn(false);
                }
            }
        }

        private IEnumerable<Vector3Int> GetExclusiveAccessibleWalls(WorldGenMemory memory, bool needPermanentWallOnSides = true)
        {
            return memory.AllRooms.Where<Room>((Room x) => x.Role != Room.LayoutRole.LockedBehindSilverDoor && x.Role != Room.LayoutRole.OptionalChallenge && x.Role != Room.LayoutRole.Secret).SelectMany<Room, Vector3Int>((Room x) => x.Shape.BottomSurfaceCuboid.WithAddedY(1).EdgeCellsXZNoCorners.Where<Vector3Int>(delegate (Vector3Int y)
            {
                if (!Get.World.AnyEntityOfSpecAt(y, Get.Entity_Wall))
                {
                    return false;
                }
                Vector3Int edge = x.Shape.GetEdge(y);
                Vector3Int vector3Int = y - edge;
                if (!Get.CellsInfo.CanPassThroughNoActors(vector3Int))
                {
                    return false;
                }
                if (!Get.CellsInfo.IsFloorUnderNoActors(vector3Int))
                {
                    return false;
                }
                if (Get.CellsInfo.AnyAIAvoidsAt(vector3Int))
                {
                    return false;
                }
                if (Get.CellsInfo.AnyLadderAt(vector3Int))
                {
                    return false;
                }
                if (Get.World.GetEntitiesAt(vector3Int).Any<Entity>((Entity z) => z is Structure && z.Spec.Structure.AttachesToBack))
                {
                    return false;
                }
                if (needPermanentWallOnSides)
                {
                    Vector3Int vector3Int2 = y + edge.RightDir();
                    Vector3Int vector3Int3 = y - edge.RightDir();
                    if (!Get.World.CellsInfo.AnyPermanentFilledImpassableAt(vector3Int2) || !Get.World.CellsInfo.AnyPermanentFilledImpassableAt(vector3Int3))
                    {
                        return false;
                    }
                }
                return x.IsExclusive(y);
            }));
        }

        private void DoSecretCompartment(WorldGenMemory memory)
        {
            Vector3Int vector3Int;
            if (this.GetExclusiveAccessibleWalls(memory, true).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                Get.World.GetFirstEntityOfSpecAt(vector3Int, Get.Entity_Wall).DeSpawn(false);
                Structure structure = Maker.Make<Structure>(Get.Entity_WallWithCompartment, null, false, false, true);
                structure.InnerEntities.Add(ItemGenerator.SmallReward(true));
                structure.Spawn(vector3Int);
            }
        }

        private void DoWallWithHole(WorldGenMemory memory)
        {
            Vector3Int vector3Int;
            if (this.GetExclusiveAccessibleWalls(memory, true).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                Get.World.GetFirstEntityOfSpecAt(vector3Int, Get.Entity_Wall).DeSpawn(false);
                Maker.Make(Get.Entity_WallWithHole, null, false, false, true).Spawn(vector3Int);
            }
        }

        private void ChainRandomEnemy(WorldGenMemory memory)
        {
            using (IEnumerator<Actor> enumerator = Get.World.Actors.Where<Actor>((Actor x) => !x.IsPlayerParty && !x.IsNowControlledActor && !Get.Player.IsPlayerFollower(x) && x.Faction != null && x.Faction.IsHostile(Get.Player.Faction) && !x.IsBaby && !x.IsBoss && x.AttachedToChainPost == null && x.Spec.Actor.GenerateSelectionWeight > 0f && x.Spec != Get.Entity_Spider && x.Spec != Get.Entity_Ghost && x.Spec != Get.Entity_Spirit && x.Spec != Get.Entity_Beggar && x.Spec != Get.Entity_Gorhorn && x.Spec != Get.Entity_Thief && x.Spec != Get.Entity_Mole && x.Spec != Get.Entity_Wasp && x.Spec != Get.Entity_Mimic && x.Spec != Get.Entity_Googon && x.Spec != Get.Entity_Demon).ToList<Actor>().InRandomOrder<Actor>()
                .GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Actor enemy = enumerator.Current;
                    Room room = memory.AllRooms.FirstOrDefault<Room>((Room x) => x.Shape.Contains(enemy.Position));
                    Vector3Int vector3Int;
                    if (room != null && room.Role != Room.LayoutRole.OptionalChallenge && room.Role != Room.LayoutRole.LockedBehindSilverDoor && room.FreeOnFloorNonBlocking.Where<Vector3Int>(delegate (Vector3Int x)
                    {
                        if (x.IsAdjacentXZ(enemy.Position) && LineOfSight.IsLineOfSight(x, enemy.Position) && LineOfSight.IsLineOfFire(x, enemy.Position))
                        {
                            if (!x.AdjacentCellsXZ().Any<Vector3Int>((Vector3Int y) => Get.CellsInfo.AnyDoorAt(y)))
                            {
                                return !Get.World.AnyEntityNearby(x, 2, Get.Entity_Portal);
                            }
                        }
                        return false;
                    }).TryGetRandomElement<Vector3Int>(out vector3Int))
                    {
                        ChainPost chainPost = Maker.Make<ChainPost>(Get.Entity_ChainPost, null, false, false, true);
                        chainPost.Spawn(vector3Int);
                        enemy.AttachedToChainPostDirect = chainPost;
                        chainPost.AttachedActorDirect = enemy;
                        break;
                    }
                }
            }
        }

        private void DoGlyphs(WorldGenMemory memory)
        {
            using (List<EntitySpec>.Enumerator enumerator = new List<EntitySpec>
            {
                Get.Entity_Glyph1,
                Get.Entity_Glyph2,
                Get.Entity_Glyph3,
                Get.Entity_Glyph4
            }.InRandomOrder<EntitySpec>().Take<EntitySpec>(3).ToList<EntitySpec>()
                .GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    EntitySpec glyph = enumerator.Current;
                    bool flag = false;
                    Func<Vector3Int, bool> <> 9__0;
                    foreach (Room room in memory.AllRooms.InRandomOrder<Room>())
                    {
                        if (room.Role != Room.LayoutRole.LockedBehindSilverDoor && room.Role != Room.LayoutRole.OptionalChallenge && room.Role != Room.LayoutRole.Secret)
                        {
                            IEnumerable<Vector3Int> freeOnFloor = room.FreeOnFloor;
                            Func<Vector3Int, bool> func;
                            if ((func = <> 9__0) == null)
                            {
                                func = (<> 9__0 = (Vector3Int x) => ItemOrStructureFallUtility.WouldHaveSupport(glyph, x, null) && x.AdjacentCardinalCellsXZ().Any<Vector3Int>((Vector3Int y) => Get.World.AnyEntityOfSpecAt(y, Get.Entity_Wall) && Get.CellsInfo.IsFloorUnderNoActors(x)));
                            }
                            Vector3Int vector3Int;
                            if (freeOnFloor.Where<Vector3Int>(func).TryGetRandomElement<Vector3Int>(out vector3Int))
                            {
                                Maker.Make(glyph, null, false, false, true).Spawn(vector3Int);
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (!flag)
                    {
                        foreach (Entity entity in Get.World.GetEntitiesOfSpec(Get.Entity_Glyph1).ToTemporaryList<Entity>())
                        {
                            entity.DeSpawn(false);
                        }
                        foreach (Entity entity2 in Get.World.GetEntitiesOfSpec(Get.Entity_Glyph2).ToTemporaryList<Entity>())
                        {
                            entity2.DeSpawn(false);
                        }
                        foreach (Entity entity3 in Get.World.GetEntitiesOfSpec(Get.Entity_Glyph3).ToTemporaryList<Entity>())
                        {
                            entity3.DeSpawn(false);
                        }
                        using (List<Entity>.Enumerator enumerator3 = Get.World.GetEntitiesOfSpec(Get.Entity_Glyph4).ToTemporaryList<Entity>().GetEnumerator())
                        {
                            while (enumerator3.MoveNext())
                            {
                                Entity entity4 = enumerator3.Current;
                                entity4.DeSpawn(false);
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}