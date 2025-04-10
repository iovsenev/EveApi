using AutoMapper;
using Eve.Application.DTOs;
using Eve.Application.QueryServices.Types.GetTypesSearch;
using Eve.Domain.Common;
using Eve.Domain.Entities;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using FluentAssertions;
using Moq;

namespace Eve.Tests.UnitTests.Application.QueryServices.Types;
public class GetTypesSearchHandlerTests
{
    private readonly GetTypesSearchHandler _handler;
    private readonly Mock<IReadTypeRepository> _repository;
    private readonly Mock<IMapper> _mapper;
    private static List<TypeEntity> _entities = new List<TypeEntity>
    {
        new TypeEntity
        {
            Id = 1,
            Name = "name 1",
        },
        new TypeEntity
        {
            Id = 2,
            Name = "name 2",
        },
        new TypeEntity
        {
            Id = 3,
            Name = "name 3",
        },
    };

    public GetTypesSearchHandlerTests()
    {
        _repository = new();
        _mapper = new();
        _handler = new(_repository.Object, _mapper.Object);
    }

    [Fact]
    public async Task Handle_ReturnsSuccess()
    {
        //arrange
        _mapper
            .Setup(c => c.Map<ShortTypeDto>(It.IsAny<TypeEntity>()))
            .Returns((TypeEntity source) => new ShortTypeDto
            {
                Id = source.Id,
                Name = source.Name,
            });

        _repository
            .Setup(c => c.GetTypesByNameContains(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_entities);

        //act
        var result = await _handler.Handle(new("123"), CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Types.Count.Should().Be(_entities.Count);
    }

    [Fact]
    public async Task Handle_ReturnsError()
    {
        //arrange
        _mapper
            .Setup(c => c.Map<ShortTypeDto>(It.IsAny<TypeEntity>()))
            .Returns((TypeEntity source) => new ShortTypeDto
            {
                Id = source.Id,
                Name = source.Name,
            });

        _repository
            .Setup(c => c.GetTypesByNameContains(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Error.NotFound());

        //act
        var result = await _handler.Handle(new("123"), CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.Error.ErrorCode.Should().Be(ErrorCodes.NotFound);
    }
}
