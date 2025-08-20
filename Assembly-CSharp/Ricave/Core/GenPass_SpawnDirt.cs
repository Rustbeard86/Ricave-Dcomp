using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class GenPass_SpawnDirt : GenPass
    {
        public override int SeedPart
        {
            get
            {
                return 1639704167;
            }
        }

        public override void DoPass(WorldGenMemory memory)
        {
            List<List<Vector3Int>> list = new List<List<Vector3Int>>();
            HashSet<Vector3Int> hashSet = memory.AllRooms.SelectMany<Room, Vector3Int>((Room x) => x.Shape).ToHashSet<Vector3Int>();
            this.FindCandidates(memory, list, hashSet);
            list.RemoveAll((List<Vector3Int> x) => x.Count < 2);
            foreach (List<Vector3Int> list2 in list)
            {
                int num = Rand.RangeInclusive(2, Math.Min(4, list2.Count));
                int num2 = list2.Count - num;
                for (int i = 0; i < num2; i++)
                {
                    if (Rand.Bool)
                    {
                        list2.RemoveAt(0);
                    }
                    else
                    {
                        list2.RemoveAt(list2.Count - 1);
                    }
                }
            }
            int num3 = 0;
            List<Structure> list3 = new List<Structure>();
            List<Vector3Int> list4;
            while (num3 < 10 && list.TryGetRandomElement<List<Vector3Int>>(out list4))
            {
                list.Remove(list4);
                num3 += list4.Count;
                foreach (Vector3Int vector3Int in list4)
                {
                    Structure structure = this.CarveAt(vector3Int, hashSet);
                    list3.Add(structure);
                }
            }
            for (int j = 0; j < 2; j++)
            {
                Structure structure2;
                if (list3.Where<Structure>((Structure x) => x.InnerEntities.Count == 0).TryGetRandomElement<Structure>(out structure2))
                {
                    Item item = ItemGenerator.SmallReward(true);
                    Actor actor = Maker.Make<Actor>(Get.Entity_Spider, delegate (Actor x)
                    {
                        x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                    }, false, false, true);
                    DifficultyUtility.AddConditionsForDifficulty(actor);
                    actor.CalculateInitialHPManaAndStamina();
                    actor.Inventory.Add(item, default(ValueTuple<Vector2Int?, int?, int?>));
                    structure2.InnerEntities.Add(actor);
                }
            }
            int num4 = 2;
            if (Rand.Chance(0.5f))
            {
                Structure structure3;
                if (list3.Where<Structure>((Structure x) => x.InnerEntities.Count == 0).TryGetRandomElement<Structure>(out structure3))
                {
                    Item item2 = Maker.Make<Item>(Get.Entity_Bandage, null, false, false, true);
                    structure3.InnerEntities.Add(item2);
                    num4--;
                }
            }
            if (Rand.Chance(0.5f))
            {
                Structure structure4;
                if (list3.Where<Structure>((Structure x) => x.InnerEntities.Count == 0).TryGetRandomElement<Structure>(out structure4))
                {
                    Item item3 = Maker.Make<Item>(Get.Entity_Fish, null, false, false, true);
                    structure4.InnerEntities.Add(item3);
                    num4--;
                }
            }
            for (int k = 0; k < num4; k++)
            {
                Structure structure5;
                if (list3.Where<Structure>((Structure x) => x.InnerEntities.Count == 0).TryGetRandomElement<Structure>(out structure5))
                {
                    Item item4 = Maker.Make<Item>(Get.Entity_Bone, null, false, false, true);
                    structure5.InnerEntities.Add(item4);
                }
            }
            if (!Get.RunConfig.ProgressDisabled)
            {
                Structure structure6;
                if (list3.Where<Structure>((Structure x) => x.InnerEntities.Count == 0).TryGetRandomElement<Structure>(out structure6))
                {
                    structure6.InnerEntities.Add(ItemGenerator.Stardust(Rand.RangeInclusive(8, 12)));
                }
            }
            Structure structure7;
            if (list3.Where<Structure>((Structure x) => x.InnerEntities.Count == 0).TryGetRandomElement<Structure>(out structure7))
            {
                Actor actor2 = Maker.Make<Actor>(Get.Entity_Skeleton, delegate (Actor x)
                {
                    x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                }, false, false, true);
                DifficultyUtility.AddConditionsForDifficulty(actor2);
                actor2.CalculateInitialHPManaAndStamina();
                structure7.InnerEntities.Add(actor2);
            }
        }

        private void FindCandidates(WorldGenMemory memory, List<List<Vector3Int>> candidates, HashSet<Vector3Int> allRoomCells)
        {
            foreach (Room room in memory.AllRooms.Where<Room>((Room x) => x.Role != Room.LayoutRole.LockedBehindSilverDoor && x.Role != Room.LayoutRole.Secret && x.Role != Room.LayoutRole.OptionalChallenge && x.Height >= 4))
            {
                foreach (Vector3Int vector3Int in Vector3IntUtility.DirectionsXZCardinal)
                {
                    List<Vector3Int> list = new List<Vector3Int>();
                    foreach (Vector3Int vector3Int2 in room.Shape.BottomSurfaceCuboid.GetEdgeCells(vector3Int))
                    {
                        if (!room.Shape.IsCorner(vector3Int2))
                        {
                            Vector3Int vector3Int3 = vector3Int2.Above();
                            if (Get.World.AnyEntityOfSpecAt(vector3Int3, Get.Entity_Wall))
                            {
                                Vector3Int vector3Int4 = vector3Int3 - vector3Int;
                                if (Get.CellsInfo.CanPassThroughNoActors(vector3Int4) && !Get.CellsInfo.AnyAIAvoidsAt(vector3Int4) && Get.CellsInfo.IsFloorUnderNoActors(vector3Int4))
                                {
                                    if (!Get.World.GetEntitiesAt(vector3Int4).Any<Entity>((Entity y) => y.Spec.IsStructure && y.Spec.Structure.AttachesToBack))
                                    {
                                        Vector3Int vector3Int5 = vector3Int3 + vector3Int;
                                        if (vector3Int5.InBounds() && !Get.World.AnyEntityAt(vector3Int5) && !allRoomCells.Contains(vector3Int5))
                                        {
                                            Vector3Int vector3Int6 = vector3Int3.Above();
                                            if (!Get.CellsInfo.CanPassThroughNoActors(vector3Int6) || (vector3Int6.y == room.Shape.yMax && room.RoomAbove != null))
                                            {
                                                Vector3Int vector3Int7 = vector3Int3.Below();
                                                if (!Get.CellsInfo.CanPassThroughNoActors(vector3Int7))
                                                {
                                                    if (!Get.World.GetEntitiesAt(vector3Int7).Any<Entity>((Entity y) => y.Spec.IsStructure && y.Spec.Structure.AttachesToCeiling))
                                                    {
                                                        Vector3Int vector3Int8 = vector3Int3 - vector3Int.RightDir();
                                                        Vector3Int vector3Int9 = vector3Int3 + vector3Int.RightDir();
                                                        if (!Get.World.GetEntitiesAt(vector3Int8).Any<Entity>((Entity y) => y.Spec.IsStructure && y.Spec.Structure.AttachesToBack))
                                                        {
                                                            if (!Get.World.GetEntitiesAt(vector3Int9).Any<Entity>((Entity y) => y.Spec.IsStructure && y.Spec.Structure.AttachesToBack))
                                                            {
                                                                Vector3Int vector3Int10 = vector3Int5.Above();
                                                                Vector3Int vector3Int11 = vector3Int5.Below();
                                                                Vector3Int vector3Int12 = vector3Int5 - vector3Int.RightDir();
                                                                Vector3Int vector3Int13 = vector3Int5 + vector3Int.RightDir();
                                                                if (!Get.World.AnyEntityAt(vector3Int10) && !Get.World.AnyEntityAt(vector3Int11) && !Get.World.AnyEntityAt(vector3Int12) && !Get.World.AnyEntityAt(vector3Int13) && !allRoomCells.Contains(vector3Int10) && !allRoomCells.Contains(vector3Int11) && !allRoomCells.Contains(vector3Int12) && !allRoomCells.Contains(vector3Int13))
                                                                {
                                                                    if (list.Count == 0 || vector3Int3.IsAdjacent(list[list.Count - 1]))
                                                                    {
                                                                        list.Add(vector3Int3);
                                                                    }
                                                                    else
                                                                    {
                                                                        candidates.Add(list);
                                                                        list = new List<Vector3Int>();
                                                                        list.Add(vector3Int3);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (list.Count != 0)
                    {
                        candidates.Add(list);
                    }
                }
            }
        }

        private Structure CarveAt(Vector3Int cell, HashSet<Vector3Int> allRoomCells)
        {
            Get.World.GetFirstEntityOfSpecAt(cell, Get.Entity_Wall).DeSpawn(false);
            Structure structure = Maker.Make<Structure>(Get.Entity_Dirt, null, false, false, true);
            structure.Spawn(cell);
            foreach (Vector3Int vector3Int in Vector3IntUtility.Directions)
            {
                Vector3Int vector3Int2 = cell + vector3Int;
                if (!allRoomCells.Contains(vector3Int2) && !Get.World.AnyEntityAt(vector3Int2))
                {
                    Maker.Make((vector3Int.y != 0) ? Get.Entity_Floor : Get.Entity_Wall, null, false, false, true).Spawn(vector3Int2);
                }
            }
            return structure;
        }

        private const int DirtCount = 10;
    }
}