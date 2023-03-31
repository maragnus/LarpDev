using Larp.Common.LifeCycle;
using Larp.Data.Mongo;
using Larp.Data.Mongo.Services;
using Larp.Data.Seeder;
using Larp.Landing.Server;
using Larp.Landing.Server.Services;
using Larp.Landing.Shared;
using Larp.Landing.Shared.MwFifth;
using Larp.Notify;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Internal;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>();

{
    // Add services to the container.
    var services = builder.Services;

    services.AddSingleton<ISystemClock, SystemClock>();

    services.AddControllersWithViews();
    services.AddRazorPages();
    services.AddHttpContextAccessor();
    services.AddSingleton<IFileProvider>(_ => new PhysicalFileProvider(Path.GetTempPath()));

    // Larp.Notify
    services.AddScoped<INotifyService, NotifyService>();
    services.Configure<NotifyServiceOptions>(
        builder.Configuration.GetSection(NotifyServiceOptions.SectionName));

    // Larp.Data.Mongo
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

    // Larp.Landing.Server
    services.AddScoped<ILandingService, LandingServiceServer>();
    services.AddScoped<IMwFifthService, MwFifthServiceServer>();
    services.AddScoped<IAdminService, AdminService>();
    services.AddScoped<IUserSession, UserSession>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseMiddleware<UserSessionMiddleware>();

app.UseRouting();

app.MapApi<ILandingService>();
app.MapApi<IMwFifthService>();
app.MapApi<IAdminService>();

app.MapMethods("/api/{*path}",
    new[] { HttpMethods.Get, HttpMethods.Put, HttpMethods.Patch, HttpMethods.Post, HttpMethods.Patch },
    (HttpContext context, string path) =>
    {
        context.Response.StatusCode = 404;
        context.Response.WriteAsync($"Path /api/{path} is not routed");
        return Task.CompletedTask;
    });

app.MapRazorPages();
app.MapFallbackToFile("index.html");

app.Run();