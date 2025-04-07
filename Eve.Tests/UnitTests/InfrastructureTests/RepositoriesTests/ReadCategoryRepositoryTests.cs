
using Eve.Domain.Common;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Eve.Infrastructure.DataBase.Contexts;
using Eve.Infrastructure.DataBase.Repositories.Read;
using Eve.Tests.UnitTests.InfrastructureTests.RepositoriesTests.Common;
using FluentAssertions;

namespace Eve.Tests.UnitTests.InfrastructureTests.RepositoriesTests;

public class ReadCategoryRepositoryTests : BaseReadRepositoryTest<IReadCategoryRepository>
{
    protected override IReadCategoryRepository CreateRepository(IAppDbContext context)
    {
        return new ReadCategoryRepository(context);
    }

    #region GetCategoryWithProduct
    [Fact]
    public async Task GetCategoryWithProduct_ReturnSuccess()
    {
        //Arrange
        //Act
        var result = await Repository.GetCategoryWithProduct(CancellationToken.None);
        //Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GetCategoryWithProduct_ReturnError()
    {
        //Arrange
        var repos = new CategoryRepositoryEmptyTest();
        await repos.InitializeAsync();
        //Act
        var result = await repos.Repository.GetCategoryWithProduct(CancellationToken.None);
        //Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.ErrorCode.Should().Be(ErrorCodes.NotFound);
    }
    #endregion
}
public class CategoryRepositoryEmptyTest : BaseReadRepositoryTest<IReadCategoryRepository>
{
    protected override IReadCategoryRepository CreateRepository(IAppDbContext context)
    {
        return new ReadCategoryRepository(context);
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
