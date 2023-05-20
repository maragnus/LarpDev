namespace KiloTx.Restful.ClientGenerator.Extensions;

internal static class DictionaryExtensions
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