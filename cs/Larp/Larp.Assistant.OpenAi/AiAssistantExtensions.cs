using Microsoft.Extensions.DependencyInjection;

namespace Larp.Assistant.OpenAi;

public static class AiAssistantExtensions
{
    public static IServiceCollection AddAiAssistant(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IAiAssistant, OpenAiAssistant>();
        return serviceCollection;
    }
}