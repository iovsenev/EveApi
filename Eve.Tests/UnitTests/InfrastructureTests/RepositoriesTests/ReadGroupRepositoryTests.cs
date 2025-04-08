
using Eve.Domain.Common;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Eve.Infrastructure.DataBase.Contexts;
using Eve.Infrastructure.DataBase.Repositories.Read;
using Eve.Tests.UnitTests.Common;
using FluentAssertions;

namespace Eve.Tests.UnitTests.InfrastructureTests.RepositoriesTests;

public class ReadGroupRepositoryTests : BaseReadRepositoryTest<IReadGroupRepository>
{
    protected override IReadGroupRepository CreateRepository(IAppDbContext context)
    {
        return new ReadGroupRepository(context);
    }

    #region GetGroupsForCategoryIdWithProducts
    [Fact]
    public async Task GetGroupsForCategoryIdWithProducts_ReturnSuccess()
    {
        //Arrange
        var testId = 1;
        //Act
        var result = await Repository.GetGroupsForCategoryIdWithProducts(testId,CancellationToken.None);
        //Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GetCategoryWithProduct_ReturnError()
    {
        //Arrange
        var testId = 132;
        //Act
        var result = await Repository.GetGroupsForCategoryIdWithProducts(testId,CancellationToken.None);
        //Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.ErrorCode.Should().Be(ErrorCodes.NotFound);
    }
    #endregion
}
