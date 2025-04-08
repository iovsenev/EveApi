using Eve.Application.QueryServices.Market.GetMarketHistory;
using Eve.Domain.Common;
using Eve.Domain.ExternalTypes;
using Eve.Domain.Interfaces.CacheProviders;
using Eve.Domain.Interfaces.ExternalServices;
using Eve.Tests.UnitTests.Common;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;

namespace Eve.Tests.UnitTests.Application.QueryServices.Market;
public class GetMarketHistoryHandlerTests
{
    private readonly Mock<IRedisProvider> _cacheProvider;
    private readonly Mock<IEveApiOpenClientProvider> _apiClientProvider;
    private readonly GetMarketHistoryHandler _handler;

    public GetMarketHistoryHandlerTests()
    {
        _cacheProvider = new Mock<IRedisProvider>();
        _apiClientProvider = new Mock<IEveApiOpenClientProvider>();
        _handler = new(_cacheProvider.Object, _apiClientProvider.Object);
    }

    [Fact]
    public async Task Handle_ReturnSuccess()
    {
        //arrange
        var history = DataStorage.GetHistory();
        var request = new GetMarketHistoryRequest(1, 1);

        _cacheProvider
            .Setup(c => c.GetOrSetAsync(
                        It.IsAny<string>(),
                        It.IsAny<Func<Task<Result<ICollection<TypeMarketHistoryInfo>>>>>(),
                        It.IsAny< DistributedCacheEntryOptions >(),
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync(history);
        
        //act

        var result = await _handler.Handle(request, CancellationToken.None);
        //assert

        result.IsSuccess.Should().BeTrue();
        result.Value.History.Should().BeInDescendingOrder(h => h.Date);
    }

    [Fact]
    public async Task Handle_ReturnError()
    {
        //arrange
        var history = DataStorage.GetHistory();
        var request = new GetMarketHistoryRequest(1, 1);

        _cacheProvider
            .Setup(c => c.GetOrSetAsync(
                        It.IsAny<string>(),
                        It.IsAny<Func<Task<Result<ICollection<TypeMarketHistoryInfo>>>>>(),
                        It.IsAny<DistributedCacheEntryOptions>(),
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync(Error.InternalServer());

        //act

        var result = await _handler.Handle(request, CancellationToken.None);
        //assert

        result.IsFailure.Should().BeTrue();
        result.Error.ErrorCode.Should().Be(ErrorCodes.InternalServer);
    }
}
