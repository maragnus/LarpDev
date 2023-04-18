using Larp.Common;
using Larp.Common.LifeCycle;
using Larp.Data.Seeder;
using Larp.Landing.Server.Import;
using Larp.Landing.Server.Services;
using Larp.Landing.Shared.MwFifth;
using Larp.Notify;
using Microsoft.Extensions.FileProviders;
using ISystemClock = Microsoft.Extensions.Internal.ISystemClock;
using SystemClock = Microsoft.Extensions.Internal.SystemClock;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>();

{
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
    services.AddScoped<IUserSessionManager, UserSessionManager>();
    services.Configure<UserSessionManagerOptions>(
        builder.Configuration.GetSection(UserSessionManagerOptions.SectionName));
    services.AddScoped<LarpContext>();
    services.Configure<LarpDataOptions>(builder.Configuration.GetSection(LarpDataOptions.SectionName));
    services.AddScoped<MwFifthCharacterManager>();
    services.AddScoped<LetterManager>();
    services.AddScoped<AttachmentManager>();
    services.AddScoped<EventManager>();

    // Larp.Data.Seeder
    services.AddScoped<LarpDataSeeder>();
    services.AddStartupTask<LarpDataSeederStartupTask>();

    // Larp.Landing.Server
    services.AddScoped<ILandingService, LandingServiceServer>();
    services.AddScoped<IMwFifthService, MwFifthServiceServer>();
    services.AddScoped<IAdminService, AdminService>();
    services.AddScoped<IUserSession, UserSession>();
    services.AddScoped<ExcelImporter>();
    services.AddScoped<BackupManager>();
    services.AddSingleton<IImageModifier, ImageModifier>();
    services.AddResponseCompression();
    services.AddAuthenticationCore(options => options.AddScheme<LarpAuthenticationHandler>("Default", null));
}

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var larpContext = scope.ServiceProvider.GetRequiredService<LarpContext>();
    await larpContext.Migrate();
}

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

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

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