using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public struct CellCuboid : IEquatable<CellCuboid>, IEnumerable<Vector3Int>, IEnumerable
    {
        public bool Empty
        {
            get
            {
                return this.width <= 0 || this.height <= 0 || this.depth <= 0;
            }
        }

        public CellRect SurfaceXZ
        {
            get
            {
                return new CellRect(this.x, this.z, this.width, this.depth);
            }
        }

        public CellCuboid BottomSurfaceCuboid
        {
            get
            {
                return new CellCuboid(this.x, this.y, this.z, this.width, Math.Min(1, this.height), this.depth);
            }
        }

        public CellCuboid TopSurfaceCuboid
        {
            get
            {
                return new CellCuboid(this.x, this.yMax, this.z, this.width, Math.Min(1, this.height), this.depth);
            }
        }

        public CellRect CellRectXZ
        {
            get
            {
                return new CellRect(this.x, this.z, this.width, this.depth);
            }
        }

        public int Volume
        {
            get
            {
                return this.width * this.height * this.depth;
            }
        }

        public Vector3Int Position
        {
            get
            {
                return new Vector3Int(this.x, this.y, this.z);
            }
        }

        public Vector3Int BiggestPosition
        {
            get
            {
                return new Vector3Int(this.x + this.width - 1, this.y + this.height - 1, this.z + this.depth - 1);
            }
        }

        public Vector3Int Size
        {
            get
            {
                return new Vector3Int(this.width, this.height, this.depth);
            }
        }

        public Vector3Int RandomCell
        {
            get
            {
                return new Vector3Int(Rand.RangeInclusive(this.xMin, this.xMax), Rand.RangeInclusive(this.yMin, this.yMax), Rand.RangeInclusive(this.zMin, this.zMax));
            }
        }

        public Vector3Int Center
        {
            get
            {
                return new Vector3Int(this.x + this.width / 2, this.y + this.height / 2, this.z + this.depth / 2);
            }
        }

        public Vector3 CenterFloat
        {
            get
            {
                return new Vector3((float)this.x + (float)this.width / 2f - 0.5f, (float)this.y + (float)this.height / 2f - 0.5f, (float)this.z + (float)this.depth / 2f - 0.5f);
            }
        }

        public Vector3Int BottomSurfaceNearLeft
        {
            get
            {
                return this.Position;
            }
        }

        public Vector3Int BottomSurfaceNearRight
        {
            get
            {
                return new Vector3Int(this.xMax, this.y, this.z);
            }
        }

        public Vector3Int BottomSurfaceFarLeft
        {
            get
            {
                return new Vector3Int(this.x, this.y, this.zMax);
            }
        }

        public Vector3Int BottomSurfaceFarRight
        {
            get
            {
                return new Vector3Int(this.xMax, this.y, this.zMax);
            }
        }

        public int xMin
        {
            get
            {
                return this.x;
            }
            set
            {
                int xMax = this.xMax;
                this.x = value;
                this.xMax = xMax;
            }
        }

        public int yMin
        {
            get
            {
                return this.y;
            }
            set
            {
                int yMax = this.yMax;
                this.y = value;
                this.yMax = yMax;
            }
        }

        public int zMin
        {
            get
            {
                return this.z;
            }
            set
            {
                int zMax = this.zMax;
                this.z = value;
                this.zMax = zMax;
            }
        }

        public int xMax
        {
            get
            {
                return this.x + this.width - 1;
            }
            set
            {
                this.width = value - this.x + 1;
            }
        }

        public int yMax
        {
            get
            {
                return this.y + this.height - 1;
            }
            set
            {
                this.height = value - this.y + 1;
            }
        }

        public int zMax
        {
            get
            {
                return this.z + this.depth - 1;
            }
            set
            {
                this.depth = value - this.z + 1;
            }
        }

        public IEnumerable<Vector3Int> Cells
        {
            get
            {
                int num;
                for (int i = 0; i < this.width; i = num + 1)
                {
                    for (int j = 0; j < this.height; j = num + 1)
                    {
                        for (int k = 0; k < this.depth; k = num + 1)
                        {
                            yield return new Vector3Int(i + this.x, j + this.y, k + this.z);
                            num = k;
                        }
                        num = j;
                    }
                    num = i;
                }
                yield break;
            }
        }

        public IEnumerable<Vector3Int> EdgeCells
        {
            get
            {
                if (this.Empty)
                {
                    yield break;
                }
                if (this.depth == 1)
                {
                    int num;
                    for (int i = 0; i < this.width; i = num + 1)
                    {
                        for (int j = 0; j < this.height; j = num + 1)
                        {
                            yield return new Vector3Int(i + this.x, j + this.y, this.z);
                            num = j;
                        }
                        num = i;
                    }
                }
                else
                {
                    int num;
                    for (int i = this.z; i <= this.zMax; i = num + 1)
                    {
                        if (i == this.z || i == this.zMax)
                        {
                            for (int j = 0; j < this.width; j = num + 1)
                            {
                                for (int k = 0; k < this.height; k = num + 1)
                                {
                                    yield return new Vector3Int(j + this.x, k + this.y, i);
                                    num = k;
                                }
                                num = j;
                            }
                        }
                        else if (this.width == 1 && this.height == 1)
                        {
                            yield return new Vector3Int(this.x, this.y, i);
                        }
                        else if (this.width == 1)
                        {
                            for (int j = this.y; j <= this.yMax; j = num + 1)
                            {
                                yield return new Vector3Int(this.x, j, i);
                                num = j;
                            }
                        }
                        else if (this.height == 1)
                        {
                            for (int j = this.x; j <= this.xMax; j = num + 1)
                            {
                                yield return new Vector3Int(j, this.y, i);
                                num = j;
                            }
                        }
                        else
                        {
                            for (int j = this.x; j <= this.xMax; j = num + 1)
                            {
                                yield return new Vector3Int(j, this.y, i);
                                yield return new Vector3Int(j, this.yMax, i);
                                num = j;
                            }
                            for (int j = this.y + 1; j < this.yMax; j = num + 1)
                            {
                                yield return new Vector3Int(this.x, j, i);
                                yield return new Vector3Int(this.xMax, j, i);
                                num = j;
                            }
                        }
                        num = i;
                    }
                }
                yield break;
            }
        }

        public IEnumerable<Vector3Int> EdgeCellsXZ
        {
            get
            {
                foreach (Vector3Int vector3Int in this.EdgeCells)
                {
                    if (this.IsOnEdgeXZ(vector3Int))
                    {
                        yield return vector3Int;
                    }
                }
                IEnumerator<Vector3Int> enumerator = null;
                yield break;
                yield break;
            }
        }

        public IEnumerable<Vector3Int> EdgeCellsXZNoCorners
        {
            get
            {
                foreach (Vector3Int vector3Int in this.EdgeCells)
                {
                    if (this.IsOnEdgeXZ(vector3Int) && !this.IsCorner(vector3Int))
                    {
                        yield return vector3Int;
                    }
                }
                IEnumerator<Vector3Int> enumerator = null;
                yield break;
                yield break;
            }
        }

        public IEnumerable<Vector3Int> Corners
        {
            get
            {
                if (this.Empty)
                {
                    yield break;
                }
                if (this.depth == 1)
                {
                    if (this.width == 1 && this.height == 1)
                    {
                        yield return new Vector3Int(this.x, this.y, this.z);
                    }
                    else if (this.width == 1)
                    {
                        yield return new Vector3Int(this.x, this.yMin, this.z);
                        yield return new Vector3Int(this.x, this.yMax, this.z);
                    }
                    else if (this.height == 1)
                    {
                        yield return new Vector3Int(this.xMin, this.y, this.z);
                        yield return new Vector3Int(this.xMax, this.y, this.z);
                    }
                    else
                    {
                        yield return new Vector3Int(this.xMin, this.yMin, this.z);
                        yield return new Vector3Int(this.xMax, this.yMin, this.z);
                        yield return new Vector3Int(this.xMin, this.yMax, this.z);
                        yield return new Vector3Int(this.xMax, this.yMax, this.z);
                    }
                }
                else if (this.width == 1 && this.height == 1)
                {
                    yield return new Vector3Int(this.x, this.y, this.zMin);
                    yield return new Vector3Int(this.x, this.y, this.zMax);
                }
                else if (this.width == 1)
                {
                    yield return new Vector3Int(this.x, this.yMin, this.zMin);
                    yield return new Vector3Int(this.x, this.yMax, this.zMin);
                    yield return new Vector3Int(this.x, this.yMin, this.zMax);
                    yield return new Vector3Int(this.x, this.yMax, this.zMax);
                }
                else if (this.height == 1)
                {
                    yield return new Vector3Int(this.xMin, this.y, this.zMin);
                    yield return new Vector3Int(this.xMax, this.y, this.zMin);
                    yield return new Vector3Int(this.xMin, this.y, this.zMax);
                    yield return new Vector3Int(this.xMax, this.y, this.zMax);
                }
                else
                {
                    yield return new Vector3Int(this.xMin, this.yMin, this.zMin);
                    yield return new Vector3Int(this.xMax, this.yMin, this.zMin);
                    yield return new Vector3Int(this.xMin, this.yMax, this.zMin);
                    yield return new Vector3Int(this.xMax, this.yMax, this.zMin);
                    yield return new Vector3Int(this.xMin, this.yMin, this.zMax);
                    yield return new Vector3Int(this.xMax, this.yMin, this.zMax);
                    yield return new Vector3Int(this.xMin, this.yMax, this.zMax);
                    yield return new Vector3Int(this.xMax, this.yMax, this.zMax);
                }
                yield break;
            }
        }

        public CellCuboid(int x, int y, int z, int width, int height, int depth)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.width = width;
            this.height = height;
            this.depth = depth;
        }

        public CellCuboid(int xCenter, int yCenter, int zCenter, int radius)
        {
            this.x = xCenter - radius;
            this.y = yCenter - radius;
            this.z = zCenter - radius;
            this.width = radius * 2 + 1;
            this.height = radius * 2 + 1;
            this.depth = radius * 2 + 1;
        }

        public CellCuboid(Vector3Int center, int radius)
        {
            this = new CellCuboid(center.x, center.y, center.z, radius);
        }

        public CellCuboid(Vector3Int pos, int width, int height, int depth)
        {
            this = new CellCuboid(pos.x, pos.y, pos.z, width, height, depth);
        }

        public static CellCuboid BoundingBox(Vector3Int a, Vector3Int b)
        {
            int num = Math.Min(a.x, b.x);
            int num2 = Math.Min(a.y, b.y);
            int num3 = Math.Min(a.z, b.z);
            int num4 = Math.Max(a.x, b.x);
            int num5 = Math.Max(a.y, b.y);
            int num6 = Math.Max(a.z, b.z);
            return new CellCuboid(num, num2, num3, num4 - num + 1, num5 - num2 + 1, num6 - num3 + 1);
        }

        public bool Overlaps(CellCuboid other)
        {
            return this.x <= other.xMax && this.xMax >= other.x && this.y <= other.yMax && this.yMax >= other.y && this.z <= other.zMax && this.zMax >= other.z;
        }

        public bool IsCorner(Vector3Int pos)
        {
            return (pos.x == this.x && pos.y == this.y && pos.z == this.z) || (pos.x == this.xMax && pos.y == this.y && pos.z == this.z) || (pos.x == this.x && pos.y == this.yMax && pos.z == this.z) || (pos.x == this.xMax && pos.y == this.yMax && pos.z == this.z) || (pos.x == this.x && pos.y == this.y && pos.z == this.zMax) || (pos.x == this.xMax && pos.y == this.y && pos.z == this.zMax) || (pos.x == this.x && pos.y == this.yMax && pos.z == this.zMax) || (pos.x == this.xMax && pos.y == this.yMax && pos.z == this.zMax);
        }

        public bool IsOnEdge(Vector3Int pos)
        {
            return this.Contains(pos) && (pos.x == this.x || pos.x == this.xMax || pos.y == this.y || pos.y == this.yMax || pos.z == this.z || pos.z == this.zMax);
        }

        public bool IsOnEdgeXZ(Vector3Int pos)
        {
            return this.Contains(pos) && (pos.x == this.x || pos.x == this.xMax || pos.z == this.z || pos.z == this.zMax);
        }

        public bool Contains(Vector3Int pos)
        {
            return pos.x >= this.x && pos.x <= this.xMax && pos.y >= this.y && pos.y <= this.yMax && pos.z >= this.z && pos.z <= this.zMax;
        }

        public CellCuboid GetEdgeCells(Vector3Int dir)
        {
            if (!dir.IsCardinalDir())
            {
                Log.Warning("Called GetEdgeCells() using an argument which is not a cardinal direction.", false);
                return new CellCuboid(this.x, this.y, this.z, 0, 0, 0);
            }
            if (this.Empty)
            {
                return this;
            }
            if (dir == Vector3IntUtility.Up)
            {
                return new CellCuboid(this.x, this.yMax, this.z, this.width, 1, this.depth);
            }
            if (dir == Vector3IntUtility.Down)
            {
                return new CellCuboid(this.x, this.y, this.z, this.width, 1, this.depth);
            }
            if (dir == Vector3Int.left)
            {
                return new CellCuboid(this.x, this.y, this.z, 1, this.height, this.depth);
            }
            if (dir == Vector3Int.right)
            {
                return new CellCuboid(this.xMax, this.y, this.z, 1, this.height, this.depth);
            }
            if (dir.z == 1)
            {
                return new CellCuboid(this.x, this.y, this.zMax, this.width, this.height, 1);
            }
            return new CellCuboid(this.x, this.y, this.z, this.width, this.height, 1);
        }

        public Vector3Int GetEdge(Vector3Int c)
        {
            if (!this.Contains(c))
            {
                return Vector3Int.zero;
            }
            if (c.x == this.xMin)
            {
                return Vector3Int.left;
            }
            if (c.x == this.xMax)
            {
                return Vector3Int.right;
            }
            if (c.z == this.zMax)
            {
                return Vector3IntUtility.Forward;
            }
            if (c.z == this.zMin)
            {
                return Vector3IntUtility.Back;
            }
            if (c.y == this.yMax)
            {
                return Vector3IntUtility.Up;
            }
            if (c.y == this.yMin)
            {
                return Vector3IntUtility.Down;
            }
            return Vector3Int.zero;
        }

        public CellCuboid GetIntersection(CellCuboid other)
        {
            int num = Math.Max(this.x, other.x);
            int num2 = Math.Min(this.xMax, other.xMax);
            if (num > num2)
            {
                return new CellCuboid(this.x, this.y, this.z, 0, 0, 0);
            }
            int num3 = Math.Max(this.y, other.y);
            int num4 = Math.Min(this.yMax, other.yMax);
            if (num3 > num4)
            {
                return new CellCuboid(this.x, this.y, this.z, 0, 0, 0);
            }
            int num5 = Math.Max(this.z, other.z);
            int num6 = Math.Min(this.zMax, other.zMax);
            if (num5 > num6)
            {
                return new CellCuboid(this.x, this.y, this.z, 0, 0, 0);
            }
            return new CellCuboid(num, num3, num5, num2 - num + 1, num4 - num3 + 1, num6 - num5 + 1);
        }

        public CellCuboid InnerCuboid(int padding)
        {
            if (padding < 0)
            {
                return this.OuterCuboid(-padding);
            }
            if (this.width <= padding * 2 || this.height <= padding * 2 || this.depth <= padding * 2)
            {
                return default(CellCuboid);
            }
            return new CellCuboid(this.x + padding, this.y + padding, this.z + padding, this.width - 2 * padding, this.height - 2 * padding, this.depth - 2 * padding);
        }

        public CellCuboid OuterCuboid(int padding)
        {
            if (padding < 0)
            {
                return this.InnerCuboid(-padding);
            }
            return new CellCuboid(this.x - padding, this.y - padding, this.z - padding, this.width + 2 * padding, this.height + 2 * padding, this.depth + 2 * padding);
        }

        public CellCuboid InnerCuboidXZ(int padding)
        {
            if (padding < 0)
            {
                return this.OuterCuboidXZ(-padding);
            }
            if (this.width <= padding * 2 || this.depth <= padding * 2)
            {
                return default(CellCuboid);
            }
            return new CellCuboid(this.x + padding, this.y, this.z + padding, this.width - 2 * padding, this.height, this.depth - 2 * padding);
        }

        public CellCuboid OuterCuboidXZ(int padding)
        {
            if (padding < 0)
            {
                return this.InnerCuboidXZ(-padding);
            }
            return new CellCuboid(this.x - padding, this.y, this.z - padding, this.width + 2 * padding, this.height, this.depth + 2 * padding);
        }

        public void RandomlyClampSizeXZ(int maxWidth, int maxDepth)
        {
            while (this.width > maxWidth)
            {
                if (Rand.Bool)
                {
                    int num = this.xMin;
                    this.xMin = num + 1;
                }
                else
                {
                    int num = this.xMax;
                    this.xMax = num - 1;
                }
            }
            while (this.depth > maxDepth)
            {
                if (Rand.Bool)
                {
                    int num = this.zMin;
                    this.zMin = num + 1;
                }
                else
                {
                    int num = this.zMax;
                    this.zMax = num - 1;
                }
            }
        }

        public CellCuboid WithAddedY(int offset)
        {
            return new CellCuboid(this.x, this.y + offset, this.z, this.width, this.height, this.depth);
        }

        public CellCuboid ClipToWorld()
        {
            if (this.xMin < 0)
            {
                this.xMin = 0;
            }
            if (this.yMin < 0)
            {
                this.yMin = 0;
            }
            if (this.zMin < 0)
            {
                this.zMin = 0;
            }
            Vector3Int size = Get.World.Size;
            if (this.xMax >= size.x)
            {
                this.xMax = size.x - 1;
            }
            if (this.yMax >= size.y)
            {
                this.yMax = size.y - 1;
            }
            if (this.zMax >= size.z)
            {
                this.zMax = size.z - 1;
            }
            return this;
        }

        public bool InBounds()
        {
            Vector3Int size = Get.World.Size;
            return this.x >= 0 && this.xMax < size.x && this.y >= 0 && this.yMax < size.y && this.z >= 0 && this.zMax < size.z;
        }

        public Vector3Int GetRotatedOrigin(Vector3Int dir)
        {
            if (dir == Vector3IntUtility.Forward)
            {
                return this.Position;
            }
            if (dir == Vector3IntUtility.Right)
            {
                return new Vector3Int(this.x, this.y, this.zMax);
            }
            if (dir == Vector3IntUtility.Back)
            {
                return new Vector3Int(this.xMax, this.y, this.zMax);
            }
            if (dir == Vector3IntUtility.Left)
            {
                return new Vector3Int(this.xMax, this.y, this.z);
            }
            Log.Warning("Called GetRotatedOrigin() but the passed direction is not a cardinal dir on the XZ plane. \"up\" vector should always stay the same.", false);
            return this.Position;
        }

        public Vector3Int GetRotatedOrigin(Vector3Int dir, out Vector3Int forward, out Vector3Int right, out int width, out int length, out Func<Vector3Int, Vector3Int> localToRotatedConverter)
        {
            Vector3Int newOrigin = this.GetRotatedOrigin(dir);
            Vector3Int forwardVec = dir;
            Vector3Int rightVec = dir.RightDir();
            forward = forwardVec;
            right = rightVec;
            localToRotatedConverter = (Vector3Int local) => newOrigin + local.x * rightVec + local.y * Vector3IntUtility.Up + local.z * forwardVec;
            if (dir == Vector3IntUtility.Forward)
            {
                width = this.width;
                length = this.depth;
            }
            else if (dir == Vector3IntUtility.Right)
            {
                width = this.depth;
                length = this.width;
            }
            else if (dir == Vector3IntUtility.Back)
            {
                width = this.width;
                length = this.depth;
            }
            else if (dir == Vector3IntUtility.Left)
            {
                width = this.depth;
                length = this.width;
            }
            else
            {
                Log.Warning("Called GetRotatedOrigin() but the passed direction is not a cardinal dir on the XZ plane. \"up\" vector should always stay the same.", false);
                width = this.width;
                length = this.depth;
            }
            return newOrigin;
        }

        public override bool Equals(object obj)
        {
            if (obj is CellCuboid)
            {
                CellCuboid cellCuboid = (CellCuboid)obj;
                return this.Equals(cellCuboid);
            }
            return false;
        }

        public bool Equals(CellCuboid other)
        {
            return this == other;
        }

        public static bool operator ==(CellCuboid a, CellCuboid b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z && a.width == b.width && a.height == b.height && a.depth == b.depth;
        }

        public static bool operator !=(CellCuboid a, CellCuboid b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return Calc.CombineHashes<int, int, int, int, int, int>(this.x, this.y, this.z, this.width, this.height, this.depth);
        }

        public override string ToString()
        {
            return string.Concat(new string[]
            {
                "(",
                this.x.ToString(),
                ", ",
                this.y.ToString(),
                ", ",
                this.z.ToString(),
                ", ",
                this.width.ToString(),
                ", ",
                this.height.ToString(),
                ", ",
                this.depth.ToString(),
                ")"
            });
        }

        public CellCuboid.Enumerator GetEnumerator()
        {
            return new CellCuboid.Enumerator(this);
        }

        IEnumerator<Vector3Int> IEnumerable<Vector3Int>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        [Saved]
        public int x;

        [Saved]
        public int y;

        [Saved]
        public int z;

        [Saved]
        public int width;

        [Saved]
        public int height;

        [Saved]
        public int depth;

        public struct Enumerator : IEnumerator<Vector3Int>, IEnumerator, IDisposable
        {
            public Enumerator(CellCuboid cuboid)
            {
                this.cuboid = cuboid;
                this.x = cuboid.xMin;
                this.y = cuboid.yMin;
                this.z = cuboid.zMin - 1;
                if (cuboid.Empty)
                {
                    this.x = cuboid.xMax + 1;
                }
            }

            public Vector3Int Current
            {
                get
                {
                    return new Vector3Int(this.x, this.y, this.z);
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return this.Current;
                }
            }

            public bool MoveNext()
            {
                this.z++;
                if (this.z > this.cuboid.zMax)
                {
                    this.z = this.cuboid.zMin;
                    this.y++;
                    if (this.y > this.cuboid.yMax)
                    {
                        this.y = this.cuboid.yMin;
                        this.x++;
                    }
                }
                return this.x <= this.cuboid.xMax;
            }

            public void Reset()
            {
                this.x = this.cuboid.xMin;
                this.y = this.cuboid.yMin;
                this.z = this.cuboid.zMin - 1;
                if (this.cuboid.Empty)
                {
                    this.x = this.cuboid.xMax + 1;
                }
            }

            void IDisposable.Dispose()
            {
            }

            private CellCuboid cuboid;

            private int x;

            private int y;

            private int z;
        }
    }
}