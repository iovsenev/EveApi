using Eve.Domain.Common;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Eve.Infrastructure.DataBase.Contexts;
using Eve.Infrastructure.DataBase.Repositories.Read;
using Eve.Tests.UnitTests.InfrastructureTests.RepositoriesTests.Common;
using FluentAssertions;

namespace Eve.Tests.UnitTests.InfrastructureTests.RepositoriesTests;

public class ReadTypeRepositoryTests : BaseReadRepositoryTest<IReadTypeReadRepository>
{
    protected override IReadTypeReadRepository CreateRepository(IAppDbContext context)
    {
        return new ReadTypeRepository(context);
    }

    #region GetTypeByNameContains
    [Fact]
    public async Task GetTypeByNameContains_ReturnsSuccess_WhenExistText()
    {
        //Arrange
        var testName = "t";

        //Act
        var result = await Repository.GetTypesByNameContains(testName, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeTrue(); 
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetTypeByNameContains_ReturnsEmpty_WhenNotExistText()
    {
        //Arrange
        var testName = "true";

        //Act
        var result = await Repository.GetTypesByNameContains(testName, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
    #endregion

    #region GetByIdAsync
    [Fact]
    public async Task GetByIdAsync_ReturnsType_WithExistId()
    {
        //Arrange
        var testId = 22;

        //Act
        var result = await Repository.GetByIdAsync(testId, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(testId);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsError_WithNotExistId()
    {
        //Arrange
        var testId = 222;

        //Act
        var result = await Repository.GetByIdAsync(testId, CancellationToken.None);

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.ErrorCode.Should().Be(ErrorCodes.NotFound);
    }
    #endregion

    #region GetReprocessMaterialsForTypeId
    [Fact]
    public async Task GetReprocessMaterialsForTypeId_ReturnsMaterials()
    {
        //Arrange
        var testId = 1;

        //Act
        var result = await Repository.GetReprocessMaterialsForTypeId(testId, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetReprocessMaterialsForTypeId_ReturnsEmpty()
    {
        //Arrange
        var testId = 2;

        //Act
        var result = await Repository.GetReprocessMaterialsForTypeId(testId, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
    #endregion

    #region GetTypeIsProductForGroupId
    [Fact]
    public async Task GetTypeIsProductForGroupId_ReturnSuccess_WithExistId()
    {
        var testId = 1; 

        var result = await Repository.GetTypeIsProductForGroupId(testId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GetTypeIsProductForGroupId_ReturnError_WithNotExistId()
    {
        var testId = 123;

        var result = await Repository.GetTypeIsProductForGroupId(testId, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }
    #endregion
}
