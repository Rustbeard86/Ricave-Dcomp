using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ricave.Core
{
    public static class Rand
    {
        public static bool Bool
        {
            get
            {
                return Rand.Float < 0.5f;
            }
        }

        public static int Int
        {
            get
            {
                return Xorshift128.NextInt();
            }
        }

        public static float Float
        {
            get
            {
                return Xorshift128.NextFloat();
            }
        }

        public static int StateCompressed
        {
            get
            {
                ValueTuple<uint, uint, uint, uint> state = Xorshift128.State;
                return Calc.CombineHashes<uint, uint, uint, uint>(state.Item1, state.Item2, state.Item3, state.Item4);
            }
        }

        public static Vector3 UnitVector3
        {
            get
            {
                float gaussian;
                float gaussian2;
                float gaussian3;
                do
                {
                    gaussian = Rand.Gaussian;
                    gaussian2 = Rand.Gaussian;
                    gaussian3 = Rand.Gaussian;
                }
                while (gaussian == 0f && gaussian2 == 0f && gaussian3 == 0f);
                return new Vector3(gaussian, gaussian2, gaussian3).normalized;
            }
        }

        public static float Gaussian
        {
            get
            {
                float @float;
                do
                {
                    @float = Rand.Float;
                }
                while (@float == 0f);
                float float2 = Rand.Float;
                return Calc.Sqrt(-2f * Calc.Log(@float)) * Calc.Sin(6.2831855f * float2);
            }
        }

        public static void EnsureRandStateStackEmpty()
        {
            if (Rand.states.Count != 0)
            {
                Log.Error("Rand state stack is not empty at the beginning of a frame. Clearing.", false);
                Rand.states.Clear();
            }
        }

        public static void PushState(int seed)
        {
            Rand.states.Add(Xorshift128.State);
            Xorshift128.InitStateWithSeed(seed);
        }

        public static void PopState()
        {
            if (Rand.states.Count == 0)
            {
                Log.Error("Tried to pop rand state but the stack is empty.", false);
                return;
            }
            Xorshift128.State = Rand.states[Rand.states.Count - 1];
            Rand.states.RemoveAt(Rand.states.Count - 1);
        }

        public static float FloatSeeded(int seed)
        {
            Rand.PushState(seed);
            float @float = Rand.Float;
            Rand.PopState();
            return @float;
        }

        public static bool BoolSeeded(int seed)
        {
            Rand.PushState(seed);
            bool @bool = Rand.Bool;
            Rand.PopState();
            return @bool;
        }

        public static bool ChanceSeeded(float chance, int seed)
        {
            Rand.PushState(seed);
            bool flag = Rand.Chance(chance);
            Rand.PopState();
            return flag;
        }

        public static float Range(float from, float to)
        {
            return Xorshift128.NextFloat(from, to);
        }

        public static float RangeSeeded(float from, float to, int seed)
        {
            Rand.PushState(seed);
            float num = Rand.Range(from, to);
            Rand.PopState();
            return num;
        }

        public static int RangeInclusive(int from, int to)
        {
            if (from == to)
            {
                return from;
            }
            return Xorshift128.NextInt(from, to + 1);
        }

        public static int RangeInclusiveSeeded(int from, int to, int seed)
        {
            Rand.PushState(seed);
            int num = Rand.RangeInclusive(from, to);
            Rand.PopState();
            return num;
        }

        public static bool EventHappens(float passed, float happensOnceEvery, float maxChance = 0.95f)
        {
            return Rand.Chance(Math.Min(passed / happensOnceEvery, maxChance));
        }

        public static bool EventHappensSeeded(float passed, float happensOnceEvery, int seed, float maxChance = 0.95f)
        {
            Rand.PushState(seed);
            bool flag = Rand.EventHappens(passed, happensOnceEvery, maxChance);
            Rand.PopState();
            return flag;
        }

        public static bool TryGetRandomElement<T>(this IEnumerable<T> collection, out T elem)
        {
            ICollection<T> collection2 = collection as ICollection<T>;
            if (collection2 != null)
            {
                if (collection2.Count == 0)
                {
                    elem = default(T);
                    return false;
                }
                if (collection2.Count == 1)
                {
                    elem = collection2.ElementAt<T>(0);
                    return true;
                }
            }
            IList<T> list = collection as IList<T>;
            if (list != null)
            {
                elem = list[Rand.RangeInclusive(0, list.Count - 1)];
                return true;
            }
            int num = 0;
            elem = default(T);
            foreach (T t in collection)
            {
                num++;
                if (Rand.RangeInclusive(0, num - 1) == 0)
                {
                    elem = t;
                }
            }
            return num != 0;
        }

        public static bool TryGetRandomElement<T>(this IEnumerable<T> collection, out T elem, Func<T, float> weightGetter)
        {
            ICollection<T> collection2 = collection as ICollection<T>;
            if (collection2 != null)
            {
                if (collection2.Count == 0)
                {
                    elem = default(T);
                    return false;
                }
                if (collection2.Count == 1)
                {
                    T t = collection2.ElementAt<T>(0);
                    float num = weightGetter(t);
                    if (num < 0f)
                    {
                        num = 0f;
                        Log.Error("Element with negative weight.", false);
                    }
                    if (num > 0f)
                    {
                        elem = t;
                        return true;
                    }
                    elem = default(T);
                    return false;
                }
            }
            IList<T> list = (collection as IList<T>) ?? collection.ToTemporaryList<T>();
            if (list.Count == 0)
            {
                elem = default(T);
                return false;
            }
            if (list.Count == 1)
            {
                float num2 = weightGetter(list[0]);
                if (num2 < 0f)
                {
                    num2 = 0f;
                    Log.Error("Element with negative weight.", false);
                }
                if (num2 > 0f)
                {
                    elem = list[0];
                    return true;
                }
                elem = default(T);
                return false;
            }
            else
            {
                float num3 = 0f;
                for (int i = 0; i < list.Count; i++)
                {
                    float num4 = weightGetter(list[i]);
                    if (num4 < 0f)
                    {
                        num4 = 0f;
                        Log.Error("Element with negative weight.", false);
                    }
                    num3 += num4;
                }
                if (num3 <= 0f)
                {
                    elem = default(T);
                    return false;
                }
                float num5 = Rand.Float * num3;
                num3 = 0f;
                for (int j = 0; j < list.Count; j++)
                {
                    float num6 = weightGetter(list[j]);
                    if (num6 > 0f)
                    {
                        num3 += num6;
                        if (num5 <= num3 || j == list.Count - 1)
                        {
                            elem = list[j];
                            return true;
                        }
                    }
                }
                elem = default(T);
                return false;
            }
        }

        public static bool TryGetRandomElementWhere<T>(this IEnumerable<T> collection, Predicate<T> predicate, out T elem)
        {
            ICollection<T> collection2 = collection as ICollection<T>;
            if (collection2 != null)
            {
                if (collection2.Count == 0)
                {
                    elem = default(T);
                    return false;
                }
                if (collection2.Count == 1)
                {
                    T t = collection2.ElementAt<T>(0);
                    if (predicate(t))
                    {
                        elem = t;
                        return true;
                    }
                    elem = default(T);
                    return false;
                }
            }
            List<T> list = collection as List<T>;
            List<T> list2;
            bool flag;
            if (list != null)
            {
                list2 = list;
                flag = false;
            }
            else
            {
                list2 = collection.ToTemporaryList<T>();
                flag = true;
            }
            if (list2.Count == 0)
            {
                elem = default(T);
                return false;
            }
            if (list2.Count != 1)
            {
                int num = Calc.RoundToInt(Calc.Sqrt((float)list2.Count));
                for (int i = 0; i < num; i++)
                {
                    T t2;
                    if (list2.TryGetRandomElement<T>(out t2) && predicate(t2))
                    {
                        elem = t2;
                        return true;
                    }
                }
                if (!flag)
                {
                    list2 = list2.ToTemporaryList<T>();
                }
                list2.Shuffle<T>();
                for (int j = 0; j < list2.Count; j++)
                {
                    if (predicate(list2[j]))
                    {
                        elem = list2[j];
                        return true;
                    }
                }
                elem = default(T);
                return false;
            }
            if (predicate(list2[0]))
            {
                elem = list2[0];
                return true;
            }
            elem = default(T);
            return false;
        }

        public static int ProbabilisticRound(float val)
        {
            if (Calc.Approximately(val, Calc.Round(val)))
            {
                return Calc.RoundToInt(val);
            }
            if (Rand.Chance(1f - (Calc.Ceil(val) - val)))
            {
                return Calc.CeilToInt(val);
            }
            return Calc.FloorToInt(val);
        }

        public static bool Chance(float chance)
        {
            return chance > 0f && (chance >= 1f || Rand.Float < chance);
        }

        public static T Enum<T>() where T : Enum
        {
            T[] values = EnumUtility.GetValues<T>();
            return values[Rand.RangeInclusive(0, values.Length - 1)];
        }

        public static T Element<T>(T a, T b)
        {
            if (!Rand.Bool)
            {
                return b;
            }
            return a;
        }

        public static T Element<T>(T a, T b, T c)
        {
            int num = Rand.RangeInclusive(0, 2);
            if (num == 0)
            {
                return a;
            }
            if (num == 1)
            {
                return b;
            }
            return c;
        }

        public static T Element<T>(T a, T b, T c, T d)
        {
            int num = Rand.RangeInclusive(0, 3);
            if (num == 0)
            {
                return a;
            }
            if (num == 1)
            {
                return b;
            }
            if (num == 2)
            {
                return c;
            }
            return d;
        }

        public static T Element<T>(T a, T b, T c, T d, T e)
        {
            int num = Rand.RangeInclusive(0, 4);
            if (num == 0)
            {
                return a;
            }
            if (num == 1)
            {
                return b;
            }
            if (num == 2)
            {
                return c;
            }
            if (num == 3)
            {
                return d;
            }
            return e;
        }

        public static T Element<T>(T a, T b, T c, T d, T e, T f)
        {
            int num = Rand.RangeInclusive(0, 5);
            if (num == 0)
            {
                return a;
            }
            if (num == 1)
            {
                return b;
            }
            if (num == 2)
            {
                return c;
            }
            if (num == 3)
            {
                return d;
            }
            if (num == 4)
            {
                return e;
            }
            return f;
        }

        public static T Element<T>(T a, T b, T c, T d, T e, T f, T g)
        {
            int num = Rand.RangeInclusive(0, 6);
            if (num == 0)
            {
                return a;
            }
            if (num == 1)
            {
                return b;
            }
            if (num == 2)
            {
                return c;
            }
            if (num == 3)
            {
                return d;
            }
            if (num == 4)
            {
                return e;
            }
            if (num == 5)
            {
                return f;
            }
            return g;
        }

        public static T Element<T>(T a, T b, T c, T d, T e, T f, T g, T h)
        {
            int num = Rand.RangeInclusive(0, 7);
            if (num == 0)
            {
                return a;
            }
            if (num == 1)
            {
                return b;
            }
            if (num == 2)
            {
                return c;
            }
            if (num == 3)
            {
                return d;
            }
            if (num == 4)
            {
                return e;
            }
            if (num == 5)
            {
                return f;
            }
            if (num == 6)
            {
                return g;
            }
            return h;
        }

        public static T Element<T>(T a, T b, T c, T d, T e, T f, T g, T h, T i)
        {
            int num = Rand.RangeInclusive(0, 8);
            if (num == 0)
            {
                return a;
            }
            if (num == 1)
            {
                return b;
            }
            if (num == 2)
            {
                return c;
            }
            if (num == 3)
            {
                return d;
            }
            if (num == 4)
            {
                return e;
            }
            if (num == 5)
            {
                return f;
            }
            if (num == 6)
            {
                return g;
            }
            if (num == 7)
            {
                return h;
            }
            return i;
        }

        public static void Shuffle<T>(this List<T> list)
        {
            for (int i = 0; i < list.Count - 1; i++)
            {
                int num = Rand.RangeInclusive(i, list.Count - 1);
                list.Swap<T>(i, num);
            }
        }

        public static IEnumerable<T> InRandomOrder<T>(this IEnumerable<T> collection)
        {
            ICollection<T> collection2 = collection as ICollection<T>;
            if (collection2 != null && collection2.Count <= 1)
            {
                return collection;
            }
            List<T> list = collection.ToTemporaryList<T>();
            list.Shuffle<T>();
            return list;
        }

        public static IEnumerable<T> InRandomOrder<T>(this IEnumerable<T> collection, Func<T, float> weightGetter)
        {
            Rand.<> c__DisplayClass42_0 < T > CS$<> 8__locals1;
            CS$<> 8__locals1.weightGetter = weightGetter;
            ICollection<T> collection2 = collection as ICollection<T>;
            if (collection2 != null && collection2.Count <= 1)
            {
                if (collection2.Count == 1)
                {
                    T t = collection2.ElementAt<T>(0);
                    float num = CS$<> 8__locals1.weightGetter(t);
                    if (num < 0f)
                    {
                        num = 0f;
                        Log.Error("Element with negative weight.", false);
                    }
                    if (num > 0f)
                    {
                        yield return t;
                    }
                }
                yield break;
            }
            CS$<> 8__locals1.copy = Pool<List<ValueTuple<T, float>>>.Get();
            try
            {
                CS$<> 8__locals1.copy.Clear();
                IList<T> list = collection as IList<T>;
                if (list != null)
                {
                    int j = 0;
                    int count2 = list.Count;
                    while (j < count2)
                    {
                        Rand.< InRandomOrder > g__CheckAddElem | 42_0 < T > (list[j], ref CS$<> 8__locals1);
                        j++;
                    }
                }
                else
                {
                    foreach (T t2 in collection)
                    {
                        Rand.< InRandomOrder > g__CheckAddElem | 42_0 < T > (t2, ref CS$<> 8__locals1);
                    }
                }
                int count = CS$<> 8__locals1.copy.Count;
                int i = 0;
                ValueTuple<T, float> valueTuple;
                while (i < count && CS$<> 8__locals1.copy.TryGetRandomElement<ValueTuple<T, float>>(out valueTuple, Rand.CachedInRandomOrderWeightGetters<T>.CachedGetter))
				{
                    CS$<> 8__locals1.copy.Remove(valueTuple);
                    yield return valueTuple.Item1;
                    int num2 = i;
                    i = num2 + 1;
                }
            }
            finally
            {
                CS$<> 8__locals1.copy.Clear();
                Pool<List<ValueTuple<T, float>>>.Return(CS$<> 8__locals1.copy);
            }
            yield break;
            yield break;
        }

        public static int UniqueID(IEnumerable<int> takenIDs)
        {
            int num = Rand.Int;
            if (!takenIDs.Contains(num) && num != -1)
            {
                return num;
            }
            for (int i = 0; i < 1000; i++)
            {
                num++;
                if (!takenIDs.Contains(num) && num != -1)
                {
                    return num;
                }
            }
            Log.Warning("Unique IDs are saturated.", false);
            if (num == -1)
            {
                return -2;
            }
            return num;
        }

        public static int UniqueID(List<Entity> takenIDs)
        {
            Rand.<> c__DisplayClass44_0 CS$<> 8__locals1;
            CS$<> 8__locals1.takenIDs = takenIDs;
            int num = Rand.Int;
            if (!Rand.< UniqueID > g__IDTaken | 44_0(num, ref CS$<> 8__locals1) && num != -1)
            {
                return num;
            }
            for (int i = 0; i < 1000; i++)
            {
                num++;
                if (!Rand.< UniqueID > g__IDTaken | 44_0(num, ref CS$<> 8__locals1) && num != -1)
                {
                    return num;
                }
            }
            Log.Warning("Unique IDs are saturated.", false);
            if (num == -1)
            {
                return -2;
            }
            return num;
        }

        [CompilerGenerated]
        internal static void <InRandomOrder>g__CheckAddElem|42_0<T>(T elem, ref Rand.<>c__DisplayClass42_0<T> A_1)
		{
            float num = A_1.weightGetter(elem);
			if (num< 0f)
			{
				num = 0f;
				Log.Error("Element with negative weight.", false);
			}
			if (num > 0f)
			{
				A_1.copy.Add(new ValueTuple<T, float>(elem, num));
			}
		}

		[CompilerGenerated]
internal static bool < UniqueID > g__IDTaken | 44_0(int ID, ref Rand.<> c__DisplayClass44_0 A_1)

        {
    for (int i = 0; i < A_1.takenIDs.Count; i++)
    {
        if (A_1.takenIDs[i].StableID == ID)
        {
            return true;
        }
    }
    return false;
}

private static List<ValueTuple<uint, uint, uint, uint>> states = new List<ValueTuple<uint, uint, uint, uint>>(10);

private class CachedInRandomOrderWeightGetters<T>
{
    public static readonly Func<ValueTuple<T, float>, float> CachedGetter = (ValueTuple<T, float> x) => x.Item2;
}
	}
}