using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using MudBlazor;

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
    private static MethodInfo stateHasChanged;

    static Extensions()
    {
        stateHasChanged =
            typeof(ComponentBase).GetMethod("StateHasChanged", BindingFlags.Instance | BindingFlags.NonPublic)!;
    }

    public static string? GetOrDefault(this Dictionary<string, string> dict, string key) =>
        dict.TryGetValue(key, out var value) ? value : default;

    public static async Task AsyncAction<TType>(this TType component, Expression<Func<TType, bool>> busyIndicator,
        Func<Task> action)
        where TType : ComponentBase
    {
        var memberAccess = busyIndicator.Body as MemberExpression;
        var fieldInfo = memberAccess!.Member as FieldInfo;
        var propertyInfo = memberAccess.Member as PropertyInfo;
        
        fieldInfo?.SetValue(component, true);
        propertyInfo?.SetValue(component, true);
        stateHasChanged.Invoke(component, Array.Empty<object>());
        try
        {
            await action();
        }
        finally
        {
            fieldInfo?.SetValue(component, false);
            propertyInfo?.SetValue(component, false);
        }
    }

    public static async Task<bool> AsyncAction<T>(this ComponentBase component, DialogService dialogService,
        Expression<Func<T, bool>> busyIndicator,
        Func<Task> action)
    {
        LambdaExpression lambda = busyIndicator;
        var memberExpression = lambda.Body is UnaryExpression expression
            ? (MemberExpression)expression.Operand
            : (MemberExpression)lambda.Body;
        var propertyInfo = (PropertyInfo)memberExpression.Member;

        propertyInfo.SetValue(component, true);
        stateHasChanged.Invoke(component, Array.Empty<object>());
        try
        {
            await action();
            return true;
        }
        catch (Exception ex)
        {
            await dialogService.ShowMessageBox("Server Error", ex.Message);
            return false;
        }
        finally
        {
            propertyInfo.SetValue(component, false);
            stateHasChanged.Invoke(component, Array.Empty<object>());
        }
    }

    public static bool IsPrintPage(this NavigationManager navigationManager)
    {
        var uri = new Uri(navigationManager.Uri.TrimEnd('/'));
        return uri.AbsolutePath.EndsWith("/print");
    }

    public static IndexedCollection<TItem> ToIndexedCollection<TItem>(this IEnumerable<TItem> items,
        Func<TItem, string> getKey) =>
        new IndexedCollection<TItem>(items, getKey);
}