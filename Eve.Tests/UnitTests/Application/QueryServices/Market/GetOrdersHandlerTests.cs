using AutoMapper;
using Eve.Application.DTOs;
using Eve.Application.QueryServices.Market.GetOrders;
using Eve.Application.QueryServices.Stations.GetStations;
using Eve.Domain.Common;
using Eve.Domain.ExternalTypes;
using Eve.Domain.Interfaces.CacheProviders;
using Eve.Domain.Interfaces.ExternalServices;
using Eve.Tests.UnitTests.Common;
using FluentAssertions;
using Moq;

namespace Eve.Tests.UnitTests.Application.QueryServices.Market;
public class GetOrdersHandlerTests
{
    private readonly Mock<IRedisProvider> _cacheProvider;
    private readonly Mock<IEveApiMarketProvider> _apiClient;
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<IService<StationNameDto>> _stationName;
    private readonly GetOrdersHandler _handler;

    public GetOrdersHandlerTests()
    {
        _apiClient = new();
        _stationName = new();
        _mapper = new();
        _cacheProvider = new();
        _handler = new(_cacheProvider.Object, _apiClient.Object, _mapper.Object, _stationName.Object);
    }

    [Fact]
    public async Task Handle_ReturnSuccess_WhenAllReturnsSuccess()
    {
        //arrange
        var request = new GetOrdersRequest(1, 1);
        _cacheProvider
            .Setup(c => c.GetOrSetAsync(
                            It.IsAny<string>(),
                            It.IsAny<Func<Task<Result<ICollection<TypeOrdersInfo>>>>>(),
                            It.IsAny<CancellationToken>()))
            .ReturnsAsync(DataStorage.GetOrders());

        _stationName
            .Setup(c => c.Handle(
                It.IsAny<long>(), 
                CancellationToken.None))
            .ReturnsAsync(new StationNameDto
                    {
                        Id = 1,
                        Name = "station",
                        SolarSystemID = 1
                    });

        _mapper
            .Setup(c => c.Map<TypeOrderDto>(It.IsAny<TypeOrdersInfo>()))
            .Returns((TypeOrdersInfo scorce) => new TypeOrderDto
            {
                IsBuyOrder = scorce.IsBuyOrder,
                Price = scorce.Price,
                OrderId = scorce.OrderId,
                LocationId = scorce.LocationId,
                SystemId = scorce.SystemId,
            });

        //act
        var result = await _handler.Handle(request, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        result.Value.SellOrders.Any().Should().BeTrue();
        result.Value.BuyOrders.Any().Should().BeTrue();
        result.Value.BuyOrders.Should().BeInAscendingOrder(c => c.Price);
        result.Value.SellOrders.Should().BeInAscendingOrder(c => c.Price);
        foreach (var order in result.Value.BuyOrders)
        {
            order.StationName.Should().Be("station");
        }
        foreach (var order in result.Value.SellOrders)
        {
            order.StationName.Should().Be("station");
        }
    }

    [Fact]
    public async Task Handle_ReturnSuccess_WhenCorrectCacheReturnStationNameReturnError()
    {
        //arrange
        var request = new GetOrdersRequest(1, 1);
        _cacheProvider
            .Setup(c => c.GetOrSetAsync(
                            It.IsAny<string>(),
                            It.IsAny<Func<Task<Result<ICollection<TypeOrdersInfo>>>>>(),
                            It.IsAny<CancellationToken>()))
            .ReturnsAsync(DataStorage.GetOrders());

        _stationName
            .Setup(c => c.Handle(
                It.IsAny<long>(),
                CancellationToken.None))
            .ReturnsAsync(Error.NotFound());

        _mapper
            .Setup(c => c.Map<TypeOrderDto>(It.IsAny<TypeOrdersInfo>()))
            .Returns((TypeOrdersInfo scorce) => new TypeOrderDto
            {
                IsBuyOrder = scorce.IsBuyOrder,
                Price = scorce.Price,
                OrderId = scorce.OrderId,
                LocationId = scorce.LocationId,
                SystemId = scorce.SystemId,
            });

        //act
        var result = await _handler.Handle(request, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        result.Value.SellOrders.Any().Should().BeTrue();
        result.Value.BuyOrders.Any().Should().BeTrue();
        result.Value.BuyOrders.Should().BeInAscendingOrder(c => c.Price);
        result.Value.SellOrders.Should().BeInAscendingOrder(c => c.Price);
        foreach (var order in result.Value.BuyOrders)
        {
            order.StationName.Should().Be("unknown");
        }
        foreach (var order in result.Value.SellOrders)
        {
            order.StationName.Should().Be("unknown");
        }
    }

    [Fact]
    public async Task Handle_ReturnError_WhenCacheReturnsError()
    {
        //arrange
        var request = new GetOrdersRequest(1, 1);
        _cacheProvider
            .Setup(c => c.GetOrSetAsync(
                            It.IsAny<string>(),
                            It.IsAny<Func<Task<Result<ICollection<TypeOrdersInfo>>>>>(),
                            It.IsAny<CancellationToken>()))
            .ReturnsAsync(Error.InternalServer());

        _stationName
            .Setup(c => c.Handle(
                It.IsAny<long>(),
                CancellationToken.None))
            .ReturnsAsync(Error.NotFound());

        _mapper
            .Setup(c => c.Map<TypeOrderDto>(It.IsAny<TypeOrdersInfo>()))
            .Returns((TypeOrdersInfo scorce) => new TypeOrderDto
            {
                IsBuyOrder = scorce.IsBuyOrder,
                Price = scorce.Price,
                OrderId = scorce.OrderId,
                LocationId = scorce.LocationId,
                SystemId = scorce.SystemId,
            });

        //act
        var result = await _handler.Handle(request, CancellationToken.None);

        //assert
        result.IsFailure.Should().BeTrue();
        result.Error.ErrorCode.Should().Be(ErrorCodes.InternalServer);
    }

    [Fact]
    public async Task Handle_ReturnSuccess_WhenCacheReturnsEmptyCollection()
    {
        //arrange
        var request = new GetOrdersRequest(1, 1);
        _cacheProvider
            .Setup(c => c.GetOrSetAsync(
                            It.IsAny<string>(),
                            It.IsAny<Func<Task<Result<ICollection<TypeOrdersInfo>>>>>(),
                            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TypeOrdersInfo>());

        _stationName
            .Setup(c => c.Handle(
                It.IsAny<long>(),
                CancellationToken.None))
            .ReturnsAsync(Error.NotFound());

        _mapper
            .Setup(c => c.Map<TypeOrderDto>(It.IsAny<TypeOrdersInfo>()))
            .Returns((TypeOrdersInfo scorce) => new TypeOrderDto
            {
                IsBuyOrder = scorce.IsBuyOrder,
                Price = scorce.Price,
                OrderId = scorce.OrderId,
                LocationId = scorce.LocationId,
                SystemId = scorce.SystemId,
            });

        //act
        var result = await _handler.Handle(request, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
    }
}
