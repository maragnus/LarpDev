using System.Collections;
using Larp.Assistant;
using Larp.Assistant.OpenAi;
using Larp.Common.LifeCycle;
using Larp.Data.Seeder;
using Larp.Landing.Server.Import;
using Larp.Landing.Server.Services;
using Larp.Landing.Shared.MwFifth;
using Larp.Notify;
using Larp.Payments;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Internal;

if (Environment.GetEnvironmentVariable("DUMP_ENV") == "1")
{
    foreach (DictionaryEntry v in Environment.GetEnvironmentVariables())
        Console.WriteLine($"{v.Key} = {v.Value}");
}

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
    services.AddScoped<CitationManager>();
    services.AddScoped<TransactionManager>();
    services.AddAiAssistant();
    services.Configure<OpenAiOptions>(builder.Configuration.GetSection(OpenAiOptions.SectionName));

    // Larp.Data.Seeder
    services.AddScoped<LarpDataSeeder>();
    services.AddStartupTask<LarpDataSeederStartupTask>();

    // Larp.Payments
    services.Configure<SquareOptions>(
        builder.Configuration.GetSection(SquareOptions.SectionName));
    services.AddSquareService();
    services.AddScoped<ISquareTransactionHandler, SquareTransactionHandler>();

    // Larp.Landing.Server
    services.AddScoped<ILandingService, LandingServiceServer>();
    services.AddScoped<IMwFifthService, MwFifthServiceServer>();
    services.AddScoped<IAdminService, AdminService>();
    services.AddScoped<IAssistantService, AssistantService>();
    services.AddScoped<IClarifyService, ClarifyService>();
    services.AddScoped<IUserSession, UserSession>();
    services.AddScoped<ExcelImporter>();
    services.AddScoped<BackupManager>();
    services.AddSingleton<IImageModifier, ImageModifier>();
    services.AddResponseCompression();
    services.AddAuthenticationCore(options => options.AddScheme<LarpAuthenticationHandler>("Default", null));
    services.AddStartupTask<SquareStartupTask>();
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
    app.UseHsts();
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapApi<ILandingService>();
app.MapApi<IMwFifthService>();
app.MapApi<IAdminService>();
app.MapApi<IAssistantService>();
app.MapApi<IClarifyService>();

app.MapMethods("/square/callback",
    new[] { HttpMethods.Get, HttpMethods.Post },
    SquareWebhook.HandleCallbackAsync);

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

GC.Collect();
GC.WaitForPendingFinalizers();

app.Run();