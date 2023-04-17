using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Larp.Landing.Client;
using Larp.Landing.Client.Services;
using Larp.Landing.Shared;
using Larp.Landing.Shared.MwFifth;
using MudBlazor.Services;
using MudExtensions.Services;
using Larp.Landing.Client.RestClient;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

{
    var services = builder.Services;

    services.AddBlazoredLocalStorageAsSingleton();

    services.AddSingleton(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

    // Larp.Landing.Client
    services.AddSingleton<DataCacheService>();
    services.AddSingleton<LandingService>();
    services.AddSingleton<LandingServiceClientLegacy>();
    services.AddSingleton<LandingServiceUpkeep>();
    // services.AddSingleton<ILandingService>(provider => provider.GetRequiredService<LandingServiceClient>());
    // services.AddSingleton<IMwFifthService>(provider => provider.GetRequiredService<LandingServiceClient>());
    // services.AddSingleton<IAdminService>(provider => provider.GetRequiredService<LandingServiceClient>());

    // Larp.Landing.Client.RestClient
    services.AddSingleton<ILandingService, LandingServiceClient>();
    services.AddSingleton<IMwFifthService, MwFifthServiceClient>();
    services.AddSingleton<IAdminService, AdminServiceClient>();
    
    services.AddMudServices();
    services.AddMudExtensions();
}

builder.Logging.SetMinimumLevel(LogLevel.Trace);

await builder.Build().RunAsync();