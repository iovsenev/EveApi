using AutoMapper;
using Eve.Application.DTOs;
using Eve.Application.QueryServices.CommonRequests;
using Eve.Domain.Common;
using Eve.Domain.Interfaces.ApiServices;
using Eve.Domain.Interfaces.DataBaseAccess.Read;

namespace Eve.Application.QueryServices.Category.GetGroupsForCategoryId;
public class GetGroupsForCategoryIdHandler : IRequestHandler<GetGroupsForCategoryIdResponse, GetCommonRequestForId>
{
    private readonly IReadCategoryRepository _repository;
    private readonly IMapper _mapper;

    public GetGroupsForCategoryIdHandler(
        IReadCategoryRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<GetGroupsForCategoryIdResponse>> Handle(GetCommonRequestForId request, CancellationToken token)
    {
        var result = await _repository.GetGroupsForCategoryIdWithProducts(request.Id, token);

        if (result.IsFailure)
            return result.Error;

        var groups = result.Value
            .Select(g => _mapper.Map<GroupDto>(g))
            .ToList();

        return new GetGroupsForCategoryIdResponse(groups);
    }
}
