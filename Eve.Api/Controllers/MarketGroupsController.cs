using Eve.Api.Controllers.Common;
using Eve.Application.Services;
using Eve.Application.Services.MarketGroups.GetChildTypes;
using Eve.Application.Services.MarketGroups.GetMarketGroups;
using Eve.Application.Services.MarketGroups.GetMarketHistory;
using Eve.Application.Services.MarketGroups.GetOrders;
using Eve.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Eve.Api.Controllers;

public class MarketGroupsController : BaseController
{
    private readonly IQueryHandler _handler;
    public MarketGroupsController(IQueryHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("groups")]
    public async Task<IActionResult> GetAll(
        CancellationToken token)
    {
        var result = await _handler.Send<GetMarketGroupsResponse>(new GetMarketGroupsRequest(), token);

        if (result.IsFailure) return StatusCode((int)result.Error.ErrorCode, result.Error);

        return Ok(result.Value);
    }

    [HttpGet("types/{groupId}")]
    public async Task<IActionResult> GetChildTypes(
        int groupId,
        CancellationToken token)
    {
        var typesResult = await _handler.Send<GetChildTypesResponse>(new GetChildTypesRequest(groupId), token);

        if (typesResult.IsFailure) return StatusCode((int)typesResult.Error.ErrorCode, typesResult.Error);

        return Ok(typesResult.Value);
    }

    [HttpGet("orders/{typeId}")]
    public async Task<IActionResult> GetOrdersByTypeId(
        int typeId,
        [FromQuery] int regionId = (int)CentralHubRegionId.Jita,
        CancellationToken token = default)
    {
        var result = await _handler.Send<GetOrdersResponse>(
            new GetOrdersRequest(
                TypeId:typeId,
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