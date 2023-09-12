using APIMatic.Core.Utilities;
using KiloTx.Restful;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Square.Models;
using Square.Utilities;

namespace Larp.Square;

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
        var signature = request.Headers.TryGetValue("x-square-hmacsha256-signature", out var sig)
            ? sig.FirstOrDefault()
            : null;
        return WebhooksHelper.IsValidWebhookEventSignature(body, signature, _options.SignatureKey,
            _options.CallbackUrl);
    }

    public async Task HandleCallbackAsync(HttpContext httpContext)
    {
        string body;
        using (var reader = new StreamReader(httpContext.Request.Body))
            body = await reader.ReadToEndAsync();

        if (!IsFromSquare(httpContext.Request, body))
            _logger.LogWarning("Request did not original from Square");

        var @event = CoreHelper.JsonDeserialize<Event>(body);
        var data = @event.Data.MObject.GetStoredObject()!;

        var payment = data["payment"]!.ToObject<Payment>()!;

        switch (@event.Type)
        {
            case "payment.created":
                await _transactionHandler.PaymentCreated(payment);
                break;
            case "payment.updated":
                await _transactionHandler.PaymentUpdated(payment);
                break;
            default:
                throw new BadRequestException($"Unexpected type of {@event.Type}");
        }
    }
}

internal class WebhookRequest
{
    public string? Type { get; set; }
    public string? Id { get; set; }
    public WebhookRequestObject? Object { get; set; }

    internal class WebhookRequestObject
    {
        public Payment? Payment { get; set; }
    }
}