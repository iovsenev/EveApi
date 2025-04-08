using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Eve.Infrastructure.DataBase.Contexts;
using Eve.Infrastructure.DataBase.Repositories.Read;
using Eve.Tests.UnitTests.Common;
using FluentAssertions;

namespace Eve.Tests.UnitTests.InfrastructureTests.RepositoriesTests;

public class ReadStationRepositoryTests : BaseReadRepositoryTest<IReadStationRepository>
{
    protected override IReadStationRepository CreateRepository(IAppDbContext context)
    {
        return new ReadStationRepository(context);
    }

    #region GetStationById
    [Fact]
    public async Task GetStationById_ReturnSuccess()
    {
        //arrange
        var testId = 1;

        //act
        var result = await Repository.GetStationById(testId, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GGetStationById_ReturnError()
    {
        //arrange
        var testId = 12314;

        //act
        var result = await Repository.GetStationById(testId, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
    }
    #endregion
}
