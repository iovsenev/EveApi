using Eve.Api.Controllers.Common;
using Eve.Application.Services;
using Eve.Application.Services.Types.GetChildTypesForGroupId;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Microsoft.AspNetCore.Mvc;

namespace Eve.Api.Controllers;

public class IndustryController : BaseController
{
    private readonly IReadCategoryRepository _categoryRepository;
    private readonly IQueryHandler _handler;

    public IndustryController(
        IReadCategoryRepository categoryRepository, 
        IQueryHandler handler)
    {
        _categoryRepository = categoryRepository;
        _handler = handler;
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategoriesForProducts(CancellationToken token)
    {
        var categories = await _categoryRepository.GetCategoryForPruduct(token);

        return Ok(categories);
    }

    [HttpGet("groups/{categoryId}")]
    public async Task<IActionResult> GetGroupsForCatIdWithProducts(int categoryId, CancellationToken token)
    {
        var groups = await _categoryRepository.GetGroupsForCategoryIdWithProducts(categoryId, token);

        return Ok(groups);
    }

    [HttpGet("types/{groupId}")]
    public async Task<IActionResult> GetTypesIsProductForGroupId(int groupId, CancellationToken token)
    {
        var types = await _handler.Send<GetChildTypesProductResponse>(new GetChildTypesRequest(groupId), token);

        return Ok(types);
    }
}
