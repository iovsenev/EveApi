using Eve.Api.Controllers.Common;
using Eve.Application.Services;
using Eve.Application.Services.Types.GetTypeForId;
using Eve.Application.Services.Types.GetTypesSearch;
using Microsoft.AspNetCore.Mvc;

namespace Eve.Api.Controllers;

public class TypeController: BaseController
{
    private readonly IQueryHandler _handler;
    public TypeController(IQueryHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("{typeId}")]
    public async Task<IActionResult> GetType(
        int typeId,
        CancellationToken token = default)
    {
        var result = await _handler.Send<GetTypeForIdResponse>(new GetTypeForIdRequest( typeId), token);

        if (result.IsFailure) 
            return StatusCode((int)result.Error.ErrorCode, result.Error);

        return Ok(result.Value);
    }

    [HttpGet("search/{search}")]
    public async Task<IActionResult> GetTypesForSearch(
        string search,
        CancellationToken token)
    {
        var result = await _handler.Send<GetTypesSearchResponse>( new GetTypesSearchRequest( search), token);

        if (result.IsFailure)
            return StatusCode((int)result.Error.ErrorCode, result.Error);

        return Ok(result.Value);
    }
}