using AutoMapper;
using Eve.Application.DTOs;
using Eve.Domain.Common;
using Eve.Domain.Constants;
using Eve.Domain.Entities;
using Eve.Domain.Interfaces.CacheProviders;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Microsoft.Extensions.Logging;

namespace Eve.Application.Services.Stations.GetStations;
public class GetStationName : IService<StationNameDto>
{
    private readonly IStationRepository _stationRepos;
    private readonly IRedisProvider _redis;
    private ILogger<GetStationName> _logger;
    private readonly IMapper _mapper;

    public GetStationName(
        IStationRepository stationRepos,
        IRedisProvider redis,
        ILogger<GetStationName> logger,
        IMapper mapper)
    {
        _stationRepos = stationRepos;
        _redis = redis;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<Result<StationNameDto>> Handle(long id, CancellationToken token)
    {
        var key = $"{GlobalKeysCacheConstants.Stations}:{id}";

        var station = await _redis.GetAsync<StationEntity>(key, token);

        StationNameDto stationDto;

        if (station is not null)
        {
            stationDto = _mapper.Map<StationNameDto>(station);
            return stationDto;
        }

        var entityResult = await _stationRepos.GetStationById(id, token);

        if (entityResult.IsFailure)
            return entityResult.Error;

        await _redis.SetAsync(key, entityResult.Value, token);

        stationDto = _mapper.Map<StationNameDto>(entityResult.Value);

        return stationDto;
    }
}
