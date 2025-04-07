using Eve.Domain.Common;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Eve.Infrastructure.DataBase.Contexts;
using Eve.Infrastructure.DataBase.Repositories.Read;
using Eve.Tests.UnitTests.InfrastructureTests.RepositoriesTests.Common;
using FluentAssertions;

namespace Eve.Tests.UnitTests.InfrastructureTests.RepositoriesTests;

public class ReadMarketGroupRepositoryTests : BaseReadRepositoryTest<IReadMarketGroupRepository>
{
    protected override IReadMarketGroupRepository CreateRepository(IAppDbContext context)
    {
        return new ReadMarketGroupRepository(context);
    }

    #region GetChildTypesAsync
    [Fact]
    public async Task GetChildTypesAsync_ReturnsTypes_WhenTypesExist()
    {
        // Arrange
        var testId = 17;

        // Act
        var result = await Repository.GetChildTypesAsync(testId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Any().Should().BeTrue();
        foreach (var item in result.Value)
        {
            item.Published.Should().BeTrue();
            item.MarketGroupId.Should().Be(testId);
        }
    }

    [Fact]
    public async Task GetChildTypesAsync_ReturnsError_WhenEmptyCollection()
    {
        //Arrange
        var marketGroup = 1;

        //Act
        var result = await Repository.GetChildTypesAsync(marketGroup, CancellationToken.None);

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.ErrorCode.Should().Be(ErrorCodes.NotFound);
    }
    #endregion

    #region GetAllAsync
    [Fact]
    public async Task GetAllAsync_ReturnsGroups_WhenMarketGroupsExist()
    {
        // Arrange

        // Act
        var result = await Repository.GetAllAsync( CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Any().Should().BeTrue();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsError_WhenEmptyCollection()
    {
        //Arrange
        var reposit = new ReadMarketGroupRepositoryEmptyTest();
        await reposit.InitializeAsync();

        //Act
        var result = await reposit.Repository.GetAllAsync( CancellationToken.None);

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.ErrorCode.Should().Be(ErrorCodes.NotFound);
    }
    #endregion

    #region GetByIdAsync
    [Fact]
    public async Task GetByIdAsync_ReturnSuccess_WithExistId()
    {
        //Arrange
        var marketGroup = 1;    

        //Act 
        var result = await Repository.GetByIdAsync(marketGroup,CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(marketGroup);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnError_WithNotExistId()
    {
        //Arrange
        var marketGroup = 123;

        //Act
        var result = await Repository.GetByIdAsync(marketGroup, CancellationToken.None);

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.ErrorCode.Should().Be(ErrorCodes.NotFound);
    }
    #endregion
}

public class ReadMarketGroupRepositoryEmptyTest : BaseReadRepositoryTest<IReadMarketGroupRepository>
{
    protected override IReadMarketGroupRepository CreateRepository(IAppDbContext context)
    {
        return new ReadMarketGroupRepository(context);
    }

    protected override async Task SeedCommonDataAsync()
    {

        await Context.Types.AddRangeAsync([]);
        await Context.ReprocessMaterials.AddRangeAsync([]);
        await Context.Categories.AddRangeAsync([]);
        await Context.Groups.AddRangeAsync([]);
        await Context.MarketGroups.AddRangeAsync([]);
        await Context.ProductMaterials.AddRangeAsync([]);

        await Context.SaveChangesAsync();
    }
}