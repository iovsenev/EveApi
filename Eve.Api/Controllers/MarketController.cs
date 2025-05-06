using Eve.Api.Controllers.Common;
using Eve.Application.QueryServices;
using Eve.Application.QueryServices.CommonRequests;
using Eve.Application.QueryServices.Market.GetMarketGroups;
using Eve.Application.QueryServices.Market.GetMarketHistory;
using Eve.Application.QueryServices.Market.GetOrders;
using Eve.Application.QueryServices.Types.GetChildTypesForMarketGroup;
using Eve.Domain.Constants;
using Eve.Domain.Interfaces.CacheProviders;
using Microsoft.AspNetCore.Mvc;

namespace Eve.Api.Controllers;

public class MarketController : BaseController
{
    private readonly IQueryHandler _handler;
    private readonly IRedisProvider _redisProvider;

    public MarketController(IQueryHandler handler,
        IRedisProvider redisProvider)
    {
        _handler = handler;
        _redisProvider = redisProvider;
    }

    [HttpGet("groups")]
    public async Task<IActionResult> GetAll(
        CancellationToken token)
    {
        var result = await _handler.Send<GetMarketGroupsResponse>(new GetCommonEmptyRequest(), token);

        if (result.IsFailure) return StatusCode((int)result.Error.ErrorCode, result.Error);

        return Ok(result.Value);
    }

    [HttpGet("types/{groupId}")]
    public async Task<IActionResult> GetChildTypes(
        int groupId,
        CancellationToken token)
    {
        var key = $"{HttpContext.Request.Path.Value}";
        var typesResult = await _handler.Send<GetChildTypesResponse>(new GetCommonRequestForId(groupId), token);

        if (typesResult.IsFailure) return StatusCode((int)typesResult.Error.ErrorCode, typesResult.Error);

        return Ok(typesResult.Value);
    }

    [HttpGet("orders/{typeId}")]
    public async Task<IActionResult> GetOrdersByTypeId(
        int typeId,
        [FromQuery] int regionId = (int)CentralHubRegionId.Jita,
        CancellationToken token = default)
    {
        var key = $"{HttpContext.Request.Path.Value}:{regionId}";

        var result = await _handler.Send<GetOrdersResponse>(
                new GetOrdersRequest(
                    TypeId: typeId,
                    RegionId: regionId),
                token);

        if (result.IsFailure)
            return StatusCode((int)result.Error.ErrorCode, result.Error);

        return Ok(result.Value);
    }

    [HttpGet("history/{typeId}")]
    public async Task<IActionResult> GetHistoryByTypeId(
        int typeId,
        [FromQuery] int regionId = (int)CentralHubRegionId.Jita,
        CancellationToken token = default)
    {
        var key = $"{HttpContext.Request.Path.Value}:{regionId}";

        var result = await _handler.Send<GetMarketHistoryResponse>(
                new GetMarketHistoryRequest(
                    RegionId: regionId,
                    TypeId: typeId),
                token);

        if (result.IsFailure)
            return StatusCode((int)result.Error.ErrorCode, result.Error);

        return Ok(result.Value);
    }
}