using AutoMapper;
using Eve.Application.DTOs;
using Eve.Application.QueryServices.Market.GetMarketGroups;
using Eve.Domain.Common;
using Eve.Domain.Entities;
using Eve.Domain.Interfaces.CacheProviders;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Eve.Tests.UnitTests.Common;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eve.Tests.UnitTests.Application.QueryServices.Market;
public class GetMarketGroupsHandlerTests
{
    private readonly Mock<IReadMarketGroupRepository> _repos;
    private readonly Mock<IRedisProvider> _redisCache;
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<ILogger<GetMarketGroupsHandler>> _logger;
    private readonly GetMarketGroupsHandler _handler;

    public GetMarketGroupsHandlerTests()
    {
        _repos = new();
        _redisCache = new();
        _mapper = new();
        _logger = new();
        _handler = new(_repos.Object, _redisCache.Object, _mapper.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_ReturnSuccess()
    {
        //arrange
        var marketGroups = DataStorage.GetMarketGroups();

        _redisCache
            .Setup(c => c.GetOrSetAsync(
                        It.IsAny<string>(),
                        It.IsAny<Func<Task<Result<ICollection<MarketGroupEntity>>>>>(),
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync(marketGroups);

        _mapper
            .Setup(c => c.Map<MarketGroupDto>(It.IsAny<MarketGroupEntity>()))
            .Returns((MarketGroupEntity scorce) => new MarketGroupDto
            {
                Id = scorce.Id,
                Name = scorce.Name,
            });
        //act
        var result = await _handler.Handle(new(), CancellationToken.None);
        //assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ReturnError()
    {
        //arrange
        var marketGroups = DataStorage.GetMarketGroups();

        _redisCache
            .Setup(c => c.GetOrSetAsync(
                        It.IsAny<string>(),
                        It.IsAny<Func<Task<Result<ICollection<MarketGroupEntity>>>>>(),
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync(Error.NotFound());

        _mapper
            .Setup(c => c.Map<MarketGroupDto>(It.IsAny<MarketGroupEntity>()))
            .Returns((MarketGroupEntity scorce) => new MarketGroupDto
            {
                Id = scorce.Id,
                Name = scorce.Name,
            });
        //act
        var result = await _handler.Handle(new(), CancellationToken.None);
        //assert
        result.IsSuccess.Should().BeFalse();
    }
}
