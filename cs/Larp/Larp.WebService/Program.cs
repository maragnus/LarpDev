using Larp.Common.LifeCycle;
using Larp.Data;
using Larp.Data.Seeder;
using Larp.Data.Services;
using Larp.WebService.Controllers;
using Larp.WebService.GrpcServices;
using Larp.WebService.LarpServices;
using Larp.WebService.ProtobufControllers;
using Larp.WebService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Internal;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

{
    var services = builder.Services!;

    services.AddSingleton<ISystemClock, SystemClock>();

    services.AddCors(cors =>
        cors.AddPolicy("Default", policy => policy
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins("https://localhost:5002", "https://localhost:5001", "http://localhost:5000",
                "http://larp.maragnus.com", "https://larp.maragnus.com")
        )
    );

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
    services.AddProtobufController<UserController>();
    services.AddProtobufController<Mw5eController>();
    services.AddScoped<UserSessionState>();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("Default");

app.MapPost("/msg/{service}/{method}",
    async (
        [FromRoute] string service,
        [FromRoute] string method,
        [FromHeader(Name = "x-session-id")] string sessionId,
        [FromServices] ProtobufControllerHub hub,
        [FromServices] IServiceProvider serviceProvider,
        [FromServices] UserSessionState userSessionState,
        HttpContext context,
        CancellationToken cancellationToken) =>
    {
        await userSessionState.Authenticate(sessionId);
        await hub.HandleRequest(service, method, context, serviceProvider, cancellationToken);
    });

app.MapFallbackToFile("index.html");

app.Run();