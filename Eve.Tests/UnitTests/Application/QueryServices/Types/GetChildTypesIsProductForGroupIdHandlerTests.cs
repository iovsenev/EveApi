using AutoMapper;
using Eve.Application.DTOs;
using Eve.Application.QueryServices.Types.GetChildTypesForGroup;
using Eve.Domain.Common;
using Eve.Domain.Entities;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using FluentAssertions;
using Moq;

namespace Eve.Tests.UnitTests.Application.QueryServices.Types;
public class GetChildTypesIsProductForGroupIdHandlerTests
{
    private readonly GetChildTypesIsProductForGroupIdHandler _handler;
    private readonly Mock<IReadTypeRepository> _repos;
    private readonly Mock<IMapper> _mapper;
    private List<TypeEntity> _entities;

    public GetChildTypesIsProductForGroupIdHandlerTests()
    {
        _repos = new();
        _mapper = new();
        _handler = new(_repos.Object, _mapper.Object);
        _entities = GetTypes();
    }

    [Fact]
    public async Task Handle_ReturnsSuccess()
    {
        //arrange
        _repos
            .Setup(c => c.GetTypeIsProductForGroupId(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_entities);

        _mapper
            .Setup(c => c.Map<ShortTypeDto>(It.IsAny<TypeEntity>()))
            .Returns((TypeEntity source) => new ShortTypeDto
            {
                Id = source.Id,
                Name = source.Name,
                GroupID = source.GroupId,
                MarketGroupID = source.MarketGroupId
            });

        //act
        var result = await _handler.Handle(new(1),CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Types.Any().Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ReturnsError()
    {
        //arrange
        _repos
            .Setup(c => c.GetTypeIsProductForGroupId(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Error.NotFound());

        _mapper
            .Setup(c => c.Map<ShortTypeDto>(It.IsAny<TypeEntity>()))
            .Returns((TypeEntity source) => new ShortTypeDto
            {
                Id = source.Id,
                Name = source.Name,
                GroupID = source.GroupId,
                MarketGroupID = source.MarketGroupId
            });

        //act
        var result = await _handler.Handle(new(1), CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.ErrorCode.Should().Be(ErrorCodes.NotFound);
    }

    private List<TypeEntity> GetTypes()
    {
        return new List<TypeEntity>
        {
            new TypeEntity
            {
                Id = 1,
                Name = "Type1",
                GroupId = 1,
                MarketGroupId = 1,
            },
            new TypeEntity
            {
                Id = 2,
                Name = "Type2",
                GroupId = 2,
                MarketGroupId = 2,
            },
            new TypeEntity
            {
                Id = 3,
                Name = "Type3",
                GroupId = 3,
                MarketGroupId = 3,
            },
        };
    }
}
