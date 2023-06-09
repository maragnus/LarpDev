namespace Larp.Landing.Shared;

public static class StringExtensions
{
    public static string? ToTitleCase(this string? value) =>
        value is null
            ? null
            : value.Length < 2
                ? value.ToUpperInvariant()
                : char.ToUpperInvariant(value[0]) + value[1..];
}