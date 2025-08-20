using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_LightSource : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            RoomPart_LightSource.<> c__DisplayClass0_0 CS$<> 8__locals1 = new RoomPart_LightSource.<> c__DisplayClass0_0();
            CS$<> 8__locals1.room = room;
            if (this.AlreadyHasMajorLightSource(CS$<> 8__locals1.room))
            {
                return;
            }
            EntitySpec lightSource = this.GetLightSource(CS$<> 8__locals1.room, memory);
            if (lightSource == null)
            {
                return;
            }
            CS$<> 8__locals1.world = Get.World;
            Vector3Int vector3Int8;
            if (lightSource.Structure.AttachesToBack)
            {
                bool requiresNoFilledAbove = lightSource == Get.Entity_Torch || lightSource == Get.Entity_VioletTorch || lightSource == Get.Entity_WallLamp || lightSource == Get.Entity_YellowWallLamp || lightSource == Get.Entity_GreenWallLamp || lightSource == Get.Entity_VioletWallLamp || lightSource == Get.Entity_PinkWallLamp || lightSource == Get.Entity_CyanWallLamp;
                Vector3Int vector3Int;
                if (CS$<> 8__locals1.room.Shape.InnerCuboid(1).BottomSurfaceCuboid.EdgeCellsXZ.Where<Vector3Int>(delegate (Vector3Int x)
                {
                if (base.< Generate > g__BaseValidator | 0(x))
                {
                    IEnumerable<Vector3Int> enumerable = x.AdjacentCardinalCellsXZ();
                    Func<Vector3Int, bool> func;
                    if ((func = CS$<> 8__locals1.<> 9__2) == null)
						{
                    func = (CS$<> 8__locals1.<> 9__2 = (Vector3Int y) => CS$<> 8__locals1.room.Shape.IsOnEdgeXZ(y) && CS$<> 8__locals1.world.CellsInfo.AnyPermanentFilledImpassableAt(y));
                }
                return enumerable.Any<Vector3Int>(func);
            }
            return false;
        }).TryGetRandomElement<Vector3Int>(out vector3Int))
				{
					Maker.Make(lightSource, null, false, false, true).Spawn(vector3Int);
					return;
				}
				Vector3Int vector3Int2;
				if (CS$<>8__locals1.room.Shape.InnerCuboid(1).BottomSurfaceCuboid.Where<Vector3Int>(delegate (Vector3Int x)
				{
					if (base.<Generate>g__BaseValidator|0(x))
					{
						IEnumerable<Vector3Int> enumerable2 = x.AdjacentCardinalCellsXZ();
    Func<Vector3Int, bool> func2;
						if ((func2 = CS$<>8__locals1.<>9__4) == null)
						{
							func2 = (CS$<>8__locals1.<>9__4 = (Vector3Int y) => CS$<>8__locals1.world.CellsInfo.AnyPermanentFilledImpassableAt(y));
						}
return enumerable2.Any<Vector3Int>(func2);
					}
					return false;
				}).TryGetRandomElement<Vector3Int>(out vector3Int2))
				{
    Maker.Make(lightSource, null, false, false, true).Spawn(vector3Int2);
    return;
}
Vector3Int vector3Int3;
if (CS$<> 8__locals1.room.Shape.InnerCuboid(1).BottomSurfaceCuboid.Where<Vector3Int>(delegate (Vector3Int x)
{
if (base.< Generate > g__BaseValidator | 0(x))
{
    IEnumerable<Vector3Int> enumerable3 = x.AdjacentCardinalCellsXZ();
    Func<Vector3Int, bool> func3;
    if ((func3 = CS$<> 8__locals1.<> 9__6) == null)
						{
    func3 = (CS$<> 8__locals1.<> 9__6 = (Vector3Int y) => CS$<> 8__locals1.world.CellsInfo.AnyFilledImpassableAt(y));
}
return enumerable3.Any<Vector3Int>(func3);
					}
					return false;
				}).TryGetRandomElement<Vector3Int>(out vector3Int3))
				{
    Maker.Make(lightSource, null, false, false, true).Spawn(vector3Int3);
    return;
}
			}

            else if (lightSource.Structure.AttachesToCeiling)
{
    CellCuboid edge = CS$<> 8__locals1.room.Shape.InnerCuboidXZ(1);
    bool flag = !lightSource.CanPassThrough || (lightSource.IsStructure && lightSource.Structure.AIAvoids);
    Vector3Int vector3Int5;
    if (flag)
    {
        Vector3Int vector3Int4;
        if (CS$<> 8__locals1.room.FreeBelowCeilingNonBlocking.Where<Vector3Int>((Vector3Int x) => !edge.IsOnEdgeXZ(x) && CS$<> 8__locals1.< Generate > g__HasSupport | 7(x)).TryGetRandomElement<Vector3Int>(out vector3Int4))
					{
            Maker.Make(lightSource, null, false, false, true).Spawn(vector3Int4);
            return;
        }
    }
    else if (CS$<> 8__locals1.room.FreeBelowCeilingNoEntranceBlocking.Where<Vector3Int>((Vector3Int x) => !edge.IsOnEdgeXZ(x) && CS$<> 8__locals1.< Generate > g__HasSupport | 7(x)).TryGetRandomElement<Vector3Int>(out vector3Int5))
				{
        Maker.Make(lightSource, null, false, false, true).Spawn(vector3Int5);
        return;
    }
    Vector3Int vector3Int7;
    if (flag)
    {
        Vector3Int vector3Int6;
        if (CS$<> 8__locals1.room.FreeBelowCeilingNonBlocking.Where<Vector3Int>((Vector3Int x) => base.< Generate > g__HasSupport | 7(x)).TryGetRandomElement<Vector3Int>(out vector3Int6))
					{
            Maker.Make(lightSource, null, false, false, true).Spawn(vector3Int6);
            return;
        }
    }
    else if (CS$<> 8__locals1.room.FreeBelowCeilingNoEntranceBlocking.Where<Vector3Int>((Vector3Int x) => base.< Generate > g__HasSupport | 7(x)).TryGetRandomElement<Vector3Int>(out vector3Int7))
				{
        Maker.Make(lightSource, null, false, false, true).Spawn(vector3Int7);
        return;
    }
}
else if (CS$<> 8__locals1.room.TryFindRandomNonBlockingPosForEntity(lightSource, out vector3Int8, true, false))
			{
    Maker.Make(lightSource, null, false, false, true).Spawn(vector3Int8);
}
		}

		private bool AlreadyHasMajorLightSource(Room room)
{
    return room.ContainsAnyEntityOfSpec(Get.Entity_Portal) || room.ContainsAnyEntityOfSpec(Get.Entity_CeilingBarsLight) || room.ContainsAnyEntityOfSpec(Get.Entity_FloorBarsLight);
}

private EntitySpec GetLightSource(Room room, WorldGenMemory memory)
{
    if (room.Storey.IsUndergroundStorey)
    {
        return null;
    }
    if (room.Role == Room.LayoutRole.OptionalChallenge && !room.AdjacentRooms.Any() && room.RoomBelow != null)
    {
        return null;
    }
    if (room.ContainsAnyEntityOfSpec(Get.Entity_CultistCircle) && room.Height > 3)
    {
        return Get.Entity_VioletTorch;
    }
    if (Rand.ChanceSeeded(0.25f, Calc.CombineHashes<int, int>(memory.config.WorldSeed, 817409081)) && !Get.World.AnyEntityOfSpec(Get.Entity_RedLamp) && Rand.Chance(0.3f) && room.Role != Room.LayoutRole.LockedBehindSilverDoor && room.Role != Room.LayoutRole.Secret && room.Role != Room.LayoutRole.OptionalChallenge && room.Height >= 4 && room.RoomAbove == null)
    {
        return Get.Entity_RedLamp;
    }
    if (Rand.ChanceSeeded(0.02f, Calc.CombineHashes<int, int>(memory.config.WorldSeed, 73083799)) && !Get.World.AnyEntityOfSpec(Get.Entity_DiscoBall) && Rand.Chance(0.3f) && room.Role != Room.LayoutRole.LockedBehindSilverDoor && room.Role != Room.LayoutRole.Secret && room.Role != Room.LayoutRole.OptionalChallenge && room.Height >= 4 && memory.config.Floor >= 3 && room.RoomAbove == null)
    {
        return Get.Entity_DiscoBall;
    }
    if (room.Height <= 3)
    {
        return null;
    }
    if (!Get.World.AnyEntityOfSpec(Get.Entity_GreenWallLamp) && !Get.World.AnyEntityOfSpec(Get.Entity_VioletWallLamp) && !Get.World.AnyEntityOfSpec(Get.Entity_PinkWallLamp) && !Get.World.AnyEntityOfSpec(Get.Entity_CyanWallLamp) && Rand.Chance(0.2f) && room.Role != Room.LayoutRole.LockedBehindSilverDoor && room.Role != Room.LayoutRole.Secret && room.Role != Room.LayoutRole.OptionalChallenge)
    {
        return Rand.Element<EntitySpec>(Get.Entity_GreenWallLamp, Get.Entity_VioletWallLamp, Get.Entity_PinkWallLamp, Get.Entity_CyanWallLamp);
    }
    return Get.Entity_Torch;
}
	}
}