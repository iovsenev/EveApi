
using Eve.Domain.Common;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Eve.Infrastructure.DataBase.Contexts;
using Eve.Infrastructure.DataBase.Repositories.Read;
using Eve.Tests.UnitTests.Common;
using FluentAssertions;

namespace Eve.Tests.UnitTests.InfrastructureTests.RepositoriesTests
{
    public class ReadProductRepositoryTest : BaseReadRepositoryTest<IReadProductRepository>
    {
        protected override IReadProductRepository CreateRepository(IAppDbContext context)
        {
            return new ReadProductRepository(context);
        }

        #region GetProductForId
        [Fact]
        public async Task GetProductForId_ReturnSuccess_WithExistId()
        {
            //Arrange
            var typeId = 1;

            //Act
            var result = await Repository.GetProductForId(typeId,CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().Be(typeId);
        }

        [Fact]
        public async Task GetProductForId_ReturnError_WithNotExistId()
        {
            //Arrange
            var typeId = 3;

            //Act
            var result = await Repository.GetProductForId(typeId, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.Error.ErrorCode.Should().Be(ErrorCodes.NotFound);
        }
        #endregion

        #region GetMaterialsForProductId
        [Fact]
        public async Task GetMaterialsForProductId_ReturnSuccess_WhitExistId()
        {
            //Arrange
            var productId = 29;

            //Act
            var result = await Repository.GetMaterialsForProductId(productId, CancellationToken.None);

            //assert 
            result.IsSuccess.Should().BeTrue();
            result.Value.Any().Should().BeTrue();
        }
        
        [Fact]
        public async Task GetMaterialsForProductId_ReturnError_WhitNotExistId()
        {
            //Arrange
            var productId = 32;

            //Act
            var result = await Repository.GetMaterialsForProductId(productId, CancellationToken.None);

            //assert 
            result.IsFailure.Should().BeTrue();
            result.Error.ErrorCode.Should().Be(ErrorCodes.NotFound);
        }
        #endregion
    }
}
