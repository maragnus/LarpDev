using System.IO.Compression;
using Larp.Common.LifeCycle;
using Larp.Data;
using Larp.Data.Seeder;
using Larp.Data.Services;
using Larp.WebService;
using Larp.WebService.GrpcServices;
using Larp.WebService.LarpServices;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using ISystemClock = Microsoft.Extensions.Internal.ISystemClock;
using SystemClock = Microsoft.Extensions.Internal.SystemClock;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ISystemClock, SystemClock>();

if (OperatingSystem.IsMacOS())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        // Setup a HTTP/2 endpoint without TLS.
        options.ListenLocalhost(5000, o => o.Protocols = HttpProtocols.Http1AndHttp2);
        options.ListenLocalhost(5001, o => o.Protocols = HttpProtocols.Http2);
    });
}

builder.Services.AddGrpc(o =>
{
    o.EnableDetailedErrors = true;
    o.ResponseCompressionLevel = CompressionLevel.SmallestSize;
    o.Interceptors.Add<AuthInterceptor>();
});

builder.Services.AddCors(cors =>
{
    cors.AddDefaultPolicy(policy => policy
        .AllowAnyMethod()
        .AllowAnyOrigin()
        .AllowAnyHeader());
    cors.AddPolicy("GrpcCors", policy => policy
        .WithMethods("POST", "OPTIONS")
        .AllowAnyHeader()
        .WithOrigins("https://localhost:5002", "https://mystwoodlanding.azurewebsites.net")
        .WithExposedHeaders("Grpc-Status", "Grpc-Message"));
});

// Larp.Data
builder.Services.AddSingleton<LarpDataCache>();
builder.Services.AddScoped<IUserSessionManager, UserSessionManager>();
builder.Services.Configure<UserSessionManagerOptions>(builder.Configuration.GetSection(UserSessionManagerOptions.SectionName));
builder.Services.AddScoped<LarpContext>();
builder.Services.Configure<LarpDataOptions>(builder.Configuration.GetSection(LarpDataOptions.SectionName));

// Larp.Data.Seeder
builder.Services.AddScoped<LarpDataSeeder>();
builder.Services.AddStartupTask<LarpDataSeederStartupTask>();

// Larp.WebService
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<AuthenticationGrpcService>();
builder.Services.AddScoped<IUserNotificationService, UserNotificationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors();
app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

app.MapFallbackToFile("index.html");

app.MapGrpcService<AuthenticationGrpcService>()
    .EnableGrpcWeb()
    .RequireCors("GrpcCors")
    .AllowAnonymous();

app.MapGrpcService<UserGrpcService>()
    .EnableGrpcWeb()
    .RequireCors("GrpcCors")
    .AllowAnonymous();

app.MapGrpcService<AdminGrpcService>()
    .EnableGrpcWeb()
    .RequireCors("GrpcCors")
    .AllowAnonymous();

app.Run();