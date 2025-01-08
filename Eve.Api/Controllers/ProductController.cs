using Eve.Api.Controllers.Common;
using Eve.Application.Services;
using Eve.Application.Services.Products;
using Microsoft.AspNetCore.Mvc;

namespace Eve.Api.Controllers;

public class ProductController : BaseController
{
    private readonly IQueryHandler _handler;
    public ProductController(IQueryHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("{typeId}")]
    public async Task<IActionResult> GetProduct (
        int typeId,
        float blueprintEff ,
        float structEff ,
        CancellationToken token = default)
    {
        var request = new GetProductRequest(TypeId: typeId, BlueprintEff: blueprintEff, StructEff:structEff);
        var result = await _handler.Send<GetProductResponse>(request, token);

        if (result.IsFailure) return StatusCode((int)result.Error.ErrorCode, result.Error);

        return Ok(result.Value);
    }
}
