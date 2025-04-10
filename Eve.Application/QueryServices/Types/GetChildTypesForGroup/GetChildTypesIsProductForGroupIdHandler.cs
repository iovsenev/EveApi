using AutoMapper;
using Eve.Application.DTOs;
using Eve.Application.QueryServices.CommonRequests;
using Eve.Domain.Common;
using Eve.Domain.Constants;
using Eve.Domain.Interfaces.ApiServices;
using Eve.Domain.Interfaces.DataBaseAccess.Read;

namespace Eve.Application.QueryServices.Types.GetChildTypesForGroup;

public class GetChildTypesIsProductForGroupIdHandler : IRequestHandler<GetChildTypesProductResponse, GetCommonRequestForId>
{
    private readonly IReadTypeRepository _repos;
    private readonly IMapper _mapper;

    public GetChildTypesIsProductForGroupIdHandler(
        IReadTypeRepository repos,
        IMapper mapper)
    {
        _repos = repos;
        _mapper = mapper;
    }

    public async Task<Result<GetChildTypesProductResponse>> Handle(GetCommonRequestForId request, CancellationToken token)
    {
        var groupId = request.Id;

        var result = await _repos.GetTypeIsProductForGroupId(groupId, token);

        if (result.IsFailure) return result.Error;

        var types = result.Value
            .Select(t => _mapper.Map<ShortTypeDto>(t))
            .ToList();

        return new GetChildTypesProductResponse(types);
    }
}
