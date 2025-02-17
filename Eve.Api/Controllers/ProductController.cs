using Eve.Api.Controllers.Common;
using Eve.Application.QueryServices;
using Eve.Application.QueryServices.Products;
using Eve.Domain.Interfaces.CacheProviders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Eve.Api.Controllers;

public class ProductController : BaseController
{
    private readonly IQueryHandler _handler;
    private readonly IRedisProvider _redisProvider;

    public ProductController(IQueryHandler handler,
        IRedisProvider redisProvider)
    {
        _handler = handler;
        _redisProvider = redisProvider;
    }

    [HttpGet("{typeId}")]
    public async Task<IActionResult> GetProduct (
        int typeId,
        float blueprintEff ,
        float structEff ,
        CancellationToken token = default)
    {
        var key = $"{HttpContext.Request.Path.Value}:{blueprintEff}:{structEff}";
        var request = new GetProductRequest(TypeId: typeId, BlueprintEff: blueprintEff, StructEff:structEff);


        var result = await _redisProvider.GetOrSetAsync(
            key,
            () => _handler.Send<GetProductResponse>(request, token),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddHours(4)
            },
            token);

        if (result.IsFailure) return StatusCode((int)result.Error.ErrorCode, result.Error);

        return Ok(result.Value);
    }
}
