using Eve.Domain.Constants;
using Eve.Domain.Entities;
using Eve.Domain.Entities.Products;
using Eve.Domain.Entities.Universe;
using Eve.Domain.ExternalTypes;

namespace Eve.Tests.UnitTests.Common;

static class DataStorage
{
    public static List<CategoryEntity> GetCategories()
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

    public static List<GroupEntity> GetGroups()
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

    public static List<MarketGroupEntity> GetMarketGroups()
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

    public static List<TypeEntity> GetTypes()
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

    public static List<ProductEntity> GetProducts()
    {
        var types = GetTypes().Where(t => t.IsProduct).ToList();

        var products = types.Select(t => new ProductEntity { Id = t.Id, Quantity = 2, Time = 1234 }).ToList();

        return products;
    }

    public static List<ReprocessMaterialEntity> GetReprocessMaterials()
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

    public static List<ProductMaterialEntity> GetProductMaterials()
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

    public static List<RegionEntity> GetRegions()
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

    public static List<StationEntity> GetStation()
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

    public static List<TypeMarketHistoryInfo> GetHistory()
    {
        var history = new List<TypeMarketHistoryInfo>();

        for (int i = 0; i<20; i++)
        {
            history.Add(new TypeMarketHistoryInfo
            {
                Date = DateTime.Now.AddSeconds(i).ToString(),
                Volume = i,
                OrderCount = i
            });
        }
        return history;
    }

    public static List<TypeOrdersInfo> GetOrders()
    {
        var orders = new List<TypeOrdersInfo>();

        for (int i = 0;i < 20; i++)
        {
            orders.Add(new TypeOrdersInfo
            {
                OrderId = i + 1,
                IsBuyOrder = i % 2 == 0,
                Price = i + 500,
                LocationId = 1,
                SystemId = (int)CentralHubSystemId.Jita
            });
        }
        return orders;
    }
}
