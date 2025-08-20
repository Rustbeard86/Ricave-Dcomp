using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ricave.Core
{
    public static class CollectionsUtility
    {
        public static void AddRange<T>(this HashSet<T> set, IEnumerable<T> elements)
        {
            IList<T> list = elements as IList<T>;
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    set.Add(list[i]);
                }
                return;
            }
            HashSet<T> hashSet = elements as HashSet<T>;
            if (hashSet != null)
            {
                using (HashSet<T>.Enumerator enumerator = hashSet.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        T t = enumerator.Current;
                        set.Add(t);
                    }
                    return;
                }
            }
            foreach (T t2 in elements)
            {
                set.Add(t2);
            }
        }

        public static string ElementsToString<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
            {
                return "";
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (T t in collection)
            {
                if (stringBuilder.Length != 0)
                {
                    stringBuilder.Append(", ");
                }
                stringBuilder.Append(t.ToString());
            }
            return stringBuilder.ToString();
        }

        public static string GroupedElementsToString<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
            {
                return "";
            }
            Dictionary<T, int> dictionary = FrameLocalPool<Dictionary<T, int>>.Get();
            foreach (T t in collection)
            {
                dictionary.SetOrIncrement(t, 1);
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (T t2 in collection)
            {
                int num;
                if (dictionary.TryGetValue(t2, out num))
                {
                    if (stringBuilder.Length != 0)
                    {
                        stringBuilder.Append(", ");
                    }
                    stringBuilder.Append(t2.ToString() + " x" + num.ToString());
                    dictionary.Remove(t2);
                }
            }
            dictionary.Clear();
            return stringBuilder.ToString();
        }

        public static void SetOrIncrement<T>(this IDictionary<T, int> dict, T key, int offset)
        {
            int num;
            if (dict.TryGetValue(key, out num))
            {
                offset += num;
            }
            dict[key] = offset;
        }

        public static bool NullOrEmpty<T>(this IEnumerable<T> elements)
        {
            if (elements == null)
            {
                return true;
            }
            ICollection collection = elements as ICollection;
            if (collection != null)
            {
                return collection.Count == 0;
            }
            return !elements.Any<T>();
        }

        public static bool Any(this IList list)
        {
            return list.Count != 0;
        }

        public static bool Any_NonGeneric(this IEnumerable enumerable)
        {
            ICollection collection = enumerable as ICollection;
            if (collection != null)
            {
                return collection.Count != 0;
            }
            using (IEnumerator enumerator = enumerable.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    object obj = enumerator.Current;
                    return true;
                }
            }
            return false;
        }

        public static bool ContainsDuplicates<T>(this IEnumerable<T> enumerable)
        {
            ICollection<T> collection = enumerable as ICollection<T>;
            return (collection == null || collection.Count > 1) && enumerable != null && enumerable.Distinct<T>().Count<T>() != enumerable.Count<T>();
        }

        public static bool TryGetMinBy<TElem, TBy>(this IEnumerable<TElem> collection, Func<TElem, TBy> by, out TElem minElement)
        {
            CollectionsUtility.<> c__DisplayClass8_0 < TElem, TBy > CS$<> 8__locals1;
            CS$<> 8__locals1.by = by;
            CS$<> 8__locals1.minValue = default(TBy);
            CS$<> 8__locals1.minElementLocal = default(TElem);
            CS$<> 8__locals1.first = true;
            CS$<> 8__locals1.comparer = Comparer<TBy>.Default;
            IList<TElem> list = collection as IList<TElem>;
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    CollectionsUtility.< TryGetMinBy > g__Check | 8_0 < TElem, TBy > (list[i], ref CS$<> 8__locals1);
            }
        }
			else
			{
				foreach (TElem telem in collection)
				{
					CollectionsUtility.<TryGetMinBy>g__Check|8_0<TElem, TBy>(telem, ref CS$<>8__locals1);
				}
}
minElement = CS$<> 8__locals1.minElementLocal;
return !CS$<> 8__locals1.first;
		}

		public static bool TryGetMaxBy<TElem, TBy>(this IEnumerable<TElem> collection, Func<TElem, TBy> by, out TElem maxElement)
{
    CollectionsUtility.<> c__DisplayClass9_0 < TElem, TBy > CS$<> 8__locals1;
    CS$<> 8__locals1.by = by;
    CS$<> 8__locals1.maxValue = default(TBy);
    CS$<> 8__locals1.maxElementLocal = default(TElem);
    CS$<> 8__locals1.first = true;
    CS$<> 8__locals1.comparer = Comparer<TBy>.Default;
    IList<TElem> list = collection as IList<TElem>;
    if (list != null)
    {
        for (int i = 0; i < list.Count; i++)
        {
            CollectionsUtility.< TryGetMaxBy > g__Check | 9_0 < TElem, TBy > (list[i], ref CS$<> 8__locals1);
    }
}

            else
{
    foreach (TElem telem in collection)
    {
        CollectionsUtility.< TryGetMaxBy > g__Check | 9_0 < TElem, TBy > (telem, ref CS$<> 8__locals1);
    }
}
maxElement = CS$<> 8__locals1.maxElementLocal;
return !CS$<> 8__locals1.first;
		}

		public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default(TValue))
{
    TValue tvalue;
    if (dict != null && dict.TryGetValue(key, out tvalue))
    {
        return tvalue;
    }
    return defaultValue;
}

public static void StableSort<T, TBy>(this List<T> list, Func<T, TBy> sortBy)
{
    List<T> list2 = Pool<List<T>>.Get();
    list2.Clear();
    try
    {
        list2.AddRange(list.OrderBy<T, TBy>(sortBy));
        list.Clear();
        list.AddRange(list2);
    }
    finally
    {
        list2.Clear();
        Pool<List<T>>.Return(list2);
    }
}

public static void StableSort<T, TBy, TBy2>(this List<T> list, Func<T, TBy> sortBy, Func<T, TBy2> thenBy)
{
    List<T> list2 = Pool<List<T>>.Get();
    list2.Clear();
    try
    {
        list2.AddRange(list.OrderBy<T, TBy>(sortBy).ThenBy<T, TBy2>(thenBy));
        list.Clear();
        list.AddRange(list2);
    }
    finally
    {
        list2.Clear();
        Pool<List<T>>.Return(list2);
    }
}

public static int IndexOf<T>(this IEnumerable<T> enumerable, T element)
{
    IList<T> list = enumerable as IList<T>;
    if (list != null)
    {
        return list.IndexOf(element);
    }
    int num = 0;
    EqualityComparer<T> @default = EqualityComparer<T>.Default;
    foreach (T t in enumerable)
    {
        if (@default.Equals(t, element))
        {
            return num;
        }
        num++;
    }
    return -1;
}

public static int Count<T>(this IEnumerable<T> enumerable, T element)
{
    int num = 0;
    EqualityComparer<T> @default = EqualityComparer<T>.Default;
    IList<T> list = enumerable as IList<T>;
    if (list != null)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (@default.Equals(list[i], element))
            {
                num++;
            }
        }
    }
    else
    {
        foreach (T t in enumerable)
        {
            if (@default.Equals(t, element))
            {
                num++;
            }
        }
    }
    return num;
}

public static bool RemoveLast<T>(this List<T> list, T elem)
{
    int num = list.LastIndexOf(elem);
    if (num >= 0)
    {
        list.RemoveAt(num);
        return true;
    }
    return false;
}

public static bool Remove_ProbablyAt<T>(this IList<T> list, T elem, int probablyAt)
{
    if (probablyAt >= 0 && probablyAt < list.Count && EqualityComparer<T>.Default.Equals(elem, list[probablyAt]))
    {
        list.RemoveAt(probablyAt);
        return true;
    }
    return list.Remove(elem);
}

public static bool Contains_ProbablyAt<T>(this IList<T> list, T elem, int probablyAt)
{
    return (probablyAt >= 0 && probablyAt < list.Count && EqualityComparer<T>.Default.Equals(elem, list[probablyAt])) || list.Contains(elem);
}

public static void Swap<T>(this IList<T> list, int index1, int index2)
{
    T t = list[index1];
    list[index1] = list[index2];
    list[index2] = t;
}

public static void AddSorted<T>(this List<T> list, T item, IComparer<T> comparer)
{
    int count = list.Count;
    if (count == 0 || comparer.Compare(list[count - 1], item) <= 0)
    {
        list.Add(item);
        return;
    }
    if (comparer.Compare(list[0], item) >= 0)
    {
        list.Insert(0, item);
        return;
    }
    int num = list.BinarySearch(item, comparer);
    if (num < 0)
    {
        num = ~num;
    }
    list.Insert(num, item);
}

public static bool RemoveSorted<T>(this List<T> list, T item, IComparer<T> comparer)
{
    int count = list.Count;
    if (count <= 2)
    {
        for (int i = 0; i < count; i++)
        {
            if (EqualityComparer<T>.Default.Equals(list[i], item))
            {
                list.RemoveAt(i);
                return true;
            }
        }
        return false;
    }
    int num = list.BinarySearch(item, comparer);
    if (num >= 0)
    {
        list.RemoveAt(num);
        return true;
    }
    return false;
}

public static void AddSortedNonGeneric(this IList list, object item, IComparer comparer)
{
    int count = list.Count;
    if (count == 0 || comparer.Compare(list[count - 1], item) <= 0)
    {
        list.Add(item);
        return;
    }
    if (comparer.Compare(list[0], item) >= 0)
    {
        list.Insert(0, item);
        return;
    }
    int num = list.BinarySearchNonGeneric(item, comparer);
    if (num < 0)
    {
        num = ~num;
    }
    list.Insert(num, item);
}

public static bool RemoveSortedNonGeneric(this IList list, object item, IComparer comparer, IEqualityComparer equalityComparer)
{
    int count = list.Count;
    if (count <= 2)
    {
        for (int i = 0; i < count; i++)
        {
            if (equalityComparer.Equals(list[i], item))
            {
                list.RemoveAt(i);
                return true;
            }
        }
        return false;
    }
    int num = list.BinarySearchNonGeneric(item, comparer);
    if (num >= 0)
    {
        list.RemoveAt(num);
        return true;
    }
    return false;
}

public static int BinarySearchNonGeneric(this IList list, object item, IComparer comparer)
{
    int i = 0;
    int num = list.Count - 1;
    while (i <= num)
    {
        int num2 = i + (num - i) / 2;
        int num3 = comparer.Compare(item, list[num2]);
        if (num3 == 0)
        {
            return num2;
        }
        if (num3 < 0)
        {
            num = num2 - 1;
        }
        else
        {
            i = num2 + 1;
        }
    }
    return ~i;
}

public static List<T> ToTemporaryList<T>(this IEnumerable<T> enumerable)
{
    List<T> list = FrameLocalPool<List<T>>.Get();
    list.AddRange(enumerable);
    return list;
}

public static void ToFile<T>(this IEnumerable<T> enumerable, string file = "temp.txt")
{
    StringBuilder stringBuilder = new StringBuilder();
    if (enumerable != null)
    {
        bool flag = true;
        foreach (T t in enumerable)
        {
            if (flag)
            {
                flag = false;
            }
            else
            {
                stringBuilder.AppendLine();
            }
            stringBuilder.Append((t == null) ? "null" : t.ToString());
        }
    }
    File.WriteAllText(file, stringBuilder.ToString());
}

public static void RemoveNullFromIEnumerableHashSet(IEnumerable hashSet)
{
    hashSet.GetType().GetMethod("Remove").Invoke(hashSet, new object[1]);
}

public static bool RemoveUnordered<T>(List<T> list, T element)
{
    int num = list.IndexOf(element);
    if (num < 0)
    {
        return false;
    }
    CollectionsUtility.RemoveAtUnordered<T>(list, num);
    return true;
}

public static void RemoveAtUnordered<T>(List<T> list, int index)
{
    list[index] = list[list.Count - 1];
    list.RemoveAt(list.Count - 1);
}

[CompilerGenerated]
internal static void < TryGetMinBy > g__Check | 8_0 < TElem, TBy > (TElem elem, ref CollectionsUtility.<> c__DisplayClass8_0 < TElem, TBy > A_1)

        {
    TBy tby = A_1.by(elem);
    if (A_1.first || A_1.comparer.Compare(tby, A_1.minValue) < 0)
    {
        A_1.first = false;
        A_1.minElementLocal = elem;
        A_1.minValue = tby;
    }
}

[CompilerGenerated]
internal static void < TryGetMaxBy > g__Check | 9_0 < TElem, TBy > (TElem elem, ref CollectionsUtility.<> c__DisplayClass9_0 < TElem, TBy > A_1)

        {
    TBy tby = A_1.by(elem);
    if (A_1.first || A_1.comparer.Compare(tby, A_1.maxValue) > 0)
    {
        A_1.first = false;
        A_1.maxElementLocal = elem;
        A_1.maxValue = tby;
    }
}
	}
}