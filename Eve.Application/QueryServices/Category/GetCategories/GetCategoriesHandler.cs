using AutoMapper;
using Eve.Application.DTOs;
using Eve.Application.QueryServices.CommonRequests;
using Eve.Domain.Common;
using Eve.Domain.Interfaces.ApiServices;
using Eve.Domain.Interfaces.DataBaseAccess.Read;

namespace Eve.Application.QueryServices.Category.GetCategories;
public class GetCategoriesHandler : IRequestHandler<GetCategoriesResponse, GetCommonEmptyRequest>
{
    private readonly IReadCategoryRepository _repository;
    private readonly IMapper _mapper;

    public GetCategoriesHandler(
        IReadCategoryRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<GetCategoriesResponse>> Handle(GetCommonEmptyRequest request, CancellationToken token)
    {
        var result = await _repository.GetCategoryWithProduct(token);

        if (result.IsFailure)
            return result.Error;

        var categories = result.Value
            .Select(c => _mapper.Map<CategoryDto>(c))
            .ToList();

        return new GetCategoriesResponse(categories);
    }
}
