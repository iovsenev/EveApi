using Eve.Application.InternalServices;

namespace Eve.Api.BackgroundServices;

public class LoadOrdersBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LoadOrdersBackgroundService> _logger;

    public LoadOrdersBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<LoadOrdersBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //await LoadOrders(stoppingToken);
    }

    private async Task LoadOrders(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var loadService = scope.ServiceProvider.GetRequiredService<ILoadOrdersService>();

            var result = await loadService.RunTaskAsync(stoppingToken);

            if (!result)
            {
                for (var i = 0; i < 5; i++)
                {
                    _logger.LogWarning($"Retry Load part {i + 1} start on 10 min");
                    await Task.Delay(600000);
                    var isSucess = await loadService.RunTaskAsync(stoppingToken);
                    if (isSucess)
                        break;
                }
            }

            var nextRunTime = CalculateNextRunTime();
            var delay = nextRunTime - DateTime.UtcNow;
            _logger.LogInformation($"Next loadin start at {nextRunTime}");
            if (delay > TimeSpan.Zero)
                await Task.Delay(delay, stoppingToken);
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

            if (nextRun > now)
                return nextRun;
        }

        return now.Date.AddDays(1).Add(runTimes[0]);
    }
}
