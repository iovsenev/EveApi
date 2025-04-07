using Eve.Domain.Entities;
using Eve.Domain.Entities.Products;
using Eve.Domain.Entities.Universe;

namespace Eve.Tests.UnitTests.InfrastructureTests.RepositoriesTests.Common;

class DataStorage
{
    public List<CategoryEntity> GetCategories()
    {
        var categories = new List<CategoryEntity>(); ;

        for (int i = 0; i < 20; i++)
        {
            categories.Add(
                new CategoryEntity
                {
                    Id = i + 1,
                    Name = $"Category {i + 1}",
                    Published = i % 3 != 0
                });
        }

        return categories;
    }

    public List<GroupEntity> GetGroups()
    {
        var groups = new List<GroupEntity>();
        var random = new Random();

        for (int i = 0; i < 30; i++)
        {
            groups.Add(
                new GroupEntity
                {
                    Id = i + 1,
                    Name = $"Grpoup {i + 1}",
                    Published = i % 3 != 0,
                    CategoryId = 1
                });
        }

        return groups;
    }

    public List<MarketGroupEntity> GetMarketGroups()
    {
        var groups = new List<MarketGroupEntity>();
        var random = new Random();

        for (int i = 0; i < 5; i++)
        {
            groups.Add(
                new MarketGroupEntity
                {
                    Id = i + 1,
                    Name = $"MarketGroup {i + 1}",
                    Description = "description",
                    HasTypes = false
                });
        }

        for (int i = 5; i < 15; i++)
        {
            groups.Add(
               new MarketGroupEntity
               {
                   Id = i + 1,
                   Name = $"MarketGroup {i + 1}",
                   ParentId = i < 10 ? i - 4 : i - 9,
                   Description = "description",
                   HasTypes = false
               });
        }

        for (int i = 15; i < 35; i++)
        {
            groups.Add(
               new MarketGroupEntity
               {
                   Id = i + 1,
                   Name = $"MarketGroup {i + 1}",
                   ParentId = i < 25 ? i - 9 : i - 19,
                   Description = "description",
                   HasTypes = true
               });
        }

        return groups;
    }

    public List<TypeEntity> GetTypes()
    {
        var types = new List<TypeEntity>();
        var random = new Random();

        for (int i = 1; i <= 60; i++)
        {
            int marketGroup;
            if (i <= 20)
            {
                marketGroup = i + 15;
            }
            else if (i <= 40)
            {
                marketGroup = i - 5;
            }
            else
            {
                marketGroup = i - 25;
            }
            types.Add(
                new TypeEntity
                {
                    Id = i,
                    Name = $"Type {i}",
                    MarketGroupId = marketGroup,
                    GroupId = i <= 30 ? i : i - 30,
                    Published = i % 3 != 0,
                    IsProduct = i % 3 != 0
                });
        }

        return types;
    }

    public List<ProductEntity> GetProducts()
    {
        var types = GetTypes().Where(t => t.IsProduct).ToList();

        var products = types.Select(t => new ProductEntity { Id = t.Id, Quantity = 2, Time = 1234 }).ToList();

        return products;
    }

    public List<ReprocessMaterialEntity> GetReprocessMaterials()
    {
        var materials = new List<ReprocessMaterialEntity>();

        for (int i = 0; i < 10; i++)
        {
            var material = new ReprocessMaterialEntity
            {
                TypeId = 1,
                MaterialId = i + 2
            };
            materials.Add(material);
        }

        return materials;
    }

    public List<ProductMaterialEntity> GetProductMaterials()
    {
        var products = new List<ProductMaterialEntity>();
        var random = new Random();

        for (int i = 0; i < 30; i++)
        {
            products.Add(
                new ProductMaterialEntity
                {
                    Id = i + 1,
                    ProductId = 30 - i,
                    TypeId = random.Next(1, 50)
                });
        }

        return products;
    }

    public List<RegionEntity> GetRegions()
    {
        var regions = new List<RegionEntity>();
        for (int i = 0; i < 10; i++)
        {
            regions.Add(new RegionEntity
            {
                Id = i + 1,
                Constellations = [],
                NameId = i + 1
            });
        }

        return regions; 
    }

    public List<StationEntity> GetStation()
    {
        var stations = new List<StationEntity>();

        for (int i = 0;i < 10; i++)
        {
            stations.Add(new StationEntity
            {
                Id = i + 1,
                Name = "name"

            });
        }

        return stations;
    }
}
