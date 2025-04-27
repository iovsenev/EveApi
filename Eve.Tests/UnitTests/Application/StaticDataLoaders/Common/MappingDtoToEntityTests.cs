using Eve.Application.StaticDataLoaders.Common;
using Eve.Application.StaticDataLoaders.ConvertFromYaml.fsd.Blueprints;
using FluentAssertions;

namespace Eve.Tests.UnitTests.Application.StaticDataLoaders.Common;
public class MappingDtoToEntityTests
{
    [Fact]
    public void GetProductEntity_ReturnsSuccess()
    {
        var dto = new BlueprintDto
        {

            BlueprintTypeId = 1,
            MaxProductionLimit = 1,
            Manufacturing = new Activity
            {
                Time = 1200,
                Products = new List<Material>
                {
                    new Material
                    {
                        Quantity = 1,
                        TypeId = 2,
                    },
                },
                Materials = new List<Material>
                {
                    new Material
                    {
                        Quantity = 1,
                        TypeId = 3,
                    },
                    new Material
                    {
                        Quantity = 1,
                        TypeId = 4,
                    },
                    new Material
                    {
                        Quantity = 1,
                        TypeId = 5,
                    },
                },
                Skills = new List<Skill>
                {
                    new Skill
                    {
                        TypeId = 6,
                        Level = 1,
                    },
                    new Skill
                    {
                        TypeId = 7,
                        Level = 1,
                    },
                }
            },
        };

        var result = MappingDtoToEntity.GetProductEntity(new List<BlueprintDto> { dto });
        var entity = result.First();

        Assert.NotNull(result);
        entity.BlueprintId.Should().Be(1);
        entity.Quantity.Should().Be(1);
        entity.Id.Should().Be(2);
        foreach (var item in entity.Materials)
        {
            item.TypeId.Should().BeInRange(3, 5);
            item.Quantity.Should().Be(1);
        }
    }

    [Fact]
    public void GetProductEntity_ReturnsSuccessWithEmptyCollection_WhenManufacturingIsNull()
    {
        var dto = new BlueprintDto
        {
            BlueprintTypeId = 1,
            MaxProductionLimit = 1,
        };

        var result = MappingDtoToEntity.GetProductEntity(new List<BlueprintDto> { dto });

        Assert.NotNull(result);
        result.Any().Should().BeFalse();
    }

    [Fact]
    public void GetProductEntity_ReturnsSuccessWithEmptyCollection_WhenEmptyProducts()
    {
        var dto = new BlueprintDto
        {

            BlueprintTypeId = 1,
            MaxProductionLimit = 1,
            Manufacturing = new Activity
            {
                Time = 1200,
                Products = new List<Material>
                {
                    
                },
                Materials = new List<Material>
                {
                    new Material
                    {
                        Quantity = 1,
                        TypeId = 3,
                    },
                    new Material
                    {
                        Quantity = 1,
                        TypeId = 4,
                    },
                    new Material
                    {
                        Quantity = 1,
                        TypeId = 5,
                    },
                },
                Skills = new List<Skill>
                {
                    new Skill
                    {
                        TypeId = 6,
                        Level = 1,
                    },
                    new Skill
                    {
                        TypeId = 7,
                        Level = 1,
                    },
                }
            },
        };

        var result = MappingDtoToEntity.GetProductEntity(new List<BlueprintDto> { dto });

        Assert.NotNull(result);
        result.Any().Should().BeFalse();
    }

    [Fact]
    public void GetProductEntity_ThrowsException_WhenProductsMoreOne()
    {
        var dto = new BlueprintDto
        {

            BlueprintTypeId = 1,
            MaxProductionLimit = 1,
            Manufacturing = new Activity
            {
                Time = 1200,
                Products = new List<Material>
                {
                    new Material
                    {
                        Quantity = 1,
                        TypeId = 2,
                    },
                    new Material
                    {
                        Quantity = 1,
                        TypeId = 2,
                    },
                },
                Materials = new List<Material>
                {
                    new Material
                    {
                        Quantity = 1,
                        TypeId = 3,
                    },
                    new Material
                    {
                        Quantity = 1,
                        TypeId = 4,
                    },
                    new Material
                    {
                        Quantity = 1,
                        TypeId = 5,
                    },
                },
                Skills = new List<Skill>
                {
                    new Skill
                    {
                        TypeId = 6,
                        Level = 1,
                    },
                    new Skill
                    {
                        TypeId = 7,
                        Level = 1,
                    },
                }
            },
        };

        Assert.Throws<Exception>(() => MappingDtoToEntity.GetProductEntity(new List<BlueprintDto> { dto}));
    }
}
