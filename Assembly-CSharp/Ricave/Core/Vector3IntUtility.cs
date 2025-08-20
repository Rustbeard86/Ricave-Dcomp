using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public static class Vector3IntUtility
    {
        public static bool IsAdjacent(this Vector3Int a, Vector3Int b)
        {
            int num = a.x - b.x;
            int num2 = a.y - b.y;
            int num3 = a.z - b.z;
            return (num | num2 | num3) != 0 && num + 1 <= 2 && num2 + 1 <= 2 && num3 + 1 <= 2;
        }

        public static bool IsAdjacentOrInside(this Vector3Int a, Vector3Int b)
        {
            return a.x - b.x + 1 <= 2 && a.y - b.y + 1 <= 2 && a.z - b.z + 1 <= 2;
        }

        public static bool IsAdjacentXZ(this Vector3Int a, Vector3Int b)
        {
            return !(a == b) && (Math.Abs(a.x - b.x) <= 1 && a.y == b.y) && Math.Abs(a.z - b.z) <= 1;
        }

        public static bool IsAdjacentXZOrInside(this Vector3Int a, Vector3Int b)
        {
            return Math.Abs(a.x - b.x) <= 1 && a.y == b.y && Math.Abs(a.z - b.z) <= 1;
        }

        public static bool IsAdjacentDiagonal(this Vector3Int a, Vector3Int b)
        {
            return !(a == b) && a.IsAdjacent(b) && !a.IsAdjacentCardinal(b);
        }

        public static bool IsAdjacentDiagonalXZ(this Vector3Int a, Vector3Int b)
        {
            return !(a == b) && (a.IsAdjacent(b) && a.x != b.x && a.y == b.y) && a.z != b.z;
        }

        public static bool IsAdjacentCardinal(this Vector3Int a, Vector3Int b)
        {
            return !(a == b) && a.IsAdjacent(b) && ((a.x == b.x && a.y == b.y) || (a.x == b.x && a.z == b.z) || (a.y == b.y && a.z == b.z));
        }

        public static bool IsAdjacentCardinalXZ(this Vector3Int a, Vector3Int b)
        {
            return !(a == b) && (a.IsAdjacent(b) && (a.x == b.x || a.z == b.z)) && a.y == b.y;
        }

        public static bool IsDirection(this Vector3Int v)
        {
            return (v.x != 0 || v.y != 0 || v.z != 0) && (Math.Abs(v.x) <= 1 && Math.Abs(v.y) <= 1) && Math.Abs(v.z) <= 1;
        }

        public static IEnumerable<Vector3Int> AdjacentCells(this Vector3Int v)
        {
            return Vector3IntUtility.Directions.Select<Vector3Int, Vector3Int>((Vector3Int x) => v + x);
        }

        public static IEnumerable<Vector3Int> AdjacentCellsAndInside(this Vector3Int v)
        {
            return Vector3IntUtility.DirectionsAndInside.Select<Vector3Int, Vector3Int>((Vector3Int x) => v + x);
        }

        public static IEnumerable<Vector3Int> AdjacentCellsXZ(this Vector3Int v)
        {
            return Vector3IntUtility.DirectionsXZ.Select<Vector3Int, Vector3Int>((Vector3Int x) => v + x);
        }

        public static IEnumerable<Vector3Int> AdjacentCellsXZAndInside(this Vector3Int v)
        {
            return Vector3IntUtility.DirectionsXZAndInside.Select<Vector3Int, Vector3Int>((Vector3Int x) => v + x);
        }

        public static IEnumerable<Vector3Int> AdjacentCardinalCells(this Vector3Int v)
        {
            return Vector3IntUtility.DirectionsCardinal.Select<Vector3Int, Vector3Int>((Vector3Int x) => v + x);
        }

        public static IEnumerable<Vector3Int> AdjacentCardinalCellsXZ(this Vector3Int v)
        {
            return Vector3IntUtility.DirectionsXZCardinal.Select<Vector3Int, Vector3Int>((Vector3Int x) => v + x);
        }

        public static string DirectionToString(this Vector3Int v)
        {
            if (v == Vector3IntUtility.Up)
            {
                return "Up".Translate();
            }
            if (v == Vector3IntUtility.Down)
            {
                return "Down".Translate();
            }
            if (v == Vector3IntUtility.West)
            {
                return "West".Translate();
            }
            if (v == Vector3IntUtility.East)
            {
                return "East".Translate();
            }
            if (v == Vector3IntUtility.North)
            {
                return "North".Translate();
            }
            if (v == Vector3IntUtility.South)
            {
                return "South".Translate();
            }
            if (v == Vector3IntUtility.NorthWest)
            {
                return "NorthWest".Translate();
            }
            if (v == Vector3IntUtility.NorthEast)
            {
                return "NorthEast".Translate();
            }
            if (v == Vector3IntUtility.SouthWest)
            {
                return "SouthWest".Translate();
            }
            if (v == Vector3IntUtility.SouthEast)
            {
                return "SouthEast".Translate();
            }
            if (v == Vector3IntUtility.UpWest)
            {
                return "UpWest".Translate();
            }
            if (v == Vector3IntUtility.UpEast)
            {
                return "UpEast".Translate();
            }
            if (v == Vector3IntUtility.UpNorth)
            {
                return "UpNorth".Translate();
            }
            if (v == Vector3IntUtility.UpSouth)
            {
                return "UpSouth".Translate();
            }
            if (v == Vector3IntUtility.UpNorthWest)
            {
                return "UpNorthWest".Translate();
            }
            if (v == Vector3IntUtility.UpNorthEast)
            {
                return "UpNorthEast".Translate();
            }
            if (v == Vector3IntUtility.UpSouthWest)
            {
                return "UpSouthWest".Translate();
            }
            if (v == Vector3IntUtility.UpSouthEast)
            {
                return "UpSouthEast".Translate();
            }
            if (v == Vector3IntUtility.DownWest)
            {
                return "DownWest".Translate();
            }
            if (v == Vector3IntUtility.DownEast)
            {
                return "DownEast".Translate();
            }
            if (v == Vector3IntUtility.DownNorth)
            {
                return "DownNorth".Translate();
            }
            if (v == Vector3IntUtility.DownSouth)
            {
                return "DownSouth".Translate();
            }
            if (v == Vector3IntUtility.DownNorthWest)
            {
                return "DownNorthWest".Translate();
            }
            if (v == Vector3IntUtility.DownNorthEast)
            {
                return "DownNorthEast".Translate();
            }
            if (v == Vector3IntUtility.DownSouthWest)
            {
                return "DownSouthWest".Translate();
            }
            if (v == Vector3IntUtility.DownSouthEast)
            {
                return "DownSouthEast".Translate();
            }
            return "";
        }

        public static bool IsCardinalDir(this Vector3Int dir)
        {
            return dir.IsDirection() && ((dir.x == 0 && dir.y == 0) || (dir.x == 0 && dir.z == 0) || (dir.y == 0 && dir.z == 0));
        }

        public static bool IsDiagonalDir(this Vector3Int dir)
        {
            return dir.IsDirection() && !dir.IsCardinalDir();
        }

        public static int GetGridDistance(this Vector3Int a, Vector3Int b)
        {
            int num = Math.Abs(a.x - b.x);
            int num2 = Math.Abs(a.y - b.y);
            int num3 = Math.Abs(a.z - b.z);
            return Math.Max(Math.Max(num, num2), num3);
        }

        public static IEnumerable<List<Vector3Int>> GetSimilarDirectionsGrouped(this Vector3Int dir)
        {
            if (!dir.IsDirection())
            {
                string text = "Called GetSimilarDirectionsGrouped() for dir ";
                Vector3Int vector3Int = dir;
                Log.Error(text + vector3Int.ToString() + ".", false);
                yield break;
            }
            yield return new List<Vector3Int> { dir };
            Vector3Int afterDir = dir * 2;
            int num;
            for (int dist = 1; dist <= 3; dist = num + 1)
            {
                List<Vector3Int> list = new List<Vector3Int>();
                for (int i = 0; i < Vector3IntUtility.Directions.Length; i++)
                {
                    if (Vector3IntUtility.Directions[i] != dir && Vector3IntUtility.Directions[i] != dir * -1 && Vector3IntUtility.Directions[i].GetGridDistance(afterDir) == dist)
                    {
                        list.Add(Vector3IntUtility.Directions[i]);
                    }
                }
                if (list.Count != 0)
                {
                    yield return list;
                }
                num = dist;
            }
            yield return new List<Vector3Int> { dir * -1 };
            yield break;
        }

        public static IEnumerable<List<Vector3Int>> GetGroupedByDistTo(this IList<Vector3Int> cells, Vector3Int distTo)
        {
            Dictionary<int, List<Vector3Int>> dictionary = FrameLocalPool<Dictionary<int, List<Vector3Int>>>.Get();
            for (int i = 0; i < cells.Count; i++)
            {
                int gridDistance = cells[i].GetGridDistance(distTo);
                List<Vector3Int> list;
                if (dictionary.TryGetValue(gridDistance, out list))
                {
                    list.Add(cells[i]);
                }
                else
                {
                    dictionary.Add(gridDistance, new List<Vector3Int> { cells[i] });
                }
            }
            foreach (KeyValuePair<int, List<Vector3Int>> keyValuePair in dictionary.OrderBy<KeyValuePair<int, List<Vector3Int>>, int>((KeyValuePair<int, List<Vector3Int>> x) => x.Key))
            {
                yield return keyValuePair.Value;
            }
            IEnumerator<KeyValuePair<int, List<Vector3Int>>> enumerator = null;
            yield break;
            yield break;
        }

        public static Vector3Int WithAddedY(this Vector3Int v, int offset)
        {
            return new Vector3Int(v.x, v.y + offset, v.z);
        }

        public static Vector3Int WithY(this Vector3Int v, int y)
        {
            return new Vector3Int(v.x, y, v.z);
        }

        public static Vector3Int Below(this Vector3Int v)
        {
            return new Vector3Int(v.x, v.y - 1, v.z);
        }

        public static Vector3Int Above(this Vector3Int v)
        {
            return new Vector3Int(v.x, v.y + 1, v.z);
        }

        public static IEnumerable<Vector3Int> GetCellsAtDist(this Vector3Int root, int dist)
        {
            return new CellCuboid(root, dist).EdgeCells;
        }

        public static CellCuboid GetCellsWithin(this Vector3Int root, int dist)
        {
            return new CellCuboid(root, dist);
        }

        public static Vector3Int AngleToCardinalXZDir(float angle)
        {
            angle = Calc.NormalizeDir(angle);
            float num = Math.Abs(angle);
            float num2 = Math.Abs(angle - 90f);
            float num3 = Math.Abs(angle - 180f);
            float num4 = Math.Abs(angle - 270f);
            float num5 = Math.Abs(angle - 360f);
            float num6 = Math.Min(num, Math.Min(num2, Math.Min(num3, Math.Min(num4, num5))));
            if (num2 == num6)
            {
                return Vector3IntUtility.Right;
            }
            if (num3 == num6)
            {
                return Vector3IntUtility.Back;
            }
            if (num4 == num6)
            {
                return Vector3IntUtility.Left;
            }
            return Vector3IntUtility.Forward;
        }

        public static int Sum(this Vector3Int vec)
        {
            return vec.x + vec.y + vec.z;
        }

        public static int Dot(Vector3Int a, Vector3Int b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static Vector3Int NormalizedToDir(this Vector3Int vec)
        {
            if (vec == default(Vector3Int))
            {
                return Vector3IntUtility.Forward;
            }
            if (vec.IsDirection())
            {
                return vec;
            }
            return vec.ToVector3IntDir();
        }

        public static Vector3Int ToVector3IntDir(this Vector3 dir)
        {
            if (dir == default(Vector3))
            {
                return Vector3IntUtility.Forward;
            }
            dir = dir.normalized;
            float num = -1f;
            for (int i = 0; i < Vector3IntUtility.Directions.Length; i++)
            {
                num = Math.Max(num, Vector3.Dot(dir, Vector3IntUtility.Directions[i]));
            }
            for (int j = 0; j < Vector3IntUtility.Directions.Length; j++)
            {
                if (Vector3.Dot(dir, Vector3IntUtility.Directions[j]) == num)
                {
                    return Vector3IntUtility.Directions[j];
                }
            }
            return Vector3IntUtility.Forward;
        }

        public static Vector3Int NormalizedToDirXZ(this Vector3Int vec)
        {
            if (vec.x == 0 && vec.z == 0)
            {
                return Vector3IntUtility.Forward;
            }
            if (vec.IsDirection() && vec.y == 0)
            {
                return vec;
            }
            return vec.ToVector3IntDirXZ();
        }

        public static Vector3Int ToVector3IntDirXZ(this Vector3 dir)
        {
            if (dir.x == 0f && dir.z == 0f)
            {
                return Vector3IntUtility.Forward;
            }
            dir = dir.normalized;
            float num = -1f;
            for (int i = 0; i < Vector3IntUtility.DirectionsXZ.Length; i++)
            {
                num = Math.Max(num, Vector3.Dot(dir, Vector3IntUtility.DirectionsXZ[i]));
            }
            for (int j = 0; j < Vector3IntUtility.DirectionsXZ.Length; j++)
            {
                if (Vector3.Dot(dir, Vector3IntUtility.DirectionsXZ[j]) == num)
                {
                    return Vector3IntUtility.DirectionsXZ[j];
                }
            }
            return Vector3IntUtility.Forward;
        }

        public static Vector3Int NormalizedToCardinalDir(this Vector3Int vec)
        {
            if (vec == default(Vector3Int))
            {
                return Vector3IntUtility.Forward;
            }
            if (vec.IsCardinalDir())
            {
                return vec;
            }
            return vec.ToCardinalVector3IntDir();
        }

        public static Vector3Int ToCardinalVector3IntDir(this Vector3 dir)
        {
            if (dir == default(Vector3))
            {
                return Vector3IntUtility.Forward;
            }
            dir = dir.normalized;
            float num = -1f;
            for (int i = 0; i < Vector3IntUtility.DirectionsCardinal.Length; i++)
            {
                num = Math.Max(num, Vector3.Dot(dir, Vector3IntUtility.DirectionsCardinal[i]));
            }
            for (int j = 0; j < Vector3IntUtility.DirectionsCardinal.Length; j++)
            {
                if (Vector3.Dot(dir, Vector3IntUtility.DirectionsCardinal[j]) == num)
                {
                    return Vector3IntUtility.DirectionsCardinal[j];
                }
            }
            return Vector3IntUtility.Forward;
        }

        public static Vector3Int NormalizedToCardinalXZDir(this Vector3Int vec)
        {
            if (vec.x == 0 && vec.z == 0)
            {
                return Vector3IntUtility.Forward;
            }
            if (vec.IsCardinalDir() && vec.y == 0)
            {
                return vec;
            }
            return vec.ToCardinalXZVector3IntDir();
        }

        public static Vector3Int ToCardinalXZVector3IntDir(this Vector3 dir)
        {
            if (dir.x == 0f && dir.z == 0f)
            {
                return Vector3IntUtility.Forward;
            }
            dir = dir.normalized;
            float num = -1f;
            for (int i = 0; i < Vector3IntUtility.DirectionsXZCardinal.Length; i++)
            {
                num = Math.Max(num, Vector3.Dot(dir, Vector3IntUtility.DirectionsXZCardinal[i]));
            }
            for (int j = 0; j < Vector3IntUtility.DirectionsXZCardinal.Length; j++)
            {
                if (Vector3.Dot(dir, Vector3IntUtility.DirectionsXZCardinal[j]) == num)
                {
                    return Vector3IntUtility.DirectionsXZCardinal[j];
                }
            }
            return Vector3IntUtility.Forward;
        }

        public static bool InBounds(this Vector3Int pos)
        {
            return Get.World.InBounds(pos);
        }

        public static bool InRoomsBounds(this Vector3Int pos)
        {
            return pos.InBounds() && Get.RetainedRoomInfo.AnyRoomAt(pos);
        }

        public static bool InPrivateRoom(this Vector3Int pos)
        {
            return pos.InBounds() && Get.RetainedRoomInfo.AnyRoomOfSpecAt(pos, Get.Room_PrivateRoom);
        }

        public static bool InRoom(this Vector3Int pos, RoomSpec roomSpec)
        {
            return pos.InBounds() && Get.RetainedRoomInfo.AnyRoomOfSpecAt(pos, roomSpec);
        }

        public static List<RetainedRoomInfo.RoomInfo> GetRooms(this Vector3Int pos)
        {
            return Get.RetainedRoomInfo.GetRoomsAt(pos);
        }

        public static Vector3Int RoundToVector3Int(this Vector3 vec)
        {
            return new Vector3Int(Calc.RoundToIntHalfUp(vec.x), Calc.RoundToIntHalfUp(vec.y), Calc.RoundToIntHalfUp(vec.z));
        }

        public static Vector3Int RightDir(this Vector3Int cardinalDir)
        {
            if (cardinalDir == Vector3IntUtility.Forward)
            {
                return Vector3IntUtility.Right;
            }
            if (cardinalDir == Vector3IntUtility.Right)
            {
                return Vector3IntUtility.Back;
            }
            if (cardinalDir == Vector3IntUtility.Back)
            {
                return Vector3IntUtility.Left;
            }
            if (cardinalDir == Vector3IntUtility.Left)
            {
                return Vector3IntUtility.Forward;
            }
            if (cardinalDir == Vector3IntUtility.Up)
            {
                return Vector3IntUtility.Right;
            }
            if (cardinalDir == Vector3IntUtility.Down)
            {
                return Vector3IntUtility.Right;
            }
            Log.Warning("Called RightDir() but the passed vector is not a cardinal dir.", false);
            return Vector3IntUtility.Forward;
        }

        public static Vector3Int LeftDir(this Vector3Int cardinalDir)
        {
            return -cardinalDir.RightDir();
        }

        public static float ToXZAngle(this Vector3Int v)
        {
            if (v.x == 0 && v.z == 0)
            {
                return 0f;
            }
            return Calc.Atan2((float)v.x, (float)v.z) * 57.29578f;
        }

        public static CellCuboid SelectLine(this Vector3Int pos, Vector3Int cardinalDir, Predicate<Vector3Int> selectUntil)
        {
            if (!selectUntil(pos))
            {
                return new CellCuboid(pos.x, pos.y, pos.z, 0, 0, 0);
            }
            Vector3Int vector3Int = pos;
            while (selectUntil(vector3Int + cardinalDir))
            {
                vector3Int += cardinalDir;
            }
            return CellCuboid.BoundingBox(pos, vector3Int);
        }

        public static CellCuboid SelectLineOnePastLast(this Vector3Int pos, Vector3Int cardinalDir, Predicate<Vector3Int> selectUntil)
        {
            Vector3Int vector3Int = pos;
            while (selectUntil(vector3Int))
            {
                vector3Int += cardinalDir;
            }
            return CellCuboid.BoundingBox(pos, vector3Int);
        }

        public static CellCuboid SelectLineInBounds(this Vector3Int pos, Vector3Int cardinalDir, Predicate<Vector3Int> selectUntil)
        {
            World world = Get.World;
            if (!world.InBounds(pos) || !selectUntil(pos))
            {
                return new CellCuboid(pos.x, pos.y, pos.z, 0, 0, 0);
            }
            Vector3Int vector3Int = pos;
            while (world.InBounds(vector3Int + cardinalDir) && selectUntil(vector3Int + cardinalDir))
            {
                vector3Int += cardinalDir;
            }
            return CellCuboid.BoundingBox(pos, vector3Int);
        }

        public static CellCuboid SelectLineOnePastLastInBounds(this Vector3Int pos, Vector3Int cardinalDir, Predicate<Vector3Int> selectUntil)
        {
            World world = Get.World;
            if (!world.InBounds(pos))
            {
                return new CellCuboid(pos.x, pos.y, pos.z, 0, 0, 0);
            }
            Vector3Int vector3Int = pos;
            while (world.InBounds(vector3Int + cardinalDir) && selectUntil(vector3Int))
            {
                vector3Int += cardinalDir;
            }
            return CellCuboid.BoundingBox(pos, vector3Int);
        }

        public static Vector2Int ToVector2IntDiscardY(this Vector3Int vec)
        {
            return new Vector2Int(vec.x, vec.z);
        }

        public static readonly Vector3Int[] Directions = new Vector3Int[]
        {
            new Vector3Int(0, 0, 1),
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, 0, -1),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(1, 0, 1),
            new Vector3Int(1, 0, -1),
            new Vector3Int(-1, 0, -1),
            new Vector3Int(-1, 0, 1),
            new Vector3Int(0, 1, 1),
            new Vector3Int(1, 1, 1),
            new Vector3Int(1, 1, 0),
            new Vector3Int(1, 1, -1),
            new Vector3Int(0, 1, -1),
            new Vector3Int(-1, 1, -1),
            new Vector3Int(-1, 1, 0),
            new Vector3Int(-1, 1, 1),
            new Vector3Int(0, -1, 1),
            new Vector3Int(1, -1, 1),
            new Vector3Int(1, -1, 0),
            new Vector3Int(1, -1, -1),
            new Vector3Int(0, -1, -1),
            new Vector3Int(-1, -1, -1),
            new Vector3Int(-1, -1, 0),
            new Vector3Int(-1, -1, 1)
        };

        public static readonly Vector3Int[] DirectionsAndInside = new Vector3Int[]
        {
            new Vector3Int(0, 0, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, 0, -1),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(1, 0, 1),
            new Vector3Int(1, 0, -1),
            new Vector3Int(-1, 0, -1),
            new Vector3Int(-1, 0, 1),
            new Vector3Int(0, 1, 1),
            new Vector3Int(1, 1, 1),
            new Vector3Int(1, 1, 0),
            new Vector3Int(1, 1, -1),
            new Vector3Int(0, 1, -1),
            new Vector3Int(-1, 1, -1),
            new Vector3Int(-1, 1, 0),
            new Vector3Int(-1, 1, 1),
            new Vector3Int(0, -1, 1),
            new Vector3Int(1, -1, 1),
            new Vector3Int(1, -1, 0),
            new Vector3Int(1, -1, -1),
            new Vector3Int(0, -1, -1),
            new Vector3Int(-1, -1, -1),
            new Vector3Int(-1, -1, 0),
            new Vector3Int(-1, -1, 1)
        };

        public static readonly Vector3Int[] DirectionsXZ = new Vector3Int[]
        {
            new Vector3Int(0, 0, 1),
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, 0, -1),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(1, 0, 1),
            new Vector3Int(1, 0, -1),
            new Vector3Int(-1, 0, -1),
            new Vector3Int(-1, 0, 1)
        };

        public static readonly Vector3Int[] DirectionsXZAndInside = new Vector3Int[]
        {
            new Vector3Int(0, 0, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, 0, -1),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(1, 0, 1),
            new Vector3Int(1, 0, -1),
            new Vector3Int(-1, 0, -1),
            new Vector3Int(-1, 0, 1)
        };

        public static readonly Vector3Int[] DirectionsXZUpDownAndInside = new Vector3Int[]
        {
            new Vector3Int(0, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, 0, -1),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(1, 0, 1),
            new Vector3Int(1, 0, -1),
            new Vector3Int(-1, 0, -1),
            new Vector3Int(-1, 0, 1)
        };

        public static readonly Vector3Int[] DirectionsXZAround = new Vector3Int[]
        {
            new Vector3Int(-1, 0, -1),
            new Vector3Int(0, 0, -1),
            new Vector3Int(1, 0, -1),
            new Vector3Int(1, 0, 0),
            new Vector3Int(1, 0, 1),
            new Vector3Int(0, 0, 1),
            new Vector3Int(-1, 0, 1),
            new Vector3Int(-1, 0, 0)
        };

        public static readonly Vector3Int[] DirectionsCardinal = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 0, -1)
        };

        public static readonly Vector3Int[] DirectionsCardinalAndInside = new Vector3Int[]
        {
            new Vector3Int(0, 0, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 0, -1)
        };

        public static readonly Vector3Int[] DirectionsXZCardinal = new Vector3Int[]
        {
            new Vector3Int(0, 0, 1),
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, 0, -1),
            new Vector3Int(-1, 0, 0)
        };

        public static readonly Vector3Int[] DirectionsXZCardinalAndInside = new Vector3Int[]
        {
            new Vector3Int(0, 0, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, 0, -1),
            new Vector3Int(-1, 0, 0)
        };

        public static readonly Vector3Int[] DirectionsXZDiagonal = new Vector3Int[]
        {
            new Vector3Int(1, 0, 1),
            new Vector3Int(1, 0, -1),
            new Vector3Int(-1, 0, -1),
            new Vector3Int(-1, 0, 1)
        };

        public static readonly Vector3Int Up = Vector3Int.up;

        public static readonly Vector3Int Down = Vector3Int.down;

        public static readonly Vector3Int Left = Vector3Int.left;

        public static readonly Vector3Int Right = Vector3Int.right;

        public static readonly Vector3Int Forward = new Vector3Int(0, 0, 1);

        public static readonly Vector3Int Back = new Vector3Int(0, 0, -1);

        public static readonly Vector3Int West = new Vector3Int(-1, 0, 0);

        public static readonly Vector3Int East = new Vector3Int(1, 0, 0);

        public static readonly Vector3Int North = new Vector3Int(0, 0, 1);

        public static readonly Vector3Int South = new Vector3Int(0, 0, -1);

        public static readonly Vector3Int NorthWest = new Vector3Int(-1, 0, 1);

        public static readonly Vector3Int NorthEast = new Vector3Int(1, 0, 1);

        public static readonly Vector3Int SouthWest = new Vector3Int(-1, 0, -1);

        public static readonly Vector3Int SouthEast = new Vector3Int(1, 0, -1);

        public static readonly Vector3Int UpWest = new Vector3Int(-1, 1, 0);

        public static readonly Vector3Int UpEast = new Vector3Int(1, 1, 0);

        public static readonly Vector3Int UpNorth = new Vector3Int(0, 1, 1);

        public static readonly Vector3Int UpSouth = new Vector3Int(0, 1, -1);

        public static readonly Vector3Int UpNorthWest = new Vector3Int(-1, 1, 1);

        public static readonly Vector3Int UpNorthEast = new Vector3Int(1, 1, 1);

        public static readonly Vector3Int UpSouthWest = new Vector3Int(-1, 1, -1);

        public static readonly Vector3Int UpSouthEast = new Vector3Int(1, 1, -1);

        public static readonly Vector3Int DownWest = new Vector3Int(-1, -1, 0);

        public static readonly Vector3Int DownEast = new Vector3Int(1, -1, 0);

        public static readonly Vector3Int DownNorth = new Vector3Int(0, -1, 1);

        public static readonly Vector3Int DownSouth = new Vector3Int(0, -1, -1);

        public static readonly Vector3Int DownNorthWest = new Vector3Int(-1, -1, 1);

        public static readonly Vector3Int DownNorthEast = new Vector3Int(1, -1, 1);

        public static readonly Vector3Int DownSouthWest = new Vector3Int(-1, -1, -1);

        public static readonly Vector3Int DownSouthEast = new Vector3Int(1, -1, -1);
    }
}