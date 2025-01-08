
using Eve.Application.ExternalDataLoaders;
using Eve.Domain.Constants;
using System.Diagnostics;

namespace Eve.Api.BackgroundServices;

public class LoadOrdersBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<LoadOrdersBackgroundService> _logger;

    public LoadOrdersBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<LoadOrdersBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //await RunTaskAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var nextRunTime = CalculateNextRunTime();
            var delay = nextRunTime - DateTime.UtcNow;

            if (delay > TimeSpan.Zero)
                await Task.Delay(delay, stoppingToken);

            await RunTaskAsync(stoppingToken);
        }
    }

    private async Task RunTaskAsync(CancellationToken stoppingToken)
    {
        try
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            _logger.LogInformation($"Starting Load orders from ESI at {DateTime.UtcNow}");

            using var scope = _serviceProvider.CreateScope();
            var ordersLoader = scope.ServiceProvider.GetRequiredService<IOrdersLoader>();

            var result = await ordersLoader.Load((int)CentralHubRegionId.Jita, stoppingToken);
            if (result.IsFailure)
                throw new Exception(result.Error.Message);

            stopWatch.Stop();
            var milSec = stopWatch.ElapsedMilliseconds;

            _logger.LogInformation($"Load orders completed is ok in {milSec} at {DateTime.UtcNow}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Load data canceled with error : {ex.Message}", ex);
        }
    }

    private DateTime CalculateNextRunTime()
    {
        var now = DateTime.UtcNow;

        var runTimes = new[]
        {
            new TimeSpan(0,0,0),
            new TimeSpan(6,0,0), 
            new TimeSpan(12,0,0),
            new TimeSpan(18,0,0),
        };

        foreach (var runTime in runTimes)
        {
            var nextRun = now.Date.Add(runTime);

            if (nextRun> now)
                return nextRun;
        }

        return now.Date.AddDays(1).Add(runTimes[0]);
    }
}
