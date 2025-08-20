using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_SignsAboveEntrances : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            foreach (Room room2 in room.AdjacentRooms)
            {
                this.CheckBetween(room, room2, memory);
                if (room2.Generated)
                {
                    this.CheckBetween(room2, room, memory);
                }
            }
        }

        private void CheckBetween(Room room, Room adj, WorldGenMemory memory)
        {
            EntitySpec signSpec = this.GetSignSpec(adj);
            if (signSpec == null)
            {
                return;
            }
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
                    Vector3Int adjacentRoomDir = adj.GetAdjacentRoomDir(room);
                    RoomPart_SignsAboveEntrances.CheckPlaceSign((existingDoor.Value + adjacentRoomDir).Above(), adjacentRoomDir, signSpec, room, adj);
                    return;
                }
            }
            else if (transitionType == Room.TransitionType.Stairs)
            {
                Vector3Int? existingStairs = RoomPart_RoomTransitions.GetExistingStairs(room, adj, memory);
                if (existingStairs != null)
                {
                    Vector3Int adjacentRoomDir2 = adj.GetAdjacentRoomDir(room);
                    Vector3Int vector3Int = existingStairs.Value + adjacentRoomDir2;
                    vector3Int.y = room.StartY + 1;
                    RoomPart_SignsAboveEntrances.CheckPlaceSign(vector3Int.Above(), adjacentRoomDir2, signSpec, room, adj);
                }
            }
        }

        private static void CheckPlaceSign(Vector3Int at, Vector3Int dir, EntitySpec signSpec, Room room, Room adjRoom)
        {
            if (Get.World.AnyEntityAt(at))
            {
                return;
            }
            if (at.y >= room.Shape.yMax)
            {
                return;
            }
            if ((adjRoom.RoomAbove == null || adjRoom.RoomAbove.Generated || Get.World.AnyEntityAt(at - dir) || at.y != adjRoom.Shape.yMax) && !ItemOrStructureFallUtility.WouldHaveSupport(signSpec, at, new Vector3Int?(dir)))
            {
                return;
            }
            Maker.Make(signSpec, null, false, false, true).Spawn(at, dir);
        }

        private EntitySpec GetSignSpec(Room room)
        {
            if (room.Role == Room.LayoutRole.End)
            {
                return Get.Entity_StaircaseRoomSign;
            }
            return null;
        }
    }
}