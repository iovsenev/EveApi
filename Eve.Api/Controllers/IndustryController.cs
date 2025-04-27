using Eve.Api.Controllers.Common;
using Eve.Application.QueryServices;
using Eve.Application.QueryServices.Category.GetCategories;
using Eve.Application.QueryServices.Category.GetGroupsForCategoryId;
using Eve.Application.QueryServices.CommonRequests;
using Eve.Application.QueryServices.Types.GetChildTypesForGroup;
using Eve.Domain.Interfaces.CacheProviders;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Microsoft.AspNetCore.Mvc;

namespace Eve.Api.Controllers;

public class IndustryController : BaseController
{
    private readonly IReadCategoryRepository _categoryRepository;
    private readonly IRedisProvider _redisProvider;
    private readonly IQueryHandler _handler;

    public IndustryController(
        IReadCategoryRepository categoryRepository,
        IRedisProvider redisProvider,
        IQueryHandler handler)
    {
        _categoryRepository = categoryRepository;
        _redisProvider = redisProvider;
        _handler = handler;
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategoriesForProducts(CancellationToken token)
    {
        var key = $"{HttpContext.Request.Path.Value}";

        //var result = await _redisProvider.GetOrSetAsync(
        //    key,
        //    () => _handler.Send<GetCategoriesResponse>(new GetCommonEmptyRequest(), token),
        //    token);

        var result = await _handler.Send<GetCategoriesResponse>(new GetCommonEmptyRequest(), token);

        if (result.IsFailure)
            return StatusCode((int)result.Error.ErrorCode, result.Error);

        return Ok(result.Value);
    }

    [HttpGet("groups/{categoryId}")]
    public async Task<IActionResult> GetGroupsForCatIdWithProducts(int categoryId, CancellationToken token)
    {
        var key = $"{HttpContext.Request.Path.Value}";

        var result = await _redisProvider.GetOrSetAsync(
            key,
            () => _handler.Send<GetGroupsForCategoryIdResponse>(new GetCommonRequestForId(categoryId), token),
            token);

        if (result.IsFailure)
            return StatusCode((int)result.Error.ErrorCode, result.Error);

        return Ok(result.Value);
    }

    [HttpGet("types/{groupId}")]
    public async Task<IActionResult> GetTypesIsProductForGroupId(int groupId, CancellationToken token)
    {
        var key = $"{HttpContext.Request.Path.Value}";
        //var response = await _redisProvider.GetAsync<GetChildTypesProductResponse>(key, token);

        //if (response is not null)
        //    return Ok(response);

        var result = await _handler.Send<GetChildTypesProductResponse>(new GetCommonRequestForId(groupId), token);

        if (result.IsFailure)
            return StatusCode((int)result.Error.ErrorCode, result.Error);

        await _redisProvider.SetAsync(key, result.Value, token);

        return Ok(result.Value);
    }
}
