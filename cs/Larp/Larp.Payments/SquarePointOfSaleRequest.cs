using System.Text.Json.Serialization;

namespace Larp.Payments;

public class SquarePointOfSaleRequest
{
    [JsonPropertyName("amount_money")] public AmountMoneyObject AmountMoney { get; set; } = new();

    [JsonPropertyName("callback_url")] public string? CallbackUrl { get; set; }

    [JsonPropertyName("client_id")] public string? ClientId { get; set; }

    [JsonPropertyName("customer_id")] public string? CustomerId { get; set; }

    [JsonPropertyName("version")] public string? Version { get; set; }

    [JsonPropertyName("notes")] public string? Notes { get; set; }

    [JsonPropertyName("options")] public OptionsObject Options { get; set; } = new();

    [JsonPropertyName("auto_return")] public bool AutoReturn { get; set; }

    public class AmountMoneyObject
    {
        [JsonPropertyName("amount")] public string? Amount { get; set; }

        [JsonPropertyName("currency_code")] public string? CurrencyCode { get; set; }
    }

    public class OptionsObject
    {
        [JsonPropertyName("supported_tender_types")]
        public string[] SupportedTenderTypes { get; set; } = Array.Empty<string>();
    }
}