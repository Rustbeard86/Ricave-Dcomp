using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_CeilingAndFloorBars : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            if (!RoomPart_CeilingAndFloorBars.DoCeilingBars(room, memory))
            {
                RoomPart_CeilingAndFloorBars.DoFloorBars(room, memory);
            }
        }

        private static bool DoCeilingBars(Room room, WorldGenMemory memory)
        {
            RoomPart_CeilingAndFloorBars.<> c__DisplayClass1_0 CS$<> 8__locals1 = new RoomPart_CeilingAndFloorBars.<> c__DisplayClass1_0();
            if (room.RoomAbove != null)
            {
                return false;
            }
            if (room.Index % 2 != 1)
            {
                return false;
            }
            if (room.Height <= 3)
            {
                return false;
            }
            CS$<> 8__locals1.world = Get.World;
            int num = Rand.RangeInclusive(2, 4);
            bool flag = Rand.Chance(0.5f) || Get.WeatherManager.IsRainingOutside;
            bool flag2 = false;
            int i = 0;
            Vector3Int vector3Int;
            while (i < num && room.Shape.InnerCuboidXZ(1).TopSurfaceCuboid.Where<Vector3Int>(delegate (Vector3Int x)
            {
            if (CS$<> 8__locals1.world.AnyEntityOfSpecAt(x, Get.Entity_Floor) && !CS$<> 8__locals1.world.AnyEntityAt(x.Below()) && (i == 0 || CS$<> 8__locals1.world.AnyCardinalAdjacentEntityOfSpecAt(x, Get.Entity_CeilingBars) || CS$<> 8__locals1.world.AnyCardinalAdjacentEntityOfSpecAt(x, Get.Entity_CeilingBarsReinforced)))
            {
                IEnumerable<Vector3Int> enumerable = x.AdjacentCardinalCellsXZ();
                Func<Vector3Int, bool> func;
                if ((func = CS$<> 8__locals1.<> 9__2) == null)
					{
                    func = (CS$<> 8__locals1.<> 9__2 = (Vector3Int y) => CS$<> 8__locals1.world.CellsInfo.AnyPermanentFilledImpassableAt(y) || CS$<> 8__locals1.world.AnyEntityOfSpecAt(y, Get.Entity_CeilingBars) || CS$<> 8__locals1.world.AnyEntityOfSpecAt(y, Get.Entity_CeilingBarsReinforced));
                }
                return enumerable.All<Vector3Int>(func);
            }
            return false;
        }).TryGetRandomElement<Vector3Int>(out vector3Int))
			{
				CS$<>8__locals1.world.GetFirstEntityOfSpecAt(vector3Int, Get.Entity_Floor).DeSpawn(false);
        Maker.Make(WorldGenUtility.ShouldUseReinforcedGates? Get.Entity_CeilingBarsReinforced : Get.Entity_CeilingBars, null, false, false, true).Spawn(vector3Int);
        flag2 = true;
				if (flag)
				{
					Vector3Int vector3Int2 = vector3Int.WithY(room.StartY + 1);
        Predicate<Vector3Int> predicate;
					if ((predicate = CS$<>8__locals1.<>9__1) == null)
					{
						predicate = (CS$<>8__locals1.<>9__1 = (Vector3Int x) => !CS$<>8__locals1.world.CellsInfo.AnyStructureAt(x));
					}
					if (room.AllCellsToOneBelowCeilingPredicate(vector3Int2, predicate))
					{
						if (!Get.WeatherManager.IsRainingOutside)
						{
							flag = false;
						}
Maker.Make(Get.Entity_Rain, null, false, false, true).Spawn(vector3Int);
					}
				}
				if (!CS$<> 8__locals1.world.AnyEntityOfSpec(Get.Entity_CeilingBarsLight) && Rand.Chance(0.15f))
				{
    Maker.Make(Get.Entity_CeilingBarsLight, null, false, false, true).Spawn(vector3Int);
}
int j = i;
i = j + 1;
			}
			return flag2;
		}

		private static bool DoFloorBars(Room room, WorldGenMemory memory)
{
    RoomPart_CeilingAndFloorBars.<> c__DisplayClass2_0 CS$<> 8__locals1 = new RoomPart_CeilingAndFloorBars.<> c__DisplayClass2_0();
    if (room.RoomBelow != null)
    {
        return false;
    }
    if (!Rand.Chance(0.65f))
    {
        return false;
    }
    CS$<> 8__locals1.world = Get.World;
    int num = Rand.RangeInclusive(2, 4);
    bool flag = false;
    int i = 0;
    Vector3Int vector3Int;
    while (i < num && room.Shape.InnerCuboidXZ(1).BottomSurfaceCuboid.Where<Vector3Int>(delegate (Vector3Int x)
    {
    if (CS$<> 8__locals1.world.AnyEntityOfSpecAt(x, Get.Entity_Floor) && !CS$<> 8__locals1.world.AnyEntityAt(x.Above()) && !CS$<> 8__locals1.world.TiledDecals.AnyForcedAt(x.Above()) && (i == 0 || CS$<> 8__locals1.world.AnyCardinalAdjacentEntityOfSpecAt(x, Get.Entity_FloorBars) || CS$<> 8__locals1.world.AnyCardinalAdjacentEntityOfSpecAt(x, Get.Entity_FloorBarsReinforced)))
    {
        IEnumerable<Vector3Int> enumerable = x.AdjacentCardinalCellsXZ();
        Func<Vector3Int, bool> func;
        if ((func = CS$<> 8__locals1.<> 9__1) == null)
					{
            func = (CS$<> 8__locals1.<> 9__1 = (Vector3Int y) => CS$<> 8__locals1.world.CellsInfo.AnyPermanentFilledImpassableAt(y) || CS$<> 8__locals1.world.AnyEntityOfSpecAt(y, Get.Entity_FloorBars) || CS$<> 8__locals1.world.AnyEntityOfSpecAt(y, Get.Entity_FloorBarsReinforced));
        }
        return enumerable.All<Vector3Int>(func);
    }
    return false;
}).TryGetRandomElement<Vector3Int>(out vector3Int))
			{
    CS$<> 8__locals1.world.GetFirstEntityOfSpecAt(vector3Int, Get.Entity_Floor).DeSpawn(false);
    Maker.Make(WorldGenUtility.ShouldUseReinforcedGates ? Get.Entity_FloorBarsReinforced : Get.Entity_FloorBars, null, false, false, true).Spawn(vector3Int);
    Maker.Make(Get.Entity_DescendTrigger, null, false, false, true).Spawn(vector3Int);
    if (!CS$<> 8__locals1.world.AnyEntityOfSpec(Get.Entity_GentleSmoke))
				{
        Maker.Make(Get.Entity_GentleSmoke, null, false, false, true).Spawn(vector3Int);
    }
    if (!CS$<> 8__locals1.world.AnyEntityOfSpec(Get.Entity_FloorBarsLight) && Rand.Chance(0.15f))
				{
        Maker.Make(Get.Entity_FloorBarsLight, null, false, false, true).Spawn(vector3Int);
    }
    flag = true;
    int j = i;
    i = j + 1;
}
return flag;
		}
	}
}