using System.Text.Json;
using APIMatic.Core.Utilities;
using KiloTx.Restful;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Square.Models;
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
            var json = JsonSerializer.SerializeToDocument(dataValues.FirstOrDefault() ?? "{}");
            _logger.LogInformation("Square Callback: {Json}", json.RootElement.ToString());

            transactionId = json.RootElement.GetProperty("transaction_id").GetString();
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