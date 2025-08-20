using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public struct CellRect : IEquatable<CellRect>, IEnumerable<Vector2Int>, IEnumerable
    {
        public bool Empty
        {
            get
            {
                return this.width <= 0 || this.height <= 0;
            }
        }

        public Vector2Int RandomCell
        {
            get
            {
                return new Vector2Int(Rand.RangeInclusive(this.xMin, this.xMax), Rand.RangeInclusive(this.yMin, this.yMax));
            }
        }

        public Vector2Int Center
        {
            get
            {
                return new Vector2Int(this.x + this.width / 2, this.y + this.height / 2);
            }
        }

        public int Area
        {
            get
            {
                return this.width * this.height;
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

        public IEnumerable<Vector2Int> Cells
        {
            get
            {
                int num;
                for (int i = 0; i < this.width; i = num + 1)
                {
                    for (int j = 0; j < this.height; j = num + 1)
                    {
                        yield return new Vector2Int(i + this.x, j + this.y);
                        num = j;
                    }
                    num = i;
                }
                yield break;
            }
        }

        public IEnumerable<Vector2Int> EdgeCells
        {
            get
            {
                if (this.Empty)
                {
                    yield break;
                }
                if (this.width == 1 && this.height == 1)
                {
                    yield return new Vector2Int(this.x, this.y);
                }
                else if (this.width == 1)
                {
                    int num;
                    for (int i = this.y; i <= this.yMax; i = num + 1)
                    {
                        yield return new Vector2Int(this.x, i);
                        num = i;
                    }
                }
                else if (this.height == 1)
                {
                    int num;
                    for (int i = this.x; i <= this.xMax; i = num + 1)
                    {
                        yield return new Vector2Int(i, this.y);
                        num = i;
                    }
                }
                else
                {
                    int num;
                    for (int i = this.x; i <= this.xMax; i = num + 1)
                    {
                        yield return new Vector2Int(i, this.y);
                        yield return new Vector2Int(i, this.yMax);
                        num = i;
                    }
                    for (int i = this.y + 1; i < this.yMax; i = num + 1)
                    {
                        yield return new Vector2Int(this.x, i);
                        yield return new Vector2Int(this.xMax, i);
                        num = i;
                    }
                }
                yield break;
            }
        }

        public IEnumerable<Vector2Int> Corners
        {
            get
            {
                if (this.Empty)
                {
                    yield break;
                }
                if (this.width == 1 && this.height == 1)
                {
                    yield return new Vector2Int(this.x, this.y);
                }
                else if (this.width == 1)
                {
                    yield return new Vector2Int(this.x, this.yMin);
                    yield return new Vector2Int(this.x, this.yMax);
                }
                else if (this.height == 1)
                {
                    yield return new Vector2Int(this.xMin, this.y);
                    yield return new Vector2Int(this.xMax, this.y);
                }
                else
                {
                    yield return new Vector2Int(this.xMin, this.yMin);
                    yield return new Vector2Int(this.xMax, this.yMin);
                    yield return new Vector2Int(this.xMin, this.yMax);
                    yield return new Vector2Int(this.xMax, this.yMax);
                }
                yield break;
            }
        }

        public CellRect(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public CellRect(int x, int y, int radius)
        {
            this.x = x - radius;
            this.y = y - radius;
            this.width = radius * 2 + 1;
            this.height = radius * 2 + 1;
        }

        public bool Overlaps(CellRect other)
        {
            return this.x <= other.xMax && this.xMax >= other.x && this.y <= other.yMax && this.yMax >= other.y;
        }

        public bool IsCorner(Vector2Int pos)
        {
            return (pos.x == this.x && pos.y == this.y) || (pos.x == this.xMax && pos.y == this.y) || (pos.x == this.x && pos.y == this.yMax) || (pos.x == this.xMax && pos.y == this.yMax);
        }

        public bool IsOnEdge(Vector2Int pos)
        {
            return this.Contains(pos) && (pos.x == this.x || pos.x == this.xMax || pos.y == this.y || pos.y == this.yMax);
        }

        public bool Contains(Vector2Int pos)
        {
            return pos.x >= this.x && pos.x <= this.xMax && pos.y >= this.y && pos.y <= this.yMax;
        }

        public CellRect InnerRect(int padding)
        {
            if (padding < 0)
            {
                return this.OuterRect(-padding);
            }
            if (this.width <= padding * 2 || this.height <= padding * 2)
            {
                return default(CellRect);
            }
            return new CellRect(this.x + padding, this.y + padding, this.width - 2 * padding, this.height - 2 * padding);
        }

        public CellRect OuterRect(int padding)
        {
            if (padding < 0)
            {
                return this.InnerRect(-padding);
            }
            return new CellRect(this.x - padding, this.y - padding, this.width + 2 * padding, this.height + 2 * padding);
        }

        public Vector2Int GetEdge(Vector2Int pos)
        {
            if (!this.IsOnEdge(pos))
            {
                return Vector2Int.zero;
            }
            if (pos.y == this.yMin)
            {
                return new Vector2Int(0, -1);
            }
            if (pos.y == this.yMax)
            {
                return new Vector2Int(0, 1);
            }
            if (pos.x == this.xMin)
            {
                return new Vector2Int(-1, 0);
            }
            if (pos.x == this.xMax)
            {
                return new Vector2Int(1, 0);
            }
            return Vector2Int.zero;
        }

        public CellRect GetIntersection(CellRect other)
        {
            int num = Math.Max(this.x, other.x);
            int num2 = Math.Min(this.xMax, other.xMax);
            if (num > num2)
            {
                return new CellRect(this.x, this.y, 0, 0);
            }
            int num3 = Math.Max(this.y, other.y);
            int num4 = Math.Min(this.yMax, other.yMax);
            if (num3 > num4)
            {
                return new CellRect(this.x, this.y, 0, 0);
            }
            return new CellRect(num, num3, num2 - num + 1, num4 - num3 + 1);
        }

        public CellRect GetIntersectionNoCorners(CellRect other)
        {
            CellRect intersection = this.GetIntersection(other);
            if (intersection.Empty)
            {
                return intersection;
            }
            if (intersection.width >= 2)
            {
                if (intersection.height >= 2)
                {
                    Log.Error("Called GetIntersectionNoCorners() but the resulting intersection is not a line, so it's not possible to exclude corners. This method only makes sense for edge intersections.", false);
                    return intersection;
                }
                intersection.x++;
                intersection.width -= 2;
                return intersection;
            }
            else
            {
                if (intersection.height >= 2)
                {
                    intersection.y++;
                    intersection.height -= 2;
                    return intersection;
                }
                return new CellRect(this.x, this.y, 0, 0);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is CellRect)
            {
                CellRect cellRect = (CellRect)obj;
                return this.Equals(cellRect);
            }
            return false;
        }

        public bool Equals(CellRect other)
        {
            return this == other;
        }

        public static bool operator ==(CellRect a, CellRect b)
        {
            return a.x == b.x && a.y == b.y && a.width == b.width && a.height == b.height;
        }

        public static bool operator !=(CellRect a, CellRect b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return Calc.CombineHashes<int, int, int, int>(this.x, this.y, this.width, this.height);
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
                this.width.ToString(),
                ", ",
                this.height.ToString(),
                ")"
            });
        }

        public CellRect.Enumerator GetEnumerator()
        {
            return new CellRect.Enumerator(this);
        }

        IEnumerator<Vector2Int> IEnumerable<Vector2Int>.GetEnumerator()
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
        public int width;

        [Saved]
        public int height;

        public struct Enumerator : IEnumerator<Vector2Int>, IEnumerator, IDisposable
        {
            public Enumerator(CellRect rect)
            {
                this.rect = rect;
                this.x = rect.xMin;
                this.y = rect.yMin - 1;
                if (rect.Empty)
                {
                    this.x = rect.xMax + 1;
                }
            }

            public Vector2Int Current
            {
                get
                {
                    return new Vector2Int(this.x, this.y);
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
                this.y++;
                if (this.y > this.rect.yMax)
                {
                    this.y = this.rect.yMin;
                    this.x++;
                }
                return this.x <= this.rect.xMax;
            }

            public void Reset()
            {
                this.x = this.rect.xMin;
                this.y = this.rect.yMin - 1;
                if (this.rect.Empty)
                {
                    this.x = this.rect.xMax + 1;
                }
            }

            void IDisposable.Dispose()
            {
            }

            private CellRect rect;

            private int x;

            private int y;
        }
    }
}