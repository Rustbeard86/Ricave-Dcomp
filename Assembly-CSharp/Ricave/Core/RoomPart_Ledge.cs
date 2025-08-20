using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_Ledge : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            World world = Get.World;
            EntitySpec entitySpec = (Rand.Chance(0.6f) ? Get.Entity_Bridge : Get.Entity_Floor);
            List<Vector3Int> list = new List<Vector3Int>();
            foreach (Vector3Int vector3Int in room.Shape.BottomSurfaceCuboid.InnerCuboidXZ(1).EdgeCellsXZ)
            {
                if (!world.AnyEntityAt(vector3Int))
                {
                    if (entitySpec == Get.Entity_Bridge && (RoomPart_Ledge.< Generate > g__AnyAttachesToCeilingBelow | 0_0(vector3Int) || RoomPart_Ledge.< Generate > g__AnyPillarBelow | 0_1(vector3Int)))
                    {
                        Maker.Make(Get.Entity_WoodenFloor, null, false, false, true).Spawn(vector3Int);
                    }
                    else if (entitySpec == Get.Entity_Bridge)
                    {
                        list.Add(vector3Int);
                    }
                    else
                    {
                        Maker.Make(entitySpec, null, false, false, true).Spawn(vector3Int);
                    }
                }
            }
            if (Rand.Chance(0.35f))
            {
                CellCuboid cellCuboid = default(CellCuboid);
                if (room.Shape.width >= 9)
                {
                    cellCuboid = new CellCuboid(room.Shape.Center.x, room.StartY, room.Shape.z + 2, 1, 1, room.Shape.depth - 4);
                }
                else if (room.Shape.depth >= 9)
                {
                    cellCuboid = new CellCuboid(room.Shape.x + 2, room.StartY, room.Shape.Center.z, room.Shape.width - 4, 1, 1);
                }
                if (!cellCuboid.Empty && cellCuboid.All<Vector3Int>((Vector3Int x) => !world.AnyEntityAt(x)))
                {
                    foreach (Vector3Int vector3Int2 in cellCuboid)
                    {
                        if (!RoomPart_Ledge.< Generate > g__AnyPillarBelow | 0_1(vector3Int2))
                        {
                            if (RoomPart_Ledge.< Generate > g__AnyAttachesToCeilingBelow | 0_0(vector3Int2))
                            {
                                Maker.Make(Get.Entity_WoodenFloor, null, false, false, true).Spawn(vector3Int2);
                            }
                            else
                            {
                                list.Add(vector3Int2);
                            }
                        }
                    }
                }
            }
            Vector3Int vector3Int3;
            if (memory.config.Floor >= 2 && Rand.Chance(0.5f))
            {
                list.Where<Vector3Int>((Vector3Int x) => !room.IsEntranceCellToAvoidBlocking(x.Above(), true)).TryGetRandomElement<Vector3Int>(out vector3Int3);
            }
            else
            {
                vector3Int3 = default(Vector3Int);
            }
            for (int i = 0; i < list.Count; i++)
            {
                Maker.Make((list[i] == vector3Int3) ? Get.Entity_UnstableBridge : Get.Entity_Bridge, null, false, false, true).Spawn(list[i]);
            }
            CellCuboid cellCuboid2 = room.Shape.BottomSurfaceCuboid.InnerCuboidXZ(2);
            if (cellCuboid2.width >= 3 && cellCuboid2.depth >= 3)
            {
                List<Vector3Int> list2 = cellCuboid2.EdgeCellsXZ.ToList<Vector3Int>();
                int num = (int)((float)list2.Count * 0.35f);
                Func<Vector3Int, bool> <> 9__5;
                for (int j = 0; j < num; j++)
                {
                    IEnumerable<Vector3Int> enumerable = list2;
                    Func<Vector3Int, bool> func;
                    if ((func = <> 9__5) == null)
                    {
                        func = (<> 9__5 = (Vector3Int x) => !world.AnyEntityAt(x));
                    }
                    Vector3Int vector3Int4;
                    if (!enumerable.Where<Vector3Int>(func).TryGetRandomElement<Vector3Int>(out vector3Int4))
                    {
                        break;
                    }
                    if (!RoomPart_Ledge.< Generate > g__AnyPillarBelow | 0_1(vector3Int4))
                    {
                        if (RoomPart_Ledge.< Generate > g__AnyAttachesToCeilingBelow | 0_0(vector3Int4))
                        {
                            Maker.Make(Get.Entity_WoodenFloor, null, false, false, true).Spawn(vector3Int4);
                        }
                        else
                        {
                            Maker.Make(Rand.Bool ? Get.Entity_UnstableBridge : Get.Entity_Bridge, null, false, false, true).Spawn(vector3Int4);
                        }
                    }
                }
            }
        }

        [CompilerGenerated]
        internal static bool <Generate>g__AnyAttachesToCeilingBelow|0_0(Vector3Int c)
		{
			if (c.Below().InBounds())
			{
				return Get.World.GetEntitiesAt(c.Below()).Any<Entity>((Entity x) => x is Structure && x.Spec.Structure.AttachesToCeiling);
			}
			return false;
		}

[CompilerGenerated]
internal static bool < Generate > g__AnyPillarBelow | 0_1(Vector3Int c)

        {
    return c.Below().InBounds() && Get.World.AnyEntityOfSpecAt(c.Below(), Get.Entity_Pillar);
}
	}
}