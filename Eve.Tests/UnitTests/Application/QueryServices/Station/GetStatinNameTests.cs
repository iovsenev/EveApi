using AutoMapper;
using Eve.Application.DTOs;
using Eve.Application.QueryServices.Stations.GetStations;
using Eve.Domain.Common;
using Eve.Domain.Entities;
using Eve.Domain.Interfaces.CacheProviders;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using FluentAssertions;
using Moq;

namespace Eve.Tests.UnitTests.Application.QueryServices.Station;
public class GetStatinNameTests
{

    private readonly Mock<IReadStationRepository> _stationRepos;
    private readonly Mock<IRedisProvider> _redis;
    private readonly Mock<IMapper> _mapper;
    private readonly GetStationName _service;
    private static StationEntity _station = new ()
    {
        Id = 1,
        Name = "Test",
        SolarSystemID = 1,
    };

    public GetStatinNameTests()
    {
        _stationRepos = new();
        _redis = new();
        _mapper = new();
        _service = new(_stationRepos.Object, _redis.Object, _mapper.Object);
    }

    [Fact]
    public async Task Handle_ReturnSucces_WhenAllReturnsSuccess()
    {
        //arrange
        _redis
            .Setup(c => c.GetOrSetAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<Result<StationEntity>>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_station);

        _mapper
            .Setup(c => c.Map<StationNameDto>(It.IsAny<StationEntity>()))
            .Returns((StationEntity source) => new StationNameDto
            {
                Name = source.Name,
                Id = source.Id,
                SolarSystemID = source.SolarSystemID,
            });

        //act
        var result = await _service.Handle(1,CancellationToken.None);
        //assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(_station.Name);
    }

    [Fact]
    public async Task Handle_ReturnError_WhenCacheReturnsError()
    {
        //arrange
        _redis
            .Setup(c => c.GetOrSetAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<Result<StationEntity>>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Error.NotFound());

        _mapper
            .Setup(c => c.Map<StationNameDto>(It.IsAny<StationEntity>()))
            .Returns((StationEntity source) => new StationNameDto
            {
                Name = source.Name,
                Id = source.Id,
                SolarSystemID = source.SolarSystemID,
            });

        //act
        var result = await _service.Handle(1, CancellationToken.None);
        //assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.ErrorCode.Should().Be(ErrorCodes.NotFound);
    }
}
