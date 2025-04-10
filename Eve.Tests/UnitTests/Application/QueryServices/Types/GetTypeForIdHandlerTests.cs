using AutoMapper;
using Eve.Application.DTOs;
using Eve.Application.QueryServices.Types.GetTypeForId;
using Eve.Domain.Common;
using Eve.Domain.Entities;
using Eve.Domain.ExternalTypes;
using Eve.Domain.Interfaces.CacheProviders;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Eve.Domain.Interfaces.ExternalServices;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Reflection;

namespace Eve.Tests.UnitTests.Application.QueryServices.Types;
public class GetTypeForIdHandlerTests
{
    private readonly GetTypeForIdHandler _handler;
    private readonly Mock<IReadTypeRepository> _repository;
    private readonly Mock<IRedisProvider> _cacheProvider;
    private readonly Mock<IEveApiOpenClientProvider> _apiProvider;
    private readonly Mock<IMapper> _mapper;
    private static TypeEntity _type = new TypeEntity
    {
        Id = 1,
        GroupId = 1,
        Description = "description",
        IsProduct = true,
        Name = "name",
        MarketGroupId = 1,
        Published = true
    };

    public GetTypeForIdHandlerTests()
    {
        _repository = new();
        _cacheProvider = new();
        _apiProvider = new();
        _mapper = new();
        _handler = new(_repository.Object, _cacheProvider.Object, _apiProvider.Object, _mapper.Object);
    }

    [Fact]
    public async Task Handle_ReturnsSuccess_WhenAllReturnsSuccess()
    {
        //arrange
        SetMapperMock();
        SetCacheProviderMock(true);
        SetRepositoryMock_GetByIdAsync(true);
        SetRepositoryMock_GetReprocessMaterialsForTypeId(true);

        //act
        var result = await _handler.Handle(new(1), CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        result.Value.BestSumPriceMaterials.Should().Be(7*3);
        result.Value.Type.Name.Should().Be("name");
    }

    [Fact]
    public async Task Handle_ReturnsSuccess_WhenReturnsOrdersIsEmptyCollection()
    {
        //arrange
        SetMapperMock();
        SetRepositoryMock_GetByIdAsync(true);
        SetRepositoryMock_GetReprocessMaterialsForTypeId(true);

        _cacheProvider
            .Setup(c => c.GetOrSetAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<Result<TypeInfoDto>>>>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(async (string key,
                Func<Task<Result<TypeInfoDto>>> func,
                DistributedCacheEntryOptions _,
                CancellationToken _) =>
                     {
                         return await func();
                     }
            );

        _cacheProvider
          .Setup(c => c.GetOrSetAsync(
              It.IsAny<string>(),
              It.IsAny<Func<Task<Result<ICollection<TypeOrdersInfo>>>>>(),
              It.IsAny<DistributedCacheEntryOptions>(),
              It.IsAny<CancellationToken>()))
          .ReturnsAsync(new List<TypeOrdersInfo>());

        //act
        var result = await _handler.Handle(new(1), CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        result.Value.BestSumPriceMaterials.Should().Be(0);
        result.Value.Type.Name.Should().Be("name");
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenGetByIdAsyncReturnsError()
    {
        //arrange
        SetMapperMock();
        SetCacheProviderMock(true);
        SetRepositoryMock_GetByIdAsync(false);
        SetRepositoryMock_GetReprocessMaterialsForTypeId(true);

        //act
        var result = await _handler.Handle(new(1), CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.Error.ErrorCode.Should().Be(ErrorCodes.InternalServer);
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenGetReprocessMaterialsForTypeIdReturnsError()
    {
        //arrange
        SetMapperMock();
        SetCacheProviderMock(true);
        SetRepositoryMock_GetByIdAsync(true);
        SetRepositoryMock_GetReprocessMaterialsForTypeId(false);

        //act
        var result = await _handler.Handle(new(1), CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.Error.ErrorCode.Should().Be(ErrorCodes.InternalServer);
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenGetCacheProviderReturnsError()
    {
        //arrange
        SetMapperMock();
        SetCacheProviderMock(false);
        SetRepositoryMock_GetByIdAsync(true);
        SetRepositoryMock_GetReprocessMaterialsForTypeId(true);

        //act
        var result = await _handler.Handle(new(1), CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.Error.ErrorCode.Should().Be(ErrorCodes.InternalServer);
    }

    private void SetMapperMock()
    {
        _mapper
            .Setup(c => c.Map<TypeInfoDto>(It.IsAny<TypeEntity>()))
            .Returns((TypeEntity source) => new TypeInfoDto
            {
                Id = source.Id,
                Name = source.Name,
                Description = source.Description,
                IsProduct = source.IsProduct,
                MarketGroupId = source.MarketGroupId,
            });

        _mapper
            .Setup(c => c.Map<MaterialDto>(It.IsAny<ReprocessMaterialEntity>()))
            .Returns((ReprocessMaterialEntity source) => new MaterialDto
            {
                TypeId = source.TypeId,
                Name = source.Material.Name,
                Quantity = source.Quantity,
            });
    }

    private void  SetCacheProviderMock(bool isSuccess)
    {
        _cacheProvider
            .Setup(c => c.GetOrSetAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<Result<TypeInfoDto>>>>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(async (string key,
                Func<Task<Result<TypeInfoDto>>> func,
                DistributedCacheEntryOptions _,
                CancellationToken _) =>
                {
                    return await func();
                }
            );

        if (isSuccess)
        {
            _cacheProvider
           .Setup(c => c.GetOrSetAsync(
               It.IsAny<string>(),
               It.IsAny<Func<Task<Result<ICollection<TypeOrdersInfo>>>>>(),
               It.IsAny<DistributedCacheEntryOptions>(),
               It.IsAny<CancellationToken>()))
           .ReturnsAsync(GetOrders());
        }
        else
        {
            _cacheProvider
           .Setup(c => c.GetOrSetAsync(
               It.IsAny<string>(),
               It.IsAny<Func<Task<Result<ICollection<TypeOrdersInfo>>>>>(),
               It.IsAny<DistributedCacheEntryOptions>(),
               It.IsAny<CancellationToken>()))
           .ReturnsAsync(Error.InternalServer());
        }
    }
    
    private void SetRepositoryMock_GetByIdAsync(bool isSuccess)
    {
        if (isSuccess)
        {
            _repository
            .Setup(c => c.GetByIdAsync(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_type);
        }
        else
        {
            _repository
            .Setup(c => c.GetByIdAsync(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Error.InternalServer());
        }
    }

    private void SetRepositoryMock_GetReprocessMaterialsForTypeId(bool isSuccess)
    {
        if (isSuccess)
        {
            _repository
            .Setup(c => c.GetReprocessMaterialsForTypeId(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetMaterials());
        }
        else
        {
            _repository
            .Setup(c => c.GetReprocessMaterialsForTypeId(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Error.InternalServer());
        }
    }

    private List<ReprocessMaterialEntity> GetMaterials()
    {
        var materials = new List<ReprocessMaterialEntity>()
        {
            new ReprocessMaterialEntity
            {
                MaterialId = 2,
                Material = new TypeEntity
                {
                    Id = 2,
                    Name = "type 2",
                },
                TypeId = 1,
                Type = _type,
                Quantity = 1,
            },
            new ReprocessMaterialEntity
            {
                MaterialId = 3,
                Material = new TypeEntity
                {
                    Id = 3,
                    Name = "type 3",
                },
                TypeId = 1,
                Type = _type,
                Quantity = 1,
            },
            new ReprocessMaterialEntity
            {
                MaterialId = 4,
                Material = new TypeEntity
                {
                    Id = 4,
                    Name = "type 4",
                },
                TypeId = 1,
                Type = _type,
                Quantity = 1,
            },
        };

        return materials;
    }

    private List<TypeOrdersInfo> GetOrders()
    {
        var orders = new List<TypeOrdersInfo>();

        for (int i = 1; i <= 8; i++)
        {
            orders.Add(new TypeOrdersInfo
            {
                IsBuyOrder = i % 2 == 0,
                OrderId = i,
                Price = i,
            });
        }
        return orders;
    }
}
