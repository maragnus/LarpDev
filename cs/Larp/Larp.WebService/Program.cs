using System.IO.Compression;
using Larp.Common.LifeCycle;
using Larp.Data;
using Larp.Data.Seeder;
using Larp.Data.Services;
using Larp.WebService.GrpcServices;
using Larp.WebService.LarpServices;
using Microsoft.Extensions.Internal;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

{
    var services = builder.Services;

    services.AddSingleton<ISystemClock, SystemClock>();

    services.AddCors(cors =>
    {
        cors.AddDefaultPolicy(policy => policy
            .AllowAnyMethod()
            .AllowAnyOrigin()
            .AllowAnyHeader());
        cors.AddPolicy("ApiCors", policy => policy
            .WithMethods("POST", "OPTIONS")
            .AllowAnyHeader()
            .WithOrigins("https://localhost:5002", "https://larp.maragnus.com")
            .WithExposedHeaders("Grpc-Status", "Grpc-Message"));
    });

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

async Task MessageHandler(HttpContext context)
{
    await context.Response.WriteAsJsonAsync(new { Success = true });
}

app.MapPost("/msg", MessageHandler);
app.MapFallbackToFile("index.html");

//app.MapControllers();

app.Run();