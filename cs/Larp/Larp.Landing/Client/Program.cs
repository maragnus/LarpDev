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

    // Larp.Landing.Client
    services.AddSingleton<DataCacheService>();
    services.AddSingleton<LandingService>();
    services.AddSingleton<LandingServiceUpkeep>();

    // Larp.Landing.Client.RestClient
    services.AddSingleton<HttpClientFactory>(_ => 
        new HttpClientFactory(builder.HostEnvironment.BaseAddress));
    services.AddSingleton<ILandingService, LandingServiceClient>();
    services.AddSingleton<IMwFifthService, MwFifthServiceClient>();
    services.AddSingleton<IAdminService, AdminServiceClient>();
    
    // MudBlazor
    services.AddMudServices();
    services.AddMudExtensions();
}

builder.Logging.SetMinimumLevel(LogLevel.Trace);

await builder.Build().RunAsync();