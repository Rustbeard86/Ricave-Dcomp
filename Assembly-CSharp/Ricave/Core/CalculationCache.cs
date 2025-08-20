using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class CalculationCache<TKey, TResult>
    {
        public CalculationCache(Func<TKey, TResult> processor, int maxSize = 300)
        {
            this.processor = processor;
            this.maxSize = maxSize;
        }

        public TResult Get(TKey key)
        {
            TResult tresult;
            if (!this.cached.TryGetValue(key, out tresult))
            {
                if (this.cached.Count + 1 > this.maxSize)
                {
                    int hashCode = this.processor.GetHashCode();
                    int num = this.pseudoRandomIterations;
                    this.pseudoRandomIterations = num + 1;
                    int num2 = Calc.HashToRangeInclusive(hashCode + num, 0, this.entries.Count - 1);
                    this.cached.Remove(this.entries[num2]);
                    this.entries[num2] = key;
                }
                else
                {
                    this.entries.Add(key);
                }
                tresult = this.processor(key);
                this.cached.Add(key, tresult);
            }
            return tresult;
        }

        public void Clear()
        {
            this.cached.Clear();
            this.entries.Clear();
        }

        private Func<TKey, TResult> processor;

        private int maxSize = 300;

        private Dictionary<TKey, TResult> cached = new Dictionary<TKey, TResult>();

        private List<TKey> entries = new List<TKey>();

        private int pseudoRandomIterations;

        private const int DefaultMaxSize = 300;
    }
}