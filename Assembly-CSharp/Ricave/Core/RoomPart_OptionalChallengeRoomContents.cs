using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_OptionalChallengeRoomContents : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            if (!room.AdjacentRooms.Any() && room.RoomBelow != null)
            {
                this.DoEntranceFromLowerStorey(room, memory);
                this.DoCeilingRoomContents(room, memory);
                return;
            }
            this.DoEnemies(room);
            this.DoItems(room);
        }

        private void DoItems(Room room)
        {
            Vector3Int vector3Int;
            if (room.FreeOnFloor.TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                ItemGenerator.Reward(true).Spawn(vector3Int);
            }
            if (!Get.RunConfig.ProgressDisabled)
            {
                Vector3Int vector3Int2;
                if (room.FreeOnFloor.TryGetRandomElement<Vector3Int>(out vector3Int2))
                {
                    Maker.Make<Item>(Get.Entity_Diamond, null, false, false, true).Spawn(vector3Int2);
                }
                Vector3Int vector3Int3;
                if (room.FreeOnFloor.TryGetRandomElement<Vector3Int>(out vector3Int3))
                {
                    ItemGenerator.Stardust(Rand.RangeInclusive(8, 30)).Spawn(vector3Int3);
                }
            }
        }

        private void DoEnemies(Room room)
        {
            List<EntitySpec> list;
            if (Get.RunSpec.AllowMobSpawners)
            {
                list = new List<EntitySpec>
                {
                    Get.Entity_Wasp,
                    Get.Entity_Wasp,
                    Get.Entity_Rat,
                    Get.Entity_Rat,
                    Get.Entity_Archer
                };
                if (Rand.Chance(0.2f))
                {
                    list.Add(Get.Entity_Gorhorn);
                }
            }
            else
            {
                list = new List<EntitySpec>
                {
                    Get.Entity_Wasp,
                    Get.Entity_Rat,
                    Get.Entity_Rat
                };
            }
            int i = 0;
            Func<Vector3Int, bool> <> 9__0;
            while (i < list.Count)
            {
                if (i != 0)
                {
                    goto IL_00DA;
                }
                IEnumerable<Vector3Int> freeOnFloor = room.FreeOnFloor;
                Func<Vector3Int, bool> func;
                if ((func = <> 9__0) == null)
                {
                    func = (<> 9__0 = (Vector3Int x) => room.IsEntranceCellToAvoidBlocking(x, true));
                }
                Vector3Int vector3Int;
                if (!freeOnFloor.Where<Vector3Int>(func).TryGetRandomElement<Vector3Int>(out vector3Int))
                {
                    goto IL_00DA;
                }
            IL_00EE:
                Actor actor = Maker.Make<Actor>(list[i], delegate (Actor x)
                {
                    x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                }, false, false, true);
                DifficultyUtility.AddConditionsForDifficulty(actor);
                actor.CalculateInitialHPManaAndStamina();
                actor.Conditions.AddCondition(new Condition_Sleeping(Get.Condition_Sleeping), -1);
                actor.Spawn(vector3Int);
                i++;
                continue;
            IL_00DA:
                if (room.FreeOnFloor.TryGetRandomElement<Vector3Int>(out vector3Int))
                {
                    goto IL_00EE;
                }
                break;
            }
        }

        private void DoEntranceFromLowerStorey(Room room, WorldGenMemory memory)
        {
            if (room.RoomBelow == null)
            {
                return;
            }
            CellCuboid cellCuboid = room.Shape.BottomSurfaceCuboid.InnerCuboidXZ(1);
            cellCuboid.width--;
            cellCuboid.depth--;
            Vector3Int vector3Int;
            if (!cellCuboid.Where<Vector3Int>(delegate (Vector3Int x)
            {
                if (Get.World.AnyEntityOfSpecAt(x, Get.Entity_Floor) && Get.CellsInfo.CanPassThroughNoActors(x.Above()) && Get.CellsInfo.CanPassThroughNoActors(x.Below()) && Get.World.AnyEntityOfSpecAt(x + Vector3IntUtility.Right, Get.Entity_Floor) && Get.CellsInfo.CanPassThroughNoActors((x + Vector3IntUtility.Right).Above()) && Get.CellsInfo.CanPassThroughNoActors((x + Vector3IntUtility.Right).Below()) && !Get.CellsInfo.AnyAIAvoidsAt(x.Above()) && !Get.CellsInfo.AnyAIAvoidsAt(x.Below()))
                {
                    return !Get.World.GetEntitiesAt(x.Below()).Any<Entity>((Entity y) => y.Spec.IsStructure && y.Spec.Structure.AttachesToCeiling);
                }
                return false;
            }).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return;
            }
            CellCuboid cellCuboid2 = new CellCuboid(vector3Int.x, vector3Int.y, vector3Int.z, 2, 2, 2);
            foreach (Vector3Int vector3Int2 in room.Shape)
            {
                if ((vector3Int2.y != room.Shape.yMax || room.RoomAbove == null) && !cellCuboid2.Contains(vector3Int2) && !Get.World.AnyEntityAt(vector3Int2))
                {
                    Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int2);
                }
            }
            foreach (Vector3Int vector3Int3 in cellCuboid2.BottomSurfaceCuboid)
            {
                Entity firstEntityOfSpecAt = Get.World.GetFirstEntityOfSpecAt(vector3Int3, Get.Entity_Floor);
                if (firstEntityOfSpecAt != null)
                {
                    firstEntityOfSpecAt.DeSpawn(false);
                }
                if (!Get.World.AnyEntityAt(vector3Int3))
                {
                    Maker.Make(Get.Entity_GlassWall, null, false, false, true).Spawn(vector3Int3);
                }
            }
        }

        private void DoCeilingRoomContents(Room room, WorldGenMemory memory)
        {
            Vector3Int vector3Int;
            if (room.FreeOnFloor.TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                ItemGenerator.Amulet(false).Spawn(vector3Int);
            }
            Vector3Int vector3Int2;
            if (!Get.RunConfig.ProgressDisabled && room.FreeOnFloor.TryGetRandomElement<Vector3Int>(out vector3Int2))
            {
                ItemGenerator.Stardust(Rand.RangeInclusive(5, 15)).Spawn(vector3Int2);
            }
            for (int i = 0; i < 2; i++)
            {
                Vector3Int vector3Int3;
                if (room.FreeOnFloor.TryGetRandomElement<Vector3Int>(out vector3Int3))
                {
                    Actor actor = Maker.Make<Actor>(Get.Entity_Spider, delegate (Actor x)
                    {
                        x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                    }, false, false, true);
                    DifficultyUtility.AddConditionsForDifficulty(actor);
                    actor.CalculateInitialHPManaAndStamina();
                    actor.Spawn(vector3Int3);
                }
            }
        }
    }
}