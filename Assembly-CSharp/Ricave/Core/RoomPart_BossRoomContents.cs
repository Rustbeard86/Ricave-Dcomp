using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_BossRoomContents : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            if (!room.IsBossRoom)
            {
                return;
            }
            Actor actor = memory.baseActors.Find((Actor x) => x.IsBoss && !x.Spawned);
            if (actor == null || actor.Spawned)
            {
                Log.Warning("No boss to spawn in boss room.", false);
                return;
            }
            Vector3Int vector3Int;
            bool flag = this.DoThrone(room, memory, out vector3Int);
            if (!this.DoBoss(actor, room, memory, flag ? new Vector3Int?(vector3Int) : null))
            {
                Log.Warning("Could not spawn boss anywhere in boss room.", false);
            }
        }

        private bool DoThrone(Room room, WorldGenMemory memory, out Vector3Int thronePos)
        {
            Vector3Int vector3Int;
            if (!room.FreeOnFloorNonBlocking.Where<Vector3Int>((Vector3Int x) => x.AdjacentCardinalCellsXZ().Any<Vector3Int>((Vector3Int y) => Get.CellsInfo.AnyFilledImpassableAt(y)) && Get.CellsInfo.IsFilled(x.Below())).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                if (!room.FreeOnFloorNonBlocking.Where<Vector3Int>((Vector3Int x) => Get.CellsInfo.IsFilled(x.Below())).TryGetRandomElement<Vector3Int>(out vector3Int) && !room.FreeOnFloorNonBlocking.TryGetRandomElement<Vector3Int>(out vector3Int))
                {
                    thronePos = default(Vector3Int);
                    return false;
                }
            }
            Structure structure = Maker.Make<Structure>(Get.Entity_Throne, null, false, false, true);
            if (!Get.RunConfig.ProgressDisabled)
            {
                structure.InnerEntities.Add(ItemGenerator.Stardust(5));
            }
            Vector3Int? vector3Int2 = (from x in vector3Int.AdjacentCardinalCellsXZ()
                                       where Get.CellsInfo.IsFilled(x)
                                       select new Vector3Int?(x)).FirstOrDefault<Vector3Int?>();
            if (vector3Int2 != null && Get.CellsInfo.CanPassThrough(vector3Int + (vector3Int - vector3Int2.Value)))
            {
                structure.Spawn(vector3Int, vector3Int - vector3Int2.Value);
            }
            else
            {
                structure.Spawn(vector3Int);
            }
            foreach (Vector3Int vector3Int3 in vector3Int.AdjacentCellsXZAndInside())
            {
                if (vector3Int3.InBounds() && !Get.CellsInfo.IsFilled(vector3Int3) && Get.CellsInfo.IsFloorUnderNoActors(vector3Int3))
                {
                    Get.TiledDecals.SetForced(Get.TiledDecals_Carpet, vector3Int3);
                }
            }
            thronePos = vector3Int;
            return true;
        }

        private bool DoBoss(Actor boss, Room room, WorldGenMemory memory, Vector3Int? thronePos)
        {
            Func<Vector3Int, bool> <> 9__1;
            Vector3Int value;
            if (!room.FreeOnFloor.Where<Vector3Int>(delegate (Vector3Int x)
            {
                IEnumerable<Vector3Int> enumerable = x.AdjacentCellsXZAndInside();
                Func<Vector3Int, bool> func;
                if ((func = <> 9__1) == null)
                {
                    func = (<> 9__1 = (Vector3Int y) => room.IsEntrance(y, true, true) || Get.CellsInfo.AnyLadderAt(y.Below()));
                }
                return !enumerable.Any<Vector3Int>(func);
            }).TryGetRandomElement<Vector3Int>(out value) && !room.FreeOnFloor.TryGetRandomElement<Vector3Int>(out value))
            {
                if (thronePos == null || !Get.CellsInfo.CanPassThrough(thronePos.Value))
                {
                    return false;
                }
                value = thronePos.Value;
            }
            boss.Spawn(value);
            return true;
        }
    }
}