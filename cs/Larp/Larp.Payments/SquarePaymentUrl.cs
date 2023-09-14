namespace Larp.Payments;

public class SquarePaymentUrl
{
    public string Url { get; init; } = default!;
    public string OrderId { get; init; } = default!;
    public string LongUrl { get; set; } = default!;
}