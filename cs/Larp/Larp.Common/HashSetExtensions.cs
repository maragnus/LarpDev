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