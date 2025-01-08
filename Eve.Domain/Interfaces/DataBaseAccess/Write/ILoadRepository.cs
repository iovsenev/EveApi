using Eve.Domain.Entities;
using Eve.Domain.Entities.Universe;

namespace Eve.Domain.Interfaces.DataBaseAccess.Write;
public interface ILoadRepository
{
    Task InitialDatabase(CancellationToken token);
    Task LoadCategoryAsync(ICollection<CategoryEntity> categories, CancellationToken token);
    Task LoadGroupsAsync(ICollection<GroupEntity> groups, CancellationToken token);
    Task LoadIconsAsync(ICollection<IconEntity> icons, CancellationToken token);
    Task LoadMarketGroupsAsync(ICollection<MarketGroupEntity> groups, CancellationToken token);
    Task LoadNamesAsync(ICollection<NameEntity> names, CancellationToken token);
    Task LoadRegionsAsync(ICollection<RegionEntity> regions, CancellationToken token);
    Task LoadStationsAsync(ICollection<StationEntity> categories, CancellationToken token);
    Task LoadTypesAsync(ICollection<TypeEntity> types, CancellationToken token);
    Task SaveChangesAsync(CancellationToken token);
}