using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_EndStaircase : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            if (room.Role != Room.LayoutRole.End)
            {
                return;
            }
            RoomPart_EndStaircase.<> c__DisplayClass0_0 CS$<> 8__locals1;
            CS$<> 8__locals1.world = Get.World;
            Vector3Int vector3Int;
            if (room.FreeOnFloorNonBlocking.TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                RoomPart_EndStaircase.< Generate > g__Do | 0_0(vector3Int, ref CS$<> 8__locals1);
                return;
            }
            if (room.FreeOnFloorNoEntranceBlocking.TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                Log.Warning("Could not find a non-blocking place for the end room staircase. Used a fallback. May block path.", false);
                RoomPart_EndStaircase.< Generate > g__Do | 0_0(vector3Int, ref CS$<> 8__locals1);
                return;
            }
            if (room.FreeOnFloor.TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                Log.Warning("Could not find a non-blocking place for the end room staircase. Used a fallback. May block path and may be entrance-blocking.", false);
                RoomPart_EndStaircase.< Generate > g__Do | 0_0(vector3Int, ref CS$<> 8__locals1);
                return;
            }
            Log.Error("Could not place end room staircase.", false);
        }

        [CompilerGenerated]
        internal static void <Generate>g__Do|0_0(Vector3Int at, ref RoomPart_EndStaircase.<>c__DisplayClass0_0 A_1)
		{
			if (Get.RunSpec.FloorCount != null)
			{
				int floor = Get.Floor;
        int? floorCount = Get.RunSpec.FloorCount;
				if ((floor == floorCount.GetValueOrDefault()) & (floorCount != null))
				{
					Maker.Make(Get.Entity_RunEndPortal, null, false, false, true).Spawn(at);
					return;
				}
}
Maker.Make(Get.Entity_Staircase, null, false, false, true).Spawn(at);
Vector3Int vector3Int = at.Below();
foreach (Entity entity in A_1.world.GetEntitiesAt(vector3Int).ToList<Entity>())
{
    entity.DeSpawn(false);
}
Maker.Make(Get.Entity_InvisibleBlocker, null, false, false, true).Spawn(at.Below());
		}
	}
}