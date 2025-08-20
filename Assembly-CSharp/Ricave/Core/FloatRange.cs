using System;

namespace Ricave.Core
{
    public struct FloatRange : IEquatable<FloatRange>
    {
        public float Range
        {
            get
            {
                return this.to - this.from;
            }
        }

        public bool None
        {
            get
            {
                return this.Range < 0f;
            }
        }

        public float Middle
        {
            get
            {
                return (this.from + this.to) / 2f;
            }
        }

        public float RandomInRange
        {
            get
            {
                if (this.None)
                {
                    Log.Warning("Called RandomInRange on empty FloatRange.", false);
                    return this.from;
                }
                return Rand.Range(this.from, this.to);
            }
        }

        public FloatRange(float from, float to)
        {
            this.from = from;
            this.to = to;
        }

        public override bool Equals(object obj)
        {
            if (obj is FloatRange)
            {
                FloatRange floatRange = (FloatRange)obj;
                return this.Equals(floatRange);
            }
            return false;
        }

        public bool Equals(FloatRange other)
        {
            return this == other;
        }

        public static bool operator ==(FloatRange a, FloatRange b)
        {
            return a.from == b.from && a.to == b.to;
        }

        public static bool operator !=(FloatRange a, FloatRange b)
        {
            return !(a == b);
        }

        public static explicit operator FloatRange(float value)
        {
            return new FloatRange(value, value);
        }

        public override int GetHashCode()
        {
            return Calc.CombineHashes<float, float>(this.from, this.to);
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
        public float from;

        [Saved]
        public float to;
    }
}