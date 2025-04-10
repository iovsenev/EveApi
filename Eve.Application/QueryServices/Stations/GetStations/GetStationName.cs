using AutoMapper;
using Eve.Application.DTOs;
using Eve.Domain.Common;
using Eve.Domain.Constants;
using Eve.Domain.Entities;
using Eve.Domain.Interfaces.CacheProviders;
using Eve.Domain.Interfaces.DataBaseAccess.Read;

namespace Eve.Application.QueryServices.Stations.GetStations;
public class GetStationName : IService<StationNameDto>
{
    private readonly IReadStationRepository _stationRepos;
    private readonly IRedisProvider _redis;
    private readonly IMapper _mapper;

    public GetStationName(
        IReadStationRepository stationRepos,
        IRedisProvider redis,
        IMapper mapper)
    {
        _stationRepos = stationRepos;
        _redis = redis;
        _mapper = mapper;
    }

    public async Task<Result<StationNameDto>> Handle(long id, CancellationToken token)
    {
        var key = $"{GlobalKeysCacheConstants.Stations}:{id}";

        var result = await _redis.GetOrSetAsync(
            key,
            () => _stationRepos.GetStationById(id, token),
            token);

        if (result.IsFailure)
            return result.Error;

        var stationDto = _mapper.Map<StationNameDto>(result.Value);

        return stationDto;
    }
}
