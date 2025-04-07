using Eve.Domain.Common;
using Eve.Domain.ExternalTypes;
using System.Net;

namespace Eve.Tests.UnitTests.InfrastructureTests.ExternalServicesTests.EveApiCleintProvider;

public class FetchAllOrdersAsyncTests
{
    private readonly EveApiClientProviderFactory _factory = new();
    private const int RegionId = 10000002;

    [Fact]
    public async Task GetTypeOrdersInfo_ReturnsOrders_WhenApiResponseIsSuccessful()
    {
        // Arrange
        var expectedOrders = new List<TypeOrdersInfo>
        {
            new TypeOrdersInfo { TypeId = 34, Price = 5.5, VolumeRemain = 100 }
        };

        var provider = _factory.GetProviderWithSuccesStatusCode(expectedOrders);

        // Act
        var result = await provider.FetchAllOrdersAsync(RegionId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedOrders.Count, result.Value.Count);
        Assert.Equal(expectedOrders[0].TypeId, result.Value.First().TypeId);
    }

    [Fact]
    public async Task GetTypeOrdersInfo_ReturnsOrders_WhenApiResponseContainsManyPages()
    {
        //Arrange
        var expectedOrders = new List<TypeOrdersInfo>();
        for (int i = 1; i <= 100; i++)
        {
            expectedOrders.Add(new TypeOrdersInfo
            {
                OrderId = i,
                TypeId = i+10,
                Price = 5.5,
                VolumeRemain = 1 + i
            });
        }
        expectedOrders = expectedOrders.OrderBy(x => x.OrderId).ToList();

        var provider = _factory.GetProviderWithManyPages(expectedOrders);

        // Act
        var resultRequest = await provider.FetchAllOrdersAsync(RegionId, CancellationToken.None);

        // Assert
        Assert.True(resultRequest.IsSuccess);
        var result = resultRequest.Value.OrderBy(x => x.OrderId).ToList();
        Assert.Equal(expectedOrders.Count, result.Count);

        for (int i = 0; i < result.Count; i++)
        {
            Assert.Equal(expectedOrders[i].OrderId, result[i].OrderId);
            Assert.Equal(expectedOrders[i].Price, result[i].Price);
            Assert.Equal(expectedOrders[i].VolumeRemain, result[i].VolumeRemain);
        }
    }

    [Fact]
    public async Task GetTypeOrdersInfo_ReturnsSuccess_WhenNotModifiedStatusCode()
    {
        //Arrange
        var provider = _factory.GetProviderWithNotModifiedStatusCode();

        //Act
        var result = await provider.FetchAllOrdersAsync(RegionId, CancellationToken.None);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCodes.NotModified, result.Error.ErrorCode);
    }

    [Fact]
    public async Task GetTypeOrdersInfo_ReturnsSuccess_WhenEmptyResponseContent()
    {
        //Arrange
        var provider = _factory.GetProviderWithEmptyContent();

        //Act
        var result = await provider.FetchAllOrdersAsync(RegionId, CancellationToken.None);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCodes.NotModified, result.Error.ErrorCode);
    }

    [Fact]
    public async Task GetTypeOrdersInfo_ReturnsError_WhenNotSuccessStatusCode()
    {
        //Arrange
        var errorMessage = $"bad request with status code {HttpStatusCode.NotFound}";

        var provider = _factory.GetProviderWithNotSuccessStatusCode();

        //Act
        var result = await provider.FetchAllOrdersAsync(RegionId, CancellationToken.None);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Equal(errorMessage, result.Error.Message);
    }

    [Fact]
    public async Task GetTypeOrdersInfo_ReturnsError_WhenSendAsyncThrowHttpRequestException()
    {
        //Arrange
        var errorMessage = "throw exception";
        var provider = _factory.GetProviderWithSendAsyncThrowHttpRequestException(errorMessage);

        //Act
        var result = await provider.FetchAllOrdersAsync(RegionId, CancellationToken.None);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Equal(errorMessage, result.Error.Message);
    }


    [Fact]
    public async Task GetTypeOrdersInfo_ReturnsError_WhenSendAsyncAnyException()
    {
        //Arrange
        var errorMessage = "throw exception";
        var provider = _factory.GetProviderWithSendAsyncThrowAnyException(errorMessage);

        //Act
        var result = await provider.FetchAllOrdersAsync(RegionId, CancellationToken.None);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Equal(errorMessage, result.Error.Message);
    }
}
