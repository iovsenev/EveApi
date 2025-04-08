using AutoMapper;
using Eve.Application.DTOs;
using Eve.Application.QueryServices.Category.GetGroupsForCategoryId;
using Eve.Application.QueryServices.CommonRequests;
using Eve.Domain.Common;
using Eve.Domain.Entities;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Eve.Tests.UnitTests.Common;
using FluentAssertions;
using Moq;

namespace Eve.Tests.UnitTests.Application.QueryServices.Categories;
public class GetGroupsForCategoryIdHandlerTests
{
    private readonly Mock<IReadGroupRepository> _repository;
    private readonly Mock<IMapper> _mapper;
    private readonly GetGroupsForCategoryIdHandler _handler;

    public GetGroupsForCategoryIdHandlerTests()
    {
        _repository = new();
        _mapper = new();
        _handler = new(_repository.Object, _mapper.Object);
    }

    [Fact]
    public async Task Handle_ReturnsSuccess()
    {
        //arrange
        var request = new GetCommonRequestForId(1);
        var groups = DataStorage.GetGroups();

        _repository
            .Setup(c => c.GetGroupsForCategoryIdWithProducts(
                                    It.IsAny<int>(),
                                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(groups);

        _mapper.Setup(x => x.Map<GroupDto>(It.IsAny<GroupEntity>()))
            .Returns((GroupEntity source) => new GroupDto
            {
                Id = source.Id,
                Name = source.Name,
            });

        //act
        var result = await _handler.Handle(request, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        result.Value.groups.Count.Should().Be(groups.Count);
    }

    [Fact]
    public async Task Handle_ReturnsError()
    {
        //arrange
        var request = new GetCommonRequestForId(1);
        var groups = DataStorage.GetGroups();

        _repository
            .Setup(c => c.GetGroupsForCategoryIdWithProducts(
                                    It.IsAny<int>(),
                                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(Error.NotFound());

        _mapper.Setup(x => x.Map<GroupDto>(It.IsAny<GroupEntity>()))
            .Returns((GroupEntity source) => new GroupDto
            {
                Id = source.Id,
                Name = source.Name,
            });

        //act
        var result = await _handler.Handle(request, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.ErrorCode.Should().Be(ErrorCodes.NotFound);
    }
}
