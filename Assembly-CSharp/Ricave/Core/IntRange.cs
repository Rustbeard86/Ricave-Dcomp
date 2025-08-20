using System;

namespace Ricave.Core
{
    public struct IntRange : IEquatable<IntRange>
    {
        public int Range
        {
            get
            {
                return this.to - this.from;
            }
        }

        public bool Empty
        {
            get
            {
                return this.Range < 0;
            }
        }

        public int RandomInRange
        {
            get
            {
                if (this.Empty)
                {
                    Log.Warning("Called RandomInRange on empty IntRange.", false);
                    return this.from;
                }
                return Rand.RangeInclusive(this.from, this.to);
            }
        }

        public IntRange(int from, int to)
        {
            this.from = from;
            this.to = to;
        }

        public override bool Equals(object obj)
        {
            if (obj is IntRange)
            {
                IntRange intRange = (IntRange)obj;
                return this.Equals(intRange);
            }
            return false;
        }

        public bool Equals(IntRange other)
        {
            return this == other;
        }

        public static bool operator ==(IntRange a, IntRange b)
        {
            return a.from == b.from && a.to == b.to;
        }

        public static bool operator !=(IntRange a, IntRange b)
        {
            return !(a == b);
        }

        public static explicit operator IntRange(int value)
        {
            return new IntRange(value, value);
        }

        public override int GetHashCode()
        {
            return Calc.CombineHashes<int, int>(this.from, this.to);
        }

        public override string ToString()
        {
            return string.Concat(new string[]
            {
                "(",
                this.from.ToString(),
                ", ",
                this.to.ToString(),
                ")"
            });
        }

        [Saved]
        public int from;

        [Saved]
        public int to;
    }
}