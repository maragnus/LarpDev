namespace Larp.Common;

public static class CurrencyExtensions
{
    public static int? ToInt32(this long? cents) =>
        cents.HasValue ? Convert.ToInt32(cents.Value) : null;

    public static long? ToInt64(this int? cents) =>
        cents.HasValue ? Convert.ToInt64(cents.Value) : null;

    public static decimal ToCurrency(this int cents) =>
        Convert.ToDecimal(cents / 100m);

    public static int ToCents(this decimal cents) =>
        Convert.ToInt32(cents * 100m);

    public static decimal? ToCurrency(this int? cents) =>
        cents.HasValue
            ? ToCurrency(cents.Value)
            : null;

    public static int? ToCents(this decimal? dollars) =>
        dollars.HasValue
            ? ToCents(dollars.Value)
            : null;
}