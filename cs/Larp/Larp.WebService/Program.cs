using System.IO.Compression;
using Grpc.AspNetCore.Server;
using Larp.Common.LifeCycle;
using Larp.Data;
using Larp.Data.Seeder;
using Larp.Data.Services;
using Larp.WebService.GrpcServices;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Internal;

var builder = WebApplication.CreateBuilder(args);

if (OperatingSystem.IsMacOS())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        // Setup a HTTP/2 endpoint without TLS.
        options.ListenLocalhost(5000, o => o.Protocols = HttpProtocols.Http1AndHttp2);
        options.ListenLocalhost(5001, o => o.Protocols = HttpProtocols.Http2);
    });
}

builder.Services.AddSingleton<ISystemClock, SystemClock>();
builder.Services.AddSingleton<LarpDataCache>();
builder.Services.AddScoped<UserSessionService>();
builder.Services.Configure<UserSessionServiceOptions>(builder.Configuration.GetSection(UserSessionServiceOptions.SectionName));

builder.Services.AddScoped<LarpContext>();
builder.Services.Configure<LarpDataOptions>(builder.Configuration.GetSection(LarpDataOptions.SectionName));

builder.Services.AddScoped<LarpDataSeeder>();
builder.Services.AddStartupTask<LarpDataSeederStartupTask>();

builder.Services.AddGrpc(o =>
{
    o.EnableDetailedErrors = true;
    o.ResponseCompressionLevel = CompressionLevel.SmallestSize;
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors();
app.UseGrpcWeb(new GrpcWebOptions() { DefaultEnabled = true});

app.MapFallbackToFile("index.html");
app.MapGrpcService<AuthenticationGrpcService>()
    .EnableGrpcWeb()
    .RequireCors("GrpcCors")
    .AllowAnonymous();

app.Run();