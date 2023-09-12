using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Square;
using Square.Exceptions;
using Square.Models;
using Environment = Square.Environment;

namespace Larp.Square;

public interface ISquareService
{
    Task<SquarePaymentUrl> CreatePaymentUrl(string transactionId, decimal amount, string itemName, string name,
        string email, string phone);
}

public class SquarePaymentUrl
{
    public string Url { get; init; }
    public string OrderId { get; init; }
}

public class SquareService : ISquareService
{
    private readonly ILogger<SquareService> _logger;
    private readonly SquareOptions _options;

    public SquareService(IOptions<SquareOptions> options, ILogger<SquareService> logger)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task<SquarePaymentUrl> CreatePaymentUrl(string transactionId, decimal amount, string itemName,
        string name, string email, string phone)
    {
        try
        {
            var client = CreateClient();

            var money = new Money.Builder()
                .Amount((int)(amount * 100))
                .Currency("USD")
                .Build();

            var body = new CreatePaymentLinkRequest.Builder()
                .IdempotencyKey(transactionId)
                .PaymentNote($"{name} {email} {phone}".Trim())
                .QuickPay(new QuickPay.Builder(itemName, money, _options.LocationId).Build())
                .PrePopulatedData(new PrePopulatedData.Builder()
                    .BuyerAddress(new Address.Builder()
                        .FirstName(name)
                        .Build())
                    .BuyerEmail(email)
                    //.BuyerPhoneNumber(phone)
                    .Build())
                .CheckoutOptions(new CheckoutOptions.Builder()
                    .RedirectUrl(_options.ReturnUrl)
                    .AllowTipping(false)
                    .EnableCoupon(false)
                    .Build())
                .Build();

            var response = await client.CheckoutApi.CreatePaymentLinkAsync(body);

            return new SquarePaymentUrl
            {
                OrderId = response.PaymentLink.OrderId,
                Url = response.PaymentLink.Url
            };
        }
        catch (ApiException ex)
        {
            _logger.LogInformation("CreatePaymentLink failed: {Json}",
                ex.HttpContext.Response.Body);
            throw;
        }
    }

    public async Task<Location[]> GetLocations()
    {
        var client = CreateClient();
        var locations = await client.LocationsApi.ListLocationsAsync();

        return locations.Locations.ToArray();
    }

    private SquareClient CreateClient() =>
        new SquareClient.Builder()
            .AccessToken(_options.AccessToken)
            .SquareVersion("2023-08-16")
            .Environment(Environment.Sandbox)
            //.CustomUrl(_options.SquareUrl)
            .Build();
}

public static class SquareWebhook
{
    public static IServiceCollection AddSquareService(this IServiceCollection serviceCollection) =>
        serviceCollection
            .AddScoped<SquareCallbackService>()
            .AddScoped<ISquareService, SquareService>();

    public static async Task HandleCallbackAsync(HttpContext httpContext, SquareCallbackService callbackService)
    {
        await callbackService.HandleCallbackAsync(httpContext);
    }
}