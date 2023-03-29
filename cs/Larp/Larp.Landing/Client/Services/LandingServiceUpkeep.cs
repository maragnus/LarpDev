using Microsoft.Extensions.Hosting;

namespace Larp.Landing.Client.Services;

/// <summary>This service periodically checks with the server to make sure the session is still valid</summary>
public class LandingServiceUpkeep : BackgroundService
{
    private readonly LandingService _landingService;

    public LandingServiceUpkeep(LandingService landingService)
    {
        _landingService = landingService;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                await _landingService.ValidateSession();   
            }
            catch
            {
                // ignored   
            }
        }
    }
}