using Larp.Common.LifeCycle;
using Larp.Data;
using Larp.Data.Seeder;
using Larp.Data.Services;
using Larp.WebService.GrpcServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<LarpDataCache>();
builder.Services.AddScoped<UserSessionService>();
builder.Services.Configure<UserSessionServiceOptions>(builder.Configuration.GetSection(UserSessionServiceOptions.SectionName));

builder.Services.AddScoped<LarpContext>();
builder.Services.Configure<LarpDataOptions>(builder.Configuration.GetSection(LarpDataOptions.SectionName));

builder.Services.AddScoped<LarpDataSeeder>();
builder.Services.AddStartupTask<LarpDataSeederStartupTask>();

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<AuthenticationGrpcService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();