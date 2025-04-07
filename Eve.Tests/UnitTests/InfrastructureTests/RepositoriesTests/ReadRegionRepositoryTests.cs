using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Eve.Infrastructure.DataBase.Contexts;
using Eve.Infrastructure.DataBase.Repositories.Read;
using Eve.Tests.UnitTests.InfrastructureTests.RepositoriesTests.Common;
using FluentAssertions;

namespace Eve.Tests.UnitTests.InfrastructureTests.RepositoriesTests;
public class ReadRegionRepositoryTests : BaseReadRepositoryTest<IReadRegionRepository>
{
    protected override IReadRegionRepository CreateRepository(IAppDbContext context)
    {
        return new ReadRegionRepository(context);
    }

    #region GetAllIdRegionsIDs
    [Fact]
    public async Task GetAllIdRegionsIDs_ReturnSuccess()
    {
        //arrange

        //act
        var result = await Repository.GetAllIdRegionsIDs(CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GetAllIdRegionsIDs_ReturnError()
    {
        //arrange
        var repos = new ReadRegionRepositoryEmptyTests();
        await repos.InitializeAsync();
        //act
        var result = await repos.Repository.GetAllIdRegionsIDs(CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
    }
    #endregion
}

internal class ReadRegionRepositoryEmptyTests : BaseReadRepositoryTest<IReadRegionRepository>
{
    protected override IReadRegionRepository CreateRepository(IAppDbContext context)
    {
        return new ReadRegionRepository(context);
    }

    protected override async Task SeedCommonDataAsync()
    {

        await Context.Types.AddRangeAsync([]);
        await Context.ReprocessMaterials.AddRangeAsync([]);
        await Context.Categories.AddRangeAsync([]);
        await Context.Groups.AddRangeAsync([]);
        await Context.MarketGroups.AddRangeAsync([]);
        await Context.ProductMaterials.AddRangeAsync([]);
        await Context.Regions.AddRangeAsync([]);

        await Context.SaveChangesAsync();
    }
}
