using Larp.Common.LifeCycle;
using Larp.Data;
using Larp.Data.Seeder;
using Larp.Data.Services;
using Larp.WebService.Controllers;
using Larp.WebService.GrpcServices;
using Larp.WebService.LarpServices;
using Larp.WebService.ProtobufControllers;
using Microsoft.Extensions.Internal;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

{
    var services = builder.Services!;

    services.AddSingleton<ISystemClock, SystemClock>();

    // Larp.Data
    services.AddSingleton<LarpDataCache>();
    services.AddScoped<IEventService, EventService>();
    services.AddScoped<IUserSessionManager, UserSessionManager>();
    services.Configure<UserSessionManagerOptions>(
        builder.Configuration.GetSection(UserSessionManagerOptions.SectionName));
    services.AddScoped<LarpContext>();
    services.Configure<LarpDataOptions>(builder.Configuration.GetSection(LarpDataOptions.SectionName));

    // Larp.Data.Seeder
    services.AddScoped<LarpDataSeeder>();
    services.AddStartupTask<LarpDataSeederStartupTask>();

    // Larp.WebService
    services.AddScoped<IAuthenticationService, AuthenticationService>();
    services.AddScoped<AuthenticationGrpcService>();
    services.AddScoped<IUserNotificationService, UserNotificationService>();
    services.AddSingleton<ProtobufControllerHub>();
    services.AddProtobufController<AuthController>();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseHsts();
}

var protobufHub = app.Services.GetRequiredService<ProtobufControllerHub>();
app.MapPost("/msg/{service:alpha}/{method:alpha}", protobufHub.HandleRequest);
app.MapFallbackToFile("index.html");

app.Run();