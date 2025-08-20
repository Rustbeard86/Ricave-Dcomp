using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_Windows : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            World world = Get.World;
            if (room.Height < 4 || room.Role == Room.LayoutRole.End || room.Role == Room.LayoutRole.LockedBehindSilverDoor || room.Role == Room.LayoutRole.OptionalChallenge || room.Role == Room.LayoutRole.Secret)
            {
                return;
            }
            Func<Vector3Int, bool> <> 9__0;
            Func<Vector3Int, bool> <> 9__2;
            Func<Vector3Int, bool> <> 9__3;
            Func<Vector3Int, bool> <> 9__1;
            foreach (Room room2 in room.AdjacentRooms)
            {
                if (room2.Height >= 4 && room2.StartY == room.StartY && room2.Role != Room.LayoutRole.End && room2.Role != Room.LayoutRole.LockedBehindSilverDoor && room2.Role != Room.LayoutRole.OptionalChallenge && room2.Role != Room.LayoutRole.Secret && (room.Storey.Index + room2.Index + room.Index) % 2 != 0)
                {
                    IEnumerable<Vector3Int> enumerable = room.GetWallBetween(room2);
                    Func<Vector3Int, bool> func;
                    if ((func = <> 9__0) == null)
                    {
                        func = (<> 9__0 = (Vector3Int x) => world.AnyEntityOfSpecAt(x, Get.Entity_Frame));
                    }
                    if (!enumerable.Any<Vector3Int>(func))
                    {
                        IEnumerable<Vector3Int> enumerable2 = room.GetWallBetween(room2);
                        Func<Vector3Int, bool> func2;
                        if ((func2 = <> 9__1) == null)
                        {
                            func2 = (<> 9__1 = delegate (Vector3Int x)
                            {
                                if (world.AnyEntityOfSpecAt(x, Get.Entity_Wall) && x.y >= room.StartY + 2 && world.CellsInfo.IsFilled(x.Below()))
                                {
                                    IEnumerable<Vector3Int> enumerable3 = x.AdjacentCardinalCellsXZ();
                                    Func<Vector3Int, bool> func3;
                                    if ((func3 = <> 9__2) == null)
                                    {
                                        func3 = (<> 9__2 = (Vector3Int y) => world.InBounds(y) && world.CellsInfo.CanPassThroughNoActors(y));
                                    }
                                    if (enumerable3.Count<Vector3Int>(func3) >= 2)
                                    {
                                        IEnumerable<Vector3Int> enumerable4 = x.AdjacentCardinalCellsXZ();
                                        Func<Vector3Int, bool> func4;
                                        if ((func4 = <> 9__3) == null)
                                        {
                                            func4 = (<> 9__3 = delegate (Vector3Int y)
                                            {
                                                if (!world.InBounds(y))
                                                {
                                                    return false;
                                                }
                                                if (!world.CellsInfo.AnyLadderAt(y))
                                                {
                                                    return world.GetEntitiesAt(y).Any<Entity>((Entity z) => z is Structure && z.Spec.Structure.AttachesToBack);
                                                }
                                                return true;
                                            });
                                        }
                                        return !enumerable4.Any<Vector3Int>(func4);
                                    }
                                }
                                return false;
                            });
                        }
                        Vector3Int vector3Int;
                        if (enumerable2.Where<Vector3Int>(func2).TryGetRandomElement<Vector3Int>(out vector3Int))
                        {
                            world.GetFirstEntityOfSpecAt(vector3Int, Get.Entity_Wall).DeSpawn(false);
                            Maker.Make(Get.Entity_Frame, null, false, false, true).Spawn(vector3Int);
                            Maker.Make(Get.Entity_Ventillation, null, false, false, true).Spawn(vector3Int, room.GetAdjacentRoomDir(room2).RightDir());
                            Maker.Make(Get.Entity_Ventillation, null, false, false, true).Spawn(vector3Int, -room.GetAdjacentRoomDir(room2).RightDir());
                            if (Rand.ChanceSeeded(0.5f, Calc.CombineHashes<int, int>(memory.config.WorldSeed, 15947720)) && !memory.generatedGoldInWindow && Rand.Chance(0.75f))
                            {
                                ItemGenerator.SmallReward(true).Spawn(vector3Int);
                                memory.generatedGoldInWindow = true;
                            }
                            if (room.Role == Room.LayoutRole.LockedBehindGate || room2.Role == Room.LayoutRole.LockedBehindGate)
                            {
                                Maker.Make(Get.Entity_Gate, null, false, false, true).Spawn(vector3Int, room.GetAdjacentRoomDir(room2).RightDir());
                            }
                        }
                    }
                }
            }
        }
    }
}