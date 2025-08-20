using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_WallProtrusions : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            this.DoWallProtrusions(room, memory);
            this.DoCeilingProtrusions(room, memory);
        }

        private void DoCeilingProtrusions(Room room, WorldGenMemory memory)
        {
            if (room.RoomAbove != null)
            {
                return;
            }
            if (room.Shape.width <= 3 || room.Shape.depth <= 3)
            {
                return;
            }
            if (room.Height <= 3)
            {
                return;
            }
            this.ceilingProtruded.Clear();
            foreach (Vector3Int vector3Int in Vector3IntUtility.DirectionsXZCardinal)
            {
                if (Rand.Chance(0.15f))
                {
                    this.DoCeilingProtrusion(room, vector3Int);
                }
            }
        }

        private void DoCeilingProtrusion(Room room, Vector3Int dir)
        {
            CellCuboid edgeCells = room.Shape.InnerCuboid(1).TopSurfaceCuboid.GetEdgeCells(dir);
            foreach (Vector3Int vector3Int in edgeCells)
            {
                if (!this.protruded.Contains(vector3Int.ToVector2IntDiscardY()) && !this.ceilingProtruded.Contains(vector3Int.ToVector2IntDiscardY()))
                {
                    if (!Get.World.AnyEntityAt(vector3Int) && !room.IsEntranceCellToAvoidBlocking(vector3Int, true))
                    {
                        if (!Get.World.GetEntitiesAt(vector3Int + dir).Any<Entity>((Entity x) => x.Spec.IsStructure && (x.Spec.Structure.Flammable || x.Spec.Structure.Fragile)))
                        {
                            continue;
                        }
                    }
                    return;
                }
            }
            foreach (Vector3Int vector3Int2 in edgeCells)
            {
                if (!this.protruded.Contains(vector3Int2.ToVector2IntDiscardY()) && !this.ceilingProtruded.Contains(vector3Int2.ToVector2IntDiscardY()))
                {
                    Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int2);
                    this.ceilingProtruded.Add(vector3Int2.ToVector2IntDiscardY());
                }
            }
        }

        private void DoWallProtrusions(Room room, WorldGenMemory memory)
        {
            if (room.RoomAbove != null)
            {
                return;
            }
            if (room.Shape.width <= 3 || room.Shape.depth <= 3)
            {
                return;
            }
            if ((room.Shape.width <= 4 || room.Shape.depth <= 4) && (room.Role == Room.LayoutRole.End || room.Role == Room.LayoutRole.LockedBehindSilverDoor))
            {
                return;
            }
            this.protruded.Clear();
            foreach (Vector3Int vector3Int in room.Shape.InnerCuboid(1).BottomSurfaceCuboid.Corners)
            {
                if (Rand.Chance(0.25f))
                {
                    this.DoProtrusionAt(room, vector3Int);
                }
            }
        }

        private void DoProtrusionAt(Room room, Vector3Int corner)
        {
            CellCuboid bottomSurfaceCuboid = room.Shape.InnerCuboid(1).BottomSurfaceCuboid;
            Vector3Int vector3Int;
            Vector3Int vector3Int2;
            if (corner == bottomSurfaceCuboid.BottomSurfaceNearLeft)
            {
                if (Rand.Bool)
                {
                    vector3Int = Vector3IntUtility.Right;
                    vector3Int2 = Vector3IntUtility.Back;
                }
                else
                {
                    vector3Int = Vector3IntUtility.Forward;
                    vector3Int2 = Vector3IntUtility.Left;
                }
            }
            else if (corner == bottomSurfaceCuboid.BottomSurfaceNearRight)
            {
                if (Rand.Bool)
                {
                    vector3Int = Vector3IntUtility.Left;
                    vector3Int2 = Vector3IntUtility.Back;
                }
                else
                {
                    vector3Int = Vector3IntUtility.Forward;
                    vector3Int2 = Vector3IntUtility.Right;
                }
            }
            else if (corner == bottomSurfaceCuboid.BottomSurfaceFarLeft)
            {
                if (Rand.Bool)
                {
                    vector3Int = Vector3IntUtility.Right;
                    vector3Int2 = Vector3IntUtility.Forward;
                }
                else
                {
                    vector3Int = Vector3IntUtility.Back;
                    vector3Int2 = Vector3IntUtility.Left;
                }
            }
            else
            {
                if (!(corner == bottomSurfaceCuboid.BottomSurfaceFarRight))
                {
                    Log.Error("Invalid corner.", false);
                    return;
                }
                if (Rand.Bool)
                {
                    vector3Int = Vector3IntUtility.Left;
                    vector3Int2 = Vector3IntUtility.Forward;
                }
                else
                {
                    vector3Int = Vector3IntUtility.Back;
                    vector3Int2 = Vector3IntUtility.Right;
                }
            }
            int num;
            if (vector3Int == Vector3IntUtility.Right || vector3Int == Vector3IntUtility.Left)
            {
                num = Rand.RangeInclusive(1, bottomSurfaceCuboid.width - 2);
            }
            else
            {
                if (!(vector3Int == Vector3IntUtility.Forward) && !(vector3Int == Vector3IntUtility.Back))
                {
                    Log.Error("Invalid dir.", false);
                    return;
                }
                num = Rand.RangeInclusive(1, bottomSurfaceCuboid.depth - 2);
            }
            int num2 = 0;
            while (num2 < num && Get.CellsInfo.IsFloorUnderNoActors(corner + vector3Int * num2) && Get.CellsInfo.IsFilled((corner + vector3Int * num2).Below()))
            {
                bool flag = true;
                int i = 0;
                while (i < room.Shape.height - 2)
                {
                    Vector3Int vector3Int3 = (corner + vector3Int * num2).WithAddedY(i);
                    if (!Get.World.AnyEntityAt(vector3Int3) && !room.WouldPotentiallyBlockPath(vector3Int3, true))
                    {
                        if (!Get.World.GetEntitiesAt(vector3Int3 + vector3Int2).Any<Entity>((Entity x) => x.Spec.IsStructure && (x.Spec.Structure.Flammable || x.Spec.Structure.Fragile)))
                        {
                            i++;
                            continue;
                        }
                    }
                    flag = false;
                    break;
                }
                if (!flag || this.protruded.Contains((corner + vector3Int * (num2 + 1)).ToVector2IntDiscardY()))
                {
                    break;
                }
                this.protruded.Add((corner + vector3Int * num2).ToVector2IntDiscardY());
                for (int j = 0; j < room.Shape.height - 2; j++)
                {
                    Vector3Int vector3Int4 = (corner + vector3Int * num2).WithAddedY(j);
                    Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int4);
                }
                num2++;
            }
        }

        private HashSet<Vector2Int> protruded = new HashSet<Vector2Int>();

        private HashSet<Vector2Int> ceilingProtruded = new HashSet<Vector2Int>();
    }
}