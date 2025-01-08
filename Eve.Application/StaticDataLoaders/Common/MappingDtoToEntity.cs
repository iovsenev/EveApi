using Eve.Application.StaticDataLoaders.ConvertFromYaml.fsd.Blueprints;
using Eve.Domain.Entities.Products;

namespace Eve.Application.StaticDataLoaders.Common;
public static class MappingDtoToEntity
{
    public static List<ProductEntity> GetProductEntity (List<BlueprintDto> blueprints)
    {
        var blueprintManufact = blueprints.Where(t => t.Manufacturing is not null);

        var products = new List<ProductEntity>();

        foreach (var entity in blueprintManufact)
        {
            if (entity.Manufacturing.Products.Count > 1) throw new Exception($"biggest 1 {entity.BlueprintTypeId}");
            if (entity.Manufacturing.Products.Count < 1) continue;

            var productuction = entity.Manufacturing.Products[0];

            var product = new ProductEntity
            {
                Id = productuction.TypeId,
                Quantity = productuction.Quantity,
                Time = entity.Manufacturing.Time,
                MaxProductionLimit = entity.MaxProductionLimit,
                BlueprintId = entity.BlueprintTypeId,
                Skills = entity.Manufacturing.Skills
                            .Select(s => new ProductSkillEntity
                            {
                                Level = s.Level,
                                TypeId = s.TypeId,
                                ProductId = productuction.TypeId
                            })
                            .ToList(),
                Materials = entity.Manufacturing.Materials
                                .Select(m => new ProductMaterialEntity
                                {
                                    Quantity = m.Quantity,
                                    TypeId = m.TypeId,
                                    ProductId = productuction.TypeId
                                })
                                .ToList()
            };

            products.Add(product);
        }

        return products;
    }
}
