using AutoMapper;
using Eve.Application.DTOs;
using Eve.Application.QueryServices.Products;
using Eve.Domain.Common;
using Eve.Domain.Entities;
using Eve.Domain.Entities.Products;
using Eve.Domain.ExternalTypes;
using Eve.Domain.Interfaces.CacheProviders;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Eve.Domain.Interfaces.ExternalServices;
using Eve.Tests.UnitTests.Common;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;

namespace Eve.Tests.UnitTests.Application.QueryServices.Products;
public class GetProductHandlerTests
{
    private readonly Mock<IReadProductRepository> _repository;
    private readonly Mock<IRedisProvider> _cacheProvider;
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<IEveApiOpenClientProvider> _apiClient;
    private readonly GetProductHandler _handler;

    private ProductEntity _product = new ProductEntity
    {
        Id = 1,
        BlueprintId = 1,
        MaxProductionLimit = 1,
        Quantity = 1,
        Time = 1,
    };
    private List<ProductMaterialEntity> _materials;

    public GetProductHandlerTests()
    {
        _repository = new();
        _cacheProvider = new();
        _mapper = new();
        _apiClient = new();
        _handler = new(_repository.Object, _cacheProvider.Object, _mapper.Object, _apiClient.Object);
        _materials = GetMaterials();
        SetMapperMock();
    }

    [Fact]
    public async Task Handle_ReturnSuccess_WhenAllReturnsSuccess()
    {
        //arrange
        Set_GetMaterialsForProductId(true);
        Set_GetProductForId(true);
        SetCacheProviderMock(true);

        //act

        var result = await _handler.Handle(new(1, 1, 1), CancellationToken.None);
        //assert

        result.IsSuccess.Should().BeTrue();
        result.Value.SellPrice.Should().Be(3.0);
        result.Value.BuyPrice.Should().Be(6.0);
        result.Value.SellPriceMaterials.Should().Be(3.0 * _materials.Count * 20);
        result.Value.BuyPriceMaterials.Should().Be(6.0 * _materials.Count * 20);
    }

    [Fact]
    public async Task Handle_ReturnSuccess_WhenReturnsEmptyOrders()
    {
        //arrange
        Set_GetMaterialsForProductId(true);
        Set_GetProductForId(true);
        SetCacheProviderMock(true);

        _cacheProvider
            .Setup(c => c.GetOrSetAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<Result<ICollection<TypeOrdersInfo>>>>>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TypeOrdersInfo>());

        //act

        var result = await _handler.Handle(new(1, 1, 1), CancellationToken.None);
        //assert

        result.IsSuccess.Should().BeTrue();
        result.Value.SellPrice.Should().Be(0);
        result.Value.BuyPrice.Should().Be(0);
        result.Value.SellPriceMaterials.Should().Be(0 * _materials.Count * 20);
        result.Value.BuyPriceMaterials.Should().Be(0 * _materials.Count * 20);
    }

    [Fact]
    public async Task Handle_ReturnError_WhenReturnsProductWithError()
    {
        //arrange
        Set_GetMaterialsForProductId(true);
        Set_GetProductForId(false);
        SetCacheProviderMock(true);

        //act

        var result = await _handler.Handle(new(1, 1, 1), CancellationToken.None);
        //assert

        result.IsSuccess.Should().BeFalse();
        result.Error.ErrorCode.Should().Be(ErrorCodes.InternalServer);
    }

    [Fact]
    public async Task Handle_ReturnError_WhenReturnsMaterialsWithError()
    {
        //arrange
        Set_GetMaterialsForProductId(false);
        Set_GetProductForId(true);
        SetCacheProviderMock(true);

        //act

        var result = await _handler.Handle(new(1, 1, 1), CancellationToken.None);
        //assert

        result.IsSuccess.Should().BeFalse();
        result.Error.ErrorCode.Should().Be(ErrorCodes.InternalServer);
    }

    [Fact]
    public async Task Handle_ReturnError_WhenReturnsCacheProviderWithError()
    {
        //arrange
        Set_GetMaterialsForProductId(true);
        Set_GetProductForId(true);
        SetCacheProviderMock(false);

        //act

        var result = await _handler.Handle(new(1, 1, 1), CancellationToken.None);
        //assert

        result.IsSuccess.Should().BeFalse();
        result.Error.ErrorCode.Should().Be(ErrorCodes.InternalServer);
    }

    private List<ProductMaterialEntity> GetMaterials()
    {
        var materials = new List<ProductMaterialEntity>
        {
            new ProductMaterialEntity
            {
                Id = 1,
                ProductId = 1,
                Quantity =20,
                TypeId = 1,
                Type = new TypeEntity
                {
                    Id = 1,
                    Name = "name 1"
                }
            },
            new ProductMaterialEntity
            {
                Id = 2,
                ProductId = 1,
                Quantity =20,
                TypeId = 2,
                Type = new TypeEntity
                {
                    Id = 2,
                    Name = "name 2"
                }
            },
            new ProductMaterialEntity
            {
                Id = 3,
                ProductId = 1,
                Quantity =20,
                TypeId = 3,
                Type = new TypeEntity
                {
                    Id = 3,
                    Name = "name 3"
                }
            },
            new ProductMaterialEntity
            {
                Id = 4,
                ProductId = 1,
                Quantity =20,
                TypeId = 4,
                Type = new TypeEntity
                {
                    Id = 4,
                    Name = "name 4"
                }
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

    private void SetCacheProviderMock(bool isReturnSuccess)
    {
        _cacheProvider
            .Setup(c => c.GetOrSetAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<Result<ProductDto>>>>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(
                async (string key,
                Func<Task<Result<ProductDto>>> factory,
                DistributedCacheEntryOptions _,
                CancellationToken _) =>
                {
                    return await factory();
                }
            );

        if (isReturnSuccess)
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

    private void SetMapperMock()
    {
        _mapper
            .Setup(c => c.Map<ProductDto>(It.IsAny<ProductEntity>()))
            .Returns((ProductEntity source) => new ProductDto
            {
                BlueprintId = source.BlueprintId,
                Id = source.Id,
                Quantity = source.Quantity,
                Time = source.Time,
            });

        _mapper
            .Setup(c => c.Map<MaterialDto>(It.IsAny<ProductMaterialEntity>()))
            .Returns((ProductMaterialEntity source) => new MaterialDto
            {
                Name = "name",
                Quantity = source.Quantity,
                TypeId = source.TypeId
            });
    }

    private void Set_GetProductForId(bool isReturnSuccess)
    {
        if (isReturnSuccess) {
            _repository
            .Setup(c => c.GetProductForId(
                It.IsAny<int>(),
                CancellationToken.None))
            .ReturnsAsync(_product);
        }
        else
        {
            _repository
           .Setup(c => c.GetProductForId(
               It.IsAny<int>(),
               CancellationToken.None))
           .ReturnsAsync(Error.InternalServer());
        }
    }

    private void Set_GetMaterialsForProductId(bool isReturnSuccess)
    {
        if (isReturnSuccess)
        {
            _repository
            .Setup(c => c.GetMaterialsForProductId(It.IsAny<int>(), CancellationToken.None))
            .ReturnsAsync(_materials);
        }
        else
        {
            _repository
            .Setup(c => c.GetMaterialsForProductId(It.IsAny<int>(), CancellationToken.None))
            .ReturnsAsync(Error.InternalServer());
        }
    }

}
