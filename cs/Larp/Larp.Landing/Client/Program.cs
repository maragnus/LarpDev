using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Larp.Landing.Client;
using Larp.Landing.Shared;
using Skclusive.Core.Component;
using Skclusive.Material.Component;
using Skclusive.Material.Core;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

{
    var services = builder.Services;

    services.AddBlazoredLocalStorage();
    
    // Larp.Landing.Server
    services.AddScoped<LandingService>();
    services.AddScoped<ILandingService, LandingInteropService>();
    services.AddScoped<IMwFifthGameService, LandingInteropService>();

    services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

    services.TryAddMaterialServices
    (
        new MaterialConfigBuilder()
            .WithIsServer(false)
            .WithIsPreRendering(false)
            .WithTheme(Theme.Light)
            .WithDisableBinding(false)
            .WithDisableConfigurer(false)
            .Build()
    );
}

builder.Logging.SetMinimumLevel(LogLevel.Trace);

await builder.Build().RunAsync();