using AutoMapper;
using Eve.Application.DTOs;
using Eve.Application.QueryServices.Category.GetCategories;
using Eve.Application.QueryServices.CommonRequests;
using Eve.Domain.Common;
using Eve.Domain.Entities;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Eve.Tests.UnitTests.Common;
using FluentAssertions;
using Moq;

namespace Eve.Tests.UnitTests.Application.QueryServices.Categories;
public class GetCategoriesHandlerTests
{
    private readonly Mock<IReadCategoryRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetCategoriesHandler _handler;

    public GetCategoriesHandlerTests()
    {
        _repositoryMock = new();
        _mapperMock = new();
        _handler = new GetCategoriesHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnSuccess()
    {
        //arrange
        var request = new GetCommonEmptyRequest();
        var categories = DataStorage.GetCategories();

        _repositoryMock
            .Setup(c => c.GetCategoryWithProduct(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        _mapperMock.Setup(x => x.Map<CategoryDto>(It.IsAny<CategoryEntity>()))
            .Returns((CategoryEntity source) => new CategoryDto
                    {
                        Id = source.Id,
                        Name = source.Name,
                    });

        //act
        var result = await _handler.Handle(request, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        result.Value.categories.Count().Should().Be(categories.Count);
    }

    [Fact]
    public async Task Handle_ReturnError()
    {
        //arrange
        var request = new GetCommonEmptyRequest();
        var categories = DataStorage.GetCategories();

        _repositoryMock
            .Setup(c => c.GetCategoryWithProduct(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Error.NotFound());

        _mapperMock.Setup(x => x.Map<CategoryDto>(It.IsAny<CategoryEntity>()))
            .Returns((CategoryEntity source) => new CategoryDto
            {
                Id = source.Id,
                Name = source.Name
            });

        //act
        var result = await _handler.Handle(request, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.ErrorCode.Should().Be(ErrorCodes.NotFound);
    }
}
