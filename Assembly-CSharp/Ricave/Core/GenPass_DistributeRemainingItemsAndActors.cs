using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class GenPass_DistributeRemainingItemsAndActors : GenPass
    {
        public override int SeedPart
        {
            get
            {
                return 712428439;
            }
        }

        public override void DoPass(WorldGenMemory memory)
        {
            List<Vector3Int> list = this.GetCellCandidates(memory.storeys, memory.playerStartPos, false).ToList<Vector3Int>();
            List<Vector3Int> list2 = this.GetCellCandidates(memory.storeys, memory.playerStartPos, true).ToList<Vector3Int>();
            IEnumerable<Vector3Int> enumerable = from x in (from x in memory.storeys.SelectMany<Storey, Room>((Storey x) => x.Rooms)
                                                            where x.Role != Room.LayoutRole.LockedBehindSilverDoor && x.Role != Room.LayoutRole.OptionalChallenge && x.Role != Room.LayoutRole.Secret
                                                            select x).SelectMany<Room, Vector3Int>((Room x) => x.Shape.InnerCuboid(1).BottomSurfaceCuboid.Where<Vector3Int>((Vector3Int x) => Get.CellsInfo.CanPassThrough(x) && !Get.CellsInfo.AnyAIAvoidsAt(x) && Get.CellsInfo.IsFloorUnderNoActors(x)))
                                                 where x != memory.playerStartPos
                                                 select x;
            for (int i = memory.unusedBaseActors.Count - 1; i >= 0; i--)
            {
                if (list.Count == 0 && list != list2)
                {
                    list = list2;
                }
                Vector3Int vector3Int;
                if (list.Where<Vector3Int>((Vector3Int x) => x.GetRooms()[0].Role != Room.LayoutRole.Start).TryGetRandomElement<Vector3Int>(out vector3Int))
                {
                    memory.unusedBaseActors[i].Spawn(vector3Int);
                    list.Remove(vector3Int);
                    memory.unusedBaseActors.RemoveAt(i);
                }
                else if (enumerable.TryGetRandomElement<Vector3Int>(out vector3Int))
                {
                    memory.unusedBaseActors[i].Spawn(vector3Int);
                    memory.unusedBaseActors.RemoveAt(i);
                }
            }
            for (int j = memory.unusedBaseItems.Count - 1; j >= 0; j--)
            {
                if (list.Count == 0 && list != list2)
                {
                    list = list2;
                }
                Vector3Int vector3Int2;
                if (list.TryGetRandomElement<Vector3Int>(out vector3Int2))
                {
                    memory.unusedBaseItems[j].Spawn(vector3Int2);
                    list.Remove(vector3Int2);
                    memory.unusedBaseItems.RemoveAt(j);
                }
                else if (enumerable.Where<Vector3Int>((Vector3Int x) => !Get.CellsInfo.AnyItemAt(x)).TryGetRandomElement<Vector3Int>(out vector3Int2))
                {
                    memory.unusedBaseItems[j].Spawn(vector3Int2);
                    memory.unusedBaseItems.RemoveAt(j);
                }
            }
            Actor actor = memory.baseActors.Find((Actor x) => x.IsBoss && !x.Spawned);
            if (actor != null && !actor.Spawned)
            {
                if (list.Count == 0 && list != list2)
                {
                    list = list2;
                }
                List<Vector3Int> list3 = list.Where<Vector3Int>((Vector3Int x) => x.GetRooms()[0].Role != Room.LayoutRole.End).ToList<Vector3Int>();
                if (list3.Count == 0)
                {
                    list3 = list2;
                }
                if (list3.Count == 0)
                {
                    list3 = enumerable.ToList<Vector3Int>();
                }
                if (list3.Any())
                {
                    int farthest = list3.Max<Vector3Int>((Vector3Int x) => x.GetGridDistance(memory.playerStartPos));
                    RetainedRoomInfo.RoomInfo bossRoom;
                    if ((from x in list3
                         where x.GetGridDistance(memory.playerStartPos) == farthest
                         select x.GetRooms()[0]).Distinct<RetainedRoomInfo.RoomInfo>().TryGetRandomElement<RetainedRoomInfo.RoomInfo>(out bossRoom))
                    {
                        Vector3Int vector3Int3;
                        if (list3.Where<Vector3Int>((Vector3Int x) => x.GetRooms()[0] == bossRoom).TryGetRandomElement<Vector3Int>(out vector3Int3))
                        {
                            actor.Spawn(vector3Int3);
                            list.Remove(vector3Int3);
                        }
                        else
                        {
                            Log.Warning("Could not spawn boss.", false);
                        }
                    }
                    else
                    {
                        Log.Warning("Could not spawn boss.", false);
                    }
                }
                else
                {
                    Log.Warning("Could not spawn boss.", false);
                }
            }
            if (memory.unusedBaseItems.Count != 0)
            {
                Log.Warning("Not all items were distributed (not enough space).", false);
            }
            if (memory.unusedBaseActors.Count != 0)
            {
                Log.Warning("Not all actors were distributed (not enough space).", false);
            }
        }

        private IEnumerable<Vector3Int> GetCellCandidates(List<Storey> storeys, Vector3Int playerStartPos, bool desperate)
        {
            World world = Get.World;
            return from x in (from x in storeys.SelectMany<Storey, Room>((Storey x) => x.Rooms)
                              where x.Role != Room.LayoutRole.LockedBehindSilverDoor && x.Role != Room.LayoutRole.OptionalChallenge && x.Role != Room.LayoutRole.Secret
                              select x).SelectMany<Room, Vector3Int>((Room x) => x.FreeOnFloor)
                   where x != playerStartPos && (desperate || !base.< GetCellCandidates > g__AdjacentToDoorStairsOrLadder | 0(x))
                   select x;
        }
    }
}