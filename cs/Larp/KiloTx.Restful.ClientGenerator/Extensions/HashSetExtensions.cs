namespace KiloTx.Restful.ClientGenerator.Extensions;

internal static class HashSetExtensions
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
}