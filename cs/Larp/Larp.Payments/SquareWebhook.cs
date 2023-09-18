using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Larp.Payments;

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