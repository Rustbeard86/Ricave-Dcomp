using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_PressurePlates : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            if (Get.World.AnyEntityOfSpec(Get.Entity_PressurePlate))
            {
                return;
            }
            foreach (Room room2 in room.AdjacentRooms)
            {
                this.CheckBetween(room, room2, memory);
            }
        }

        private void CheckBetween(Room room, Room adj, WorldGenMemory memory)
        {
            Room.TransitionType transitionType = room.GetTransitionType(adj);
            if (transitionType == Room.TransitionType.None)
            {
                return;
            }
            if (transitionType == Room.TransitionType.SameLevel)
            {
                Vector3Int? existingDoor = RoomPart_RoomTransitions.GetExistingDoor(room, adj, memory);
                if (existingDoor != null)
                {
                    Vector3Int vector3Int = existingDoor.Value.AdjacentCardinalCellsXZ().First<Vector3Int>((Vector3Int x) => room.Shape.Contains(x) && !room.Shape.IsOnEdgeXZ(x));
                    RoomPart_PressurePlates.CheckPlacePressurePlate(room, vector3Int, adj);
                    return;
                }
            }
            else if (transitionType == Room.TransitionType.Stairs)
            {
                Vector3Int? existingStairs = RoomPart_RoomTransitions.GetExistingStairs(room, adj, memory);
                if (existingStairs != null)
                {
                    Vector3Int vector3Int2 = existingStairs.Value.AdjacentCardinalCellsXZ().First<Vector3Int>((Vector3Int x) => room.Shape.Contains(x) && !room.Shape.IsOnEdgeXZ(x));
                    vector3Int2.y = room.StartY + 1;
                    RoomPart_PressurePlates.CheckPlacePressurePlate(room, vector3Int2, adj);
                }
            }
        }

        private static void CheckPlacePressurePlate(Room room, Vector3Int at, Room adjacentRoom)
        {
            if (!room.Storey.IsMainStorey)
            {
                return;
            }
            if (room.Role == Room.LayoutRole.Start || room.Role == Room.LayoutRole.End || room.Role == Room.LayoutRole.LockedBehindGate || room.Role == Room.LayoutRole.LockedBehindSilverDoor || room.Role == Room.LayoutRole.OptionalChallenge || room.Role == Room.LayoutRole.Secret)
            {
                return;
            }
            if (adjacentRoom.Role == Room.LayoutRole.End || adjacentRoom.Role == Room.LayoutRole.LockedBehindGate || adjacentRoom.Role == Room.LayoutRole.LockedBehindSilverDoor || adjacentRoom.Role == Room.LayoutRole.OptionalChallenge || adjacentRoom.Role == Room.LayoutRole.Secret)
            {
                return;
            }
            if (Get.World.AnyEntityOfSpec(Get.Entity_PressurePlate))
            {
                return;
            }
            if (Get.World.AnyEntityAt(at))
            {
                return;
            }
            if (!Get.CellsInfo.IsFloorUnder(at))
            {
                return;
            }
            Dictionary<Room, int> dictionary = new Dictionary<Room, int>();
            Func<Room, bool> <> 9__2;
            BFS<Room>.TraverseAll(adjacentRoom, delegate (Room x)
            {
                IEnumerable<Room> adjacentRooms = x.AdjacentRooms;
                Func<Room, bool> func;
                if ((func = <> 9__2) == null)
                {
                    func = (<> 9__2 = (Room y) => y != room);
                }
                return adjacentRooms.Where<Room>(func);
            }, dictionary, null);
            if (!dictionary.Any<KeyValuePair<Room, int>>((KeyValuePair<Room, int> x) => x.Key.Role == Room.LayoutRole.Start))
            {
                return;
            }
            Maker.Make(Get.Entity_PressurePlate, null, false, false, true).Spawn(at);
        }
    }
}