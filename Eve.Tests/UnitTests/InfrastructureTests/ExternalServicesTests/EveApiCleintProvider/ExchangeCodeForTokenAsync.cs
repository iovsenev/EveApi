using Eve.Domain.Common;
using Eve.Domain.ExternalTypes;

namespace Eve.Tests.UnitTests.InfrastructureTests.ExternalServicesTests.EveApiCleintProvider;

public class ExchangeCodeForTokenAsync
{
    private readonly EveApiClientProviderFactory _factory = new();
    private TokenResponse Response = new TokenResponse
    {
        AccessToken = "AccessToken",
        ExpiresIn = 1234,
        RefreshToken = "ResponseToken",
        TokenType = "TokenType"
    };
    private string Code = "CodeforToken";

    [Fact]
    public async Task ExchangeCodeForTokenAsync_ReturnToken_WhenSuccess()
    {
        //Arrange
        var provider = _factory.GetProviderWithSuccesStatusCode(Response);

        //Act
        var result = await provider.ExchangeCodeForTokenAsync(Code, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(Response.AccessToken, result.Value.AccessToken);
        Assert.Equal(Response.RefreshToken, result.Value.RefreshToken);
        Assert.Equal(Response.TokenType, result.Value.TokenType);
        Assert.Equal(Response.ExpiresIn, result.Value.ExpiresIn);
    }

    [Fact]
    public async Task ExchangeCodeForTokenAsync_ReturnError_WhenNotSuccessStatusCode()
    {
        //Arrange
        var provider = _factory.GetProviderWithNotSuccessStatusCode();

        //Act
        var result = await provider.ExchangeCodeForTokenAsync(Code, CancellationToken.None);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCodes.InternalServer, result.Error.ErrorCode);
    }

    [Fact]
    public async Task ExchangeCodeForTokenAsync_ReturnError_WhenSendAsyncThrowException()
    {
        //Arrange
        var messsage = "errorMessage";
        var provider = _factory.GetProviderWithSendAsyncThrowAnyException(messsage);

        //Act
        var result = await provider.ExchangeCodeForTokenAsync(Code, CancellationToken.None);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCodes.InternalServer, result.Error.ErrorCode);
        Assert.Equal(messsage, result.Error.Message);
    }

    [Fact]
    public async Task ExchangeCodeForTokenAsync_ReturnError_WhenEmptyContentResponse()
    {
        //Arrange
        var provider = _factory.GetProviderWithEmptyContent();

        //Act
        var result = await provider.ExchangeCodeForTokenAsync(Code, CancellationToken.None);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCodes.InternalServer, result.Error.ErrorCode);
    }

    [Fact]
    public async Task ExchangeCodeForTokenAsync_ReturnError_WhenEmtyCode()
    {
        //Arrange
        var provider = _factory.GetProviderWithSuccesStatusCode(Response);

        //Act
        var result = await provider.ExchangeCodeForTokenAsync("", CancellationToken.None);

        //Assert
        Assert.True(result.IsFailure);
    }
}
