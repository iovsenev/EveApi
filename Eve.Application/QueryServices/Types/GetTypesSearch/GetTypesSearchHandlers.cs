﻿using AutoMapper;
using Eve.Application.DTOs;
using Eve.Domain.Common;
using Eve.Domain.Interfaces.ApiServices;
using Eve.Domain.Interfaces.CacheProviders;
using Eve.Domain.Interfaces.DataBaseAccess.Read;

namespace Eve.Application.QueryServices.Types.GetTypesSearch;
public class GetTypesSearchHandler : IRequestHandler<GetTypesSearchResponse, GetTypesSearchRequest>
{
    private readonly IReadTypeRepository _repository;
    private readonly IMapper _mapper;

    public GetTypesSearchHandler(
        IReadTypeRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<GetTypesSearchResponse>> Handle(GetTypesSearchRequest request, CancellationToken token)
    {
        var query = request.Query;
        var result = await _repository.GetTypesByNameContains(query, token);

        if (result.IsFailure) return result.Error;

        var types = result.Value
            .Select(t => _mapper.Map<ShortTypeDto>(t))
            .OrderBy(t => t.Name)
            .Take(8)
            .ToList();

        return new GetTypesSearchResponse(types);
    }
}
