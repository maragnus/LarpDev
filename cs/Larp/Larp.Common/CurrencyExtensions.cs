namespace Larp.Common;

public static class CurrencyExtensions
{
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