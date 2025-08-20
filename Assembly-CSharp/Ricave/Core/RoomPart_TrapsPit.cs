using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_TrapsPit : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            World world = Get.World;
            Vector3Int vector3Int;
            int num;
            int num2;
            Func<Vector3Int, Vector3Int> func;
            RoomPart_TrapsPit.GetDirectionalDimensions(room, out vector3Int, out num, out num2, out func);
            CellRect cellRect = default(CellRect);
            CellRect cellRect2 = default(CellRect);
            CellRect bridgeRect = default(CellRect);
            CellRect pitRect = default(CellRect);
            if (vector3Int == Vector3IntUtility.Forward)
            {
                cellRect = new CellRect(room.Surface.xMin + 1, room.Surface.yMin + 1, room.Surface.width - 2, 1);
                cellRect2 = new CellRect(room.Surface.xMin + 1, room.Surface.yMax - 1, room.Surface.width - 2, 1);
                pitRect = new CellRect(room.Surface.xMin + 1, room.Surface.yMin + 2, room.Surface.width - 2, room.Surface.height - 4);
                bridgeRect = new CellRect(RoomPart_TrapsPit.< Generate > g__GetRandomBridgePos | 1_0(pitRect.xMin, pitRect.xMax), pitRect.yMin, 1, pitRect.height);
            }
            else if (vector3Int == Vector3IntUtility.Back)
            {
                cellRect = new CellRect(room.Surface.xMin + 1, room.Surface.yMax - 1, room.Surface.width - 2, 1);
                cellRect2 = new CellRect(room.Surface.xMin + 1, room.Surface.yMin + 1, room.Surface.width - 2, 1);
                pitRect = new CellRect(room.Surface.xMin + 1, room.Surface.yMin + 2, room.Surface.width - 2, room.Surface.height - 4);
                bridgeRect = new CellRect(RoomPart_TrapsPit.< Generate > g__GetRandomBridgePos | 1_0(pitRect.xMin, pitRect.xMax), pitRect.yMin, 1, pitRect.height);
            }
            else if (vector3Int == Vector3IntUtility.Right)
            {
                cellRect = new CellRect(room.Surface.xMin + 1, room.Surface.yMin + 1, 1, room.Surface.height - 2);
                cellRect2 = new CellRect(room.Surface.xMax - 1, room.Surface.yMin + 1, 1, room.Surface.height - 2);
                pitRect = new CellRect(room.Surface.xMin + 2, room.Surface.yMin + 1, room.Surface.width - 4, room.Surface.height - 2);
                bridgeRect = new CellRect(pitRect.xMin, RoomPart_TrapsPit.< Generate > g__GetRandomBridgePos | 1_0(pitRect.yMin, pitRect.yMax), pitRect.width, 1);
            }
            else if (vector3Int == Vector3IntUtility.Left)
            {
                cellRect = new CellRect(room.Surface.xMax - 1, room.Surface.yMin + 1, 1, room.Surface.height - 2);
                cellRect2 = new CellRect(room.Surface.xMin + 1, room.Surface.yMin + 1, 1, room.Surface.height - 2);
                pitRect = new CellRect(room.Surface.xMin + 2, room.Surface.yMin + 1, room.Surface.width - 4, room.Surface.height - 2);
                bridgeRect = new CellRect(pitRect.xMin, RoomPart_TrapsPit.< Generate > g__GetRandomBridgePos | 1_0(pitRect.yMin, pitRect.yMax), pitRect.width, 1);
            }
            else
            {
                Log.Error("Unexpected dir in RoomPart_TrapsPit.", false);
            }
            foreach (Vector2Int vector2Int in cellRect)
            {
                Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(new Vector3Int(vector2Int.x, room.StartY, vector2Int.y));
            }
            foreach (Vector2Int vector2Int2 in cellRect2)
            {
                Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(new Vector3Int(vector2Int2.x, room.StartY, vector2Int2.y));
            }
            Vector2Int vector2Int3;
            if (memory.config.Floor >= 2 && Rand.Chance(0.5f))
            {
                bridgeRect.TryGetRandomElement<Vector2Int>(out vector2Int3);
            }
            else
            {
                vector2Int3 = default(Vector2Int);
            }
            foreach (Vector2Int vector2Int4 in bridgeRect)
            {
                Maker.Make((vector2Int4 == vector2Int3) ? Get.Entity_UnstableBridge : Get.Entity_Bridge, null, false, false, true).Spawn(new Vector3Int(vector2Int4.x, room.StartY, vector2Int4.y));
            }
            foreach (Vector2Int vector2Int5 in pitRect.InRandomOrder<Vector2Int>().Take<Vector2Int>(Calc.RoundToIntHalfUp((float)pitRect.Area * 0.5f)))
            {
                Maker.Make(Get.Entity_Spikes, null, false, false, true).Spawn(new Vector3Int(vector2Int5.x, room.StartY, vector2Int5.y));
            }
            Func<Vector2Int, bool> <> 9__1;
            Func<Vector2Int, bool> <> 9__2;
            Func<Vector2Int, bool> <> 9__3;
            Func<Vector2Int, bool> <> 9__4;
            Func<Vector2Int, bool> <> 9__5;
            Func<Vector2Int, bool> <> 9__6;
            for (int i = 0; i < 2; i++)
            {
                IEnumerable<Vector2Int> edgeCells = cellRect.OuterRect(1).EdgeCells;
                Func<Vector2Int, bool> func2;
                if ((func2 = <> 9__1) == null)
                {
                    func2 = (<> 9__1 = (Vector2Int x) => pitRect.Contains(x) && !bridgeRect.Contains(x));
                }
                IEnumerable<Vector2Int> enumerable = edgeCells.Where<Vector2Int>(func2);
                if (vector3Int == Vector3IntUtility.Left || vector3Int == Vector3IntUtility.Right)
                {
                    if (i == 0)
                    {
                        IEnumerable<Vector2Int> enumerable2 = enumerable;
                        Func<Vector2Int, bool> func3;
                        if ((func3 = <> 9__2) == null)
                        {
                            func3 = (<> 9__2 = (Vector2Int x) => x.y < bridgeRect.y);
                        }
                        enumerable = enumerable2.Where<Vector2Int>(func3);
                    }
                    else
                    {
                        IEnumerable<Vector2Int> enumerable3 = enumerable;
                        Func<Vector2Int, bool> func4;
                        if ((func4 = <> 9__3) == null)
                        {
                            func4 = (<> 9__3 = (Vector2Int x) => x.y > bridgeRect.y);
                        }
                        enumerable = enumerable3.Where<Vector2Int>(func4);
                    }
                }
                else if (i == 0)
                {
                    IEnumerable<Vector2Int> enumerable4 = enumerable;
                    Func<Vector2Int, bool> func5;
                    if ((func5 = <> 9__4) == null)
                    {
                        func5 = (<> 9__4 = (Vector2Int x) => x.x < bridgeRect.x);
                    }
                    enumerable = enumerable4.Where<Vector2Int>(func5);
                }
                else
                {
                    IEnumerable<Vector2Int> enumerable5 = enumerable;
                    Func<Vector2Int, bool> func6;
                    if ((func6 = <> 9__5) == null)
                    {
                        func6 = (<> 9__5 = (Vector2Int x) => x.x > bridgeRect.x);
                    }
                    enumerable = enumerable5.Where<Vector2Int>(func6);
                }
                IEnumerable<Vector2Int> enumerable6 = enumerable;
                Func<Vector2Int, bool> func7;
                if ((func7 = <> 9__6) == null)
                {
                    func7 = (<> 9__6 = (Vector2Int x) => !world.AnyEntityAt(new Vector3Int(x.x, room.StartY, x.y)));
                }
                Vector2Int vector2Int6;
                if (enumerable6.Where<Vector2Int>(func7).TryGetRandomElement<Vector2Int>(out vector2Int6) || enumerable.TryGetRandomElement<Vector2Int>(out vector2Int6))
                {
                    Maker.Make(Get.Entity_Ladder, null, false, false, true).Spawn(new Vector3Int(vector2Int6.x, room.StartY, vector2Int6.y));
                }
            }
            Item item;
            if (memory.UnusedBaseGear.TryGetRandomElement<Item>(out item) || memory.UnusedBaseMiscItems.TryGetRandomElement<Item>(out item))
            {
                Vector2Int randomCell = cellRect2.RandomCell;
                Structure structure = Maker.Make<Structure>(Get.Entity_Chest, null, false, false, true);
                structure.InnerEntities.Add(item);
                structure.Spawn(new Vector3Int(randomCell.x, room.StartY + 1, randomCell.y));
                memory.unusedBaseItems.Remove(item);
            }
        }

        public static void GetDirectionalDimensions(Room room, out Vector3Int dir, out int width, out int length, out Func<Vector3Int, Vector3Int> localToRotatedConverter)
        {
            Room room2 = room.AdjacentRooms.FirstOrDefault<Room>();
            if (room2 == null)
            {
                Log.Error("Tried to use TrapsPit room logic on a room with no adjacent rooms.", false);
                dir = Vector3IntUtility.Forward;
            }
            else
            {
                dir = room2.GetAdjacentRoomDir(room);
            }
            Vector3Int vector3Int;
            Vector3Int vector3Int2;
            room.Shape.GetRotatedOrigin(dir, out vector3Int, out vector3Int2, out width, out length, out localToRotatedConverter);
        }

        [CompilerGenerated]
        internal static int <Generate>g__GetRandomBridgePos|1_0(int pitStart, int pitEnd)
		{
			if (pitEnd - pitStart + 1 <= 2)
			{
				return Rand.RangeInclusive(pitStart, pitEnd);
			}
			return Rand.RangeInclusive(pitStart + 1, pitEnd - 1);
		}

private const float TrapCountPct = 0.5f;
	}
}