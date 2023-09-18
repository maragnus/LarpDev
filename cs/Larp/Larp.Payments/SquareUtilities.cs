using System.Text.RegularExpressions;

namespace Larp.Payments;

public static partial class SquareUtilities
{
    public static async Task<TItem[]> ListAsync<TItem, TResponse>(
        Func<Task<TResponse>> firstRequest,
        Func<string, Task<TResponse>> nextRequest,
        Func<TResponse, IList<TItem>?> getItems,
        Func<TResponse, string?> getCursor)
    {
        var items = new List<TItem>();
        var response = await firstRequest();
        var responseItems = getItems(response);
        while (responseItems is not null)
        {
            var count = items.Count;
            items.AddRange(responseItems);

            var cursor = getCursor(response);
            if (items.Count == count || string.IsNullOrEmpty(cursor)) break;
            response = await nextRequest(cursor);
            responseItems = getItems(response)?.ToList();
        }

        return items.ToArray();
    }

    public static bool TryFixPhoneNumber(string? phone, out string sanitizedPhone)
    {
        sanitizedPhone = "";

        if (string.IsNullOrWhiteSpace(phone))
            return false;

        sanitizedPhone = RegexToNumeric().Replace(phone.Trim(), "");

        if (phone.StartsWith('+') && sanitizedPhone.Length > 5)
        {
            sanitizedPhone = $"+1{sanitizedPhone}";
            return true;
        }

        if (sanitizedPhone.Length == 10)
        {
            sanitizedPhone = $"+1{sanitizedPhone}";
            return true;
        }

        if (sanitizedPhone.Length == 11 && sanitizedPhone.StartsWith("1"))
        {
            sanitizedPhone = $"+{sanitizedPhone}";
            return true;
        }

        return false;
    }

    [GeneratedRegex("[^\\d]")]
    private static partial Regex RegexToNumeric();

    public static string CreateIdempotencyKey() => Guid.NewGuid().ToString();
}