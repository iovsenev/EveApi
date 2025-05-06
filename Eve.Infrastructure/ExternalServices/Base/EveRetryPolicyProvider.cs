using Eve.Domain.Common;
using Eve.Infrastructure.ExternalServices.Interfaces;
using Microsoft.Extensions.Logging;
using Polly.Retry;
using Polly;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Eve.Infrastructure.ExternalServices.Base;
public class EveRetryPolicyProvider : IEveRetryPolicyProvider
{
    private readonly ILogger<EveRetryPolicyProvider> _logger;
    private readonly RetryPolicySettings _settings;

    public EveRetryPolicyProvider(
        ILogger<EveRetryPolicyProvider> logger,
        IConfiguration config)
    {
        _logger = logger;
        _settings = config.GetSection("RetryPolicy").Get<RetryPolicySettings>() 
            ?? new RetryPolicySettings();
    }

    public AsyncRetryPolicy<Result<T>> GetEntityRetryPolicy<T>()
    {
        return Policy<Result<T>>
            .Handle<HttpRequestException>()
            .OrResult(r => r.IsFailure)
            .WaitAndRetryAsync(
            _settings.MaxRetries,
            attempt => TimeSpan.FromMilliseconds(_settings.BaseDelayMs*Math.Pow(2,attempt-1)),
            onRetry: (outcome, delay, attempt, _) =>
            {
                LogRetryAttempt(outcome, delay, attempt);
            });
    }

    public AsyncRetryPolicy<Result<List<T>>> GetPaginatedRetryPolicy<T>()
    {
        return Policy<Result<List<T>>>
            .Handle<HttpRequestException>()
            .OrResult(r => r.IsFailure)
            .WaitAndRetryAsync(
            _settings.MaxRetries,
            attempt => TimeSpan.FromMicroseconds(_settings.BaseDelayMs*Math.Pow(2,attempt-1)),
            onRetry: (outcome,delay, attempt, _) =>
            {
                LogRetryAttempt(outcome,delay, attempt);
            });
    }


    public AsyncRetryPolicy<HttpResponseMessage> GetHttpRetryPolicy()
    {
        return Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(r => (int)r.StatusCode >= 500 || r.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
            _settings.MaxRetries,
            attempt => TimeSpan.FromMilliseconds(
                _settings.BaseDelayMs * Math.Pow(2, attempt - 1)));
    }

    private void LogRetryAttempt<T>(DelegateResult<Result<T>> outcome, TimeSpan delay, int attempt)
    {
        var error = outcome.Exception?.Message ?? (outcome.Result as Result)?.Error.ToString();
        _logger.LogWarning(
            "Retry attempt {attempt}/{MaxRetries} after {Delay}ms. Error: {Error}",
            attempt, _settings.MaxRetries, delay.TotalMicroseconds, error);
    }

    private static bool ShouldRetry(Error error)
    {
        return true;
    }
}

public class RetryPolicySettings
{
    public int MaxRetries { get; set; } = 3;
    public int BaseDelayMs { get; set; } = 1000;
}
