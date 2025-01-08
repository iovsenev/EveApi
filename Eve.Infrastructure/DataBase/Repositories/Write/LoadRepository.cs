using Eve.Domain.Entities;
using Eve.Domain.Entities.Universe;
using Eve.Domain.Interfaces.DataBaseAccess.Write;
using Eve.Infrastructure.DataBase.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Eve.Infrastructure.DataBase.Repositories.Write;
public class LoadRepository : ILoadRepository
{
    private readonly AppDbContext _context;

    public LoadRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task InitialDatabase(CancellationToken token)
    {
        await _context.Database.EnsureDeletedAsync(token);
        await _context.Database.MigrateAsync(token);
    }

    public async Task LoadNamesAsync(
        ICollection<NameEntity> names,
        CancellationToken token)
    {
        await _context.Names.AddRangeAsync(names, token);
    }

    public async Task LoadCategoryAsync(
        ICollection<CategoryEntity> categories, 
        CancellationToken token)
    {
        await _context.Categories.AddRangeAsync(categories, token);
    }

    public async Task LoadStationsAsync(
        ICollection<StationEntity> categories, 
        CancellationToken token)
    {
        await _context.Stations.AddRangeAsync(categories, token);
    }
    
    public async Task LoadGroupsAsync(
        ICollection<GroupEntity> groups, 
        CancellationToken token)
    {
        await _context.Groups.AddRangeAsync(groups, token);
    }
    
    public async Task LoadMarketGroupsAsync(
        ICollection<MarketGroupEntity> groups, 
        CancellationToken token)
    {
        await _context.MarketGroups.AddRangeAsync(groups, token);
    }
    
    public async Task LoadTypesAsync(
        ICollection<TypeEntity> types, 
        CancellationToken token)
    {
        await _context.Types.AddRangeAsync(types, token);
    }

    public async Task LoadRegionsAsync(
        ICollection<RegionEntity> regions,
        CancellationToken token)
    {
        await _context.Regions.AddRangeAsync(regions, token);
    }

    public async Task LoadIconsAsync(
        ICollection<IconEntity> icons,
        CancellationToken token)
    {
        await _context.Icons.AddRangeAsync(icons, token);
    }

    public async Task SaveChangesAsync(CancellationToken token)
    {
        await _context.SaveChangesAsync(token);
    }
}
