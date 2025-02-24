using Eve.Domain.Entities;
using Eve.Domain.Entities.Products;
using Eve.Domain.Entities.Universe;
using Microsoft.EntityFrameworkCore;

namespace Eve.Infrastructure.DataBase.Contexts;
public interface IAppDbContext
{
    DbSet<CategoryEntity> Categories { get; }
    DbSet<ConstellationEntity> Constellations { get; }
    DbSet<GroupEntity> Groups { get; }
    DbSet<IconEntity> Icons { get; }
    DbSet<MarketGroupEntity> MarketGroups { get; }
    DbSet<NameEntity> Names { get; }
    DbSet<ProductMaterialEntity> ProductMaterials { get; }
    DbSet<ProductEntity> Products { get; }
    DbSet<ProductSkillEntity> ProductSkills { get; }
    DbSet<RegionEntity> Regions { get; }
    DbSet<ReprocessMaterialEntity> ReprocessMaterials { get; }
    DbSet<SolarSystemEntity> SolarSystems { get; }
    DbSet<StationEntity> Stations { get; }
    DbSet<TypeEntity> Types { get; }
}