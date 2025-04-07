using Eve.Domain.Common;
using Eve.Domain.ExternalTypes;
using Moq;

namespace Eve.Tests.UnitTests.InfrastructureTests.ExternalServicesTests.EveApiCleintProvider;

public class FetchMarketHistoryForTypeIdAsyncTests
{
    private readonly EveApiClientProviderFactory _factory = new();
    private const int RegionId = 10000002;
    private const int TypeId = 34;

    [Fact]
    public async Task GetTypeHistoryInfo_ReturnsHistory_WhenSuccessStatusCode()
    {
        //Arrange
        var expectedHistory = new List<TypeMarketHistoryInfo>()
        {
            new TypeMarketHistoryInfo{
                Date = DateTime.UtcNow.ToShortDateString(),
                Volume = 1234,
                OrderCount = 5}
        };

        var provider = _factory.GetProviderWithSuccesStatusCode(expectedHistory);

        //act
        var result = await provider.FetchMarketHistoryForTypeIdAsync(TypeId, RegionId, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedHistory[0].Date, result.Value.First().Date);
        Assert.Equal(expectedHistory[0].Volume, result.Value.First().Volume);
    }

    [Fact]
    public async Task GetTypeHistoryInfo_ReturnsSuccess_WhenNotModifiedStatusCode()
    {
        //Arrange
        var provider = _factory.GetProviderWithNotModifiedStatusCode();

        //Act
        var result = await provider.FetchMarketHistoryForTypeIdAsync(TypeId, RegionId, CancellationToken.None);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCodes.NotModified, result.Error.ErrorCode);
    }

    [Fact]
    public async Task GetTypeHistoryInfo_ReturnsSuccess_WhenEmptyContent()
    {
        //Arrange
        var provider = _factory.GetProviderWithEmptyContent();

        //Act
        var result = await provider.FetchMarketHistoryForTypeIdAsync(TypeId, RegionId, CancellationToken.None);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCodes.NotModified, result.Error.ErrorCode);
    }

    [Fact]
    public async Task GetTypeHistoryInfo_ReturnsError_WhenNotSuccessStatusCode()
    {
        //Arrange
        var provider = _factory.GetProviderWithNotSuccessStatusCode();

        //Act
        var result = await provider.FetchMarketHistoryForTypeIdAsync(TypeId, RegionId, CancellationToken.None);

        //Assert
        Assert.True(result.IsFailure);
        _factory.RateLimitMock
            .Verify(l => l.RunTaskAsync(
                It.IsAny<Func<Task<HttpResponseMessage>>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()), 
                Times.Exactly(4));
    }

    [Fact]
    public async Task GetTypeHistoryInfo_ReturnsError_WhenHttpRequestException()
    {
        //Arrange
        var errrorMessage = "Error message";
        var provider = _factory.GetProviderWithSendAsyncThrowHttpRequestException(errrorMessage);

        //Act
        var result = await provider.FetchMarketHistoryForTypeIdAsync(TypeId, RegionId, CancellationToken.None);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Equal(errrorMessage, result.Error.Message);
        _factory.RateLimitMock
            .Verify(l => l.RunTaskAsync(
                It.IsAny<Func<Task<HttpResponseMessage>>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()),
                Times.Exactly(4));
    }

    [Fact]
    public async Task GetTypeHistoryInfo_ReturnsError_WhenAnyException()
    {
        //Arrange
        var errrorMessage = "Error message";
        var provider = _factory.GetProviderWithSendAsyncThrowAnyException(errrorMessage);

        //Act
        var result = await provider.FetchMarketHistoryForTypeIdAsync(TypeId, RegionId, CancellationToken.None);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Equal(errrorMessage, result.Error.Message);
        _factory.RateLimitMock
            .Verify(l => l.RunTaskAsync(
                It.IsAny<Func<Task<HttpResponseMessage>>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
