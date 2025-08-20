using System;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_SecretRoomContents : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            if (!room.AdjacentRooms.Any() && room.RoomBelow != null)
            {
                this.DoEntranceFromLowerStorey(room, memory);
                this.DoChest(room, memory);
                return;
            }
            if (!room.AdjacentRooms.Any() && room.RoomAbove != null)
            {
                Vector3Int vector3Int = this.DoEntranceFromHigherStorey(room, memory);
                this.DoUndergroundEnemies(room, memory, vector3Int);
                return;
            }
            this.DoChest(room, memory);
        }

        private void DoEntranceFromLowerStorey(Room room, WorldGenMemory memory)
        {
            if (room.RoomBelow == null)
            {
                return;
            }
            Vector3Int vector3Int;
            if (!room.Shape.BottomSurfaceCuboid.InnerCuboidXZ(1).Where<Vector3Int>(delegate (Vector3Int x)
            {
                if (Get.World.AnyEntityOfSpecAt(x, Get.Entity_Floor) && Get.CellsInfo.CanPassThroughNoActors(x.Above()) && Get.CellsInfo.CanPassThroughNoActors(x.Below()) && !Get.CellsInfo.AnyAIAvoidsAt(x.Above()) && !Get.CellsInfo.AnyAIAvoidsAt(x.Below()))
                {
                    return !Get.World.GetEntitiesAt(x.Below()).Any<Entity>((Entity y) => y.Spec.IsStructure && y.Spec.Structure.AttachesToCeiling);
                }
                return false;
            }).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return;
            }
            Get.World.GetFirstEntityOfSpecAt(vector3Int, Get.Entity_Floor).DeSpawn(false);
            Maker.Make(Get.Entity_SecretRoomTrigger, null, false, false, true).Spawn(vector3Int.Above());
            CellCuboid cellCuboid = new CellCuboid(vector3Int.x - 1, vector3Int.y, vector3Int.z - 1, 3, 3, 3);
            foreach (Vector3Int vector3Int2 in room.Shape)
            {
                if ((vector3Int2.y != room.Shape.yMax || room.RoomAbove == null) && !cellCuboid.Contains(vector3Int2) && !Get.World.AnyEntityAt(vector3Int2))
                {
                    Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int2);
                }
            }
        }

        private Vector3Int DoEntranceFromHigherStorey(Room room, WorldGenMemory memory)
        {
            if (room.RoomAbove == null)
            {
                return room.Shape.Center;
            }
            Vector3Int vector3Int;
            if (!room.Shape.TopSurfaceCuboid.InnerCuboidXZ(1).Where<Vector3Int>(delegate (Vector3Int x)
            {
                if (!Get.World.AnyEntityAt(x) && Get.CellsInfo.CanPassThroughNoActors(x.Above()) && Get.CellsInfo.CanPassThroughNoActors(x.Below()) && !Get.CellsInfo.AnyAIAvoidsAt(x.Above()) && !Get.CellsInfo.AnyAIAvoidsAt(x.Below()))
                {
                    return !Get.World.GetEntitiesAt(x.Above()).Any<Entity>((Entity y) => y.Spec.IsStructure && y.Spec.Structure.FallBehavior != StructureFallBehavior.None && !y.Spec.Structure.AttachesToAnything);
                }
                return false;
            }).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return room.Shape.Center;
            }
            Maker.Make(Get.Entity_Dirt, null, false, false, true).Spawn(vector3Int);
            CellCuboid cellCuboid = new CellCuboid(vector3Int.x - 1, vector3Int.y - 2, vector3Int.z - 1, 3, 3, 3);
            foreach (Vector3Int vector3Int2 in room.Shape)
            {
                if ((vector3Int2.y != room.Shape.yMax || room.RoomAbove == null) && !cellCuboid.Contains(vector3Int2) && !Get.World.AnyEntityAt(vector3Int2))
                {
                    Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int2);
                }
            }
            return vector3Int;
        }

        private void DoChest(Room room, WorldGenMemory memory)
        {
            Vector3Int vector3Int;
            if (!room.FreeOnFloorNonBlocking.Where<Vector3Int>((Vector3Int x) => x == room.Shape.InnerCuboid(1).BottomSurfaceCuboid.Center).TryGetRandomElement<Vector3Int>(out vector3Int) && !room.FreeOnFloorNonBlocking.TryGetRandomElement<Vector3Int>(out vector3Int) && !room.FreeOnFloor.TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return;
            }
            Structure structure = Maker.Make<Structure>(Get.Entity_Chest, null, false, false, true);
            structure.InnerEntities.Add(ItemGenerator.Reward(true));
            Vector3Int vector3Int2;
            if (room.OnFloorLevelEntrances.TryGetRandomElement<Vector3Int>(out vector3Int2))
            {
                Vector3Int vector3Int3 = (vector3Int2 - vector3Int).NormalizedToCardinalXZDir();
                structure.Spawn(vector3Int, vector3Int3);
                return;
            }
            structure.Spawn(vector3Int);
        }

        private void DoUndergroundEnemies(Room room, WorldGenMemory memory, Vector3Int entrance)
        {
            for (int i = room.Shape.yMin + 1; i <= room.Shape.yMax; i++)
            {
                Vector3Int vector3Int = new Vector3Int(entrance.x, i, entrance.z);
                if (!Get.World.AnyEntityAt(vector3Int) || vector3Int.y == room.Shape.yMax)
                {
                    Maker.Make(Get.Entity_Ladder, null, false, false, true).Spawn(vector3Int);
                }
            }
            Vector3Int vector3Int2;
            if (room.FreeOnFloor.TryGetRandomElement<Vector3Int>(out vector3Int2))
            {
                ItemGenerator.SmallReward(true).Spawn(vector3Int2);
            }
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