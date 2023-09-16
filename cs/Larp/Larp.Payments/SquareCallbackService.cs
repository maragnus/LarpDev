using System.Text.Json;
using System.Text.Json.Serialization;
using APIMatic.Core.Utilities;
using KiloTx.Restful;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Square.Utilities;

namespace Larp.Payments;

public class SquareCallbackService
{
    private readonly ILogger<SquareCallbackService> _logger;
    private readonly SquareOptions _options;
    private readonly ISquareTransactionHandler _transactionHandler;

    public SquareCallbackService(IOptions<SquareOptions> options, ISquareTransactionHandler transactionHandler,
        ILogger<SquareCallbackService> logger)
    {
        _options = options.Value;
        _transactionHandler = transactionHandler;
        _logger = logger;
    }

    private bool IsFromSquare(HttpRequest request, string body)
    {
        if (string.IsNullOrEmpty(SquareService.SignatureKey))
        {
            _logger.LogWarning("SignatureKey unset");
            return false;
        }

        var signature = request.Headers.TryGetValue("x-square-hmacsha256-signature", out var sig)
            ? sig.FirstOrDefault()
            : null;
        return WebhooksHelper.IsValidWebhookEventSignature(body, signature, SquareService.SignatureKey,
            _options.Webhook.CallbackUrl);
    }

    public async Task HandleCallbackAsync(HttpContext httpContext)
    {
        if (httpContext.Request.Method == "POST")
            await HandlePost(httpContext);
        else
            await HandleSquare(httpContext);
    }

    private async Task HandleSquare(HttpContext httpContext)
    {
        var request = httpContext.Request;
        var response = httpContext.Response;

        if (!request.Query.TryGetValue("data", out var dataValues)
            || dataValues.Count != 1
            || string.IsNullOrEmpty(dataValues[0]))
        {
            _logger.LogError("Unexpected request on Square POS callback: {Query}", request.Query);
            httpContext.Response.StatusCode = 400;
            return;
        }

        string? transactionId;
        try
        {
            var json = JsonSerializer.Deserialize<SquarePointOfSaleResponse>(dataValues.FirstOrDefault() ?? "{}");
            transactionId = json?.TransactionId;
            if (string.IsNullOrEmpty(transactionId))
                throw new BadRequestException("Expected transaction_id");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse Square POS callback: {Json}", dataValues.FirstOrDefault());
            httpContext.Response.StatusCode = 400;
            return;
        }

        await _transactionHandler.PointOfSaleComplete(transactionId);
        response.StatusCode = 303; // See Other
        response.Headers["Location"] = _options.PointOfSale.ReturnUrl;
    }

    private async Task HandlePost(HttpContext httpContext)
    {
        string body;
        using (var reader = new StreamReader(httpContext.Request.Body))
            body = await reader.ReadToEndAsync();

        if (!IsFromSquare(httpContext.Request, body))
            _logger.LogWarning("Request did not original from Square");

        var @event = CoreHelper.JsonDeserialize<EventRequest>(body);
        var id = @event.Data.Id ?? throw new BadRequestException("event.data.id expected");

        _logger.LogInformation("Square Callback {EventType}: {Json}",
            @event.Type, body);

        switch (@event.Type)
        {
            case "payment.created":
                await _transactionHandler.PaymentCreated(id);
                break;
            case "payment.updated":
                await _transactionHandler.PaymentUpdated(id);
                break;
            case "order.created":
                await _transactionHandler.OrderUpdated(id);
                break;
            case "order.updated":
                await _transactionHandler.OrderUpdated(id);
                break;
            default:
                throw new BadRequestException($"Unexpected type of {@event.Type}");
        }
    }

    private class EventRequest
    {
        [JsonPropertyName("type")] public string? Type { get; set; }

        [JsonPropertyName("data")] public DataObject Data { get; set; } = new();

        public class DataObject
        {
            [JsonPropertyName("type")] public string? Type { get; set; }
            [JsonPropertyName("id")] public string? Id { get; set; }
        }
    }
}

public class SquarePointOfSaleResponse
{
    [JsonPropertyName("status")] public string? Status { get; set; }

    [JsonPropertyName("transaction_id")] public string? TransactionId { get; set; }

    [JsonPropertyName("client_transaction_id")]
    public string? ClientTransactionId { get; set; }
}

public class OrderCreatedRequest
{
    [JsonPropertyName("type")] public string? Type { get; set; }

    [JsonPropertyName("id")] public string? OrderId { get; set; }

    [JsonPropertyName("object")] public ObjectObject Object { get; set; } = new ObjectObject();

    public class ObjectObject
    {
        [JsonPropertyName("order_created")] public OrderCreatedObject OrderCreated { get; set; } = new();
    }

    public class OrderCreatedObject
    {
        [JsonPropertyName("created_at")] public string? CreatedAt { get; set; }

        [JsonPropertyName("location_id")] public string? LocationId { get; set; }

        [JsonPropertyName("order_id")] public string? OrderId { get; set; }

        [JsonPropertyName("state")] public string? State { get; set; }

        [JsonPropertyName("version")] public int? Version { get; set; }
    }
}