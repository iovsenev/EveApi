using Eve.Infrastructure.ExternalServices.Base;
using System.Diagnostics;

namespace Eve.Tests.UnitTests.InfrastructureTests.ExternalServicesTests;
public class EveGlobalRateLimitTests
{
    private readonly EveGlobalRateLimit _rateLimiter = new EveGlobalRateLimit();

    [Fact]
    public async Task UserRunTaskAsync_respectsRateLimit()
    {
        var tasks = new List<Task>();
        var stopwatch = Stopwatch.StartNew();
        int executedTasks = 0;

        for (int i = 0; i < 15; i++)
        {
            tasks.Add(_rateLimiter.RunTaskAsync(async () =>
            {
                Interlocked.Increment(ref executedTasks);
                await Task.Delay(10);
                return new HttpResponseMessage();
            }, false, CancellationToken.None));
        }

        await Task.WhenAll(tasks);
        stopwatch.Stop();

        Assert.Equal(15, executedTasks);
        Assert.True(stopwatch.Elapsed.TotalSeconds > 1, $"ограничение в 10 запросов в секунду : {stopwatch.Elapsed.TotalSeconds}");
    }

    [Fact]
    public async Task ServiceRunTaskAsync_respectsRateLimit()
    {
        var tasks = new List<Task>();
        var stopwatch = Stopwatch.StartNew();
        int executedTasks = 0;

        for (int i = 0; i < 25; i++)
        {
            tasks.Add(_rateLimiter.RunTaskAsync( async () =>
            {
                Interlocked.Increment(ref executedTasks);
                await Task.Delay(10);
                return new HttpResponseMessage();
            }, true, CancellationToken.None));
        }

        await Task.WhenAll(tasks);
        stopwatch.Stop();

        Assert.Equal(25, executedTasks);
        Assert.True(stopwatch.Elapsed.TotalSeconds > 1, $"ограничение в 20 запросов в секунду : {stopwatch.Elapsed.TotalSeconds}");
    }

    [Fact]
    public async Task RunTaskAsync_HandlesCancellation()
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(100); // Отменяем через 100 мс

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _rateLimiter.RunTaskAsync( async () =>
            {
                await Task.Delay(500, cts.Token);
                return new HttpResponseMessage();
            }, false, cts.Token);
        });
    }

}
