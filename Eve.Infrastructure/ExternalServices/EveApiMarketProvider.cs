using Eve.Domain.Common;
using Eve.Domain.Constants;
using Eve.Domain.ExternalTypes;
using Eve.Domain.Interfaces.ExternalServices;
using Eve.Infrastructure.ExternalServices.Base;
using Eve.Infrastructure.ExternalServices.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Eve.Infrastructure.ExternalServices;
public class EveApiMarketProvider : EveApiBaseClient, IEveApiMarketProvider
{
    public EveApiMarketProvider(
        HttpClient httpClient,
        IEveGlobalRateLimit globalRateLimit,
        IEveRetryPolicyProvider retryPolicy,
        ILogger<EveApiMarketProvider> logger,
        IConfiguration config)
        : base(httpClient, globalRateLimit, retryPolicy, logger, config)
    {


    }

    public async Task<Result<List<TypeOrdersInfo>>> GetOrdersForRegionAsync(
        int regionId,
        CancellationToken token,
        int? typeId = null,
        OrderType orderType = OrderType.All,
        int maxPages = 100)
    {
        var orderTypeStr = orderType switch
        {
            OrderType.Buy => "buy",
            OrderType.Sell => "sell",
            _ => "all"
        };

        var baseUrl = $"markets/{regionId}/orders/?datasource=tranquility&order_type={orderTypeStr}";

        if (typeId.HasValue)
        {
            baseUrl += $"&type_id={typeId.Value}";

            return await FetchPaginatedDataAsync<TypeOrdersInfo>(
            baseUrl,
            token,
            maxPages: maxPages,
            progressCallback: (current, total) =>
            _logger.LogInformation($"Loading market orders page {current}/{total}"));
        }

        return await FetchPaginatedDataParallelAsync<TypeOrdersInfo>(
            baseUrl,
            token,
            maxDegreeOfParallelism: MaxDegreeOfParallelism);
    }

    public async Task<Result<List<TypeMarketHistoryInfo>>> GetMarketHistoryAsync(
        int regionId,
        int typeId,
        CancellationToken token)
    {
        var url = $"markets/{regionId}/history/?datasource=tranquility&type_id={typeId}";

        return await SendRequestAsync<List<TypeMarketHistoryInfo>>(
            new HttpRequestMessage(HttpMethod.Get, url),
            token);
    }
}
