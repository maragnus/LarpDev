using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Larp.Landing.Client;
using Larp.Landing.Shared;
using MudBlazor.Services;

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

    services.AddMudServices();
}

builder.Logging.SetMinimumLevel(LogLevel.Trace);

await builder.Build().RunAsync();