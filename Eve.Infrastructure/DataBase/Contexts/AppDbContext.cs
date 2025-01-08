using Eve.Domain.Entities;
using Eve.Domain.Entities.Products;
using Eve.Domain.Entities.Universe;
using Eve.Infrastructure.DataBase.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Eve.Infrastructure.DataBase.Contexts;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        //Database.Migrate();
    }

    public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();
    public DbSet<GroupEntity> Groups => Set<GroupEntity>();
    public DbSet<TypeEntity> Types => Set<TypeEntity>();
    public DbSet<MarketGroupEntity> MarketGroups => Set<MarketGroupEntity>();
    public DbSet<ReprocessMaterialEntity> ReprocessMaterials => Set<ReprocessMaterialEntity>();
    public DbSet<ProductEntity> Products => Set<ProductEntity>();
    public DbSet<ProductSkillEntity> ProductSkills => Set<ProductSkillEntity>();
    public DbSet<ProductMaterialEntity> ProductMaterials => Set<ProductMaterialEntity>();
    public DbSet<SolarSystemEntity> SolarSystems => Set<SolarSystemEntity>();
    public DbSet<ConstellationEntity> Constellations => Set<ConstellationEntity>();
    public DbSet<RegionEntity> Regions => Set<RegionEntity>();
    public DbSet<IconEntity> Icons => Set<IconEntity>();
    public DbSet<NameEntity> Names => Set<NameEntity>();
    public DbSet<StationEntity> Stations => Set<StationEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CategoryEntityConfiguration).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
        optionsBuilder.EnableSensitiveDataLogging(false);
        base.OnConfiguring(optionsBuilder);
    }
}