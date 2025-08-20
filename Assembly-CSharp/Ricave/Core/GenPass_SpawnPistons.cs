using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class GenPass_SpawnPistons : GenPass
    {
        public override int SeedPart
        {
            get
            {
                return 280068217;
            }
        }

        public override void DoPass(WorldGenMemory memory)
        {
            this.pistonsInFront.Clear();
            for (int i = 0; i < 2; i++)
            {
                this.DoHorizontalPiston(memory);
            }
            this.DoVerticalPiston(memory);
        }

        private void DoHorizontalPiston(WorldGenMemory memory)
        {
            ValueTuple<Room, Vector3Int> valueTuple;
            if (memory.AllRooms.Where<Room>((Room x) => x.Role != Room.LayoutRole.Start && x.Role != Room.LayoutRole.LockedBehindSilverDoor && x.Role != Room.LayoutRole.OptionalChallenge && x.Role != Room.LayoutRole.Secret && x.AssignedRoomSpec != Get.Room_Pit).SelectMany<Room, ValueTuple<Room, Vector3Int>>((Room x) => from y in x.Shape.BottomSurfaceCuboid.WithAddedY(1).EdgeCellsXZNoCorners.Where<Vector3Int>(delegate (Vector3Int y)
                {
                    if (!Get.World.AnyEntityOfSpecAt(y, Get.Entity_Wall))
                    {
                        return false;
                    }
                    Vector3Int edge2 = x.Shape.GetEdge(y);
                    Vector3Int vector3Int = y - edge2;
                    Vector3Int vector3Int2 = vector3Int - edge2;
                    return vector3Int2.InBounds() && !Get.World.AnyEntityAt(vector3Int) && !this.pistonsInFront.Contains(vector3Int) && !Get.World.AnyEntityOfSpecAt(vector3Int2, Get.Entity_BigMagicTrap) && !Get.World.AnyEntityOfSpecAt(vector3Int2, Get.Entity_BigPoisonTrap) && (Get.CellsInfo.AnyFilledImpassableAt(vector3Int2) || (Get.CellsInfo.IsFloorUnderNoActors(vector3Int) && Get.CellsInfo.IsFallingAt(vector3Int2, Vector3IntUtility.Down, false, true, true)));
                })
                                                                                                                                                                                                                                                                                                                                 select new ValueTuple<Room, Vector3Int>(x, y)).TryGetRandomElement<ValueTuple<Room, Vector3Int>>(out valueTuple))
            {
                Vector3Int edge = valueTuple.Item1.Shape.GetEdge(valueTuple.Item2);
                Get.World.GetFirstEntityOfSpecAt(valueTuple.Item2, Get.Entity_Wall).DeSpawn(false);
                Maker.Make(Get.Entity_Piston, null, false, false, true).Spawn(valueTuple.Item2, -edge);
                this.pistonsInFront.Add(valueTuple.Item2 - edge);
            }
        }

        private void DoVerticalPiston(WorldGenMemory memory)
        {
            Vector3Int vector3Int;
            if (memory.AllRooms.Where<Room>((Room x) => x.Role != Room.LayoutRole.Start && x.Role != Room.LayoutRole.LockedBehindSilverDoor && x.Role != Room.LayoutRole.OptionalChallenge && x.Role != Room.LayoutRole.Secret).SelectMany<Room, Vector3Int>((Room x) => x.Shape.BottomSurfaceCuboid.InnerCuboidXZ(1).Where<Vector3Int>(delegate (Vector3Int y)
            {
                if (!Get.World.AnyEntityOfSpecAt(y, Get.Entity_Floor))
                {
                    return false;
                }
                Vector3Int vector3Int2 = y + Vector3IntUtility.Up;
                Vector3Int vector3Int3 = vector3Int2 + Vector3IntUtility.Up;
                if (!vector3Int3.InBounds())
                {
                    return false;
                }
                if (Get.World.AnyEntityAt(vector3Int2))
                {
                    return false;
                }
                if (this.pistonsInFront.Contains(vector3Int2))
                {
                    return false;
                }
                if (Get.CellsInfo.CanPassThroughNoActors(vector3Int3))
                {
                    bool flag = false;
                    foreach (Vector3Int vector3Int4 in vector3Int2.AdjacentCardinalCellsXZ())
                    {
                        if (vector3Int4.InBounds() && Get.CellsInfo.AnyFilledImpassableAt(vector3Int4) && vector3Int4.Above().InBounds() && Get.CellsInfo.CanPassThroughNoActors(vector3Int4.Above()))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        return false;
                    }
                }
                if (!Get.CellsInfo.CanPassThroughNoActors(vector3Int3))
                {
                    int num = 0;
                    foreach (Vector3Int vector3Int5 in vector3Int2.AdjacentCardinalCellsXZ())
                    {
                        if (vector3Int5.InBounds() && Get.CellsInfo.AnyFilledImpassableAt(vector3Int5))
                        {
                            num++;
                        }
                    }
                    if (num >= 3)
                    {
                        return false;
                    }
                }
                return true;
            })).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                Get.World.GetFirstEntityOfSpecAt(vector3Int, Get.Entity_Floor).DeSpawn(false);
                Maker.Make(Get.Entity_Piston, null, false, false, true).Spawn(vector3Int, Vector3IntUtility.Up);
                this.pistonsInFront.Add(vector3Int + Vector3IntUtility.Up);
            }
        }

        private List<Vector3Int> pistonsInFront = new List<Vector3Int>();
    }
}