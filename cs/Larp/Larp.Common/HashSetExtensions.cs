namespace Larp.Common;

public static class HashSetExtensions
{
    public static void AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> items)
    {
        foreach (var item in items)
            hashSet.Add(item);
    }
    
    public static void RemoveRange<T>(this HashSet<T> hashSet, IEnumerable<T> items)
    {
        foreach (var item in items)
            hashSet.Remove(item);
    }
    
    public static void SetRange<T>(this HashSet<T> hashSet, IEnumerable<T> items, bool includeRange)
    {
        if (includeRange)
            hashSet.AddRange(items);
        else
            hashSet.RemoveRange(items);
    }
}

public static class DictionaryExtensions
{
    public static IEnumerable<TValue> TryFromKeys<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
        IEnumerable<TKey>? keys)
    {
        if (keys == null) yield break;
        foreach (var key in keys)
        {
            if (dictionary.TryGetValue(key, out var item))
                yield return item;
        }
    }

    public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
        IEnumerable<KeyValuePair<TKey, TValue>> values)
    {
        foreach (var (key, item) in values)
            dictionary.Add(key, item);
    }
}