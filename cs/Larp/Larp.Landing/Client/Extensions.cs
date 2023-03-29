using System.Collections;
using Microsoft.AspNetCore.Components;

namespace Larp.Landing.Client;

public class IndexedCollection<TItem> : IEnumerable<TItem>
{
    private readonly TItem[] _items;
    private readonly Dictionary<string, int> _dict;

    public IndexedCollection(IEnumerable<TItem> items, Func<TItem, string> getKey)
    {
        _items = items.ToArray();
        _dict = _items.Select((item, index) => (item, index))
            .ToDictionary(x => getKey(x.item), x => x.index);
    }

    public TItem? this[int? index] =>
        index == null || index < 0 || _items.Length <= index
            ? default
            : _items[index.Value];

    public TItem? this[string? key] =>
        key == null
            ? default
            : _dict.TryGetValue(key, out var index)
                ? _items[index]
                : default;

    public int? IndexOf(string? key) => key == null
        ? null
        : _dict.TryGetValue(key, out var index)
            ? index
            : null;

    public IEnumerable<(int? Index, TItem Item)> IndexedItems => _items.Select((item, index) => ((int?)index, item));
    
    public IEnumerator<TItem> GetEnumerator() => _items.Cast<TItem>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
}

public static class Extensions
{
    public static bool IsPrintPage(this NavigationManager navigationManager)
    {
        var uri = new Uri(navigationManager.Uri.TrimEnd('/'));
        return uri.AbsolutePath.EndsWith("/print");
    }
    
    public static IndexedCollection<TItem> ToIndexedCollection<TItem>(this IEnumerable<TItem> items,
        Func<TItem, string> getKey) =>
        new IndexedCollection<TItem>(items, getKey);
}